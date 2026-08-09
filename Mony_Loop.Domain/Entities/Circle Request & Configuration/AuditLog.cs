using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Circle_Request___Configuration
{
    public class AuditLog
    {
        //        Attribute Type
        //Id Guid
        //PerformedByUserId   Guid(FK → User)
        //EntityType string
        //ActionType  string
        //OldStatus string?
        //NewStatus   string?
        //ActionDescription   string?
        //CreatedAt   DateTime
        public Guid AuditLogId { get; set; }
        public Guid PerformedByUserId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? ActionDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        //navigation property
        // public User? PerformedByUser { get; set; }


    }
}
