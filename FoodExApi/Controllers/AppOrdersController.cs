using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodExApi.Models;

namespace FoodExApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppOrdersController : ControllerBase
    {
        private readonly FoodExContext _context;

        public AppOrdersController(FoodExContext context)
        {
            _context = context;
        }

        // GET: api/AppOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppOrder>>> GetAppOrder()
        {
            return await _context.AppOrder.ToListAsync();
        }

        // GET: api/AppOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppOrder>> GetAppOrder(int id)
        {
            var appOrder = await _context.AppOrder.FindAsync(id);

            if (appOrder == null)
            {
                return NotFound();
            }

            return appOrder;
        }

        // PUT: api/AppOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppOrder(int id, AppOrder appOrder)
        {
            if (id != appOrder.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(appOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AppOrders
        [HttpPost]
        public async Task<ActionResult<AppOrder>> PostAppOrder(AppOrder appOrder)
        {
            _context.AppOrder.Add(appOrder);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AppOrderExists(appOrder.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAppOrder", new { id = appOrder.OrderId }, appOrder);
        }

        // DELETE: api/AppOrders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AppOrder>> DeleteAppOrder(int id)
        {
            var appOrder = await _context.AppOrder.FindAsync(id);
            if (appOrder == null)
            {
                return NotFound();
            }

            _context.AppOrder.Remove(appOrder);
            await _context.SaveChangesAsync();

            return appOrder;
        }

        private bool AppOrderExists(int id)
        {
            return _context.AppOrder.Any(e => e.OrderId == id);
        }
    }
}
