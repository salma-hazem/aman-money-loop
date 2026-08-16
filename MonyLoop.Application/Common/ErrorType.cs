using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Common
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Unauthorized = 3,
        Forbidden = 4,
        InvalidCrendentials = 5
    }
}
