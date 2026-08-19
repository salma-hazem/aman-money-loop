using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;

namespace MonyLoop.Domain.Interfaces;

public interface IUnitOfWork
{
    IOnboardingCaseRepository OnboardingCases { get; }
    IDocumentRequirementRepository DocumentRequirements { get; }
    IDocumentRepository Documents { get; }
    IMemberLedgerRepository MemberLedgers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
