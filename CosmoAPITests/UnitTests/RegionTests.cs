using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using CosmoAPI.Models;

namespace CosmoAPITests.UnitTests
{
    //Class is responsible for testing all cases which pertain to the Region object
    public class RegionTests
    {
        /// <summary>
        /// This Test evaluates a new region failing to create without locations added to it 
        /// </summary>
        [Fact]
        public void isValidRegionCreated_ValidRegionWithoutLocations_RegionCreated()
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = 10,
                firstDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);

            Assert.Empty(results);

        }

        
        /// <summary>
        /// This Test evaluates a new region failing to create with an ID being passed in
        /// 
        /// Maybe change this test into an End-to-End and handled in POST methods? 
        /// Validate is being called on PUT calls and causes errors as object PK_IDs are already set.
        /// Think we only need PK_ID errors on POST calls
        /// </summary>
        //[Fact]
        //public void isRegionIDSetInvalid_RegionIDIsSet_ErrorReturnedRegionNotCreated()
        //{
        //    DateTime date = DateTime.Today;

        //    Region rOne = new Region()
        //    {
        //        regionID = 111,
        //        regionName = "South End",
        //        frequency = 10,
        //        firstDate = new DateTime(date.Year + 1, date.Month, date.Day)
        //    };

        //    List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);

        //    Assert.Single(results);
        //    Assert.Equal("Region ID cannot be set", results[0].ToString());
        //}


        /// <summary>
        /// This Test evaluates the Valid Region Names on Region Creation
        /// </summary>
        /// <param name="validName"></param>
        [Theory]
        [InlineData("SE")]
        [InlineData("South End")]
        [InlineData("The area around the river and the bridge")]
        public void isRegionNameValid_validRegionName_RegionCreated(string validName)
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = validName,
                frequency = 10,
                firstDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);

            Assert.Empty(results);
        }

        /// <summary>
        /// This Test evaluates Invalid Region Names on Region Creation
        /// </summary>
        /// <param name="invalidName"></param>
        [Theory]
        [InlineData("")]
        [InlineData("S")]
        [InlineData("The area around the river and the bridges")]
        public void isRegionNameInvalid_inValidRegionName_RegionIsNotCreated(string invalidName)
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = invalidName,
                frequency = 10,
                firstDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);
            Assert.Single(results);

            switch (rOne.regionName.Length)
            {
                case int n when n < 2:
                    Assert.Equal("Region name must be at least 2 characters long", results[0].ToString());
                    break;
                case int n when n > 40:
                    Assert.Equal("Region name cannot exceed 40 characters in length", results[0].ToString());
                    break;
            }
        }

        /// <summary>
        /// This Test evaluates edge cases on the Allowed Dates on region Creation
        /// </summary>
        /// <param name="day">A value to signify a Day</param>
        /// <param name="month">A value to signify a month</param>
        /// <param name="year">A value to signify a Year</param>
        [Theory]
        [InlineData(1,1)]
        [InlineData(31, 1)]
        [InlineData(30, 4)]
        
        [InlineData(1, 12)]
        public void isRegionDateValid_ValidRegionFirstDate_RegionIsCreated(int day, int month)
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = 10,
                firstDate = new DateTime(date.Year + 1, month, day)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);

            Assert.Empty(results);
        }


        /// <summary>
        /// This Test evaluates the creation of a region that has a date set on the highest date value for Feburary, Leap Year or not
        /// </summary>
        /// <param name="day">A value to signify a Day</param>
        /// <param name="month">A value to signify a month</param>
        /// <param name="year">A value to signify a Year</param>
        [Theory]
        [InlineData(29, 2,  2020)]
        [InlineData(28, 2,  2021)]
        public void areLeapYearsValid_ValidLeapYearFirstDates_RegionIsCreated(int day, int month, int year)
        {
            DateTime date = new DateTime(year, month, day);

            //used to FUTURE PROOF the test cases, since the days may be in the past and fail validation
            int yearIncrement = 1;

            
            if (DateTime.IsLeapYear(date.Year))
            {
                yearIncrement = 4;
            }
            else
            {
                yearIncrement = 1;
            }
            while (date < DateTime.Today)
            {
                date = new DateTime(year + yearIncrement, month, day);
            }



            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = 10,
                firstDate = new DateTime(date.Year, month, day)
            };
        }

        /// <summary>
        /// This Test eevaulates edge cases on the incorrect Dates on Region Creation
        /// </summary>
        /// <param name="day">A value to signify a Day</param>
        /// <param name="month">A value to signify a month</param>
        /// <param name="year">A value to signify a Year</param>
        [Theory]
        [InlineData(0, 1, 2020)]
        [InlineData(32, 1, 2020)]
        [InlineData(30, 2, 2020)]
        [InlineData(31, 4, 2020)]
        [InlineData(29, 2, 2021)]
        [InlineData(1, 0, 2020)]
        [InlineData(1, 13, 2020)]
        public void isFirstDateInvalid_InvalidRegionFirstDate_ErrorReturnedRegionNotCreated(int day, int month, int year)
        {
            var ex = Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                Region rOne = new Region()
                {
                    regionName = "South End",
                    frequency = 10,
                    firstDate = new DateTime(year, month, day)
                };
            });

            

        }

        /// <summary>
        /// This test evaluates invalid Region Creation when a first date is set in the past
        /// </summary>
        //[Fact]
        //public void isFirstDatePast_InvalidRegionFirstDateInPast_ErrorReturnedRegionNotCreated()
        //{
        //    DateTime date = new DateTime(1920, 1, 1);

        //    Region rOne = new Region()
        //    {
        //        regionName = "South End",
        //        frequency = 10,
        //        firstDate = date
        //    };

        //    List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);
        //    Assert.Single(results);
        //    Assert.Equal("Invalid Date", results[0].ToString());

        //}

        /// <summary>
        /// This Test evaulates a Region without a first Date on Region Creation
        /// </summary>
        //[Fact]
        //public void isFirstDateRequired_InvalidRegionNoFirstDate_ErrorReturnedRegionNotCreated()
        //{
        //    Region rOne = new Region()
        //    {
        //        regionName = "South End",
        //        frequency = 10
        //    };

        //    List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);

        //    Assert.Single(results);

        //    Assert.Equal("Invalid Date", results[0].ToString());
        //}

        /// <summary>
        /// This Test evaulates the Invalid Frequency Boundary case on Region Creation
        /// </summary>
        /// <param name="Frequency">The value corresponding the Number of weeks between pick ups</param>
        [Theory]
        [InlineData(0)]
        [InlineData(53)]
        public void isFrequencyInvalid_InvalidRegionFrequency_ErrorReturnedRegionNotCreated(int Frequency)
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = Frequency,
                firstDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);
            Assert.Single(results);

            Assert.Equal("Region frequency must be between 1 and 52", results[0].ToString());
        }


        /// <summary>
        /// This Test evaulates the Valid Frequency Boundary case on Region Creation
        /// </summary>
        /// <param name="Frequency">The value corresponding the Number of weeks between pick ups</param>
        [Theory]
        [InlineData(1)]
        [InlineData(52)]
        public void isFrequencyValid_ValidRegionFrequency_regionIsCreated(int Frequency)
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = Frequency,
                firstDate = new DateTime(date.Year + 1, 1, 1)
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);
            Assert.Empty(results);
        }


        #region Story 62 Region Tests

        [Fact]

        public void isRegionCreatedWithStatus_RegionWithStatus_RegionIsCreated()
        {
            DateTime date = DateTime.Today;

            Region rOne = new Region()
            {
                regionName = "South End",
                frequency = 10,
                firstDate = new DateTime(date.Year + 1, 1, 1),
                inactive = false
            };

            List<ValidationResult> results = ClassValidator<Region>.Validate(rOne);
            Assert.Empty(results);
        }

        #endregion  
    }
}
