using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;

namespace CosmoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationRoutesController : ControllerBase
    {
        private readonly CosmoContext _context;

        public LocationRoutesController(CosmoContext context)
        {
            _context = context;
        }

        // GET: api/LocationRoutes
        [HttpGet]
        public IEnumerable<LocationRoute> GetLocationRoutes()
        {
            return _context.LocationRoute;
        }

        // GET: api/LocationRoutes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationRoute([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationRoute = await _context.LocationRoute.FindAsync(id);

            if (locationRoute == null)
            {
                return NotFound();
            }

            return Ok(locationRoute);
        }

        // GET: api/LocationRoutes
        [HttpGet("Active")]
        public IEnumerable<LocationRoute> GetActiveLocationRoutes()
        {
            return _context.LocationRoute.Include("route").Where(x => !x.route.completed);
        }

        // PUT: api/LocationRoutes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocationRoute([FromRoute] int id, [FromBody] LocationRoute locationRoute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != locationRoute.locationRouteID)
            {
                return BadRequest();
            }

            _context.Entry(locationRoute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationRouteExists(id))
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

        // POST: api/LocationRoutes
        [HttpPost]
        public async Task<IActionResult> PostLocationRoute([FromBody] LocationRoute locationRoute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LocationRoute.Add(locationRoute);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocationRoute", new { id = locationRoute.locationRouteID }, locationRoute);
        }

        // DELETE: api/LocationRoutes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationRoute([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationRoute = await _context.LocationRoute.FindAsync(id);
            if (locationRoute == null)
            {
                return NotFound();
            }

            _context.LocationRoute.Remove(locationRoute);
            await _context.SaveChangesAsync();

            return Ok(locationRoute);
        }

        private bool LocationRouteExists(int id)
        {
            return _context.LocationRoute.Any(e => e.locationRouteID == id);
        }
    }
}