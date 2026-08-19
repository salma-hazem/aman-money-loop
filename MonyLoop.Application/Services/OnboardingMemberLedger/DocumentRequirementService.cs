using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services.OnboardingMemberLedger
{
    public class DocumentRequirementService : IDocumentRequirementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentRequirementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<DocumentRequirementResponseDto>>> GetActiveOrderedAsync(CancellationToken ct = default)
        {
            var requirements = await _unitOfWork.DocumentRequirements.GetActiveOrderedAsync(ct);

            if (requirements == null)
            {
                return (Result<IEnumerable<DocumentRequirementResponseDto>>)Enumerable.Empty<DocumentRequirementResponseDto>();
            }

            var responseDtos = _mapper.Map<IEnumerable<DocumentRequirementResponseDto>>(requirements);

            return (Result<IEnumerable<DocumentRequirementResponseDto>>)responseDtos;
        }

        public async Task<Result<IEnumerable<DocumentRequirementResponseDto>>> GetRequiredOnlyAsync(CancellationToken ct = default)
        {
            var requirements = await _unitOfWork.DocumentRequirements.GetRequiredOnlyAsync(ct);

            if (requirements == null)
            {
                return (Result<IEnumerable<DocumentRequirementResponseDto>>)Enumerable.Empty<DocumentRequirementResponseDto>();
            }

            var responseDtos = _mapper.Map<IEnumerable<DocumentRequirementResponseDto>>(requirements);

            return (Result<IEnumerable<DocumentRequirementResponseDto>>)responseDtos;
        }
    }
}