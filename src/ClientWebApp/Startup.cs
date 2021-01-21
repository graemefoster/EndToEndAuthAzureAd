using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;

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

            services.AddInMemoryTokenCaches();

            var orderScope = $"api://{settings.BackEndAppClientId}/Orders";
            var orderUsingOnBehalfOfScope = $"api://{settings.IntegrationApiClientId}/.default";

            //Used for incremental consent on my razor page. 
            //https://github.com/AzureAD/microsoft-identity-web/wiki/Managing-incremental-consent-and-conditional-access
            Configuration["DownstreamApi:CalledApiScopes"] = orderScope;
            Configuration["IntegrationApi:CalledApiScopes"] = orderUsingOnBehalfOfScope;

            services
                .AddMicrosoftIdentityWebAppAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddDownstreamWebApi("Orders", options =>
                {
                    options.Scopes = orderScope;
                    options.BaseUrl = settings.IntegrationApiUri;
                }).AddDownstreamWebApi("OrdersViaOnBehalfOf", options =>
                {
                    options.Scopes = orderUsingOnBehalfOfScope;
                    options.BaseUrl = settings.IntegrationApiUri;
                });

            //Needed for incremental consent.
            //https://github.com/AzureAD/microsoft-identity-web/wiki/Managing-incremental-consent-and-conditional-access
            services
                .AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            services.AddRazorPages();

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
}