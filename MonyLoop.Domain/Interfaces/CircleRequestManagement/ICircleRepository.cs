using MonyLoop.Domain.Entities.CircleRequestManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.CircleRequestManagement
{

    public interface ICircleRepository
    {
        Task<Circle?> GetByIdAsync(
            Guid circleId,
            CancellationToken cancellationToken = default);

        Task<Circle?> GetDetailsByIdAsync(
            Guid circleId,
            CancellationToken cancellationToken = default);

        Task<Circle?> GetByRequestIdAsync(
            Guid requestId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsForRequestAsync(
            Guid requestId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Circle circle,
            CancellationToken cancellationToken = default);
    }
}