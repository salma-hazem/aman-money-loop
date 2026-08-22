using System;
using MonyLoop.Domain.Constants.Verification;

namespace MonyLoop.Application.DTOs.Verification
{
    public class CreateVerificationScheduleDto
    {
        public Guid ApplicationId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LocationLink { get; set; }
        public string? VideoLink { get; set; }
    }

    public class VerificationScheduleResponseDto
    {
        public Guid VerificationScheduleId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LocationLink { get; set; }
        public string? VideoLink { get; set; }
        public ScheduleStatus Status { get; set; }
    }

    public class UpdateVerificationScheduleDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LocationLink { get; set; }
        public string? VideoLink { get; set; }
        public ScheduleStatus Status { get; set; }
    }
}