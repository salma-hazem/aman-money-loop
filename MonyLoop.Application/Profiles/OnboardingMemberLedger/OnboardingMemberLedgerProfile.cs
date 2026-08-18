using AutoMapper;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Profiles.OnboardingMemberLedger
{
    public class OnboardingMemberLedgerProfile:Profile
    {
        public OnboardingMemberLedgerProfile()
        {
            CreateMap<DocumentRequestDto, Document>();
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
