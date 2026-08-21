using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces.UserAuth;

namespace MonyLoop.Domain.Interfaces;

public interface IUnitOfWork
{
    IOnboardingCaseRepository OnboardingCases { get; }
    IDocumentRequirementRepository DocumentRequirements { get; }
    IDocumentRepository Documents { get; }
    IMemberLedgerRepository MemberLedgers { get; }
    IOTPTokenRepository OTPTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
