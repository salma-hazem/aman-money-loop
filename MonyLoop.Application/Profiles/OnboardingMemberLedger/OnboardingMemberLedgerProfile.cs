using AutoMapper;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;

namespace MonyLoop.Application.Profiles.OnboardingMemberLedger
{
    public class OnboardingMemberLedgerProfile : Profile
    {
        public OnboardingMemberLedgerProfile()
        {
            CreateMap<DocumentRequestDto, Document>()
                .ForMember(d => d.DocumentId, o => o.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Status, o => o.MapFrom(_ => DocumentStatus.Pending))
                .ForMember(d => d.UploadedAt, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.ReviewedByUserId, o => o.Ignore())
                .ForMember(d => d.ReviewedAt, o => o.Ignore())
                .ForMember(d => d.RejectionReason, o => o.Ignore())
                .ForMember(d => d.OnboardingCase, o => o.Ignore())
                .ForMember(d => d.DocumentRequirement, o => o.Ignore())
                .ForMember(d => d.ReviewedByUser, o => o.Ignore());

            CreateMap<DocumentReviewRequestDto, Document>()
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => Enum.Parse<DocumentStatus>(s.NewStatus)))
                .ForMember(d => d.ReviewedAt,
                    o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.RejectionReason,
                    o => o.MapFrom(s => s.RejectionReason))
                .ForMember(d => d.DocumentId, o => o.Ignore())
                .ForMember(d => d.OnboardingCaseId, o => o.Ignore())
                .ForMember(d => d.DocumentRequirementId, o => o.Ignore())
                .ForMember(d => d.ReviewedByUserId, o => o.Ignore())
                .ForMember(d => d.FileName, o => o.Ignore())
                .ForMember(d => d.FilePath, o => o.Ignore())
                .ForMember(d => d.FileSize, o => o.Ignore())
                .ForMember(d => d.UploadedAt, o => o.Ignore())
                .ForMember(d => d.OnboardingCase, o => o.Ignore())
                .ForMember(d => d.DocumentRequirement, o => o.Ignore())
                .ForMember(d => d.ReviewedByUser, o => o.Ignore());

            CreateMap<Document, DocumentResponseDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<DocumentRequirement, DocumentRequirementResponseDto>();
            CreateMap<OnboardingCaseRequestDto, OnboardingCase>();
            CreateMap<OnboardingCase, OnboardingCaseResponseDto>();
            CreateMap<MemberLedgerRequestDto, MemberLedger>();
            CreateMap<MemberLedger, MemberLedgerResponseDto>();
        }
    }
}