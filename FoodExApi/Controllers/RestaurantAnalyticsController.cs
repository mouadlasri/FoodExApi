using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FoodExApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET api/RestaurantAnalytics/{id}     id = RestaurantId
        // Get Count of all order categories of the last 24 hours
        [HttpGet("{id}/daily")]
        public ActionResult GetDailyOrders(int restaurantId)
        {
            var db = this.db;

            // Getting the daily revenues
            var dailyRevenuesResults = (from orderDetails in db.OrderDetails
                           join appOrder in db.AppOrder on orderDetails.OrderId equals appOrder.OrderId
                           join item in db.Item on orderDetails.ItemId equals item.ItemId
                           join makeOrder in db.MakeOrder on appOrder.OrderId equals makeOrder.OrderId
                           where appOrder.RestaurantId == restaurantId
                           && makeOrder.DateOrdered.Value.Day == 29  // replace by DateTime.Now.Day
                           && makeOrder.DateOrdered.Value.Month == 3 // replace by DateTime.Now.Month
                           && makeOrder.DateOrdered.Value.Year == 2020 // replace by DateTime.Now.Year
                           select new
                           {
                               RestaurantId = appOrder.RestaurantId,
                               Quantity = orderDetails.Quantity,
                               Price = item.Price
                           }).ToList();

            decimal dailyRevenues = 0;
            foreach (var item in dailyRevenuesResults)
            {
                dailyRevenues += item.Quantity.Value * item.Price.Value;
            }

            // Getting the daily orders received
            var dailyOrdersReceived = (from makeOrder in db.MakeOrder
                                       join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                                       where appOrder.RestaurantId == restaurantId
                                       && makeOrder.DateOrdered.Value.Day == 29 // replace by DateTime.Now.Day
                                       && makeOrder.DateOrdered.Value.Month == 3 // replace by DateTime.Now.Month
                                       && makeOrder.DateOrdered.Value.Year == 2020 // replace by DateTime.Now.Year
                                       select appOrder
                                       ).Count();

            // Getting the unique daily users 
            var dailyUniqueUsers = (from makeOrder in db.MakeOrder
                                    join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                                    where appOrder.RestaurantId == restaurantId
                                    && makeOrder.DateOrdered.Value.Day == 29 // replace by DateTime.Now.Day
                                    && makeOrder.DateOrdered.Value.Month == 3 // replace by DateTime.Now.Month
                                    && makeOrder.DateOrdered.Value.Year == 2020 // replace by DateTime.Now.Year
                                    select makeOrder.UserId
                                    ).Distinct().Count();

            // Getting the total revenues of restaurant
            var totalRevenuesResults = (from orderDetails in db.OrderDetails
                                 join appOrder in db.AppOrder on orderDetails.OrderId equals appOrder.OrderId
                                 join item in db.Item on orderDetails.ItemId equals item.ItemId
                                 join makeOrder in db.MakeOrder on appOrder.OrderId equals makeOrder.OrderId
                                 where appOrder.RestaurantId == restaurantId
                                 select new
                                 {
                                     RestaurantId = appOrder.RestaurantId,
                                     Quantity = orderDetails.Quantity,
                                     Price = item.Price
                                 }).ToList();

            decimal totalRevenues = 0;
            foreach (var item in totalRevenuesResults)
            {
                totalRevenues += item.Quantity.Value * item.Price.Value;
            }

            // Return anonymous type
            return Ok(new { DailyRevenues = dailyRevenues, DailyOrdersReceived = dailyOrdersReceived, DailyUniqueUsers = dailyUniqueUsers, TotalRevenues = totalRevenues });
            
        }
    }
}