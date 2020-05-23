using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;
using CosmoAPI.Authorization;

namespace CosmoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly CosmoContext _context;

        public LocationsController(CosmoContext context)
        {
            _context = context;
        }

        // GET: api/Locations
        [HttpGet]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetLocation()
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            return Ok(_context.Location);
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetLocation([FromRoute] int id)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var location = await _context.Location.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        //GET: api/Locations/regcount=10
        [HttpGet("regcount={id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetRegionLocationCount([FromRoute] int id)
        {
            if(_context.Region.Find(id) == null)
            {
                return BadRequest("Invalid Region ID");
            }

            return Ok(_context.Location.AsNoTracking().Where(loc => loc.regionID == id 
            && 0 < _context.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count()).Count());
        }

        //GET: api/Locations/unassigncount
        [HttpGet("unassigncount")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetUnassignLocationCount()
        {
            return Ok(_context.Location.AsNoTracking().Where(loc => loc.regionID == null 
            && 0 < _context.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count()).Count());
        }

        // GET: api/Locations/regid=5/1
        /// <summary>
        /// Gets all locations associated with a Region given a Region ID.
        /// Returns a sublist of eight sequential locations
        /// </summary>
        /// <param name="id">Region ID to search for</param>
        /// <returns>List of locations assocaited with given Region ID</returns>
        [HttpGet("regid={id}/{count}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetRegionLocations([FromRoute] int id, [FromRoute] int count)
        {
            int numReturnRecs = 8;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_context.Region.Find(id) == null)
            {
                return BadRequest("Invalid Region ID");
            }

            //Gets Locations which have a matching Region ID,
            //and has at least one active subsciber associated with the location
            //List is ordered by CITY, then ADDRESS, then UNIT to account for multiple subscribers within a single dwelling
            //By using the passed in Count value, only 8 locations at a time will be passed back
            //on each request
            List<Location> locations =
                _context.Location.AsNoTracking()
                .Where(loc => loc.regionID == id
                && 0 < _context.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count())
                .OrderBy(loc => loc.city).ThenBy(loc => loc.address).ThenBy(loc => loc.unit)
                .Skip(numReturnRecs * (count - 1)).Take(numReturnRecs)
                .ToList();

            if (locations.Count <= 0)
            {
                return Ok("No active location records found");
            }

            //Returns 200 Code with list of Locations associated with the given Region ID and have at least one active subscriber
            return Ok(locations);
        }

        /// <summary>
        /// Gets all locations which do not have a Region ID associated with them
        /// Locations must have at least one active subscriber associated with them 
        /// to be returned by this method
        /// Returns a list of all unassigned locations
        /// </summary>
        /// <returns>List of unassigned locations</returns>\
        [HttpGet("unassigned/{count}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetUnassignedLocations([FromRoute] int count)
        {
            int numReturnRecs = 8;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Gets Locations which are unassigned (regionID == null)
            //and has at least one active subsciber associated with the location
            //List is ordered by CITY, then ADDRESS, then UNIT to account for multiple subscribers within a single dwelling
            //By using the passed in Count value, only 8 locations at a time will be passed back
            //on each request
            List<Location> locations =
                _context.Location.AsNoTracking()
                .Where(loc => loc.regionID == null
                && 0 < _context.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count())
                .OrderBy(loc => loc.city).ThenBy(loc => loc.address).ThenBy(loc => loc.unit)
                .Skip(numReturnRecs * (count - 1)).Take(numReturnRecs)
                .ToList();

            if(locations.Count <= 0)
            {
                return Ok("No active location records found");
            }
            //Returns 200 Code with list of Locations associated with the given Region ID and have at least one active subscriber

            return Ok(locations);
        }

        /// <summary>
        /// Updates an incoming list of locations with a new Region ID
        /// URL Route contains the Region ID to update to
        ///     If regID is 0 or less, locations will be assigned to a NULL region
        /// Incoming list contains the Location IDs to update
        /// </summary>
        /// <param name="idList">List of IDs related to the update</param>
        /// <returns></returns>
        [HttpPut("assign/{regID}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> AssignLocations([FromRoute] int regID, [FromBody] List<Location> locations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Checks if the regionID is greater than 0 and if it cannot be found
            if(regID > 0 && _context.Region.Find(regID) == null)
            {
                return BadRequest("Invalid Region ID");
            }
            
            foreach(Location loc in locations)
            {
                if (regID == 0)
                {
                    loc.regionID = null;
                }
                else
                {
                    if(regID < 0)
                    {
                        return BadRequest("Invalid Region ID");
                    }
                    else
                    {
                        loc.regionID = regID;
                    }
                    
                }
            }

            _context.UpdateRange(locations);
            _context.SaveChanges();
            return Ok(locations);
        }

        // POST: api/Locations
        [HttpPost]
        [Authorize(Roles.SUBSCRIBER)]
        public async Task<IActionResult> PostLocation([FromBody] Location location)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Location.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.locationID }, location);
        }



        /// <summary>
        /// This will take a location id in, Find the location Object Associated with it
        ///     Find the region that is part of the object, and call the DateCalculationMethod to retrive
        ///     the dates for the subscibers location. 
        /// NOTE: This will only return 3 becuase this is for the Subscriber, The subscriber will never need more or less than 3
        /// </summary>
        /// <param name="locationID">The location ID thats associated with a subscriber</param>
        /// <returns>3 Dates </returns>
        [HttpGet("locationID-c={locationID}")]
        [Authorize(Roles.SUBSCRIBER)]
        public IActionResult GetSubscriberDates([FromRoute] int locationID)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //Declare The Util method so we can call it later 
            DateCalculation dc = new DateCalculation();
            //Get all the locations from the DB
            Location location = _context.Location.FirstOrDefault(l => l.locationID == locationID);
            
            int regID;
			//This is to check to see if the subscribers location is assigned to a region 
            if(location.regionID != null)
            {
                //Set an int equal to the locations regionID
                regID = (int)location.regionID;
            }
            else
            {
				//Return OK here so we do not search and execute the rest of the code on a region that does not exist
                return Ok();
            }

            //Get all the Regions From the DB
            Region region = _context.Region.FirstOrDefault(r => r.regionID == regID);

            //Return the results of the Helper method along with OK as an indication of success     
            return Ok(dc.GetNextDates(3, region));
        }

        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.locationID == id);
        }


        /// <summary>
        /// This method will take in the token supplied by the HTTP request, unencrypt it and return the id if it is valid
        /// </summary>
        /// <param name="token">The token string to examine</param>
        /// <returns>The id number associated with the token, if valid, -1 otherwise</returns>
        private int ValidateToken(string token)
        {
            return 0;
        }
    }

}