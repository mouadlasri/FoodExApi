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
    public class UserAnalyticsController : ControllerBase
    {
        private readonly FoodExContext db;

        public UserAnalyticsController(FoodExContext dbContext)
        {
            this.db = dbContext;
        }

        // [LineChart] Get all orders over time of user
        [HttpGet("{id}/ordersOverTime")]
        public ActionResult AnalyticsOrdersOverTime(int id)
        {
            var db = this.db;
          
            var numberOrdersOverTime = (from makeOrder in db.MakeOrder
                                        join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                                        join restaurant in db.Restaurant on appOrder.RestaurantId equals restaurant.RestaurantId
                                        where makeOrder.UserId == id
                                        select new
                                        {
                                            OrderId = appOrder.OrderId,
                                            RestaurantName = restaurant.RestaurantName,
                                            DateOrdered = makeOrder.DateOrdered
                                        }).ToList();


            if (numberOrdersOverTime != null)
            {
                return Ok(numberOrdersOverTime);
            }

            else
            {
                return BadRequest();
            }
        }

        // [PieChart] Get overall number of orders made by the user for each restaurnats
        [HttpGet("{id}/numberOrdersByRestaurant")]
        public ActionResult AnalyticsNumberOrdersByRestaurant(int id)
        {
            var db = this.db;

            var numberOrdersByRestaurant = (from orders in db.AppOrder
                                            join makeOrder in db.MakeOrder on orders.OrderId equals makeOrder.OrderId
                                            join restaurant in db.Restaurant on orders.RestaurantId equals restaurant.RestaurantId
                                            where makeOrder.UserId == id
                                            group restaurant by new { orders.RestaurantId, restaurant.RestaurantName }
                                            into grp
                                            select new
                                            {
                                                RestaurantId = grp.Key.RestaurantId,
                                                Name = grp.Key.RestaurantName,
                                                TotalOrders = grp.Count()
                                            }
                                            );


            if (numberOrdersByRestaurant != null)
            {
                return Ok(numberOrdersByRestaurant);
            }

            else
            {
                return BadRequest();
            }
        }


        // [PieChart] Get the Top 5 orders a user has made (all restaurants included)
        [HttpGet("{id}/topFiveOrders")]
        public ActionResult AnalyticsTopFiveOrders(int id)
        {
            var db = this.db;

            var topFiveOrders = (from orderDetails in db.OrderDetails
                                 join makeOrder in db.MakeOrder on orderDetails.OrderId equals makeOrder.OrderId
                                 join items in db.Item on orderDetails.ItemId equals items.ItemId
                                 join restaurant in db.Restaurant on items.RestaurantId equals restaurant.RestaurantId
                                 where makeOrder.UserId == id
                                 group restaurant by new { orderDetails.ItemId, items.Name, restaurant.RestaurantName }
                                            into grp
                                 orderby grp.Count()  descending 
                                 select new
                                    {
                                         ItemId = grp.Key.ItemId,
                                         Name = grp.Key.Name,
                                         RestaurantName = grp.Key.RestaurantName,
                                         QuantityOrdered = grp.Count()
                                    }
                                 ).Take(5); // take top 5 items


            if (topFiveOrders != null)
            {
                return Ok(topFiveOrders);
            }

            else
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}/ExpensesOverMonths")]
        public ActionResult AnalyticsExpensesOverMonths(int id)
        {
            var db = this.db;

            var expenses = (from makeOrders in db.MakeOrder
                            group makeOrders by new { makeOrders.DateOrdered.Value.Month } into grouping
                            select new
                            {
                                DateOrdered = grouping.Key.Month,
                                CountOrders = grouping.Count()
                            });

             //var groups = db.MakeOrder.AsEnumerable().GroupBy(item => item.DateOrdered.Value.Month);
            return Ok(expenses);
            //if (topFiveOrders != null)
            //{
            //    return Ok(topFiveOrders);
            //}

            //else
            //{
            //    return BadRequest();
            //}
        }
    }
}