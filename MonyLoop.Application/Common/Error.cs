using MonyLoop.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Common
{
    public class Error
    {
        public String Code { get; }
        public String Description { get; }
        public ErrorType Type { get; }

        private Error(String code, String description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }


        #region Static Factory Method 
        public static Error Failure(String code = "General.Failure", String description = "General Failure Has Occured")
        {
            return new Error(code, description, ErrorType.Failure);
        }

        public static Error Validation(String code = "General.Validation", String description = "Validation Error Has Occured")
        {
            return new Error(code, description, ErrorType.Validation);
        }

        public static Error NotFound(String code = "General.NotFound", String description = "The Requested Resource Was Not Found")
        {
            return new Error(code, description, ErrorType.NotFound);
        }

        public static Error Unauthorized(String code = "General.Unauthorized", String description = "You are not authorized to access this resource.")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }
        public static Error Forbidden(String code = "General.Forbidden", String description = "You Do Not Have Permission To Access")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }

        public static Error InvalidCredentials(String code = "General.InvalidCredentials", String description = "General Failure Has Occured")
        {
            return new Error(code, description, ErrorType.InvalidCredentials);
        }



        #endregion

    }
}
