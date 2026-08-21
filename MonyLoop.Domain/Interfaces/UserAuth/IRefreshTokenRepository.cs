using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.UserAuth
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    }
}
