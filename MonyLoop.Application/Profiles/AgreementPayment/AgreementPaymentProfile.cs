using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Domain.Entities.Agreement___Payment;

namespace MonyLoop.Application.Profiles.AgreementPayment
{
    public class AgreementPaymentProfile : Profile
    {
        public AgreementPaymentProfile()
        {
            CreateMap<MembershipAgreement, MembershipAgreementResponse>();
        }
    }
}