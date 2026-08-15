using AutoMapper;
using Mony_Loop.Application.DTOs.AgreementPayment.MembershipAgreement;
using Mony_Loop.Domain.Entities.Agreement___Payment;

namespace Mony_Loop.Application.Profiles.AgreementPayment
{
    public class AgreementPaymentProfile : Profile
    {
        public AgreementPaymentProfile()
        {
            CreateMap<MembershipAgreement, MembershipAgreementResponse>();
        }
    }
}