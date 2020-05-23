using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using CosmoAPI.Models;

namespace CosmoAPITests.Fixtures
{
    public class RegionFixture
    {

        /// <summary>
        /// Method for creating a list of pre-determined regions. This will generate dates relative to today's date
        /// </summary>
        /// <returns></returns>
        public static List<Region> GetRegionFixture()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1.0); //Gets today's date

            return new List<Region>()   //Defining a list of Regions to load into context
            {
                new Region()
                {
                    regionID = default(int),
                    regionName ="Harbor Creek",
                    frequency = 10,
                    firstDate = tomorrow   //Adds tomorrow as region's firstDate
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "Downtown",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(1.0) //Adds the day after as region's firstDate
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "South End",
                    frequency = 10,
                    firstDate = tomorrow.AddDays(2.0) //Adds the date in three days as region's firstDate
                },
                new Region()
                {
                    regionID = default(int),
                    regionName = "The New World",
                    frequency = 10,
                    firstDate = new DateTime(2017,12,9)
                }
            };
        }

        /// <summary>
        /// Calls GetRegionFixture to get a list of preset regions to load to context
        /// </summary>
        /// <param name="context"></param>
        public static void Load(CosmoContext context)
        {
            context.AddRange(GetRegionFixture());
            context.SaveChanges();
        }

        /// <summary>
        /// Removes any regions currently in the context
        /// </summary>
        /// <param name="context"></param>
        public static void Unload(CosmoContext context)
        {
            context.RemoveRange(context.Region);
            context.SaveChanges();
        }


    }
}
