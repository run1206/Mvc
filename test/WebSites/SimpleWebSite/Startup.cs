
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace SimpleWebSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Example 1
            services
                .AddMvcCore()
                .AddAuthorization()
                .AddFormatterMappings(m => m.SetMediaTypeMappingForFormat("js", new MediaTypeHeaderValue("application/json")))
                .AddJsonFormatters(j => j.Formatting = Formatting.Indented);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }
    }
}
