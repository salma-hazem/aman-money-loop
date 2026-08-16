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

        private IOnboardingCaseRepository? _onboardingCases;
        private IDocumentRequirementRepository? _documentRequirements;
        private IDocumentRepository? _documents;
        private IMemberLedgerRepository? _memberLedgers;

        public UnitOfWork(MonyLoopDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IOnboardingCaseRepository OnboardingCases
            => _onboardingCases ??= new OnboardingCaseRepository(_dbcontext);

        public IDocumentRequirementRepository DocumentRequirements
            => _documentRequirements ??= new DocumentRequirementRepository(_dbcontext);

        public IDocumentRepository Documents
            => _documents ??= new DocumentRepository(_dbcontext);

        public IMemberLedgerRepository MemberLedgers
            => _memberLedgers ??= new MemberLedgerRepository(_dbcontext);

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
