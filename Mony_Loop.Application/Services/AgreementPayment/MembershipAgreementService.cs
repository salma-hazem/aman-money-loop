using AutoMapper;
using Mony_Loop.Application.DTOs.AgreementPayment.MembershipAgreement;
using Mony_Loop.Application.ServicesAbstractions.AgreementPayment;
using Mony_Loop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;

namespace Mony_Loop.Application.Services.AgreementPayment
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

        public Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}