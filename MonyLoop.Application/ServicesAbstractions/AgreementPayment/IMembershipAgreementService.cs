using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;


namespace MonyLoop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IMembershipAgreementService
    {
        Task<MembershipAgreementResponse> CreateAgreementAsync(
            CreateMembershipAgreementRequest request);

        Task<MembershipAgreementResponse?> GetAgreementByIdAsync(Guid id);

        Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id,string token);

        Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id,string token);

        Task<MembershipAgreementResponse?> GetAgreementForResponseAsync(Guid id, string token);
    }
}