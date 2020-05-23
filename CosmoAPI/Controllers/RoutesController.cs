using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;
using CosmoAPI.Utils;
using CosmoAPI.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CosmoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly CosmoContext _context;

        /// <summary>
        /// This method will take in a routeID and return a CSV string and title with the contents of locations and subscriber data that exist on a route.
        /// </summary>
        /// <param name="routeID">the route to be exported</param>
        /// <returns>CSV string</returns>
        [Authorize(Roles.ADMIN)]
        [HttpGet("export-r{routeID}")]
        public IActionResult ExportToCSV([FromRoute]int routeID)
        {
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }
            //get the route matching routeID passed in the request
            Route route = _context.Route.AsQueryable().Where(e => e.routeID == routeID).FirstOrDefault();

            if(route == null)
            {
                return NotFound("Route does not exist");
            }
            
            //get all locations in the same region as this route
            List<Location> locations = _context.Location.AsQueryable<Location>().Where(e => e.regionID == route.regionID && !isSkipped(route, e)).ToList<Location>();

            List<Subscriber> subscribers = new List<Subscriber>();
            //for each location in the list get its associated subscriber only if the subscriber is currently active. Inactive subscribers will not have their locations be completed on this route.
            foreach (Location l in locations)
            {
                subscribers.Add(_context.Subscriber.AsQueryable().Where(e => e.locationID == l.locationID && !e.inactive).FirstOrDefault());
            }

            //remove any locations that have a subscriber of null. This prevents null pointer exceptions when a subscriber is set to inactive
            for (int i = 0; i < locations.Count; i++)
            {
                if (subscribers[i] == null)
                {
                    subscribers.RemoveAt(i);
                    locations.RemoveAt(i);
                }
            }

            //cerate a list of an anonymous object that contains parameters of the subscriber and the location.
            var rows = subscribers.AsQueryable().Select(s => new
            {
                s.location.address,
                s.location.city,
                s.location.province,
                s.location.postalCode,
                s.firstName,
                s.lastName,
                s.phoneNumber,
                s.email
            }).ToList();
            //set the headers of the CSV string
            String content = "Address,City,Province,Postal Code,First Name,Last Name,Phone Number,Email\n";

            //go through each row in teh list of Subscriber-Location objects and add their data in csv format to the string to be returned.
            foreach (var row in rows)
            {
                content += row.address + ',' +
                row.city + ',' +
                row.province + ',' +
                row.postalCode + ',' +
                row.firstName + ',' +
                row.lastName + ',' +
                row.phoneNumber + ',' +
                row.email + "\n";
            }

            //create a variable containing CSV data and file name of the route name with '.csv' appended to the end.
            var data = new { cnt = content, fileName = route.routeName + ".csv" };

            //return 200 status with data
            return Ok(data);


        }

        /// <summary>
        /// This helper method will be used to determine if a location exists in a route's op out list.
        /// false will be returned if the locations does not exist in the routes op out list
        /// </summary>
        /// <param name="route">route to check</param>
        /// <param name="e">locations to look for</param>
        /// <returns></returns>
        private bool isSkipped(Route route, Location e)
        {
            bool skipped = false;
            //if nothing exists in the opt out list just return false
            if(route.optoutLocationRouteList == null)
            {
                return false;
            }
            //go through each location in the op out list and if it matches the location passed in, set skipped to true
            route.optoutLocationRouteList.ForEach((i) => {
                if (i.locationID == e.locationID)
                {
                    skipped = true;
                }
            });

            
            return skipped;
        }

        [Authorize(Roles.ADMIN)]
        [HttpGet("count-route-r{routeID}")]
        public IActionResult GetLocationCount([FromRoute]int routeID)
        {
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            if(_context.Route.Find(routeID) == null)
            {
                return BadRequest("Invalid Route ID");
            }
            //get the route matching routeID passed in the request
            Route route = _context.Route.AsNoTracking().FirstOrDefault(e => e.routeID == routeID);
            Region region = _context.Region.AsNoTracking().FirstOrDefault(e => e.regionID == route.regionID);
            //get all locations in the same region as this route
            List<Location> locations = _context.Location.AsNoTracking().Where(e => e.regionID == route.regionID && !isSkipped(route, e)).ToList<Location>();
            List<Subscriber> subscribers = new List<Subscriber>();
            //for each location in the list get its associated subscriber only if the subscriber is currently active. Inactive subscribers will not have their locations be completed on this route.
            foreach(Location l in locations)
            {
                subscribers.Add(_context.Subscriber.AsQueryable().Where(e => e.locationID == l.locationID && !e.inactive).FirstOrDefault());
            }

            //remove any locations that dont contain subscribers. This prevents null pointer exceptions when a user is set to inactive.
            for (int i = 0; i < locations.Count; i++)
            {
                if(subscribers[i] == null)
                {
                    subscribers.RemoveAt(i);
                    locations.RemoveAt(i);
                }
            }
            //return the count of active subscribers in this regions, they will be completed on this route.
            return Ok(subscribers.Count());
        }

        /// <summary>
        /// This will return the total amount of completed routes for a given subscriber
        /// </summary>
        /// <returns>amount of completed routes</returns>
        [Authorize(Roles.SUBSCRIBER)]
        [HttpGet("totalRows-rsubscriberID-s{subscriberID}")]
        public IActionResult GetSubscribersRouteCount([FromRoute]int subscriberID)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            var tokenIn = this.HttpContext.Request.Headers.GetCommaSeparatedValues("Authorization");

            tokenIn[0] = tokenIn[0].Substring(7);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenIn[0]);

            bool valid = false;

            foreach (Claim claim in token.Claims)
            {
                if (claim.Type.Equals("ID"))
                {
                    if (claim.Value.Equals(subscriberID.ToString()))
                    {
                        valid = true;
                    }
                }
            }

            if (!valid)
            {
                return Unauthorized();
            }

            //get subscriber based on SubscriberID passed in
            Subscriber sub = (Subscriber)_context.Subscriber.AsQueryable<Subscriber>().Where(e => e.subscriberID == subscriberID).FirstOrDefault();
            //get location based on the Subscriber's locationID
            Location location = (Location)_context.Location.AsQueryable<Location>().Where(e => e.locationID == sub.locationID).FirstOrDefault();
            //grab region based on locations regionID
            Region region = (Region)_context.Region.AsQueryable<Region>().Where(e => e.regionID == location.regionID).FirstOrDefault();
            try
            {
                //get the count of routes matching the subscribers location region.
                return Ok(_context.Route.AsQueryable().Where(e => e.regionID == region.regionID && e.completed == true).Count());
            }
            catch (Exception e) //will throw an exception if no routes are found so just return 0
            {
                return Ok(0);
            }


        }

        /// <summary>
        /// This method will return an Ienumerable list of DateTime objects for a given subscriber by using LINQ to query tables.
        /// </summary>
        /// <param name="sortBy">The order to sort by</param>
        /// <param name="size">the number of rows to return</param>
        /// <param name="currentPage">the current page of the UI table</param>
        /// <param name="SubscriberID">The subscriber to search with</param>
        /// <returns>list of routes that correspond to the subscriberID passed in</returns>
        [HttpGet("sortBy-s{sortBy}subscriberID-i{subscriberID}sizePerPage-p{size}currentPage-c{currentPage}")]
        [Authorize(Roles.SUBSCRIBER)]
        public IActionResult GetCompletedRoutesBySubscriber([FromRoute] string sortBy, int subscriberID, int size, int currentPage)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            var tokenIn = this.HttpContext.Request.Headers.GetCommaSeparatedValues("Authorization");

            tokenIn[0] = tokenIn[0].Substring(7);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenIn[0]);

            bool valid = false;

            foreach (Claim claim in token.Claims)
            {
                if (claim.Type.Equals("ID"))
                {
                    if (claim.Value.Equals(subscriberID.ToString()))
                    {
                        valid = true;
                    }
                }
            }

            if (!valid)
            {
                return Unauthorized();
            }

            //grab subscriber based on the subscriberID passed in
            Subscriber sub = (Subscriber)_context.Subscriber.AsQueryable<Subscriber>().Where(e => e.subscriberID == subscriberID).FirstOrDefault();
            //grab location based on subscriber locationID
            Location location = (Location)_context.Location.AsQueryable<Location>().Where(e => e.locationID == sub.locationID).FirstOrDefault();
            //grab region based on locations regionID
            Region region = (Region)_context.Region.AsQueryable<Region>().Where(e => e.regionID == location.regionID).FirstOrDefault();
            //based on the sortBy value
            switch (sortBy)
            {
                //get dates in ascending order by routeDate
                case "asc":
                    return Ok(_context.Route.AsQueryable<Route>().Where(e => e.regionID == region.regionID && e.completed == true).OrderBy(e => e.routeDate).Skip((currentPage - 1) * size).Take(size));
                //Get dates in Descending order by routeDate
                case "desc":
                    return Ok(_context.Route.AsQueryable<Route>().Where(e => e.regionID == region.regionID && e.completed == true).OrderByDescending(e => e.routeDate).Skip((currentPage - 1) * size).Take(size));
                //By default, sort ascending by routeDate
                default:
                    return Ok(_context.Route.AsQueryable<Route>().Where(e => e.regionID == region.regionID && e.completed == true).OrderBy(e => e.routeDate).Skip((currentPage - 1) * size).Take(size));
            }
        }

        public RoutesController(CosmoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// <summary>
        /// HTTP GET request method to retrieve all Route objects in the DB and return them
        /// </summary>
        /// <returns></returns>
        // GET: api/Routes
        [HttpGet("{id}")]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetRoute([FromRoute] int id)
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

            Route route = _context.Route.Include("optoutLocationRouteList").FirstOrDefault(r => r.routeID == id);

            //If not found, return 404 with message "Specified route was not found"
            if (route == null)
            {
                return NotFound("Specified route was not found");
            }

            return Ok(route);
        }

        /// <summary>
        /// HTTP GET request method to retrieve all Route objects in the DB and return them
        /// </summary>
        /// <returns></returns>

        // GET: api/Routes/Regions
        [HttpGet("Regions")]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetAllActiveRoutes()
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

            //Gets a list of active incomplete Routes ordered by completed status and then by routeDate

            List<Route> dbRoutes = _context.Route
                //.Include("region")
                .Where(route => !route.completed && !route.inactive)
                .OrderBy(route => route.routeDate)
                .ToList<Route>();

            //If route list is null or contains no routes, return 404 Code with "No Routes Found" message
            if (dbRoutes == null || dbRoutes.Count <= 0)
            {
                return NotFound("No routes found");
            }
            else
            {
                List<Region> dbRegions = _context.Region
                    .Select(r => new Region { regionID = r.regionID, regionName = r.regionName, frequency = r.frequency, inactive = r.inactive, firstDate = r.firstDate }).ToList();

                foreach (Route r in dbRoutes)
                {
                    r.region = dbRegions.Find(x => x.regionID == r.regionID);
                }

                //Return 200 Code with Route List as content
                return Ok(dbRoutes);
            }
        }
 
        /// <summary>
        /// HTTP GET method for getting a specific Route from the database given a region ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Routes/5
        [HttpGet("regionID-r={id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetUpcomingRouteFromRegionID([FromRoute] int id)
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

            //Searches the Route table for a given ID
            List<Route> foundRoutes = _context.Route.Include("optoutLocationRouteList").Where(x =>
                x.regionID == id && !x.completed
            ).ToList<Route>();


            //If not found, return 404 with message "Specified route was not found"
            if (foundRoutes.Count != 1)
            {
                return NotFound("Specified route was not found");
            }

            //Cast found route to a Route object, return 200 Code with route as content
            Route route = foundRoutes[0];
            return Ok(route);
        }

        //GET: api/Routes/showComplete
        [HttpGet("showComplete")]
        [Authorize(Roles.ADMIN)]
        public IActionResult GetCompleteRoute()
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

            //Gets active routes, both incomplete and complete, sorted by Completed status and then by Route Date, both ascending
            List<Route> dbRoutes = _context.Route
                        .AsNoTracking()
                        //.Where(route => route.inactive == false) taken out to display routes that were inactive when A Region is deleted
                        .OrderBy(route => route.completed)
                        .ThenBy(route => route.routeDate)
                        .ToList<Route>();

            if (dbRoutes == null || dbRoutes.Count <= 0)
            {
                return NotFound("No routes found");
            }
            else
            {
                List<Region> dbRegions = _context.Region
                    .Select(r => new Region { regionID = r.regionID, regionName = r.regionName, frequency = r.frequency, firstDate = r.firstDate, inactive = r.inactive }).ToList();

                foreach (Route r in dbRoutes)
                {
                    r.region = dbRegions.Where(x => x.regionID == r.regionID).FirstOrDefault();
                }

                //Return 200 Code with Route List as content
                return Ok(dbRoutes);
            }
        }

        // PUT: api/Routes/5
        /// <summary>
        /// Updates a Route given an ID and Route object with updated information.
        /// If the completed status is changed from FALSE to TRUE, a check will occur
        /// to determine if the associated Region is inactive or not.
        /// If the region is active, a check will occur looking for an incomplete Route
        /// associated with the region.
        /// If no Route is found, a new Route object will be created with
        /// the original Route's Name and Region, copmpletion status at FALSE, and
        /// the Region's next pickup date.
        ///     If the region is inactive, a new Route will not be created
        /// </summary>
        /// <param name="id"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> PutRoute([FromRoute] int id, [FromBody] Route route)
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

            if (id != route.routeID)
            {
                return BadRequest();
            }

            //Get the date of the corresponding Route in the DB. Using "AsNoTracking" to prevent the entity from being tracked as information is being read, not edited
            DateTime dbRouteDate = _context.Route.AsNoTracking().Where(r => r.routeID == id).Single().routeDate;


            //If the incoming Route's date is in the past, and the date has been changed, return an error
            if (route.routeDate.Date < DateTime.Now.Date && !route.routeDate.ToShortDateString().Equals(dbRouteDate.ToShortDateString()))
            {
                //Return a Bad Request is the incoming Route's date was chnaged to a date in the past
                return BadRequest(new ValidationResult("Invalid Date", new[] { "routeDate" }));
            }
            else
            {
                _context.Route.Update(route);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RouteExists(id))
                {
                    return NotFound("Route was not found");
                }
                else
                {
                    throw;
                }
            }
            
            CompareModels cmpRoutes = new CompareModels();
            //Route dbRoute = (Route) await _context.Route.FindAsync(id);
            Route dbRoute = _context.Route.AsNoTracking().Where(r => r.routeID == id).Single();
            dbRoute.region = _context.Region.AsNoTracking().Single(reg => reg.regionID == dbRoute.regionID);

            //Route dbRoute = (Route)await _context.Route.FindAsync(id);
            
            if (cmpRoutes.CompareRoutes(dbRoute, route) == 0)

            {
                
                //If original route was incomplete and updated Route is complete, 
                //no incomplete Routes exist for the Region
                //and the Region is Active, Create a new Route
                if (dbRoute.completed &&
                    _context.Route.AsNoTracking().Where(r => r.regionID == route.regionID && !r.inactive && !r.completed).Count() == 0 &&
                    _context.Region.AsNoTracking().Where(r => r.regionID == route.regionID).FirstOrDefault().inactive == false)
                {
                    Region dbRegion = _context.Region.AsNoTracking().Where(r => r.regionID == route.regionID).FirstOrDefault();     //Get the associated Region for the Route
                    DateCalculation calc = new DateCalculation();
                    //Create a new Route object with values taken from old Route
                    
                    Route newRoute = new Route
                    {
                        routeName = route.routeName,
                        regionID = dbRegion.regionID,
                        region = dbRegion,
                        completed = false,
                        inactive = false,
                        routeDate = calc.GetOneDate(dbRegion)
                    };
                    
                    //Add the new Route to DB and save it
                    await _context.Route.AddAsync(newRoute);

                }
                await _context.SaveChangesAsync();
                //Return resulting updated Route
                return Ok(dbRoute);
            }
            else
            {
                return BadRequest();
            }

        }

        // DELETE: api/Routes/5
            /// <summary>
            /// Archives a Route based on a given ID.
            /// If the Route's Region is active, a check will occur looking for
            /// another Incomplete Route associated with the Region. 
            /// If no Route is found, a new Route will be generated
            /// with the same Route Name, Region, INCOMPLETE status, and the 
            /// Region's next pick up date.
            ///     If the Region is inactive, a new Route will not be generated
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> DeleteRoute([FromRoute] int id)
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

            Route result = (Route)await _context.Route.FindAsync(id);

            if (result == null)
            {
                return BadRequest("Archiving Failed - Route ID was not found");
            }

            if (!result.inactive)
            {
                result.inactive = true;
                _context.Route.Update(result);
                await _context.SaveChangesAsync();
            }

            if (_context.Route.AsNoTracking().Where(r => r.routeID == id).Single().inactive)
            {
                //Get Region associated with archived Route
                Region region = _context.Region.AsNoTracking().Where(re => re.regionID == result.regionID).Single();

                //If the Region is ACTIVE and there are no INCOMPLETE Routes associated with the Region
                if (!region.inactive && _context.Route.AsNoTracking().Where(r => r.regionID == region.regionID && !r.inactive && !r.completed).Count() == 0)
                {
                    DateCalculation calc = new DateCalculation();
                    Route newRoute = new Route
                    {
                        //routeID = default(int),
                        routeName = result.routeName,
                        regionID = region.regionID,
                        completed = false,
                        inactive = false,
                        routeDate = calc.GetOneDate(region)
                    };
                    await _context.Route.AddAsync(newRoute);

                }

                await _context.SaveChangesAsync();
                return Ok("Route " + result.routeName + " was archived");
            }
            else
            {
                return BadRequest();
            }

        }

        private bool RouteExists(int id)
        {
            return _context.Route.Any(e => e.routeID == id);
        }

    }
}