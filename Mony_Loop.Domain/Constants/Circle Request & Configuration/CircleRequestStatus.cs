using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Constants
{
    public static class CircleRequestStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string ModificationRequested = "ModificationRequested";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public const string Fulfilled = "Fulfilled";
    }
}
