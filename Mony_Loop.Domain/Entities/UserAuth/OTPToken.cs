using MonyLoop.Domain.Constants.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Entities.UserAuth
{
    public class OTPToken
    {
        public Guid OTPTokenId { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = null!;
        public OTPPurpose Purpose { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nav prop
        public ApplicationUser User { get; set; } = null!;
    }
}
