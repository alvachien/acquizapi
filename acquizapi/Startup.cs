
#if RELEASE
//#define USE_AZURE
#define USE_ALIYUN
#undef USE_AZURE
#else
#define DEBUG
#undef USE_ALIYUN
#undef USE_AZURE
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        internal static String DBConnectionString { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Add framework services.
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddAuthorization();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
#if DEBUG
                    options.Authority = "http://localhost:41016";
#else
#if USE_AZURE
                    options.Authority = "http://acidserver.azurewebsites.net";
#elif USE_ALIYUN
                    options.Authority = "http://118.178.58.187:5100";
#endif
#endif
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api.acquiz";
                    //options.AutomaticAuthenticate = true;
                    //options.AutomaticChallenge = true;
                });

#if DEBUG
            DBConnectionString = Configuration["ConnectionStrings:DebugConnection"];
#else
#if USE_ALIYUN
            DBConnectionString = Configuration["ConnectionStrings:AliyunConnection"];
#elif USE_AZURE
            DBConnectionString = Configuration["ConnectionStrings:AzureConnection"];
#endif
#endif
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseCors(builder =>
#if DEBUG
                builder.WithOrigins(
                    "http://localhost:20000", // AC math exercies
                    "https://localhost:20000"
                    )
#else
#if USE_MICROSOFTAZURE
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
#endif
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );

            app.UseMvc();
        }
    }
}
