using Mony_Loop.Infrastructure.Data;
using Mony_Loop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MonyLoopDbContext _dbcontext;

        public IOnboardingCaseRepository OnboardingCases { get; }
        public IDocumentRequirementRepository DocumentRequirements { get; }
        public IDocumentRepository Documents { get; }
        public IMemberLedgerRepository MemberLedgers { get; }

        public UnitOfWork(
            MonyLoopDbContext dbcontext,
            IOnboardingCaseRepository onboardingCases,
            IDocumentRequirementRepository documentRequirements,
            IDocumentRepository documents,
            IMemberLedgerRepository memberLedgers)
        {
            _dbcontext = dbcontext;
            OnboardingCases = onboardingCases;
            DocumentRequirements = documentRequirements;
            Documents = documents;
            MemberLedgers = memberLedgers;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbcontext.SaveChangesAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            await _dbcontext.DisposeAsync();
        }
    }
}
