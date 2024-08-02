using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Mantis.Shared.DataAccess;
using Mantis.Domain.Carriers.Models;

namespace Mantis.Server.Controllers
{
    [Route("bpm/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IQueryable<Carrier> data = _orderService.GetOrdersAsync().Result.AsQueryable();
            var count = data.Count();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                return Ok(new { Items = data.Skip(skip).Take(top), Count = count });
            }
            else
            {
                return Ok(data);
            }
        }
        [HttpPost]
        public void Post([FromBody] Carrier Carrier)
        {

            Random rand = new Random();

            _orderService.AddOrder(Carrier);

        }
        [HttpPut]
        public object Put([FromBody] Carrier Carrier)
        {
            string test = "eee";
            return Carrier;
            //_orderService.UpdateOrder(Carrier);
            //return Carrier;
        }
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{

        //    _orderService.DeleteOrder(id);
        //    return id;

        //}
    }
}
