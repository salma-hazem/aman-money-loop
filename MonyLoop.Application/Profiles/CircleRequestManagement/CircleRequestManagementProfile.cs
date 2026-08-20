using AutoMapper;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Application.Profiles.CircleRequestManagement;

public sealed class CircleRequestManagementProfile : Profile
{
    public CircleRequestManagementProfile()
    {
        CreateMap<CircleRequest, CircleRequestSummaryDto>();
        CreateMap<CircleRequest, CircleRequestResponseDto>();
        CreateMap<MarketplaceListing, MarketplaceListingResponseDto>();
        CreateMap<CircleSlot, CircleSlotResponseDto>();
        CreateMap<AuditLog, AuditLogResponseDto>();
        CreateMap<Circle, CircleResponseDto>()
            .ForMember(destination => destination.CircleTitle,
                options => options.MapFrom(source => source.CircleRequest == null
                    ? string.Empty
                    : source.CircleRequest.CircleTitle));
    }
}
