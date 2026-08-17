using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;

namespace MonyLoop.Application.Services.AgreementPayment
{
    public class MembershipAgreementService : IMembershipAgreementService
    {
        private readonly IMembershipAgreementRepository _membershipAgreementRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipAgreementService(
            IMembershipAgreementRepository membershipAgreementRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _membershipAgreementRepository = membershipAgreementRepository;
            _unitOfWork = unitOfWork;
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

            // Check if the agreement has expired
            if (agreement.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                agreement.Status = AgreementStatus.Expired;

                _membershipAgreementRepository.Update(agreement);

                await _unitOfWork.SaveChangesAsync();

                throw new InvalidOperationException(
                    "The agreement has expired and can no longer be accepted.");
            }

            agreement.Status = AgreementStatus.Accepted;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            await _unitOfWork.SaveChangesAsync();

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

            // Check if the agreement has expired
            if (agreement.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                agreement.Status = AgreementStatus.Expired;

                _membershipAgreementRepository.Update(agreement);

                await _unitOfWork.SaveChangesAsync();

                throw new InvalidOperationException(
                    "The agreement has expired and can no longer be declined.");
            }

            agreement.Status = AgreementStatus.Declined;
            agreement.RespondedAt = DateTime.UtcNow;

            _membershipAgreementRepository.Update(agreement);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipAgreementResponse>(agreement);
        }
    }
}