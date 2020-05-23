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
    public class RegionsController : ControllerBase
    {
        private readonly CosmoContext _context;
        private DateCalculation dateCalculation;

        public RegionsController(CosmoContext context)
        {
            _context = context;
            dateCalculation = new DateCalculation();
        }

        // GET: api/Regions
        [HttpGet]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetRegion()
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //Checks if model state is valid - checks for errors coming from model binding and validation
            if (!ModelState.IsValid)
            {
                //Returns 400 status code with the model state as content
                return BadRequest(ModelState);
            }

            //Create and instance of the datecalculation 
            DateCalculation dc = new DateCalculation();
            //Gets a list of all Routes in the context table and appends a next collection date to the returned regions 
            var dbRegions = _context.Region.Where<Region>(e => e.inactive == false).Select(r => new { regionID = r.regionID, regionName = r.regionName, frequency = r.frequency, firstDate = r.firstDate, nextDate = dc.GetNextDates(1,r), inactive = r.inactive}).ToList();
            

            //If region list is null or contains no regions, return 404 Code with "No regions Found" message
            if (dbRegions == null || dbRegions.Count <= 0)
            {
                return NotFound("No regions found");
            }
            else
            {
                //Return 200 Code with Route List as content
                return Ok(dbRegions);
            }
        }


        // GET: api/Regions
        [HttpGet("showInactive")]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetAllRegions()
        {
            //Checks if model state is valid - checks for errors coming from model binding and validation
            if (!ModelState.IsValid)
            {
                //Returns 400 status code with the model state as content
                return BadRequest(ModelState);
            }

            //Create and instance of the datecalculation 
            DateCalculation dc = new DateCalculation();
            //Gets a list of all Routes in the context table and appends a next collection date to the returned regions 
            var dbRegions = _context.Region.Select(r => new { regionID = r.regionID, regionName = r.regionName, frequency = r.frequency, firstDate = r.firstDate, nextDate = dc.GetNextDates(1, r), inactive = r.inactive }).ToList();


            //If region list is null or contains no regions, return 404 Code with "No regions Found" message
            if (dbRegions == null || dbRegions.Count <= 0)
            {
                return NotFound("No regions found");
            }
            else
            {
                //Return 200 Code with Route List as content
                return Ok(dbRegions);
            }
        }




        /// <summary>
        /// HTTP GET request method to get a specific Region object given an ID value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Regions/5
        [HttpGet("{id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetRegion([FromRoute] int id)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //Checks if model state is valid - checks for errors coming from model binding and validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Searches the context Region table for a region with the given ID
            var region = await _context.Region.FindAsync(id);

            //If region is null (not found), return 404 Status Code
            if (region == null)
            {
                return NotFound();
            }

            //Returns 200 Status Code with specified region object
            return Ok(region);
        }

        /// <summary>
        /// This will take a region id in, Find the region Object Associated with it
        ///     and call the DateCalculationMethod to retrive
        ///     the dates for the subscibers location. 
        /// </summary>
        /// <param name="regionID">The region ID</param>
        /// <returns>1 Date </returns>
        [HttpGet("regionID-c={regionID}")]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetSubscriberDates([FromRoute] int regionID)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //Declare The Util method so we can call it later 
            DateCalculation dc = new DateCalculation();

            //Get all the Regions From the DB
            Region region = _context.Region.FirstOrDefault(r => r.regionID == regionID);



            //Return the results of the Helper method along with OK as an indication of success     
            return Ok(dc.GetOneDate(region));
        }

        /// <summary>
        /// This method will take a region ID and its associated object with it
        /// Will save the changes made to the region object.
        /// If the Regions inactive status was set back to being active and there are pending routes that are associated with it
        /// it will generate a new route
        /// </summary>
        /// <param name="id">The id for the Region Object</param>
        /// <param name="region">The Region object to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> PutRegion([FromRoute] int id, [FromBody] Region region)
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

            if (id != region.regionID)
            {
                return BadRequest();
            }


            DateCalculation dc = new DateCalculation();

            //get the oldRegion to check if the active changed after saving changes
            Region oldRegion = _context.Region.AsNoTracking().Where<Region>(e => e.regionID == id).FirstOrDefault();

            _context.Region.Update(region);
            await _context.SaveChangesAsync();

            //Check to see if the region has been set back to active
            if (oldRegion.inactive == true && region.inactive == false)
            {
                //Get a list of all the routes associated with the region ID that are not completed 
                List<Route> routes = _context.Route.Where<Route>(e => e.regionID == region.regionID && e.completed == false && e.inactive == false).ToList();

                //if routes active incomplete is 0 Create a new route
                if (routes.Count == 0)
                {
                    DateTime nextDate = dc.GetNextDates(1,region).Single();

                    Route newRoute = new Route
                    {
                        routeName = region.regionName + " - " + nextDate.ToString("MMMM dd, yyyy"),
                        regionID = region.regionID,
                        region = region,
                        completed = false,
                        inactive = false,
                        routeDate = nextDate
                    };

                    _context.Route.Update(newRoute);
                    

                    await _context.SaveChangesAsync();
                }
            }

            //Create an anonymous object that contains the nextPickupDate
            //Is used to send back the next collection date, to be displayed on the front end
            var returnRegion = new
            {
                regionID = region.regionID,
                regionName = region.regionName,
                frequency = region.frequency,
                firstDate = region.firstDate,
                nextDate = dc.GetNextDates(1,region),
                inactive = region.inactive
            };


            return Ok(returnRegion);
        }



        // POST: api/Regions
        /// <summary>
        /// Create a new Region record in the database given a Region object passed in.
        /// After creation, a check will occur to see if a Route exists already for the
        /// given Region ID.
        /// If no Route exists, a Route will be generated with the following properties:
        ///     routeName: "RegionName - NextPickupDate"
        ///     regionID: newRegion's ID
        ///     region: newRegion
        ///     completed: false
        ///     routeDate: Region's Next Pickup Date
        ///     inactive: false
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> PostRegion([FromBody] Region region)
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

            _context.Region.Add(region);
            await _context.SaveChangesAsync();

            //Queries the Route table for Routes which have a region ID matching the created regions ID.
            //If no Routes are found AND added region is not inactive, create a Route with derived information
            if(_context.Route.Where(r => r.regionID == region.regionID).Count() == 0 && !region.inactive)
            {
                DateTime nextDate = dateCalculation.GetNextDates(1, region).FirstOrDefault();

                Route newRoute = new Route
                {
                    routeName = region.regionName + " - " + nextDate.ToString("MMMM dd, yyyy"),
                    regionID = region.regionID,
                    region = region,
                    completed = false,
                    inactive = false,
                    routeDate = nextDate
                };

                _context.Route.Add(newRoute);
                await _context.SaveChangesAsync();
            }

            DateCalculation dc = new DateCalculation();

            //Create an anonymous object that contains the nextPickupDate
            //Is used to send back the next collection date, to be displayed on the front end
            var returnRegion = new
            {
                regionID = region.regionID,
                regionName = region.regionName,
                frequency = region.frequency,
                firstDate = region.firstDate,
                nextDate = dc.GetNextDates(1, region),
                inactive = region.inactive
            };


            return CreatedAtAction("GetRegion", new { id = returnRegion.regionID }, returnRegion);
        }

        // DELETE: api/Regions/5
        //Method takes in a Region ID in order to archive the associated Region
        [HttpDelete("{id}routes{removeRoutes}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> DeleteRegion([FromRoute] int id, bool removeRoutes)
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

            Region result = (Region)await _context.Region.FindAsync(id);

            if (result == null)
            {
                return BadRequest("Archiving Failed - Region ID was not found");
            }

            if (!result.inactive)
            {
                result.inactive = true;
                _context.Region.Update(result);

                if(removeRoutes)
                {
                    List<Route> routes = _context.Route.Where<Route>(e => e.regionID == id).ToList();

                    foreach (Route route in routes)
                    {
                        route.inactive = true;
                        _context.Route.Update(route);
                    }
                }
                
            }

            await _context.SaveChangesAsync();

            if (((Region)await _context.Region.FindAsync(id)).inactive)
            {
                return Ok("Region " + result.regionName + " was archived");
            }
            else
            {
                return BadRequest();
            }

        }

        private bool RegionExists(int id)
        {
            return _context.Region.Any(e => e.regionID == id);
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