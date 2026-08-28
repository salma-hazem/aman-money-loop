using MonyLoop.Application.Common;
using MonyLoop.Domain.Constants.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth
{
    public interface IOTPService
    {
        Task<Result> GenerateAndSendAsync(Guid userId, string email, string userName, OTPPurpose purpose, CancellationToken ct = default);
        Task<Result> VerifyAsync(Guid userId, string code, OTPPurpose purpose, CancellationToken ct = default);
    }
}
