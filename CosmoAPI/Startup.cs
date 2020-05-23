using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CosmoAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CosmoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Added for Enable CORS Tutorial https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private DateCalculation dateCalculation;

        //private List<Route> routes;
        //private List<Location> locations;
        //private List<Region> regions;
        //private List<Subscriber> subscribers;


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Sets compatibility version to match behavour of ASP.NET Core MVC 2.1 - We're using .Net Core 2.1, not latest version (3.0)
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects);
           


            //Uncomment this line and comment the next to use the In Memory DB
            //Setting to In-Memory database WILL BREAK LOGIN UI TESTS
            services.AddDbContext<CosmoContext>(options => options.UseInMemoryDatabase("InMemoryCosmoDB"));
            //services.AddDbContext<CosmoContext>(options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CosmoContextDB;Trusted_Connection=True;MultipleActiveResultSets=true"));


            //Original Code
            //services.AddMvc();
            //services.AddDbContext<CosmoContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("CosmoContext")));

            //create a secret key for the authorization token
            var SecretKey = Encoding.ASCII.GetBytes("Ca5nQTanmoOutL0gTaken313578");

            //add token authorization
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //create a JWT token brearer
                .AddJwtBearer(token =>
                {
                    token.RequireHttpsMetadata = false; //dont need to require meta data
                    token.SaveToken = true; //we will save this token
                    token.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true, //the issuer signing key must be valid
                        IssuerSigningKey = new SymmetricSecurityKey(SecretKey), //create an issuer signing key with the secret key value
                        ValidateIssuer = true, //isuer will be validated
                        ValidIssuer = "http://localhost:5002/api", //set the valid url of the issuer (API)
                        ValidateAudience = true, //the audience will be validated (front end)
                        ValidAudience = "http://localhost:8080", //set the valid audience url
                        RequireExpirationTime = true, //expiration time is required
                        ValidateLifetime = true //the lifetime will be validated for this token
                    };
                });

            //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1 used to bypass coors errors     

            //Added for Enable CORS Tutorial https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
            //Allows for Cross Origin Requests to occur (Requests to a different domain/page than where the web page is hosted)

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:8080", "http://localhost:5002" , "http://localhost:8081").AllowAnyHeader().AllowAnyMethod();   //Specifies a URL to accept requests from

                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CosmoContext context)
        {
            //Checks if the environment the webApp is hosted in is a development environment
            if (env.IsDevelopment())
            {
                dateCalculation = new DateCalculation();
                app.UseDeveloperExceptionPage();        //Provides the Developer Exception Page. Page gives information on errors thrown, requests, queries, and others

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();       //Checks that the context's database is created. Returns true if it is, false if not

                unloadDB(context); //Helper method to unload items in the context database (ensures fresh dummy data)


                List<Region> regions = loadRegions();   //Helper method to load dummy Regions into Region table
                context.AddRange(regions);       //Adds Regions list to context
                context.SaveChanges();      //Saves changes

                //List<Location> locations = loadLocations(regions); //Create locations with regions passed in
                //context.Location.AddRange(locations);
                //context.SaveChanges();

                loadLocations(context, regions);
                List<Location> locations = context.Location.ToList();

                List<Subscriber> subscribers = loadSubscribers(locations); //Create the subscribers with the locations passed in
                context.Subscriber.AddRange(subscribers);
                context.SaveChanges(); //Save changes to DB

                List<Route> routes = loadRoutes(regions);    //Helper method to load dummy Routes into Route table
                context.AddRange(routes);       //Adds routes list to context
                context.SaveChanges();      //Saves changes

                List<Admin> admins = loadAdmins(); //Create the admins
                context.Admin.AddRange(admins);
                context.SaveChanges(); //Save changes to DB

                List<LocationRoute> locationRoutes = loadLocationRoutes(locations, routes); //Create the locationRoutes with the locations and routes passed in
                context.LocationRoute.AddRange(locationRoutes);
                context.SaveChanges(); //Save changes to DB
                //Highly populate route 'S90 A Past Route'
                HighlyPopulateRoute(context, routes[0]);

                //Added for Enable CORS Tutorial https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
                //Tells the application to run the given policy name
                app.UseCors(MyAllowSpecificOrigins);

                context.SaveChanges();
            }
            app.UseAuthentication();
            app.UseMvc();   //Adds MVC to the .Net Core REquest Execution Pipeline

        }

        /// <summary>
        /// Helper Method to remove contents of the Context
        /// </summary>
        /// <param name="context"></param>
        private void unloadDB(CosmoContext context)
        {
            //Unload DB
            context.RemoveRange(context.Subscriber);
            context.RemoveRange(context.Location);
            context.RemoveRange(context.Region); //Remove contents of Region table from context
            context.RemoveRange(context.Route); //Remove contents of Route table from context
            context.RemoveRange(context.Admin);
            context.RemoveRange(context.LocationRoute);

            context.SaveChanges();  //Save changes
        }

        /// <summary>
        /// Helper method to load test Regions into context
        /// </summary>
        /// <param name="context"></param>
        private List<Region> loadRegions()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1.0); //Gets today's date

            List<Region> regions = new List<Region>()   //Defining a list of Regions to load into context
            {
                new Region()
                {
                    regionID = default(int),
                    regionName ="Harbor Creek",
                    frequency = 10,
                    inactive = false,//Adds tomorrow as region's firstDate
                    firstDate = tomorrow,   //Adds tomorrow as region's firstDate
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "Downtown",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(1.0), //Adds the day after as region's firstDate
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "South End",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(2.0), //Adds the date in three days as region's firstDate
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region() //Region 1 year in the past 
                { //IMPORTANT--------------------------------------> DO NOT CHANGE THE DATE, Katalon tests have a hard coded version of this region 
                    regionID = default(int),
                    regionName = "Nathans Test Region",
                    frequency = 10,
                    firstDate = new DateTime(2019,1,1),
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                { //IMPORTANT--------------------------------------> DO NOT CHANGE THE DATE, Katalon tests have a hard coded version of this region 
                    regionID = default(int),
                    regionName = "Nathans 2nd Test Region",
                    frequency = 10,
                    firstDate = new DateTime(2018,1,1),
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "My name is Brett",
                    frequency = 10,
                    firstDate = new DateTime(2019,2,1),
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "Kyle's S90 Active Region",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(3.0),
                    inactive = false,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "Kyle's S90 Inactive Region",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(4.0),
                    inactive = true,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "A Past Region",
                    frequency = 10,
                    firstDate = DateTime.Now.AddDays(-15.0),
                    inactive = false
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "Kyle's Second S90 Inactive Region",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(4.0),
                    inactive = true,
                    locationList = new List<Location>(),
                    routeList = new List<Route>()
                },
                 new Region()
                {
                    regionID = default(int),
                    regionName = "The New World",
                    frequency = 10,
                    firstDate = new DateTime(2017,12,9)
                },
                 new Region()
                 {
                     regionID = default(int),
                     regionName = "Iceborne",
                     frequency = 10,
                     firstDate = new DateTime(2019, 9, 6)
                 }
            };

            return regions;
        }

        /// <summary>
        /// Helper Method for loading Routes into context
        /// </summary>
        /// <param name="context"></param>
        private List<Route> loadRoutes(List<Region> regions)
        {
            //DateTime date = DateTime.Today; //Gets todays date. Later used to ensure dates are in the future

            List<Route> routes = new List<Route>()  //Creating route objects. Dates are generated based on region's first pickup date. Set it up to generate the next three upcoming sets of route dates
            {
                //Week 1 Routes
                new Route() { routeName="S90 A Past Route", regionID=regions[8].regionID, region=regions[8], completed=false, routeDate = regions[8].firstDate, optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=false, routeDate = regions[8].firstDate.AddDays(1.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=false, routeDate = regions[8].firstDate.AddDays(2.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=false, routeDate = regions[8].firstDate.AddDays(3.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="S90 Active Route", regionID=regions[6].regionID, region=regions[6], completed=false, routeDate = regions[8].firstDate.AddDays(4.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="S90 Inactive Route One", regionID=regions[7].regionID, region=regions[7], completed=false, routeDate = regions[8].firstDate.AddDays(5.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="S90 Inactive Route Two", regionID=regions[9].regionID, region=regions[9], completed=false, routeDate = regions[8].firstDate.AddDays(5.0), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Way to the New World", regionID=regions[10].regionID, region=regions[10], completed=false, routeDate= regions[8].firstDate, optoutLocationRouteList = new List<LocationRoute>() },
                //Week 2 Routes
                new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=true, routeDate = dateCalculation.GetOneDate(regions[0]).AddDays(7.0 * regions[0].frequency),  optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=true, routeDate = dateCalculation.GetOneDate(regions[1]).AddDays(7.0 * regions[1].frequency),  optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=true, routeDate = dateCalculation.GetOneDate(regions[2]).AddDays(7.0 * regions[2].frequency), optoutLocationRouteList = new List<LocationRoute>() },

                //Week 3 Routes
     			new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=true, routeDate = dateCalculation.GetOneDate(regions[0]).AddDays(7.0 * 2.0 * regions[0].frequency), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=true, routeDate = dateCalculation.GetOneDate(regions[1]).AddDays(7.0 * 2.0 * regions[1].frequency), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=true, routeDate = dateCalculation.GetOneDate(regions[2]).AddDays(7.0 * 2.0 * regions[2].frequency), optoutLocationRouteList = new List<LocationRoute>() },


                //Story 87 routes
                new Route() { routeName="Route Z",  regionID=regions[3].regionID, region=regions[3], completed=true, routeDate = new DateTime(2020, 2, 20), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Route Z",  regionID=regions[3].regionID, region=regions[3], completed=true, routeDate = new DateTime(2020, 2, 3), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Route Z",  regionID=regions[3].regionID, region=regions[3], completed=true, routeDate = new DateTime(2020, 1, 25), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Route Z",  regionID=regions[3].regionID, region=regions[3], completed=true, routeDate = new DateTime(2020, 1, 15), optoutLocationRouteList = new List<LocationRoute>() },
                new Route() { routeName="Route Z",  regionID=regions[3].regionID, region=regions[3], completed=true, routeDate = new DateTime(2020, 1, 5), optoutLocationRouteList = new List<LocationRoute>() }
            };

            return routes;
        }

        private List<Subscriber> loadSubscribers(List<Location> locations)
        {
			//Create a list of hardcoded subscribers so we can sign in on the front end 
            List<Subscriber> subscribers = new List<Subscriber>()
            {
                new Subscriber()
                {
                    email = "cans4cosmo@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Cosmo",
                    location = locations[0],
                    billingLocation = locations[0]
                },
                new Subscriber()
                {
                    email = "nathanissupercool101@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Nathan",
                    location = locations[1],
                    billingLocation = locations[1]
                },
                new Subscriber()
                {
                    email = "cosmonoregion@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "NathanNoRegion",
                    location = locations[2],
                    billingLocation = locations[2]
				},
				new Subscriber()
				{
                    email = "Everyone@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Cosmo",
                    location = locations[3],
                    billingLocation = locations[3]
                },
                //Use this one to test optoutID
                new Subscriber()
                {
                    email = "Moneybags@hotmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Money",
                    location = locations[4],
                    billingLocation = locations[4]
                },
                //3 more
                 new Subscriber()
                {
                    email = "NathisIsCool@hotmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Nathan",
                    location = locations[5],
                    billingLocation = locations[5]
                },
                new Subscriber()
                {
                    email = "RobMiller@saskpolytech.ca",
                    phoneNumber = "1234567890",
                    firstName = "Rob",
                    lastName = "Miller",
                    location = locations[0],
                    billingLocation = locations[0]
                },
                new Subscriber()
                {
                    email = "49ersSupportGroup@hotmail.com",
                    phoneNumber = "1234567890",
                    location = locations[1],
                    billingLocation = locations[1]
                },
                new Subscriber()
                {
                    email = "aaron_atkinson@hotmail.com",
                    firstName = "Aaron",
                    lastName = "Atkinson",
                    phoneNumber = "1234567890",
                    location = locations[2],
                    billingLocation = locations[2]
                },
                new Subscriber()
                {
                    email = "JessieSmith@gmail.com",
                    firstName = "Jessie",
                    lastName = "Smith",
                    phoneNumber = "1234567890",
                    location = locations[3],
                    billingLocation = locations[3]
                },
                new Subscriber()
                {
                    email = "Mario@mushroomkingdom.com",
                    firstName= "Mario",
                    phoneNumber = "1234567890",
                    location = locations[4],
                    billingLocation = locations[4]
                },
                new Subscriber()
                {
                    email = "cans4cosmotest@gmail.com",
                    firstName = "Luigi",
                    phoneNumber = "1234567890",
                    location = locations[5],
                    billingLocation = locations[5]
                },
                new Subscriber()
                {
                    email = "peach@mushroomkingdom.com",
                    firstName = "Peach",
                    phoneNumber = "1234567890",
                    location = locations[0],
                    billingLocation = locations[0]
                }

            
            };

            List<Subscriber> s79Subs = new List<Subscriber>();
            for (int i = 0; i < locations.Count - 6; i++)
            {
                Subscriber newSub = new Subscriber
                {
                    email = "hunter" + (i + 1) + "@thenewworld.com",
                    firstName = "Hunter" + (i + 1),
                    phoneNumber = "1111111111",
                    locationID = locations[6 + i].locationID,
                    billingLocationID = locations[6 + i].locationID
                };

                subscribers.Add(newSub);
            }
            return subscribers;
        }
        private List<LocationRoute> loadLocationRoutes(List<Location> locations, List<Route> routes)
        {
            return new List<LocationRoute>();
        }

        private List<Admin> loadAdmins()
        {
            //create a random salt for this test admin
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            
            //store the password by using the salt to encrypt the original password
            string password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: "9135f73499f84c7e70bcb5f582e4bf13ed78c899261e6b87c05454e2515c578a", //The SHA-256 hash of Cosmo123
                salt: salt, //salt value
                prf: KeyDerivationPrf.HMACSHA256, //encrypt using SHA256
                iterationCount: 10000, //run through the algorithm 10000 times
                numBytesRequested: 256 / 8 //return 64 byte string
                ));

            //save the admin to the database with the username and encrypted password
            Admin admin = new Admin { username = "Cans4Cosmo", password = password, salt = salt };
            List<Admin> admins = new List<Admin>();
            admins.Add(admin);

            return admins;
        }

        private void HighlyPopulateRoute(CosmoContext context, Route r)
        {
            Region reg = context.Region.Where(rg => rg.regionName.Equals("A Past Region")).FirstOrDefault(); //Gets Regions from context. Needed to assign required regionID and region properties
            List<Location> l = context.Location.ToList<Location>();
            List<Subscriber> s = context.Subscriber.ToList<Subscriber>();
            for (int i = 0; i <= 100; i++)
            {
                Location loc = new Location
                {
                    regionID = reg.regionID,
                    address = i + " Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                };

                Subscriber sub = new Subscriber()
                {
                    email = "Z" + i + "@mushroomkingdom.com",
                    firstName = "Test" + i,
                    phoneNumber = "1234567890",
                    location = loc,
                    billingLocation = loc
                };

                context.Subscriber.Add(sub);
                context.Location.Add(loc);
            }
        }

        private void loadLocations(CosmoContext context, List<Region> regions)
        {
            List<Location> locations = new List<Location>()
            {
                //Location for the first hard coded subscribers
                //Do not change
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                    region = regions[3]
                },
                //Location for the second hard coded subscribers
                //Do not change
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                    region = regions[4]
                },
                //Location for the third hard coded subscribers
                //Do not change
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp"//,
                    //region = regions[7]
                },
                //Using this for testing due to it having a proper route that is active and not completed
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                    region = regions[0]
                },
                //Location for the second hard coded subscribers
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                    region = regions[4]
                },
                //Location for the third hard coded subscribers
                new Location
                {
                    address = "123 Street",
                    city = "Saskatoon",
                    postalCode = "A1A1A1",
                    province = "SK",
                    locationType = "PickUp",
                    region = regions[5]
                }
            };

            context.AddRange(locations);
            context.SaveChanges();
            //S79 Locations
            Region s79Reg = regions.Where(reg => reg.regionName == "The New World").Single();
            List<Location> S79LocAssigned = new List<Location>();
            List<Location> S79LocUnassigned = new List<Location>();

            string[] strAssignedLocs = { "Astera", "Ancient Forest", "Wildspire Wastes", "Coral Highlands", "Rotten Vale", "Elder's Recess", "Great Ravine", "Everstream", "Confluence of Fates", "Research Base" };
            string[] strAssignedUnits = { "1A", "2B", "3C", "11A", "11B", "11C"};
            for (int i = 0; i < strAssignedLocs.Length; i++)
            {
                if(i < 6)
                {
                    S79LocAssigned.Add(new Location()
                    {
                        city = "World",
                        region = s79Reg,
                        regionID = s79Reg.regionID,
                        address = "123 " + strAssignedLocs[i] + " Path",
                        unit = strAssignedUnits[i],
                        postalCode = "A1A 1A1",
                        province = "SK",
                        locationType = "Pickup",
                    });
                }
                else
                {

                    S79LocAssigned.Add(new Location()
                    {
                        city = "World",
                        region = s79Reg,
                        regionID = s79Reg.regionID,
                        address = "123 " + strAssignedLocs[i] + " Path",
                        postalCode = "A1A 1A1",
                        province = "SK",
                        locationType = "Pickup",
                    });
                }

            }

            context.AddRange(S79LocAssigned);
            context.SaveChanges();

            string[] strUnassignedLocs = { "Seliana", "Hoarfrost Reach", "Guiding Lands", "Origin Isle", "Canteen", "Smithy", "The Gathering Hub", "Living Quarters", "Private Quarters", "Private Suite" };
            string[] strUnassignedUnits = { "123", "456", "789", "001" };

            for (int i = 0; i < strUnassignedLocs.Length; i++)
            {
                if(i < 4)
                {
                    S79LocUnassigned.Add(new Location()
                    {
                        city = "Iceborne",
                        address = "123 " + strUnassignedLocs[i] + " Way",
                        unit = strUnassignedUnits[i],
                        postalCode = "A1A 1A1",
                        province = "SK",
                        locationType = "Pickup",
                    });
                }
                else
                {
                    S79LocUnassigned.Add(new Location()
                    {
                        city = "Iceborne",
                        address = "123 " + strUnassignedLocs[i] + " Way",
                        postalCode = "A1A 1A1",
                        province = "SK",
                        locationType = "Pickup",
                    });
                }

            }

            context.AddRange(S79LocUnassigned);
            context.SaveChanges();
            //return S79Locs;
        }

    }

}
