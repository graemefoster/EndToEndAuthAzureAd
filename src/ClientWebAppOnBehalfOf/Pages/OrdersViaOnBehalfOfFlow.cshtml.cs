using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace ClientWebAppOnBehalfOf.Pages
{
    [Authorize(Policy = "Orders")]
    [AuthorizeForScopes(ScopeKeySection = "IntegrationApi:CalledApiScopes")]
    public class OrdersViaOnBehalfOfFlow : PageModel
    {
        private readonly IDownstreamWebApi _integrationWebApi;

        public Order[] OrdersList { get; set; }

        public OrdersViaOnBehalfOfFlow(IDownstreamWebApi integrationWebApi)
        {
            _integrationWebApi = integrationWebApi;
        }
        public async Task OnGet()
        {
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-web-app-call-api-call-api?tabs=aspnetcore
            OrdersList = await  _integrationWebApi.CallWebApiForUserAsync<Order[]>("OrdersViaOnBehalfOf", options =>
            {
                options.RelativePath = "OrdersOnBehalfOf";
            });
        }
    }
}