using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FoodExApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodExApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly FoodExContext db;

        // Constructor
        public UsersController(FoodExContext dbContext)
        {
            this.db = dbContext;
        }



        // GET api/Users
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {

            return Content("Hello");
            return new string[] { "value1", "value2" };
        }

        // GET api/Users/{id}
        // Get user profile data given the id
        [HttpGet("{id}")]
        public ActionResult<string> GetUser(int id)
        {
            var db = this.db;

            AppUser User = db.AppUser.SingleOrDefault(user => user.UserId == id);
            
            if(User != null)
            {
                return Ok(User);
            }
            else
            {
                return BadRequest();
            }
        }
        
        // POST api/Users
        [HttpPost]
        public ActionResult Login(Account credentials)
        {
            var db = this.db;

            Account User = db.Account.SingleOrDefault(user => user.UserId == credentials.UserId && user.UserPassword == credentials.UserPassword);
            Debug.WriteLine("USER => ", User);
            if (User != null)
            {
                return Ok(User);
            }
            else
            {
                return BadRequest("User does not exist");
            }
        }


        // GET api/Users/{id}/OrdersPending
        // Get pending orders of user
        [HttpGet("{id}/OrdersPending")]
        public ActionResult OrdersPending(int id)
        {
            var db = this.db;

            int ordersPending = db.AppOrder.Where(appOrder => appOrder.OrderStatus == 1).Count(); // 1 = pending orders

            return Ok(ordersPending);
        }

        // GET api/Users/{id}/OrdersCompleted
        // Get completed orders of user
        [HttpGet("{id}/OrdersCompleted")]
        public ActionResult OrdersCompleted(int id)
        {
            var db = this.db;

            int ordersCompleted = db.AppOrder.Where(appOrder => appOrder.OrderStatus == 2).Count(); // 2 = completed orders

            return Ok(ordersCompleted);
        }

        // GET api/Users/{id}/OrdersCompleted
        // Get completed orders of user
        [HttpGet("{id}/DeliveredOrders")]
        public ActionResult DeliveredOrders(int id)
        {
            var db = this.db;

            int ordersCompleted = db.AppOrder.Where(appOrder => appOrder.OrderStatus == 3).Count(); // 2 = completed orders

            return Ok(ordersCompleted);
        }

        [HttpGet("{id}/ListOrdersCompleted")]
        public ActionResult ListOrdersCompleted(int id)
        {
            var db = this.db;


            // Get the list of order items made by users with their respective order status and user information
            var listOrders = (from makeOrder in db.MakeOrder
                              join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                              where makeOrder.UserId == 67887 && appOrder.OrderStatus == 2
                              let userId = makeOrder.UserId
                              let orderId = makeOrder.OrderId
                              select new
                              {
                                  OrderId = appOrder.OrderId,
                                  DateCompleted = appOrder.DateCompleted,
                                  // Get the list of items of this order
                                  ItemsOrdered = (from orderDetails in db.OrderDetails
                                                  join item in db.Item on orderDetails.ItemId equals item.ItemId
                                                  join appOrder in db.AppOrder on orderDetails.OrderId equals appOrder.OrderId
                                                  where orderDetails.OrderId == orderId
                                                  select new
                                                  {
                                                      ItemId = item.ItemId,
                                                      ItemName = item.Name,
                                                      ItemCategory = item.Category,
                                                      ItemPrice = item.Price,
                                                      ItemQuantity = orderDetails.Quantity,
                                                      ItemImage = item.ItemImage,
                                                  }
                                        )
                              }
                           );

            if (listOrders != null)
            {
                return Ok(listOrders);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/ListOrdersDelivered")]
        public ActionResult ListOrdersDelivered(int id)
        {
            var db = this.db;


            // Get the list of order items made by users with their respective order status and user information
            var listOrders = (from makeOrder in db.MakeOrder
                              join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                              where makeOrder.UserId == 67887 && appOrder.OrderStatus == 3
                              let userId = makeOrder.UserId
                              let orderId = makeOrder.OrderId
                              select new
                              {
                                  OrderId = appOrder.OrderId,
                                  DateCompleted = appOrder.DateCompleted,
                                  // Get the list of items of this order
                                  ItemsOrdered = (from orderDetails in db.OrderDetails
                                                  join item in db.Item on orderDetails.ItemId equals item.ItemId
                                                  join appOrder in db.AppOrder on orderDetails.OrderId equals appOrder.OrderId
                                                  where orderDetails.OrderId == orderId
                                                  select new
                                                  {
                                                      ItemId = item.ItemId,
                                                      ItemName = item.Name,
                                                      ItemCategory = item.Category,
                                                      ItemPrice = item.Price,
                                                      ItemQuantity = orderDetails.Quantity,
                                                      ItemImage = item.ItemImage,
                                                  }
                                        )
                              }
                           );

            if (listOrders != null)
            {
                return Ok(listOrders);
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}/ListOrdersPending")]
        public ActionResult ListOrdersPending(int id)
        {
            var db = this.db;


            // Get the list of order items made by users with their respective order status and user information
            var listOrders = (from makeOrder in db.MakeOrder
                              join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                              where makeOrder.UserId == 67887 && appOrder.OrderStatus == 1
                              let userId = makeOrder.UserId
                              let orderId = makeOrder.OrderId
                              select new
                              {
                                  OrderId = appOrder.OrderId,
                                  DateCompleted = appOrder.DateCompleted,
                                  // Get the list of items of this order
                                  ItemsOrdered = (from orderDetails in db.OrderDetails
                                                  join item in db.Item on orderDetails.ItemId equals item.ItemId
                                                  join appOrder in db.AppOrder on orderDetails.OrderId equals appOrder.OrderId
                                                  where orderDetails.OrderId == orderId
                                                  select new
                                                  {
                                                      ItemId = item.ItemId,
                                                      ItemName = item.Name,
                                                      ItemCategory = item.Category,
                                                      ItemPrice = item.Price,
                                                      ItemQuantity = orderDetails.Quantity,
                                                      ItemImage = item.ItemImage,
                                                  }
                                        )
                              }
                           );

            if (listOrders != null)
            {
                return Ok(listOrders);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("/CreateAccount")]
        public IActionResult CreateAccount(Account newAccount)
        {

            return Ok();
        }



        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
