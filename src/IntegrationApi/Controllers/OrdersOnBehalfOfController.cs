using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "OnBehalfOfFlow")]
    public class OrdersOnBehalfOfController : ControllerBase
    {
        private readonly BackEndApiWithOnBehalfOfFlow _apiClient;

        public OrdersOnBehalfOfController(BackEndApiWithOnBehalfOfFlow apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public Task<IEnumerable<Order>> Get()
        {
            return _apiClient.List();
        }
    }
}