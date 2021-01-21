using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IntegrationApi
{
    /// <summary>
    /// Grabs the token from the incoming requests and forwards it on to the back end
    /// In this implementation the token is completely opaque. The integration tier is not looking into it.
    /// Any checking of this token would be manual as it is not intended for this api to see.
    /// </summary>
    public class BackEndApiWithTokenPassThruClient
    {
        private readonly HttpClient _client;

        public BackEndApiWithTokenPassThruClient(
            HttpClient client, 
            IHttpContextAccessor contextAccessor,
            IOptions<IntegrationApiSettings> settings)
        {
            client.BaseAddress = new Uri(settings.Value.BackEndApiUri);
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(contextAccessor.HttpContext!.Request.Headers["Authorization"][0]);
            _client = client;
        }

        public Task<IEnumerable<Order>> List()
        {
            return _client.GetFromJsonAsync<IEnumerable<Order>>("Orders");
        }
    }
}