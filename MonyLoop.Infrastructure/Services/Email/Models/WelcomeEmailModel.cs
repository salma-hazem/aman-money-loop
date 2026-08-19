using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Services.Email.Models
{
    public class WelcomeEmailModel
    {
        public string UserName { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string TemporaryPassword { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
    }
}
