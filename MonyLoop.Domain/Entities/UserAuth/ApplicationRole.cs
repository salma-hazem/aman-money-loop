using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Entities.UserAuth
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public const string Admin = "Admin";
        public const string Organizer = "Organizer";
        public const string Member = "Member";
    }
}
