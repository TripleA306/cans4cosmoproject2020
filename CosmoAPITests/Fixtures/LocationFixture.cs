using System;
using System.Collections.Generic;
using System.Text;
using CosmoAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CosmoAPITests.Fixtures
{
    public static class LocationFixture
    {

        public static List<Location> GetDerivedLocations(CosmoContext context)
        {

            List<Location> locations = new List<Location>();

            for (int i = 0; i < 5; i++)
            {
                string sCity = "";
                switch (i)
                {
                    case (0):
                        sCity = "Saskatoon";
                        break;

                    case (1):
                        sCity = "Regina";
                        break;

                    case (2):
                        sCity = "Prince Albert";
                        break;

                    case (3):
                        sCity = "Swift Current";
                        break;

                    case (4):
                        sCity = "North Battleford";
                        break;
                }

                locations.Add(new Location()
                {
                    city = sCity,
                    locationType = "Pickup",
                    postalCode = "A1A1A1",
                    province = "Saskatchewan",
                    address = "123 Street"
                });
            }

            //S79 Locations
            Region s79Reg = context.Region.AsNoTracking().Where(reg => reg.regionName == "The New World").Single();

            string[] strAssignedLocs = { "Astera", "Ancient Forest", "Wildspire Wastes", "Coral Highlands", "Rotten Vale", "Elder's Recess" };
            string[] strAssignedUnits = { "1A", "2B", "3C", "11A", "11B", "11C" };
            for (int i = 0; i < strAssignedLocs.Length; i++)
            {
                locations.Add(new Location()
                {
                    city = strAssignedLocs[i],
                    region = s79Reg,
                    regionID = s79Reg.regionID,
                    address = "123 Illusory Path",
                    unit = strAssignedUnits[i],
                    postalCode = "A1A 1A1",
                    province = "SK",
                    locationType = "Pickup",
                });
            }

            string[] strUnassignedLocs = { "Seliana", "Hoarfrost Reach", "Guiding Lands", "Origin Isle" };
            string[] strUnassignedUnits = { "123", "456", "789", "001" };

            for (int i = 0; i < strUnassignedLocs.Length; i++)
            {
                locations.Add(new Location()
                {
                    city = strUnassignedLocs[i],
                    address = "123 Hidden Way",
                    unit = strUnassignedUnits[i],
                    postalCode = "A1A 1A1",
                    province = "SK",
                    locationType = "Pickup",
                });
            }

            return locations;
        }


        public static void Load(CosmoContext context)
        {
            context.AddRange(GetDerivedLocations(context));
            context.SaveChanges();

        }

        public static void Reload(CosmoContext context)
        {
            context.RemoveRange(context.Location);
            context.SaveChanges();

            Load(context);
        }

        public static void Unload(CosmoContext context)
        {
            context.RemoveRange(context.Location);
            context.SaveChanges();
        }
    }
}
