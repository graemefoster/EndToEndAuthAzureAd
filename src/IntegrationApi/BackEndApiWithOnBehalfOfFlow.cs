using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationApi.Controllers;
using Microsoft.Identity.Web;

namespace IntegrationApi
{
    /// <summary>
    /// Grabs the token from the incoming requests and forwards it on to the back end
    /// In this implementation the token is completely opaque. The integration tier is not looking into it.
    /// Any checking of this token would be manual as it is not intended for this api to see.
    /// </summary>
    public class BackEndApiWithOnBehalfOfFlow
    {
        private readonly IDownstreamWebApi _downstreamWebApi;

        public BackEndApiWithOnBehalfOfFlow(IDownstreamWebApi downstreamWebApi)
        {
            _downstreamWebApi = downstreamWebApi;
        }

        public Task<IEnumerable<Order>> List()
        {
            return _downstreamWebApi.CallWebApiForUserAsync<IEnumerable<Order>>("BackEndApi", options => options.RelativePath = "Orders");
        }
    }
}