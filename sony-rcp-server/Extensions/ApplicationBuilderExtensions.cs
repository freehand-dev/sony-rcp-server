using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sony_rcp_server.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureStaticFiles(this IApplicationBuilder app, string workDirectory)
        {

            // configure UI StaticFiles
            var _uiPath = System.IO.Path.Combine(workDirectory, "ui");
            // create UI Directory if not exists
            System.IO.Directory.CreateDirectory(_uiPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_uiPath),
                RequestPath = "/ui"
            });
        }
    }
}












