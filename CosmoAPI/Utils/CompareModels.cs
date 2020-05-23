using CosmoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CosmoAPI.Utils
{
    public class CompareModels
    {
        public int CompareRoutes(Route route1, Route route2)
        {
            int retVal = 0;
            foreach (PropertyInfo propertyInfo in route1.GetType().GetProperties())
            {
                if (!propertyInfo.Equals(route2.GetType().GetProperty(propertyInfo.Name)))
                {
                    retVal = -1;
                }
            }

            return retVal;
        }
    }
}
