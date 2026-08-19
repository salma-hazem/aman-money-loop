using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger
{
    public class DocumentRequirementRepository : GenericRepository<DocumentRequirement>, IDocumentRequirementRepository
    {
        private readonly MonyLoopDbContext _dbcontext;

        public DocumentRequirementRepository(MonyLoopDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<IEnumerable<DocumentRequirement>> GetActiveOrderedAsync(CancellationToken ct = default)
        {
            return await _dbcontext.DocumentRequirements
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(ct);

        }

        public async Task<IEnumerable<DocumentRequirement>> GetRequiredOnlyAsync(CancellationToken ct = default)
        {
            return await _dbcontext.DocumentRequirements
                .Where(x => x.IsRequired && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
