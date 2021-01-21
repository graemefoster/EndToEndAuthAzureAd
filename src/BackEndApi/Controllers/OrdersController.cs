using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackEndApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "OrderPolicy")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOptions<BackEndSettings> _settings;

        public OrdersController(ILogger<OrdersController> logger, IOptions<BackEndSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            if (_settings.Value.User1Name == User.Claims.First(c => c.Type == ClaimTypes.Upn).Value)
            {
                return new List<Order>()
                {
                    new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdae"), "Order One - User One"),
                    new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdaf"), "Order Two - User One"),
                    new Order(Guid.Parse("d49cd5b2-6eb6-433e-82e0-7dfcde66fdaa"), "Order Three - User One"),
                };
            }

            if (_settings.Value.User2Name == User.Claims.First(c => c.Type == ClaimTypes.Upn).Value)
            {
                return new List<Order>()
                {
                    new Order(Guid.Parse("a49cd5b2-6eb6-433e-82e0-7dfcde66fdae"), "Order One - User Two"),
                    new Order(Guid.Parse("a49cd5b2-6eb6-433e-82e0-7dfcde66fdaf"), "Order Two - User Two"),
                    new Order(Guid.Parse("a49cd5b2-6eb6-433e-82e0-7dfcde66fdaa"), "Order Three - User Two"),
                };
            }

            return new List<Order>();
        }
    }
}