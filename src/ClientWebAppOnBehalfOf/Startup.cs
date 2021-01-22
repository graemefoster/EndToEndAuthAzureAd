using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;

namespace ClientWebAppOnBehalfOf
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

            //.default allows the client to 'be honest' and show all the scopes the middle tier flow wants to use.
            //If you use this you DO need to add this scope as a required permission in the client's aad permission list. This results in a nicer experience for the end user
            var orderUsingOnBehalfOfScope = $"api://{settings.IntegrationApiClientId}/.default";

            //This is the scope defined by the middle tier. Using this doesn't show the scopes the middle tier will use on-behalf-of you, to the user.
            //If you use this you do not need to add this scope as a required permission in the client's aad permission list
            //var orderUsingOnBehalfOfScope = $"api://{settings.IntegrationApiClientId}/on-behalf-of";

            //Used for incremental consent on my razor page. 
            //https://github.com/AzureAD/microsoft-identity-web/wiki/Managing-incremental-consent-and-conditional-access
            Configuration["IntegrationApi:CalledApiScopes"] = orderUsingOnBehalfOfScope;

            services
                .AddMicrosoftIdentityWebAppAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(new[] {orderUsingOnBehalfOfScope})
                .AddDownstreamWebApi("OrdersViaOnBehalfOf", options =>
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