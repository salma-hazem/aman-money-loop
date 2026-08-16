using Mony_Loop.Application.DTOs.AgreementPayment.MembershipAgreement;


namespace Mony_Loop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IMembershipAgreementService
    {
        Task<MembershipAgreementResponse> CreateAgreementAsync(
            CreateMembershipAgreementRequest request);

        Task<MembershipAgreementResponse?> GetAgreementByIdAsync(Guid id);

        Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id);

        Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id);
    }
}