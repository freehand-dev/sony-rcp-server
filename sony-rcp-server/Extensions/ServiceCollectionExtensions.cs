using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sony_rcp_server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure CorsPolicy (AllowAnyOrigin, AllowAnyMethod, AllowAnyHeader)
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        /// <summary>
        /// Register the Swagger generator, defining 1 or more Swagger documents
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SONY RCP Server API",
                    Description = "SONY virtual RCP server",
                    TermsOfService = new Uri("https://freehand.com.ua/projects/sony-rcp-server/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Oleksandr Nazaruk",
                        Email = "mail@freehand.com.ua",
                        Url = new Uri("https://github.com/freehand-dev"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://freehand.com.ua/projects/sony-rcp-server/license"),
                    }
                });
            });
        }



    }
}
