using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationApi
{
    public class LookForButIgnoreJwtSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class LookForButIgnoreJwtScheme : AuthenticationHandler<LookForButIgnoreJwtSchemeOptions>
    {
        public static string SchemeName = "LookForButIgnoreJwtScheme";

        public LookForButIgnoreJwtScheme(IOptionsMonitor<LookForButIgnoreJwtSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var jwt = Request.Headers["Authorization"][0];
            var token = jwt.Substring("Bearer ".Length);

            //validate the token. This example is _very_ simple and full of security holes. Motorola will be 
            //able to tell you what validations they are doing.
            var validateResult = new JsonWebTokenHandler().ValidateToken(token, new TokenValidationParameters());

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme)),
                JwtBearerDefaults.AuthenticationScheme)));
        }
    }
}