using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.UserAuth
{
    public class ConfirmOtpRequestDto
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
