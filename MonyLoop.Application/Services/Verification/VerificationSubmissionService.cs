namespace MonyLoop.Application.Services;

public class VerificationSubmissionService : IVerificationSubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVerificationScheduleService _scheduleService;

    public VerificationSubmissionService(IUnitOfWork unitOfWork, IVerificationScheduleService scheduleService)
    {
        _unitOfWork = unitOfWork;
        _scheduleService = scheduleService;
    }

    public async Task<VerificationStage> EvaluateSubmissionAsync(VerificationSchedule schedule, decimal score, decimal passingScore, CancellationToken ct)
    {
        var finalStage = score >= passingScore 
            ? VerificationStage.Completed 
            : VerificationStage.Rejected;

        _scheduleService.UpdateScheduleStage(schedule, finalStage);
        
        return finalStage;
    }
}
