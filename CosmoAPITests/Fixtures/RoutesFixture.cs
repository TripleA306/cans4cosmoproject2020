
using CosmoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CosmoAPITests.Fixtures
{
    public class RoutesFixture
    {
        /// <summary>
        /// This method will create a new list of routes for testing. Routes are created without RouteIDs
        /// so they can be added into the test DB without issues
        /// </summary>
        /// <returns></returns>
        public static List<Route> GetRouteFixtures(CosmoContext context)
        {
            //DateTime date = DateTime.Today; //Gets todays date. Later used to ensure dates are in the future

            List<Region> regions = context.Region.ToList<Region>(); //Gets Regions from context. Needed to assign required regionID and region properties

            return new List<Route>()  //Creating route objects. Dates are generated based on region's first pickup date. Set it up to generate the next three upcoming sets of route dates
            {
                //Week 1 Routes
                new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=true },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=true },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=true },

                //Week 2 Routes
                new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=true },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=true },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=true },

                //Week 3 Routes
                new Route() { routeName="Logan's Loop",     regionID=regions[0].regionID, region=regions[0], completed=false  },
                new Route() { routeName="Nathan's Circuit", regionID=regions[1].regionID, region=regions[1], completed=false },
                new Route() { routeName="Original Name 3",  regionID=regions[2].regionID, region=regions[2], completed=false },
            };
        }

        /// <summary>
        /// Calls GetRoutesFixture and adds the newly created routes into the Route table of TestCosmoDB
        /// This method will need to be modified once additional data fixtures are added
        /// </summary>
        /// <param name="context"></param>
        public static void Load(CosmoContext context)
        {
            context.AddRange(GetRouteFixtures(context));
            context.SaveChanges();
        }


        /// <summary>
        /// This method will remove the existing routes from the TestCosmoDB and commits the changes
        /// Performed asynchronously to ensure fixtures are unloaded in the correct order
        /// </summary>
        /// <param name="context"></param>
        public static void Unload(CosmoContext context)
        {
            context.RemoveRange(context.Route);
            context.SaveChanges();
        }
    }
}
