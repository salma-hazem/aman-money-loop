using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Services.Email
{
    public class RazorLightEmailRenderer : IEmailTemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorLightEmailRenderer()
        {
            var templatesPath = Path.Combine(AppContext.BaseDirectory, "Services", "Email", "Templates");

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templatesPath)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync<TModel>(string templateName, TModel model)
        {
            return await _engine.CompileRenderAsync($"{templateName}.cshtml", model);
        }
    }
}
