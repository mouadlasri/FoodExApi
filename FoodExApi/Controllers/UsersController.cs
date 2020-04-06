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
