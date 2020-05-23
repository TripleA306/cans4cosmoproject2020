using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;
using CosmoAPI.Authorization;
using Google.Apis.Auth;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace CosmoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly CosmoContext _context;

        public SubscribersController(CosmoContext context)
        {
            _context = context;
        }

        //GET: api/Subscribers
        [HttpGet]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetSubscribers()
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }
            return Ok(_context.Subscriber);
        }

        /// <summary>
        /// This will return the total amount of subscribers that are currently inactive in the database
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles.ADMIN)]
        [HttpGet("totalRows-r")]
        public async Task<IActionResult> GetSubscriberCount()
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //query for subscribers that are active and return the count
            return Ok(_context.Subscriber.AsQueryable<Subscriber>().Where(e => !e.inactive).Count());
        }

        /// <summary>
        /// This method will take in a sort by key, size per page, and current page number and return the correct Subscribers offset by the currentPage,
        /// sorted by the sort key. 
        /// </summary>
        /// <param name="sortBy">method to sort</param>
        /// <param name="size">amount of Subscribers to return</param>
        /// <param name="currentPage">The current Page of the table</param>
        /// <returns>Subscribers that result from the query</returns>
        // GET: api/Subscribers/5
        [HttpGet("sortBy-s{sortBy}sizePerPage-p{size}currentPage-c{currentPage}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetSubscribersBySort([FromRoute] string sortBy, int size, int currentPage)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }
            //based on the sortBy value
            switch (sortBy)
            {
                //get subscribers in ascending order
                case "asc":
                    return Ok(_context.Subscriber.AsQueryable<Subscriber>().Where(e => !e.inactive).OrderBy(e => e.email).Skip((currentPage - 1) * size).Take(size));
                //Get Subscribers in Descending order
                case "desc":
                    return Ok(_context.Subscriber.AsQueryable<Subscriber>().Where(e => !e.inactive).OrderByDescending(e => e.email).Skip((currentPage - 1) * size).Take(size));
                //By default, sort ascending
                default:
                    return Ok(_context.Subscriber.AsQueryable<Subscriber>().Where(e => !e.inactive).OrderBy(e => e.email).Skip((currentPage - 1) * size).Take(size));
            }
        }

        // GET: api/Subscribers/5
        [HttpGet("{id}")]
        [Authorize(Roles.ADMIN, Roles.SUBSCRIBER)]
        public async Task<IActionResult> GetSubscriber([FromRoute] int id)
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
                if (claim.Type.Equals(Roles.SUBSCRIBER.ToString()))
                {
                    if (claim.Value.Equals(Roles.SUBSCRIBER.ToString()))
                    {
                        valid = true;
                    }
                }
            }

            //If the accessing user is a subscriber, we need to ensure they can only view their own information
            //Admins can view any subscriber
            if (valid)
            {
                valid = false;

                foreach (Claim claim in token.Claims)
                {
                    if (claim.Type.Equals("ID"))
                    {
                        if (claim.Value.Equals(id.ToString()))
                        {
                            valid = true;
                        }
                    }
                }

                if (!valid)
                {
                    return Unauthorized();
                }
            }

            //If the object is invalid, return statuc code 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Find the subscriber object with the given ID
            var subscriber = await _context.Subscriber.FindAsync(id);

            //If it is null, return 404
            if (subscriber == null)
            {
                return NotFound();
            }

            //Return 200 if found
            return Ok(subscriber);
        }


        /// <summary>
        /// This method will take in an email from the get request and return 'true' or 'false' depending on if a subscriber is found matching the given email.
        /// Added routing for it to work
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("email={email}")]
        public async Task<IActionResult> GetSubscriber([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if the subscriber exists
            var emailExists = SubscriberExists(email);
            IActionResult returnVal;

            var tokenIn = this.HttpContext.Request.Headers.GetCommaSeparatedValues("GoogleAuth");

            var validPayload = await GoogleJsonWebSignature.ValidateAsync(tokenIn[0]);

            if (emailExists && validPayload != null)
            {
                if (validPayload.Email.Equals(email))
                {

                    List<Subscriber> subscriberList = _context.Subscriber.ToList<Subscriber>();

                    var subscriber = subscriberList.Find(s => s.email == email);

                    returnVal = Ok(subscriber);
                }
                else
                {
                    returnVal = Unauthorized();
                }
            }
            else
            {
                returnVal = NotFound();
            }

            return returnVal;

            //if subscriber does not exist, return false and 404 status code otherwise return true and 200 status code
        }

        /// <summary>
        /// This method is used to assign the login token to the user. It is called by the api after the subscriber object is gotten from GetSubscriber(email)
        /// It is done because the subscriber ui needs to know the current subscriber id
        /// </summary>
        /// <param name="email">The subscribers email</param>
        /// <returns></returns>
        [HttpGet("email-d={email}")]
        public async Task<IActionResult> GetLoginToken([FromRoute] string email)
        {
            var tokenIn = this.HttpContext.Request.Headers.GetCommaSeparatedValues("GoogleAuth");

            if(tokenIn.Count<string>() == 0)
            {
                return Unauthorized();
            }

            var validPayload = await GoogleJsonWebSignature.ValidateAsync(tokenIn[0]);

            if (validPayload != null)
            {
                if (validPayload.Email.Equals(email.ToLower()))
                {
                    List<Subscriber> subscriberList = _context.Subscriber.ToList<Subscriber>();

                    var subscriber = subscriberList.Find(s => s.email.Equals(email.ToLower()));

                    //create a JWT key
                    var key = Encoding.ASCII.GetBytes("Ca5nQTanmoOutL0gTaken313578");

                    IEnumerable<Claim> claims = new Claim[] {
                        new Claim(Roles.SUBSCRIBER.ToString(), Roles.SUBSCRIBER.ToString()),
                        new Claim("EMAIL", subscriber.email),
                        new Claim("ID", subscriber.subscriberID.ToString())
                    };

                    //creat a JWT token
                    var JWToken = new JwtSecurityToken(
                        issuer: "http://localhost:5002/api", //set the issuer, this api
                        audience: "http://localhost:8080", //set the audience we except to get requests from
                        claims: claims,
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime, //token will not be valid until now
                        expires: new DateTimeOffset(DateTime.Now.AddHours(2)).DateTime, //token will expire in 2 hours
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //sign the token
                    );

                    //write the token out to token
                    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);

                    return Ok(token);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// This is an extra Get method in the controller that like the other, will tak in an email
        ///     It is used to get the LocationID that is associated with the subscribers email that is passed in
        /// </summary>
        /// <param name="email">The subscribers email passed in</param>
        /// <returns>The locationID associated with the subcriber account that matches the email passed in</returns>
        [HttpGet("email-c={email}")]
        [Authorize(Roles.SUBSCRIBER)]
        public IActionResult GetSubscribersLocationID([FromRoute] string email)
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
                if (claim.Type.Equals("EMAIL"))
                {
                    if (claim.Value.Equals(email.ToString()))
                    {
                        valid = true;
                    }
                }
            }

            if (!valid)
            {
                return Unauthorized();
            }

            //Get a list of all the subscribers so we can compare our email against all the entries
            //All Subscribers will have an email as it is required
            Subscriber sub = _context.Subscriber.FirstOrDefault(s => s.email == email);

            //Return the locationID associated with the matching subscriber
            //Return OK so the status code is available
            return Ok(sub.locationID);
        }


        // POST: api/Subscribers
        [HttpPost]
        public async Task<IActionResult> PostSubscriber([FromBody] Subscriber subscriber)
        {
            var tokenIn = this.HttpContext.Request.Headers.GetCommaSeparatedValues("GoogleAuth");

            if(tokenIn.Count<string>() == 0)
            {
                return Unauthorized();
            }

            var validPayload = await GoogleJsonWebSignature.ValidateAsync(tokenIn[0]);

            if (validPayload != null)
            {
                if (validPayload.Email.Equals(subscriber.email.ToLower()))
                {
                    //If the object is not valid, return 422
                    if (!ModelState.IsValid)
                    {
                        return UnprocessableEntity(ModelState);
                    }

                    //Add the object to the context
                    _context.Subscriber.Add(subscriber);
                    //Save the changes
                    await _context.SaveChangesAsync();
                    //Return 201
                    return CreatedAtAction("GetSubscriber", new { id = subscriber.subscriberID }, subscriber);
                }
                else
                {
                    return Unauthorized();
                }
            }
            //If the object is not valid, return 422
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            //Add the object to the context
            _context.Subscriber.Add(subscriber);
            //Save the changes
            await _context.SaveChangesAsync();
            //Return 201
            return CreatedAtAction("GetSubscriber", new { id = subscriber.subscriberID }, subscriber);
        }


        // PUT: api/Subscribers/5
        [HttpPut("{id}")]
        [Authorize(Roles.SUBSCRIBER)]
        public async Task<IActionResult> PutSubscriber([FromRoute] int id, [FromBody] Subscriber subscriber)
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

            foreach(Claim claim in token.Claims)
            {
                if(claim.Type.Equals("ID"))
                {
                    if (claim.Value.Equals(id.ToString()))
                    {
                        valid = true;
                    }
                }
            }

            if (!valid)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscriber.subscriberID)
            {
                return BadRequest();
            }

            _context.Entry(subscriber).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberExists(id))
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




        //Check to see if a subscriber exists given an ID
        private bool SubscriberExists(int id)
        {
            return _context.Subscriber.Any(e => e.subscriberID == id);
        }

        //This method will take in an email and return a boolean value if a subscriber is found using the email provided
        private bool SubscriberExists(string email)
        {
            return _context.Subscriber.Any(e => e.email.Equals(email));
        }


        /// <summary>
        /// This method will get an email from the route address and remove the subscriber associated with that email address
        /// </summary>
        /// <param name="email">The email of the subscriber to delete</param>
        /// <returns>200 status code if the subscriber was successfully set to inactive, 400 if not</returns>
        ///   change email to ID. Dont want to send email accross internet.
        [HttpDelete("{id}")]
        [Authorize(Roles.ADMIN)]
        public IActionResult SetSubscriberInactive([FromRoute] int id)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //check if the object is invalid 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //get an existing subscriber matching the email taken from the route
            Subscriber subscriberInactive = _context.Subscriber.FirstOrDefault(e => e.subscriberID == id);

            //check if the subscriber id is a valid id
            if (subscriberInactive == null)
            {
                return NotFound();
            }
            //set the subscriber's inactive status to true
            subscriberInactive.inactive = true;

            //update the subscriber in the database using the subscriber "subscriberInactive" object.
            var isInactive = _context.Subscriber.Update(subscriberInactive) != null ? (IActionResult)Ok(subscriberInactive) : (IActionResult)NotFound(subscriberInactive);

            //save the changes to the database
            _context.SaveChanges();

            return isInactive;

        }


        
        // GET: api/Subscribers/optoutID=5
        [HttpGet("optoutID={id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> SubscriberOptOut([FromRoute] int id)
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

            if(id < 1)
            {
                return NotFound();
            }

            //Get the specific subscriber with the ID
            Subscriber subscriber = _context.Subscriber.FirstOrDefault(x => x.subscriberID == id);

            //var subscriberList = _context.Subscriber.Include("location").Select(s => new Subscriber
            //{
            //    subscriberID = s.subscriberID,
            //    locationID = s.locationID,
            //    location = new Location
            //    {
            //        locationID = (int)s.location.locationID, regionID = s.location.regionID,
            //        region = new Region
            //        {

            //        }
            //    },
            //    billingLocation = null
            //}).Where(s => s.subscriberID == id);

            if (subscriber == null || id != subscriber.subscriberID)
            {
                return BadRequest();
            }

            //Get the location based on the subscriber's locationID
            Location location = _context.Location.FirstOrDefault(x => x.locationID == subscriber.locationID);


            if (location == null)
            {
                return BadRequest();
            }

            //Get the upcoming route based on the location's regionID
            Route route = _context.Route.FirstOrDefault(x => x.regionID == location.regionID && !x.completed && !x.inactive);
            //return Ok(route);

            if (route == null)
            {
                return BadRequest();
            }

            LocationRoute locationRoute = new LocationRoute();
            locationRoute.locationID = location.locationID;
            locationRoute.routeID = route.routeID;

            //Returns true if properly removed, otherwise false
            //_context.Configuration.LazyLoadingEnabled = false;
            _context.Route.Include("optoutLocationRouteList").FirstOrDefault<Route>(x => x.routeID == route.routeID).optoutLocationRouteList.Add(locationRoute);
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            List<Subscriber> obList = new List<Subscriber>();
            obList.Add(subscriber);
            subscriber = obList.Select(s => new Subscriber
            {
                subscriberID = s.subscriberID,
                locationID = s.locationID,
                location = new Location
                {
                    locationID = s.location.locationID,
                    regionID = s.location.regionID,
                    optoutLocationRouteList = s.location.optoutLocationRouteList
                    .Select(o => new LocationRoute { locationRouteID = o.locationRouteID, locationID = o.locationID, routeID = o.routeID }).ToList()
                }
            }).ToList()[0];

            return Ok(subscriber);
        }

        // PUT: api/Subscribers/subID-r=5
        [HttpGet("subID-r={id}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> GetRouteFromSubscriberID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the specific subscriber with the ID
            Subscriber subscriber = _context.Subscriber.FirstOrDefault<Subscriber>(x => x.subscriberID == id);

            if (id != subscriber.subscriberID)
            {
                return BadRequest();
            }

            //Get the location based on the subscriber's locationID
            Location location = _context.Location.First<Location>(x => x.locationID == subscriber.locationID);

            //Get the upcoming route based on the location's regionID
            Route route = _context.Route.First<Route>(x => x.regionID == location.regionID && !x.completed);

            return Ok(route);
        }


    }
}