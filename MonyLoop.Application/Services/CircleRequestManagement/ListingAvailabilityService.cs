using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services.CircleRequestManagement;

public sealed class ListingAvailabilityService : IListingAvailabilityService
{
    private readonly IMarketplaceListingRepository _listingRepository;
    private readonly IMapper _mapper;

    public ListingAvailabilityService(
        IMarketplaceListingRepository listingRepository,
        IMapper mapper)
    {
        _listingRepository = listingRepository;
        _mapper = mapper;
    }

    public async Task<Result<MarketplaceListingResponseDto>> GetActiveListingAsync(
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingRepository.GetDetailsByIdAsync(listingId, cancellationToken);
        if (listing is null ||
            listing.ListingStatus != MarketplaceListingStatus.Active ||
            listing.Circle?.Status != CircleStatus.InRecruitment)
        {
            return CircleRequestErrors.ListingNotFound;
        }

        return _mapper.Map<MarketplaceListingResponseDto>(listing);
    }

    public async Task<bool> IsActiveAsync(
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        var result = await GetActiveListingAsync(listingId, cancellationToken);
        return result.IsSuccess;
    }
}
