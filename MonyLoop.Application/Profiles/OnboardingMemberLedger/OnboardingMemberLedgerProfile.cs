using AutoMapper;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Profiles.OnboardingMemberLedger
{
    public class OnboardingMemberLedgerProfile : Profile
    {
        public OnboardingMemberLedgerProfile()
        {
            CreateMap<DocumentReviewRequestDto, Document>()
                    .ForMember(dest => dest.Status,
                        opt => opt.MapFrom(src => Enum.Parse<DocumentStatus>(src.NewStatus)))
                    .ForMember(dest => dest.ReviewedAt,
                               opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<DocumentReviewRequestDto, Document>();
            CreateMap<Document, DocumentResponseDto>();

            CreateMap<DocumentRequirement, DocumentRequirementResponseDto>();

            CreateMap<OnboardingCaseRequestDto, OnboardingCase>();
            CreateMap<OnboardingCase, OnboardingCaseResponseDto>();

            CreateMap<MemberLedgerRequestDto, MemberLedger>();
            CreateMap<MemberLedger, MemberLedgerResponseDto>();
        }
    }
}
