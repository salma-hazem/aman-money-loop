using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.UserAuth
{
    public interface IOTPTokenRepository
    {
        Task AddAsync(OTPToken otpToken, CancellationToken ct = default);
        Task<OTPToken?> GetLatestActiveAsync(Guid userId, OTPPurpose purpose, CancellationToken ct = default);
        Task InvalidateExistingTokensAsync(Guid userId, OTPPurpose purpose, CancellationToken ct = default);

    }
}
