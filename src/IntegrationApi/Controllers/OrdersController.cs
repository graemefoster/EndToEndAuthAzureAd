using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntegrationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "CheckForIncomingJwt")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly BackEndApiWithTokenPassThruClient _apiClient;

        public OrdersController(ILogger<OrdersController> logger, BackEndApiWithTokenPassThruClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [HttpGet]
        public Task<IEnumerable<Order>> Get()
        {
            return _apiClient.List();
        }
    }
}