using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodExApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodExApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantAnalyticsController : ControllerBase
    {
        private readonly FoodExContext db;

        public RestaurantAnalyticsController(FoodExContext dbContext)
        {
            this.db = dbContext;
        }

        // GET api/RestaurantsAnalytics/{id}     id = RestaurantId
        // Get Count of all order categories of the last 24 hours
        [HttpGet("{id}/daily")]
        public ActionResult GetDailyOrders(int restaurantId)
        {

            int ordersReceivedDaily = db.AppOrder.Where(appOrder => appOrder.RestaurantId == restaurantId).Count();

            var x = new
            {
                orderCompleted = 3,
                ordersReceived = 5
            };

            return Ok();
        }


    }
}