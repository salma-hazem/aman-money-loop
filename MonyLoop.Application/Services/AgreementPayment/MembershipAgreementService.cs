using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Interfaces.AgreementPayment;


namespace MonyLoop.Application.Services.AgreementPayment
{
    public class MembershipAgreementService : IMembershipAgreementService
    {
        private readonly IMembershipAgreementRepository _membershipAgreementRepository;
        private readonly IMapper _mapper;

        public MembershipAgreementService(
            IMembershipAgreementRepository membershipAgreementRepository,
            IMapper mapper)
        {
            _membershipAgreementRepository = membershipAgreementRepository;
            _mapper = mapper;
        }

        public Task<MembershipAgreementResponse> CreateAgreementAsync(
            CreateMembershipAgreementRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<MembershipAgreementResponse?> GetAgreementByIdAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            if (agreement.Status != AgreementStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending agreements can be accepted.");
            }

            agreement.Status = AgreementStatus.Accepted;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            // TODO:
            // SaveChangesAsync will be called through IUnitOfWork
            // once the shared UnitOfWork implementation is merged.

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }

        public async Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id)
        {
            var agreement =
                await _membershipAgreementRepository.GetByIdAsync(id);

            if (agreement is null)
                return null;

            if (agreement.Status != AgreementStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending agreements can be declined.");
            }

            agreement.Status = AgreementStatus.Declined;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            // TODO:
            // await _unitOfWork.SaveChangesAsync();
            // Add when the shared UnitOfWork implementation is merged.

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }
    }
}