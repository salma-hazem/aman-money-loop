using AutoMapper;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;
using MonyLoop.Domain.Entities.Agreement___Payment;

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