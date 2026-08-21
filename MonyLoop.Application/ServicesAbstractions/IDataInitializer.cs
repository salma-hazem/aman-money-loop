using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IDataInitializer
    {
        Task InitializeAsync();
    }
}
