﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class RestaurantsController : ControllerBase
    {
        readonly FoodExContext db;

        public RestaurantsController(FoodExContext dbContext)
        {
            this.db = dbContext;
        }

        // GET: api/Restaurants
        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> Get()
        {
            var db = this.db;

            List<Restaurant> restaurantLists = db.Restaurant.ToList();

            return Ok(restaurantLists);

        }

        // GET: api/Restaurants/5
        // Returns all menu items of a specific restaurant
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            var db = this.db;

            var restaurantItems = db.Item.Join(db.ItemCategory,
                item => item.Category,
                itemCategory => itemCategory.CategoryId,
                (item, itemCategory) => new
                {
                    ItemId = item.ItemId,
                    ItemName = item.Name,
                    ItemCategoryId = itemCategory.CategoryId,
                    ItemCategoryName = itemCategory.CategoryName,
                    ItemPrice = item.Price
        
                }).ToList();

            return Ok(restaurantItems);
        }

        // GET: api/Restaurants/{id}/ItemCategories
        // Return all unique categories of a specific restaurants with their names
        [HttpGet("{id}/ItemCategories")]
        public IActionResult GetItemCategories(int id)
        {
            var db = this.db;

            var restaurantItems = (from item in db.Item
                        join itemCategory in db.ItemCategory
                        on item.Category equals itemCategory.CategoryId
                        where item.RestaurantId == id
                        select new
                        {
                            CategoryName = itemCategory.CategoryName,
                            ItemId = item.ItemId,
                            RestaurantId = item.RestaurantId,
                            ItemName = item.Name,
                            ItemPrice = item.Price,
                            ItemImage = item.ItemImage,
                            ItemDescription = item.ItemDescription,
                            ItemWaitingTime = item.WaitingTime
                        });

            if (restaurantItems != null)
            {
                return Ok(restaurantItems);
            }
            else
            {
                return BadRequest();
            }
        }

        // GET: api/Restaurants/filter/{id}/ItemCategories
        // Return all menu items of a specific category of a specific resetaurant
        [HttpGet("filter/{id}/{categoryId}")]
        public ActionResult RestaurantCategoryItems(int id, int categoryId)
        {
            var db = this.db;

            var items = (from item in db.Item
                                   join restaurant in db.Restaurant on item.RestaurantId equals restaurant.RestaurantId
                                   where restaurant.RestaurantId == id && item.Category == categoryId
                                   select new
                                   {
                                       ItemId = item.Category,
                                       RestaurantId = restaurant.RestaurantId,
                                       ItemName = item.Name,
                                       ItemPrice = item.Price
                                   }).ToList();

            if (items != null)
            {
                return Ok(items);
            }
            else
            {
                return BadRequest();
            }
            
        }

        // GET: api/Restaurants/{restaurantId}/{orderStatus}
        // Get orders of specific status
        [HttpGet("ListOrders/{restaurantId}/{orderStatus}")]
        public IActionResult ListOrders(int restaurantId, int orderStatus)
        {
            var db = this.db;
           

            // Get the list of order items made by users with their respective order status and user information
            var listOrders = (from appOrder in db.AppOrder
                              join makeOrder in db.MakeOrder on appOrder.OrderId equals makeOrder.OrderId
                              join appUser in db.AppUser on makeOrder.UserId equals appUser.UserId
                              where appOrder.OrderStatus == orderStatus && appOrder.RestaurantId == restaurantId
                              orderby makeOrder.DateOrdered descending
                              let orderId = appOrder.OrderId
                              select new
                              {
                                  OrderId = appOrder.OrderId,
                                  RestaurantId = appOrder.RestaurantId,
                                  CommentText = appOrder.CommentText,
                                  Cancelled = appOrder.Cancelled,
                                  OrderStatus = appOrder.OrderStatus,
                                  UserId = appUser.UserId,
                                  FirstName = appUser.FirstName,
                                  LastName = appUser.LastName,
                                  DateOrdered = makeOrder.DateOrdered,
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
            
            if(listOrders != null)
            {
                return Ok(listOrders);
            }
            else
            {
                return BadRequest();
            }
        }


        // POST: api/Restaurants/order
        // POST confirm the order of the user and add it to the database
        [HttpPost("order")]
        public IActionResult ConfirmOrder(ConfirmOrder order)
        {   
            // To return a status code of 400, all three DB Inserts need to be successfull
            // Thus the try/catch statement after each change save
            var db = this.db;

            // First, create a new order with the user id in the MakeOrder table
            MakeOrder newOrder = new MakeOrder
            {
                UserId = int.Parse(order.UserId)
                // The OrderId is an Identity field, will be filled automatically
                // The DateOrdered will be automatically filled with SQL trigger
            };

            db.Add<MakeOrder>(newOrder);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }

            // Second, populate the others tables: app_order (1)
            int orderId = newOrder.OrderId;

            AppOrder appOrder = new AppOrder
            {
                OrderId = orderId,
                RestaurantId = int.Parse(order.RestaurantId),
                OrderStatus = 1
            };

            db.Add<AppOrder>(appOrder);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
            // Third, populate the others tables: order_details (1) with individual items

            foreach (CartItem item in order.ItemsList)
            {
                // Create a new instance of MakeOrder object
                OrderDetails orderDetails = new OrderDetails
                {
                    OrderId = orderId,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                };
                db.Add<OrderDetails>(orderDetails);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }

            Console.WriteLine("Order Id just inserted => ", orderId);
            
            Console.WriteLine("Order => ", order);


            return Ok(orderId);
        }


        [HttpPost("CompleteOrder")]
        public IActionResult CompleteOrder(int orderId)
        {
            var db = this.db;
            var appOrder = db.AppOrder.SingleOrDefault(order => order.OrderId == orderId);

            //return Ok(appOrder);
            // if order exists
            if (appOrder != null)
            {
                // Update the status of the order with that id
                appOrder.OrderStatus = 2;

                // Save changes
                db.SaveChanges();

                return Ok(orderId);
            }
            else
            {
                return BadRequest();

            }
        }


        // POST: api/Restaurants/{restaurantId}/DeliverOrder
        [HttpPost("{restaurantId}/DeliverOrder")]
        public IActionResult DeliverOrder(int orderId)
        {
            var db = this.db;
            var appOrder = db.AppOrder.SingleOrDefault(order => order.OrderId == orderId);

            //return Ok(appOrder);
            // if order exists
            if (appOrder != null)
            {
                // Update the status of the order with that id
                appOrder.OrderStatus = 3;

                // Save changes
                db.SaveChanges();

                return Ok(orderId);
            }
            else
            {
                return BadRequest();
            }
        }


        // GET: api/Restaurants/{restaurantId}/users
        // Get overall data about users in Sidebar users
        [HttpGet("{restaurantId}/users")]
        public IActionResult GetUsersData(int restaurantId)
        {
            var db = this.db;

            //SELECT app_user.user_id, app_user.first_name, app_user.last_name, app_user.phone_number, app_user.date_joined, COUNT(make_order.order_id) AS 'TotalOrders'
            //FROM make_order
            //INNER JOIN app_order ON make_order.order_id = app_order.order_id
            //INNER JOIN app_user ON make_order.user_id = app_user.user_id
            //WHERE restaurant_id = 1
            //GROUP BY app_user.user_id, app_user.first_name, app_user.last_name, app_user.phone_number, app_user.date_joined;

            var results = (from makeOrder in db.MakeOrder
                           join appOrder in db.AppOrder on makeOrder.OrderId equals appOrder.OrderId
                           join appUser in db.AppUser on makeOrder.UserId equals appUser.UserId
                           where appOrder.RestaurantId == restaurantId
                           orderby makeOrder.DateOrdered
                           group makeOrder by new { appUser.UserId, appUser.FirstName, appUser.LastName, appUser.PhoneNumber, appUser.DateJoined }
                           into grp
                           select new
                           {
                                UserId = grp.Key.UserId,
                                FirstName = grp.Key.FirstName,
                                LastName = grp.Key.LastName,
                                PhoneNUmber = grp.Key.PhoneNumber,
                                DateJoined = grp.Key.DateJoined,
                                TotalOrders = grp.Count(),
                           }
                          );

            if(results != null)
            {
                return Ok(results);
            }
            else
            {
                return BadRequest();
            }
        }


        // PUT: api/Restaurants/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
