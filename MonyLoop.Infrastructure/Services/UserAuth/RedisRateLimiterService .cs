using MonyLoop.Application.ServicesAbstractions.UserAuth;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Services.UserAuth
{
    public class RedisRateLimiterService : IRateLimiterService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisRateLimiterService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> IsAllowedAsync(string key, TimeSpan window)
        {
            var db = _redis.GetDatabase();

            // NX = يحط القيمة بس لو المفتاح مش موجود أصلاً
            var wasSet = await db.StringSetAsync(key, "1", window, When.NotExists);

            // لو اتحط = مفيش طلب سابق، مسموح
            // لو معتحطش = فيه طلب حصل قبل كده لسه جوه الـ window، ممنوع
            return wasSet;
        }

        public async Task<bool> IsWithinLimitAsync(string key, int limit, TimeSpan window)
        {
            var db = _redis.GetDatabase();
            var count = await db.StringIncrementAsync(key);

            if (count == 1)
            {
                await db.KeyExpireAsync(key, window);
            }

            return count <= limit;
        }

        public async Task ResetAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
