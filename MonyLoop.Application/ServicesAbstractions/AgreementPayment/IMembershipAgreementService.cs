using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;


namespace MonyLoop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IMembershipAgreementService
    {
        Task<MembershipAgreementResponse> CreateAgreementAsync(
            CreateMembershipAgreementRequest request,Guid organizerId);

        Task<MembershipAgreementResponse?> GetAgreementByIdAsync(Guid id,Guid requesterId,bool isAdmin);

        Task<MembershipAgreementResponse?> AcceptAgreementAsync(Guid id,string token);

        Task<MembershipAgreementResponse?> DeclineAgreementAsync(Guid id,string token);

        Task<MembershipAgreementResponse?> GetAgreementForResponseAsync(Guid id, string token);
    }
}