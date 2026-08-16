using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IOnboardingCaseRepository OnboardingCases { get; }
        IDocumentRequirementRepository DocumentRequirements { get; }
        IDocumentRepository Documents { get; }
        IMemberLedgerRepository MemberLedgers { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
