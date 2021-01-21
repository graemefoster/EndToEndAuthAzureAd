using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace ClientWebApp.Pages
{
    [Authorize(Policy = "Orders")]
    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:CalledApiScopes")]
    public class Orders : PageModel
    {
        private readonly IDownstreamWebApi _integrationWebApi;
        private readonly IOptions<ClientAppSettings> _settings;

        public Order[] OrdersList { get; set; }

        public Orders(IDownstreamWebApi integrationWebApi, IOptions<ClientAppSettings> settings)
        {
            _integrationWebApi = integrationWebApi;
            _settings = settings;
        }
        public async Task OnGet()
        {
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-web-app-call-api-call-api?tabs=aspnetcore
            OrdersList = await  _integrationWebApi.CallWebApiForUserAsync<Order[]>("Orders", options =>
            {
                options.RelativePath = "orders";
            });
        }
    }
}