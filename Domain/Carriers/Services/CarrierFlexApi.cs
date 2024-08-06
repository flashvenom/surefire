using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Mantis.Shared.DataAccess;
using Mantis.Domain.Carriers.Models;
using Microsoft.AspNetCore.Identity;

namespace Mantis.Server.Controllers
{
    [Route("bpm/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        //private readonly OrderService _orderService;

        //public OrderController(OrderService orderService)
        //{
        //    _orderService = orderService;
        //}
        OrderDataAccessLayer db = new OrderDataAccessLayer();

        [HttpGet]
        public object Get()
        {
            IQueryable<Carrier> data = db.GetAllOrders().AsQueryable();
            var count = data.Count();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                return new { Items = data.Skip(skip).Take(top), Count = count };
            }
            else
            {
                return data;
            }
        }

        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        public void Post([FromBody] Carrier carrier)
        {

            Random rand = new Random();
            db.AddOrder(carrier);

        }
        [HttpPut]
        public object Put([FromBody] Carrier carrier)
        {
            db.UpdateOrder(carrier);
            return carrier;
        }
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            db.DeleteOrder(id);
        }
        //[HttpPost]
        //public void Post([FromBody] Carrier Carrier)
        //{

        //    Random rand = new Random();

        //    _orderService.AddOrder(Carrier);

        //}
        //[HttpPut]
        //public object Put([FromBody] Carrier Carrier)
        //{
        //    string test = "eee";
        //    return Carrier;
        //    //_orderService.UpdateOrder(Carrier);
        //    //return Carrier;
        //}
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{

        //    _orderService.DeleteOrder(id);
        //    return id;

        //}
    }
}
