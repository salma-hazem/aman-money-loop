namespace MonyLoop.Application.Services;

public class VerificationScheduleService : IVerificationScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public VerificationScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VerificationSchedule> ScheduleRoundAsync(int applicationId, int roundId, DateTime scheduledAt, CancellationToken ct)
    {
        var schedule = new VerificationSchedule
        {
            ApplicationId = applicationId,
            RoundId = roundId,
            ScheduledAt = scheduledAt,
            Stage = VerificationStage.Scheduled
        };

        await _unitOfWork.VerificationSchedules.AddAsync(schedule, ct);
        return schedule;
    }

    public void UpdateScheduleStage(VerificationSchedule schedule, VerificationStage newStage)
    {
        schedule.Stage = newStage;
        _unitOfWork.VerificationSchedules.Update(schedule);
    }
}
