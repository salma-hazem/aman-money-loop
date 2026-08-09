using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Onboarding___Member_Ledger
{
    public class DocumentRequirement
    {

        //        DocumentRequirement
        //Attribute   Type
        //Id  Guid
        //DocumentName    string
        //Description string?
        //IsRequired  bool
        //IsActive    bool
        //DisplayOrder    int
        public Guid DocumentRequirementId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        // Navigation Properties
        public ICollection<Document> Documents { get; set; } = new List<Document>();


    }
}
