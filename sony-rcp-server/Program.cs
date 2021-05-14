using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.IO;
using System.Reflection;
using System.Security.Authentication;

namespace sony_rcp_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PrintProductVersion();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                 .ConfigureAppConfiguration((builderContext, config) =>
                 {
                     config.Sources.Clear();
                     IHostEnvironment env = builderContext.HostingEnvironment;

                     #region WorkingDirectory
                     var workingDirectory = env.ContentRootPath;
                     if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                     {
                         workingDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "FreeHand", env.ApplicationName);
                     }
                     else if (Environment.OSVersion.Platform == PlatformID.Unix)
                     {
                         workingDirectory = System.IO.Path.Combine($"/opt/", env.ApplicationName, "etc", env.ApplicationName);
                     }
                     System.IO.Directory.CreateDirectory(workingDirectory);

                     config.SetBasePath(workingDirectory);

                     // add workingDirectory service configuration
                     config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                       {"WorkingDirectory", workingDirectory}
                    });
                     #endregion

                     //
                     Console.WriteLine($"$Env:EnvironmentName={ env.EnvironmentName }");
                     Console.WriteLine($"$Env:ApplicationName={ env.ApplicationName }");
                     Console.WriteLine($"$Env:ContentRootPath={ env.ContentRootPath }");
                     Console.WriteLine($"WorkingDirectory={ workingDirectory }");

                     config.AddIniFile($"{ env.ApplicationName }.conf", optional: true, reloadOnChange: true);
                     config.AddJsonFile($"{ env.ApplicationName }.json", optional: true, reloadOnChange: true);
                     config.AddCommandLine(args);
                     config.AddEnvironmentVariables();
                 })
                 .ConfigureLogging((builderContext, logging) =>
                 {
                     logging.AddConfiguration((IConfiguration)builderContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, serverOptions) =>
                    {
                        serverOptions.Configure((IConfiguration)context.Configuration.GetSection("Kestrel"))
                            .Endpoint("HTTPS", listenOptions =>
                            {
                                listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls12;
                            });
                    }).UseStartup<Startup>();
                });

        private static void PrintProductVersion()
        {
            var assembly = typeof(Program).Assembly;
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            Console.WriteLine($"Starting {product} v{version}...");
        }
    }
}
