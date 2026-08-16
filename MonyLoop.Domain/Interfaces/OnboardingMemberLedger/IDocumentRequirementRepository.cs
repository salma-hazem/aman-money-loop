using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.OnboardingMemberLedger
{
    public interface IDocumentRequirementRepository : IGenericRepository<DocumentRequirement>
    {
        Task<IEnumerable<DocumentRequirement>> GetActiveOrderedAsync(CancellationToken ct = default);
        Task<IEnumerable<DocumentRequirement>> GetRequiredOnlyAsync(CancellationToken ct = default);
    }
}
