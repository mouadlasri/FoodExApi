using System;
using System.Collections.Generic;
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

        // GET: api/Restaurants/{id}/ItemCategories
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

        // POST: api/Restaurants
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

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
