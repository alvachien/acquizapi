
//#define USE_MICROSOFTAZURE
#define USE_ALIYUN
#if RELEASE
#undef DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace acquizapi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        internal static String DBConnectionString { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors();

            services.AddMvcCore()
                .AddJsonFormatters()
                ;

            DBConnectionString = Configuration.GetConnectionString("DefaultConnection");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder =>
#if DEBUG
                builder.WithOrigins(
                    "http://localhost:20000", // AC math exercies
                    "https://localhost:20000"
                    )
#elif USE_MICROSOFTAZURE
                builder.WithOrigins(
                    "http://acmathexercise.azurewebsites.net",
                    "https://acmathexercise.azurewebsites.net"
                    )
#elif USE_ALIYUN
                builder.WithOrigins(
                    "http://118.178.58.187:5230",
                    "https://118.178.58.187:5230"
                    )
#endif
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
#if DEBUG
                Authority = "http://localhost:41016",
#elif USE_MICROSOFTAZURE
                Authority = "http://acidserver.azurewebsites.net",
#elif USE_ALIYUN
                Authority = "http://118.178.58.187:5100",
#endif
                RequireHttpsMetadata = false,

                AllowedScopes = { "api.acquiz" },
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc();
        }
    }
}
