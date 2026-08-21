using MonyLoop.Infrastructure.Data;
using MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyLoop.Domain.Interfaces.UserAuth;

namespace MonyLoop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MonyLoopDbContext _dbcontext;

        public IOnboardingCaseRepository OnboardingCases { get; }
        public IDocumentRequirementRepository DocumentRequirements { get; }
        public IDocumentRepository Documents { get; }
        public IMemberLedgerRepository MemberLedgers { get; }
        public IOTPTokenRepository OTPTokens { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(MonyLoopDbContext dbcontext,
            IOnboardingCaseRepository onboardingCases,
            IDocumentRequirementRepository documentRequirements,
            IDocumentRepository documents,
            IMemberLedgerRepository memberLedgers,
            IOTPTokenRepository oTPToken,
            IRefreshTokenRepository refreshTokens)
        {
            _dbcontext = dbcontext;
            OnboardingCases = onboardingCases;
            DocumentRequirements = documentRequirements;
            Documents = documents;
            MemberLedgers = memberLedgers;
            OTPTokens = oTPToken;
            RefreshTokens = refreshTokens;
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
