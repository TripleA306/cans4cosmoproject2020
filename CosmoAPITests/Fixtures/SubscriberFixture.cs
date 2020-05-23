using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CosmoAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CosmoAPITests.Fixtures
{
    public class SubscriberFixture
    {
        /// <summary>
        /// This method will create a new list of routes for testing. Routes are created without RouteIDs
        /// so they can be added into the test DB without issues
        /// </summary>
        /// <returns></returns>
        public static List<Subscriber> GetDerivedSubscribers()
        {
            List<Subscriber> subscribers = new List<Subscriber>();

            for (int i = 1; i <= 10; i++)
            {
                subscribers.Add(new Subscriber()
                {
                    firstName = "John",
                    lastName = "Doe",
                    email = "cans4cosmotest@gmail.com",
                    phoneNumber = "1234567890",
                    isBusiness = false
                });
            }

            return subscribers;
        }

        /// <summary>
        /// Calls GetHardCodedRoutes and adds the newly created routes into the Route table of TestCosmoDB
        /// This method will need to be modified once additional data fixtures are added
        /// </summary>
        /// <param name="context"></param>
        public static void Load(CosmoContext context)
        {
            LocationFixture.Reload(context);
            List<Location> locations = context.Location.ToList<Location>();
            List<Subscriber> subscribers = GetDerivedSubscribers();

            int count = 0;
            foreach(Location l in locations)
            {
                subscribers[count].location = l;
                subscribers[count].billingLocation = l;
                count++;
            }

            context.AddRange(subscribers);
            context.SaveChanges();
        }

        /// <summary>
        /// This method will remove all Routes from the TestCosmoDB and then add new routes back into the TestCosmoDB
        /// Will need to update later to account for additional fixtures
        /// </summary>
        /// <param name="context"></param>
        public static void Reload(CosmoContext context)
        {
            context.RemoveRange(context.Subscriber); //Tracks the routes that are removed.
            context.SaveChanges();

            Load(context); //calls the Load method to load new routes into the TestCosmoDB
        }

        /// <summary>
        /// This method will remove the existing routes from the TestCosmoDB and commits the changes
        /// Will need to update later to account for additional fixtures
        /// </summary>
        /// <param name="context"></param>
        public static void Unload(CosmoContext context)
        {
            context.RemoveRange(context.Subscriber);
            context.SaveChanges();
        }
    }
}