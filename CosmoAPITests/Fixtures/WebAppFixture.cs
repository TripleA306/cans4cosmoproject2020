using CosmoAPI;
using CosmoAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CosmoAPITests.Fixtures
{
    
    public class WebAppFixture<TStartup> : IDisposable
    {
        private static DateCalculation dateCalculation;

        private readonly TestServer _testServer;
        public HttpClient TestClient { get; }
        public CosmoContext DBContext
        {
            get
            {
                var context = _testServer.Host.Services.GetService<CosmoContext>();
                context.Database.EnsureCreated();
                return context;
            }
        }

        public WebAppFixture() : this(Path.Combine("Intro")) { }//folder name of ASPNet Core project

        protected WebAppFixture(string relativeTargetProjectPatentDir)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    //Find and Remove the webapp's database context "CosmoContext" registration.
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CosmoContext>));
                    if (descriptor != null) { services.Remove(descriptor); }

                    // Add new database context "CosmoContext" that points to testing database "TestCosmoDB"
                    //services.AddDbContext<CosmoContext>(options => { options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestCosmoDB;Trusted_Connection=True;MultipleActiveResultSets=true"); });
                    services.AddDbContext<CosmoContext>(options => options.UseInMemoryDatabase("TestDB"));
                })
                .UseEnvironment("Testing")
                .UseStartup<Startup>();

            _testServer = new TestServer(builder) { BaseAddress = new Uri("http://localhost:5002") };

            TestClient = _testServer.CreateClient();
            TestClient.BaseAddress = _testServer.BaseAddress;

            dateCalculation = new DateCalculation();
        }

        public async void Load(CosmoContext context)
        {
            await unloadDB(context);
            
            List<Region> regionList = getRegions();
            context.Region.AddRange(regionList);
            context.SaveChanges();
            regionList = regionList.OrderBy(reg => reg.regionID).ToList();

            getLocations(context, regionList);
            List<Location> locationList = context.Location.OrderBy(loc => loc.locationID).ToList();

            List<Route> routeList = getRoutes(regionList);
            context.Route.AddRange(routeList);
            context.SaveChanges();

            List<Subscriber> subscriberList = getSubscribers(locationList, context);
            //context.Subscriber.AddRange(subscriberList);
            //context.SaveChanges();

            List<Admin> adminList = getAdmins();
            context.Admin.AddRange(adminList);
            context.SaveChanges(); //Save changes to DB
        }


        public void LoadLogansLoopLocations(CosmoContext context, List<Region> regionList)
        {
            unloadDB(context);
            List<Location> locations = new List<Location>();
            List<Subscriber> subscribers = new List<Subscriber>();


            locations.Add(new Location
            {
                address = "123 street",
                province = "Saskatchewan",
                postalCode = "A1A1A1",
                optoutLocationRouteList = new List<LocationRoute>(),
                regionID = regionList[0].regionID
            });
            subscribers.Add(new Subscriber
            {
                email = "Cans4Cosmo@gmail.com",
                phoneNumber = "1234567890",
                firstName = "Cosmo",
                location = locations[0]
            }) ;
            locations.Add(new Location
            {
                address = "456 street",
                province = "Saskatchewan",
                postalCode = "A2A2A2",
                optoutLocationRouteList = new List<LocationRoute>(),
                regionID = regionList[0].regionID
            });
            subscribers.Add(new Subscriber
            {
                email = "nathanissupercool101@gmail.com",
                phoneNumber = "1234567890",
                firstName = "Nathan",
                location = locations[1]
            });
            locations.Add(new Location
            {
                address = "123 street",
                province = "Saskatchewan",
                postalCode = "A2A2A2",
                optoutLocationRouteList = new List<LocationRoute>(),
                regionID = regionList[0].regionID
            });
            subscribers.Add(new Subscriber
            {
                email = "Everyone@gmail.com",
                phoneNumber = "1234567890",
                firstName = "Cosmo",
                location = locations[2]
            });

            context.AddRange(locations);
            context.AddRange(subscribers);

        }

            private static Task<int> unloadDB(CosmoContext context)
        {
            context.RemoveRange(context.Subscriber); //Remove contents of Subscriber table from context
            context.RemoveRange(context.Location); //Remove contents of Location table from context
            context.RemoveRange(context.Route); //Remove contents of Route table from context
            context.RemoveRange(context.Region); //Remove contents of Region table from context
            context.RemoveRange(context.Admin);
            return context.SaveChangesAsync();  //Save changes
        }

        public void Dispose()
        {  
            TestClient.Dispose();
            _testServer.Dispose();
        }

        private static void getLocations(CosmoContext context, List<Region> regions)
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
            string[] strAssignedUnits = { "1A", "2B", "3C", "11A", "11B", "11C" };
            for (int i = 0; i < strAssignedLocs.Length; i++)
            {
                if (i < 6)
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
                if (i < 4)
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
        }

        private static List<Region> getRegions()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1.0); //Gets today's date

            List<Region> regionList = new List<Region>()   //Defining a list of Regions to load into context
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
            return regionList;
        }

        private static List<Route> getRoutes(List<Region> regions)
        {
            //List<Region> regions = context.Region.ToList<Region>(); //Gets Regions from context. Needed to assign required regionID and region properties

            List<Route> routeList = new List<Route>()  //Creating route objects. Dates are generated based on region's first pickup date. Set it up to generate the next three upcoming sets of route dates
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

            return routeList;
        }

        private static List<Subscriber> getSubscribers(List<Location> locationList, CosmoContext context)
        {
            List<Subscriber> subscriberList = new List<Subscriber>()
            {
                new Subscriber()
                {
                    email = "cans4cosmo@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Cosmo",
                    location = locationList[0],
                    billingLocation = locationList[0]
                },
                new Subscriber()
                {
                    email = "nathanissupercool101@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Nathan",
                    location = locationList[1],
                    billingLocation = locationList[1]
                },
                new Subscriber()
                {
                    email = "cosmonoregion@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "NathanNoRegion",
                    location = locationList[2],
                    billingLocation = locationList[2]
                },
                new Subscriber()
                {
                    email = "Everyone@gmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Cosmo",
                    location = locationList[3],
                    billingLocation = locationList[3]
                },
                //Use this one to test optoutID
                new Subscriber()
                {
                    email = "Moneybags@hotmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Money",
                    location = locationList[4],
                    billingLocation = locationList[4]
                },
                //3 more
                 new Subscriber()
                {
                    email = "NathisIsCool@hotmail.com",
                    phoneNumber = "1234567890",
                    firstName = "Nathan",
                    location = locationList[5],
                    billingLocation = locationList[5]
                },
                new Subscriber()
                {
                    email = "RobMiller@saskpolytech.ca",
                    phoneNumber = "1234567890",
                    firstName = "Rob",
                    lastName = "Miller",
                    location = locationList[0],
                    billingLocation = locationList[0]
                },
                new Subscriber()
                {
                    email = "49ersSupportGroup@hotmail.com",
                    phoneNumber = "1234567890",
                    location = locationList[1],
                    billingLocation = locationList[1]
                },
                new Subscriber()
                {
                    email = "aaron_atkinson@hotmail.com",
                    firstName = "Aaron",
                    lastName = "Atkinson",
                    phoneNumber = "1234567890",
                    location = locationList[2],
                    billingLocation = locationList[2]
                },
                new Subscriber()
                {
                    email = "JessieSmith@gmail.com",
                    firstName = "Jessie",
                    lastName = "Smith",
                    phoneNumber = "1234567890",
                    location = locationList[3],
                    billingLocation = locationList[3]
                },
                new Subscriber()
                {
                    email = "Mario@mushroomkingdom.com",
                    firstName= "Mario",
                    phoneNumber = "1234567890",
                    location = locationList[4],
                    billingLocation = locationList[4]
                },
                new Subscriber()
                {
                    email = "cans4cosmotest@gmail.com",
                    firstName = "Luigi",
                    phoneNumber = "1234567890",
                    location = locationList[5],
                    billingLocation = locationList[5]
                },
                new Subscriber()
                {
                    email = "peach@mushroomkingdom.com",
                    firstName = "Peach",
                    phoneNumber = "1234567890",
                    isBusiness = false,
                    location = locationList[locationList.Count - 2],
                    billingLocation = locationList[locationList.Count - 2]
                }
            };

            context.Subscriber.AddRange(subscriberList);
            context.SaveChanges();
            List<Subscriber> hunters = new List<Subscriber>();

            for (int i = 0; i < locationList.Count - 6; i++)
            {
                Subscriber newSub = new Subscriber
                {
                    email = "hunter" + (i + 1) + "@thenewworld.com",
                    firstName = "Hunter" + (i + 1),
                    phoneNumber = "1111111111",
                    locationID = locationList[6 + i].locationID,
                    billingLocationID = locationList[6 + i].locationID
                };

                hunters.Add(newSub);
            }
            context.Subscriber.AddRange(hunters);
            context.SaveChanges();

            Route r = context.Route.Where(e => e.routeName.Equals("S90 A Past Route")).FirstOrDefault();
            List<Location> l = context.Location.ToList<Location>();
            for (int i = 0; i <= 100; i++)
            {
                Location loc = new Location
                {
                    regionID = r.regionID,
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
            context.SaveChanges();



            return subscriberList;
        }

        private List<Admin> getAdmins()
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

    }
}
