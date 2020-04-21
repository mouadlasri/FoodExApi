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
        [HttpGet("{restaurantId}/daily")]
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

            decimal dailyRevenue = 0;
            foreach (var item in dailyRevenuesResults)
            {
                dailyRevenue += item.Quantity.Value * item.Price.Value;
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

            decimal totalRevenue = 0;
            foreach (var item in totalRevenuesResults)
            {
                totalRevenue += item.Quantity.Value * item.Price.Value;
            }

            // Return anonymous type
            return Ok(new { DailyRevenue = dailyRevenue, DailyOrdersReceived = dailyOrdersReceived, DailyUniqueUsers = dailyUniqueUsers, TotalRevenue = totalRevenue });
            
        }


        [HttpGet("{restaurantId}/recentOrders")]
        public ActionResult GetRecentOrders(int restaurantId)
        {
            var db = this.db;

            var recentOrders = (from makeOrder in db.MakeOrder
                                join appUser in db.AppUser on makeOrder.UserId equals appUser.UserId
                                join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                                where appOrder.RestaurantId == restaurantId
                                orderby makeOrder.DateOrdered descending
                                select new
                                {
                                    OrderId = makeOrder.OrderId,
                                    FirstName = appUser.FirstName,
                                    LastName = appUser.LastName,
                                    UserId = appUser.UserId,
                                    DateOrdered = makeOrder.DateOrdered.Value.ToString("MM-dd-yyyy HH:mm:ss"),
                                    OrderStatus = appOrder.OrderStatus
                                }).Take(6);

            if(recentOrders != null)
            {
                return Ok(recentOrders);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{restaurantId}/topCategories")]
        public ActionResult GetTopCategories(int restaurantId)
        {
            var db = this.db;
            //SELECT TOP 5 order_details.item_id, SUM(order_details.quantity) AS 'Total Ordered' FROM order_details
            //INNER JOIN item ON order_details.item_id = item.item_id
            //WHERE item.restaurant_id = 1
            //GROUP BY(order_details.item_id)
            //ORDER BY 'Total Ordered' DESC;

            var topItemsCategories = (from item in db.Item 
                                      join orderDetails in db.OrderDetails on item.ItemId equals orderDetails.ItemId
                                      where item.RestaurantId == restaurantId
                                      group orderDetails by new {item.ItemId, item.Name}  into grp
                                      orderby grp.Sum(item => item.Quantity)
                                      select new
                                      {
                                          ItemId = grp.Key.ItemId,
                                          ItemName = grp.Key.Name,
                                          TotalOrdered = grp.Sum(item => item.Quantity)
                                      }
                                     ).Take(5);

            if(topItemsCategories != null)
            {
                return Ok(topItemsCategories);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{restaurantId}/weeklyRevenue")]
        public ActionResult GetWeeklyRevenue(int restaurantId)
        {
            var db = this.db;

            var results = ( from makeOrder in db.MakeOrder
                            join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                            where appOrder.RestaurantId == restaurantId 
                            && makeOrder.DateOrdered.Value.Year == 2020 // replace by DateTime.Now.Year
                            group makeOrder by new { month = makeOrder.DateOrdered.Value.Month, year = makeOrder.DateOrdered.Value.Year } into grp
                            select new { month = string.Format("{0}", grp.Key.month, grp.Key.year), totalOrders = grp.Count() }
                            );

            
            //SELECT COUNT(order_id) AS 'Total Orders made', FORMAT(date_ordered, 'dddd') AS 'Day' FROM make_order
            //WHERE date_ordered >= DATEADD(day, -7, '2020-03-10')
            //GROUP BY FORMAT(date_ordered, 'dddd');
            return Ok(results);
        }

        [HttpGet("{restaurantId}/monthlyRevenues/{year}")]
        public ActionResult GetMonthlyRevenues(int restaurantId, int year)
        {
            return Ok();
        }

        // Get 4 most trending orders
        [HttpGet("{restaurantId}/trendingOrders")]
        public ActionResult GetTrendingOrders(int restaurantId)
        {
            var db = this.db;

            var trendingOrders = (from item in db.Item
                                      join orderDetails in db.OrderDetails on item.ItemId equals orderDetails.ItemId
                                      where item.RestaurantId == restaurantId
                                      group orderDetails by new { item.ItemId, item.Name, item.Price, item.ItemImage } into grp
                                      orderby grp.Sum(item => item.Quantity) descending
                                      select new
                                      {
                                          ItemId = grp.Key.ItemId,
                                          ItemName = grp.Key.Name,
                                          ItemPrice = grp.Key.Price,
                                          ItemImage = grp.Key.ItemImage,
                                          TotalOrdered = grp.Sum(item => item.Quantity),
                                          TotalRevenues = grp.Sum(item => item.Item.Price * item.Quantity)
                                      }
                                     ).Take(4);

            if (trendingOrders != null)
            {
                return Ok(trendingOrders);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}