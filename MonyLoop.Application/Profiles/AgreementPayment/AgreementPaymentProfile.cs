using AutoMapper;
<<<<<<< Updated upstream:MonyLoop.Application/Profiles/AgreementPayment/AgreementPaymentProfile.cs
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Domain.Entities.Agreement___Payment;
=======
using Mony_Loop.Application.DTOs.AgreementPayment.MembershipAgreement;
using Mony_Loop.Application.DTOs.AgreementPayment.PaymentTransaction;
using Mony_Loop.Domain.Entities.Agreement___Payment;
>>>>>>> Stashed changes:Mony_Loop.Application/Profiles/AgreementPayment/AgreementPaymentProfile.cs

namespace MonyLoop.Application.Profiles.AgreementPayment
{
    public class AgreementPaymentProfile : Profile
    {
        public AgreementPaymentProfile()
        {
            CreateMap<MembershipAgreement, MembershipAgreementResponse>();

            CreateMap<PaymentTransaction, PaymentTransactionResponse>()
                .ForMember(
                    dest => dest.TransactionType,
                    opt => opt.MapFrom(src => src.TransactionType.ToString()))
                .ForMember(
                    dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(
                    dest => dest.TransactionStatus,
                    opt => opt.MapFrom(src => src.TransactionStatus.ToString()));

            CreateMap<PaymentTransaction, PaymentReceiptResponse>()
            .ForMember(
                dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()));
                }
    }
}