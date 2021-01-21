using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.OpenApi.Models;

namespace ClientWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new ClientAppSettings();
            Configuration.GetSection("ClientAppSettings").Bind(settings);

            services.Configure<ClientAppSettings>(Configuration.GetSection("ClientAppSettings"));
            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddInMemoryTokenCaches();
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(new[] {$"api://{settings.BackEndAppClientId}/Orders"});

            services.AddAuthorization(options =>
            {
                //Policies are attached to controllers / actions using the [Authorization(Policy="...")] attribute
                options.AddPolicy("Orders", builder => builder.RequireAuthenticatedUser());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }

    public class ClientAppSettings
    {
        public string BackEndAppClientId { get; set; }
    }
}