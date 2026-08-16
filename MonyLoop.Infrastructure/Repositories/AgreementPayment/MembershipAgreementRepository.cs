using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Infrastructure.Data;

namespace MonyLoop.Infrastructure.Repositories.AgreementPayment
{
    public class MembershipAgreementRepository
        : IMembershipAgreementRepository
    {
        private readonly MonyLoopDbContext _context;

        public MembershipAgreementRepository(
            MonyLoopDbContext context)
        {
            _context = context;
        }

        public async Task<MembershipAgreement?> GetByIdAsync(
            Guid agreementId,
            CancellationToken cancellationToken = default)
        {
            return await _context.MembershipAgreements
                .FirstOrDefaultAsync(
                    x => x.MembershipAgreementId == agreementId,
                    cancellationToken);
        }

        public async Task<MembershipAgreement?> GetByMembershipApplicationIdAsync(
            Guid membershipApplicationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.MembershipAgreements
                .FirstOrDefaultAsync(
                    x => x.MembershipApplicationId == membershipApplicationId,
                    cancellationToken);
        }

        public async Task<bool> ExistsForMembershipApplicationAsync(
            Guid membershipApplicationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.MembershipAgreements
                .AnyAsync(
                    x => x.MembershipApplicationId == membershipApplicationId,
                    cancellationToken);
        }

        public async Task AddAsync(
            MembershipAgreement agreement,
            CancellationToken cancellationToken = default)
        {
            await _context.MembershipAgreements
                .AddAsync(agreement, cancellationToken);
        }

        public void Update(MembershipAgreement agreement)
        {
            _context.MembershipAgreements.Update(agreement);
        }
    }
}