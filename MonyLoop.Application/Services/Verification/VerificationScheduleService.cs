using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;
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
        private readonly IUnitOfWork _unitOfWork;

        public VerificationScheduleService(
            IVerificationScheduleRepository scheduleRepository,
            IMembershipApplicationRepository applicationRepository, // ===== EXTRA CODE ADDED HERE =====
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _applicationRepository = applicationRepository; // ===== EXTRA CODE ADDED HERE =====
            _unitOfWork = unitOfWork;
        }

        public async Task<VerificationScheduleResponseDto> CreateScheduleAsync(CreateVerificationScheduleDto dto, CancellationToken ct = default)
        {
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
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application != null)
            {
                application.Stage = MembershipApplicationStage.VerificationScheduled;
                await _applicationRepository.UpdateAsync(application);
            }

            await _unitOfWork.SaveChangesAsync(ct);

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
    }
}