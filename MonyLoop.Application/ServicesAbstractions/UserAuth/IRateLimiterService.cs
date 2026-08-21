using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth
{
    public interface IRateLimiterService
    {
        Task<bool> IsAllowedAsync(string key, TimeSpan window);
    }
}
