using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonyLoop.Domain.Entities.UserAuth
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public bool MustChangePassword { get; set; }
        public Guid? RegisteredByAdminId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Nav props
        public ApplicationUser? RegisteredByAdmin { get; set; }
        public ICollection<OTPToken> OTPTokens { get; set; } = new List<OTPToken>();
    }
}
