using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace IntegrationApi
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
            services.Configure<IntegrationApiSettings>(Configuration.GetSection("IntegrationApiSettings"));
            services.AddHttpClient<BackEndApiWithTokenPassThruClient>();
            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "IntegrationApi", Version = "v1"});
            });

            ConfigureAuthenticationForOnBehalfOfFlow(services);
            ConfigureAuthenticationForPassThruTokenScenario(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CheckForIncomingJwt",
                    builder => builder
                        .AddRequirements().RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(LookForButIgnoreJwtScheme.SchemeName));

                options.AddPolicy("OnBehalfOfFlow",
                    builder => builder
                        .AddRequirements().RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
            });
        }

        private void ConfigureAuthenticationForOnBehalfOfFlow(IServiceCollection services)
        {
            var intermediateSettings = new IntegrationApiSettings();
            Configuration.GetSection("IntegrationApiSettings").Bind(intermediateSettings);

            services
                .AddMicrosoftIdentityWebApiAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches()
                .AddDownstreamWebApi("BackEndApi", options =>
                {
                    options.BaseUrl = intermediateSettings.BackEndApiUri;
                    options.Scopes = $"api://{intermediateSettings.BackEndAppClientId}/Orders";
                });
        }

        private static void ConfigureAuthenticationForPassThruTokenScenario(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddScheme<LookForButIgnoreJwtSchemeOptions, LookForButIgnoreJwtScheme>(
                    LookForButIgnoreJwtScheme.SchemeName, null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IntegrationApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}