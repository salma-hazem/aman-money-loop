using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Services.Email
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync<TModel>(string templateName, TModel model);
    }
}
