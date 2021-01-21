using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntegrationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return new List<Order>
            {
                new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdae"), "Order One"),
                new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdaf"), "Order Two"),
                new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdaa"), "Order Three"),
            };
        }
    }
}