using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoAPI.Models
{
    //Helper class for calculating the dates for the routes
    public class DateCalculation
    {

        /// <summary>
        /// This method will calculate a given amount of dates for a given region
        /// </summary>
        /// <param name="nAmountOfDatesToGet">The amount of dates you would like to recieve</param>
        /// <param name="region">The region for which dates you would like to see</param>
        /// <returns>A list of DateTime objects if successful, or an error message if not successful</returns>
        public List<DateTime> GetNextDates(int nAmountOfDatesToGet, Region region)
        {
            //region.regionID = default(int);

            //Check to validate inputs
            if (nAmountOfDatesToGet < 1 || nAmountOfDatesToGet > 6)
            {
                throw new ArgumentException("The amount of dates to get must be between 1 and 6");
            }

            //Create an instance of the validator
            ValidationContext context = new ValidationContext(region);
            //Declare a List to store the results after validating the region 
            List<ValidationResult> results = new List<ValidationResult>();

            //Use the validator to validate the region passed in 
            if (!Validator.TryValidateObject(region, context, results, true))
            {
                //Failed validation so throw error message
                throw new ArgumentException("The region is invalid");
            }


            //Initialize a new list to return once populated 
            List<DateTime> expectedDates = new List<DateTime>();
            //Set a variable to todays date
            DateTime today = DateTime.Now;
            //Set a variable to the regions first date initialy, will be changed later on 
            DateTime newDate = region.firstDate;

            //If the regions date is already in the future
            if (DateTime.Compare(today, region.firstDate) < 0)
            {
                //Add the dates to the list to return 
                for (int i = 0; i < nAmountOfDatesToGet; i++)
                {

                    //Apply the correct amounts to our date variable and add it to the list to be returned later
                    newDate = newDate.AddDays(region.frequency * i * 7);
                    expectedDates.Add(newDate);
                }
            }
            else
            {
                //While the date variable is still in the past, apply the correct amount of days to it
                //Until it is in the future, This will also be the first valid date
                while (DateTime.Compare(today, newDate) > 0)
                {
                    newDate = newDate.AddDays(region.frequency * 7);
                }

                for (int i = 0; i < nAmountOfDatesToGet; i++)
                {
                    //Add the date first here because the above loop already calculated the proper date
                    expectedDates.Add(newDate);
                    newDate = newDate.AddDays(region.frequency * 7);
                }
            }

            //Return the expected dates  
            return expectedDates;
        }

        public DateTime GetOneDate(Region region)
        {
            return GetNextDates(1, region).FirstOrDefault();
            //DateTime nextPickup = region.firstDate;

            //if (nextPickup < DateTime.Today)
            //{
            //    do
            //    {
            //        nextPickup = nextPickup.AddDays(region.frequency * 7);
            //    }
            //    while (nextPickup.Date < DateTime.Today);
            //}

            //return nextPickup;
        }
    }
}
