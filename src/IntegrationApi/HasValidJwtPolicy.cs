using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationApi
{
    public class HasValidJwtPolicy : AuthorizationHandler<HasValidJwtRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HasValidJwtPolicy(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HasValidJwtRequirement requirement)
        {
            var jwt = _contextAccessor.HttpContext!.Request.Headers["Authorization"][0];
            var token = jwt.Substring("Bearer ".Length);

            //validate the token. This example is _very_ simple and full of security holes. Motorola will be 
            //able to tell you what validations they are doing.
            var validateResult = new JsonWebTokenHandler().ValidateToken(token, new TokenValidationParameters());

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}