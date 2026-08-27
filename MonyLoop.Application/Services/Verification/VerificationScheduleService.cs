using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Application.ServicesAbstractions.Verification;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Constants.Verification;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.Verification;

namespace MonyLoop.Application.Services.Verification
{
    public class VerificationScheduleService : IVerificationScheduleService
    {
        private readonly IVerificationScheduleRepository _scheduleRepository;
        private readonly IMembershipApplicationRepository _applicationRepository;
        private readonly IVerificationRoundRepository _roundRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender; // NEW: Injected Email Sender

        public VerificationScheduleService(
            IVerificationScheduleRepository scheduleRepository,
            IMembershipApplicationRepository applicationRepository,
            IVerificationRoundRepository roundRepository,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender) // NEW: Constructor parameter
        {
            _scheduleRepository = scheduleRepository;
            _applicationRepository = applicationRepository;
            _roundRepository = roundRepository;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<VerificationScheduleResponseDto> CreateScheduleAsync(CreateVerificationScheduleDto dto, CancellationToken ct = default)
        {
            // 1. Fetch Application & Validate Existence
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId)
                ?? throw new KeyNotFoundException($"Application with ID {dto.ApplicationId} not found.");

            // 2. SRS Rule: The application must currently be Shortlisted
            if (application.Stage != MembershipApplicationStage.Shortlisted)
            {
                throw new InvalidOperationException("Verification can only be scheduled for applicants in the 'Shortlisted' stage.");
            }

            // 3. Fetch Round & Validate Existence
            var round = await _roundRepository.GetVerificationRoundByIdAsync(dto.VerificationRoundId, ct)
                ?? throw new KeyNotFoundException($"Verification round with ID {dto.VerificationRoundId} not found.");

            // 4. SRS Rule: The selected round must belong to the applicant’s circle
            if (round.CircleId != application.CircleId)
            {
                throw new InvalidOperationException("The selected verification round does not belong to the applicant's circle.");
            }

            // 5. SRS Rule: The date and time are in the future
            var scheduledDateTime = dto.Date.ToDateTime(dto.Time);
            if (scheduledDateTime <= DateTime.UtcNow)
            {
                throw new ArgumentException("The scheduled date and time must be in the future.");
            }

            // 6. SRS Rule: Format specifics (Location for In-Person, Video Link for Video)
            var formatStr = round.Format?.ToString() ?? string.Empty;
            if (formatStr.Contains("InPerson", StringComparison.OrdinalIgnoreCase) || formatStr.Contains("In-Person", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.LocationLink))
                    throw new ArgumentException("A location must be supplied for in-person verification rounds.");
            }
            else if (formatStr.Contains("Video", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.VideoLink))
                    throw new ArgumentException("A video link must be supplied for video verification rounds.");
            }

            // 7. SRS Rule: Another active schedule does not already exist
            var existingSchedules = await _scheduleRepository.GetByApplicationIdAsync(dto.ApplicationId, ct);
            if (existingSchedules.Any(s => s.VerificationRoundId == dto.VerificationRoundId && s.Status == ScheduleStatus.Scheduled))
            {
                throw new InvalidOperationException("An active schedule is already pending for this verification round.");
            }

            // All validations passed, proceed to create the schedule
            var schedule = new VerificationSchedule
            {
                VerificationScheduleId = Guid.NewGuid(),
                ApplicationId = dto.ApplicationId,
                VerificationRoundId = dto.VerificationRoundId,
                Date = dto.Date,
                Time = dto.Time,
                LocationLink = dto.LocationLink,
                VideoLink = dto.VideoLink,
                Status = ScheduleStatus.Scheduled
            };

            await _scheduleRepository.AddAsync(schedule, ct);

            // Update application stage to VerificationScheduled
            application.Stage = MembershipApplicationStage.VerificationScheduled;
            await _applicationRepository.UpdateAsync(application);

            await _unitOfWork.SaveChangesAsync(ct);

            // NEW: Send the calendar invite if the flag is true
            if (dto.SendCalendarInvite)
            {
                // Note: Assumes `application.Email` exists. If the email is inside a related User entity, adjust to `application.User.Email`
                string applicantEmail = application.Email ?? "no-reply@monyloop.com";

                string icsContent = GenerateIcsContent(schedule, round);
                string locationText = !string.IsNullOrWhiteSpace(schedule.LocationLink) ? schedule.LocationLink : schedule.VideoLink;

                string htmlBody = $@"
                    <h3>Your Verification Interview is Scheduled</h3>
                    <p><strong>Round:</strong> {round.RoundName}</p>
                    <p><strong>Date & Time:</strong> {schedule.Date} at {schedule.Time}</p>
                    <p><strong>Location/Link:</strong> {locationText}</p>
                    <p>Please find the attached calendar invitation.</p>";

                await _emailSender.SendCalendarInviteAsync(
                    toEmail: applicantEmail,
                    subject: $"MonyLoop Verification: {round.RoundName}",
                    htmlBody: htmlBody,
                    icsContent: icsContent,
                    ct: ct
                );
            }

            return MapToScheduleDto(schedule);
        }

        public async Task<VerificationScheduleResponseDto?> GetScheduleByIdAsync(Guid verificationScheduleId, CancellationToken ct = default)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(verificationScheduleId, ct);
            return schedule == null ? null : MapToScheduleDto(schedule);
        }

        public async Task<IReadOnlyList<VerificationScheduleResponseDto>> GetSchedulesByApplicationIdAsync(Guid applicationId, CancellationToken ct = default)
        {
            var schedules = await _scheduleRepository.GetByApplicationIdAsync(applicationId, ct);
            return schedules.Select(MapToScheduleDto).ToList();
        }

        public async Task UpdateScheduleStatusAsync(Guid verificationScheduleId, UpdateVerificationScheduleDto dto, CancellationToken ct = default)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(verificationScheduleId, ct)
                ?? throw new KeyNotFoundException($"Schedule with ID {verificationScheduleId} was not found.");

            schedule.Date = dto.Date;
            schedule.Time = dto.Time;
            schedule.LocationLink = dto.LocationLink;
            schedule.VideoLink = dto.VideoLink;
            schedule.Status = dto.Status;

            await _scheduleRepository.UpdateByIdAsync(verificationScheduleId, schedule, ct);

            if (dto.Status == ScheduleStatus.Cancelled)
            {
                var application = await _applicationRepository.GetByIdAsync(schedule.ApplicationId);
                if (application != null)
                {
                    application.Stage = MembershipApplicationStage.Shortlisted;
                    await _applicationRepository.UpdateAsync(application);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private static VerificationScheduleResponseDto MapToScheduleDto(VerificationSchedule schedule)
        {
            return new VerificationScheduleResponseDto
            {
                VerificationScheduleId = schedule.VerificationScheduleId,
                ApplicationId = schedule.ApplicationId,
                VerificationRoundId = schedule.VerificationRoundId,
                Date = schedule.Date,
                Time = schedule.Time,
                LocationLink = schedule.LocationLink,
                VideoLink = schedule.VideoLink,
                Status = schedule.Status
            };
        }

        // NEW: Helper method to generate the raw .ics file content
        private static string GenerateIcsContent(VerificationSchedule schedule, VerificationRound round)
        {
            var startDateTime = schedule.Date.ToDateTime(schedule.Time);
            var endDateTime = startDateTime.AddHours(1); // Defaulting interview duration to 1 hour

            var locationOrVideo = !string.IsNullOrWhiteSpace(schedule.LocationLink)
                ? schedule.LocationLink
                : schedule.VideoLink;

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//MonyLoop//EN");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{Guid.NewGuid()}@monyloop.com");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTSTART:{startDateTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTEND:{endDateTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
            sb.AppendLine($"SUMMARY:MonyLoop Verification - {round.RoundName}");
            sb.AppendLine($"LOCATION:{locationOrVideo}");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }
    }
}