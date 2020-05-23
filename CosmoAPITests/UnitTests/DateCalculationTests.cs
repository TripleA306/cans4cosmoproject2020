using CosmoAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;

namespace CosmoAPITests.UnitTests
{
    public class DateCalculationTests
    {
        DateCalculation DateCalculator = new DateCalculation();


        //Valid Region
        Region region = new Region()
        {
            regionName = "Test Region",
            frequency = 10,
            firstDate = new DateTime(2019, 1, 1) //Jan 1st 2019 (In the past)
        };

        

        //This method will use todays date and the regions firstDate and frequency to populate a list of the 6 next dates
        private List<DateTime> GetExpectedResults(int amountOfDates)
        {

            //Initialize a new list to return once populated 
            List<DateTime> expectedDates = new List<DateTime>();
            DateTime today = DateTime.Now;
            DateTime newDate = region.firstDate;

            //If the regions date is already in the future
            if (DateTime.Compare(today, region.firstDate) < 0)
            {
                //Add the dates to the list to return 
                for (int i = 0; i < amountOfDates; i++)
                {
                    newDate = newDate.AddDays(region.frequency * 7);
                    expectedDates.Add(newDate);
                }
            }
            else
            {
                while (DateTime.Compare(today, newDate) > 0)
                {
                    newDate = newDate.AddDays(region.frequency * 7);
                }

                for (int i = 0; i < amountOfDates; i++)
                {
                    expectedDates.Add(newDate);
                    newDate = newDate.AddDays(region.frequency * 7);
                }
            }

            return expectedDates;
        }



        //Upper and Lower Boundary
        [Theory]
        [InlineData(6)]
        [InlineData(1)]
        public void isValidAmountOfDatesToGet_ValidAmount_ListReturned(int amountOfDates)
        {
            //List of what the actual method returns
            List<DateTime> results = DateCalculator.GetNextDates(amountOfDates, region);
            //List of what we are expecting results to look like 
            List<DateTime> expectedResults = GetExpectedResults(amountOfDates);

            //Check to see if they are the same 
            Assert.Equal(expectedResults, results);
        }

        //Upper and Lower Invalid Boundary
        [Theory]
        [InlineData(7)]
        [InlineData(0)]
        public void isValidAmountOfDatesToGet_InvalidAmount_ErrorReturned(int amountOfDates)
        {
            //If you want more info on how this works 
            //https://stackoverflow.com/questions/1609536/how-do-i-use-assert-throws-to-assert-the-type-of-the-exception

            //Set a variable to what the method returns, The assert.throws check the kind of exception we are to be expecting
            var ex = Assert.Throws<ArgumentException>(() => DateCalculator.GetNextDates(amountOfDates, region));

            //This assert will check that the error message is correct 
            Assert.Equal("The amount of dates to get must be between 1 and 6", ex.Message);

        }

        [Theory]
        [InlineData("2020-03-01")] //March 1st 2020
        public void firstDateInTheFuture_FirstDateInFuture_ListReturned(string futureDate)
        {
            region.firstDate = DateTime.ParseExact(futureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //List of what the actual method returns
            List<DateTime> results = DateCalculator.GetNextDates(3, region);
            //List of what we are expecting results to look like 
            List<DateTime> expectedResults = GetExpectedResults(3);

            //Check to see if they are the same 
            Assert.Equal(expectedResults, results);
        }

        [Theory]
        [InlineData("2018-01-01")] //Jan 1st 2018
        public void firstDateInThePast_FirstDateInPast_ListReturned(string pastDate)
        {
            region.firstDate = DateTime.ParseExact(pastDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //List of what the actual method returns
            List<DateTime> results = DateCalculator.GetNextDates(3, region);
            //List of what we are expecting results to look like 
            List<DateTime> expectedResults = GetExpectedResults(3);

            //Check to see if they are the same 
            Assert.Equal(expectedResults, results);
        }

        [Theory]
        [InlineData(100)]
        public void isValidRegion_invalidRegion_ErrorReturned(int frequency)
        {
            //Make the region invalid
            region.frequency = frequency;
            //Set a variable to what the method returns, The assert.throws check the kind of exception we are to be expecting
            var ex = Assert.Throws<ArgumentException>(() => DateCalculator.GetNextDates(3, region));

            //This assert will check that the error message is correct 
            Assert.Equal("The region is invalid", ex.Message);
        }


    }
}
