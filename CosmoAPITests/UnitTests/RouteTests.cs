using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using CosmoAPI.Models;

namespace CosmoAPITests
{
    public class RouteTests
    {
        /// <summary>
        /// This Mock region is used to satisfy the Region attribute for the Route Creation Tests
        /// </summary>
        Region rOne = new Region()
        {
            regionID = 1,
            regionName = "South End",
            frequency = 10,
            firstDate = new DateTime(2020, 1, 1)
        };

        DateCalculation dateCalculation = new DateCalculation();


        #region story64_admin_adds_views_route
        /// <summary>
        /// This test evaluates a general Route being created
        /// </summary>
        [Fact]
        public void isValidRouteCreated_ValidRoute_RouteCreated()
        {
            DateTime date = DateTime.Now;

            Route routeOne = new Route()
            {
                routeName = "Harbor Creek",
                regionID = rOne.regionID,
                region = rOne,
                completed = false,
                routeDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

            Assert.Empty(results);
        }

        /// <summary>
        /// This test evaulates the Successfull creation of a Route using Valid Route Dates
        /// </summary>
        /// <param name="day">A value to signify a Day</param>
        /// <param name="month">A value to signify a month</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(31, 1)]
        [InlineData(30, 4)]
        [InlineData(1, 12)]
        public void isDateValid_ValidRouteDate_RouteCreated(int day, int month)
        {
            DateTime date = DateTime.Today;


            Route routeOne = new Route()
            {
                routeName = "Harbor Creek",
                regionID = rOne.regionID,
                region = rOne,
                completed = false,
                routeDate = new DateTime(date.Year + 1, month, day)
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

            Assert.Empty(results);
        }

        /// <summary>
        /// This test evaulated a unsuccessful attempt at route creation using Invalid Route Date Values
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
        public void isRouteDateInvalid_InvalidRouteDate_ErrorReturnedRouteNotCreated(int day, int month, int year)
        {
            var ex = Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                Route routeOne = new Route()
                {
                    routeName = "Harbor Creek",
                    regionID = rOne.regionID,
                    region = rOne,
                    completed = false,
                    routeDate = new DateTime(year, month, day)
                };
            });

        }

        /// <summary>
        /// This test evaulates a successfull Route being created with route Date values on the maximum day value in FEB for leap years and non leap years
        /// </summary>
        /// <param name="day">A value to signify a Day</param>
        /// <param name="month">A value to signify a month</param>
        /// <param name="year">A value to signify a Year</param>
        [Theory]
        [InlineData(29, 2, 2020)]
        [InlineData(28, 2, 2021)]
        public void areLeapYearDatesValid_ValidRouteDateLeapYear_RouteIsCreated(int day, int month, int year)
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

            Route routeOne = new Route()
            {
                routeName = "Harbor Creek",
                regionID = rOne.regionID,
                region = rOne,
                completed = false,
                routeDate = new DateTime(date.Year, month, day)
            };
        }

        /// <summary>
        /// This test evaluates unsuccessful Route Creation using invalid upper and lower boundary Route Names
        /// </summary>
        /// <param name="routeName">A value signifying a RouteName</param>
        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("Sask polytechnic and 33rd street in saska")]
        public void isRouteNameInvalid_InvalidRouteName_ErrorReturnedRouteNotCreated(string routeName)
        {
            DateTime regionNextDate = dateCalculation.GetOneDate(rOne);

            Route routeOne = new Route()
            {
                routeName = routeName,
                regionID = rOne.regionID,
                region = rOne,
                completed = false,
                routeDate = regionNextDate
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);
            Assert.Single(results);

            switch (routeOne.routeName.Length)
            {
                case 0:
                    Assert.Equal("Route Name cannot be empty", results[0].ToString());
                    break;
                case int n when n > 0 && n < 2:
                    Assert.Equal("Route Name must be at least 2 characters", results[0].ToString());
                    break;
                case int n when n > 40:
                    Assert.Equal("Route Name cannot be greater than 40 characters", results[0].ToString());
                    break;
            }

        }

        /// <summary>
        /// This test evaluates successfull Route Creation using valid Upper and lower boundary Route Names 
        /// </summary>
        /// <param name="routeName">A value signifying a RouteName</param>
        [Theory]
        [InlineData("SA@EndOfStreet")]
        [InlineData("SK")]
        [InlineData("Sask poltechnic and 33rd street in sask")]
        public void isRouteNameValid_ValidRouteName_RouteIsCreated(string routeName)
        {
            DateTime date = DateTime.Today;

            Route routeOne = new Route()
            {
                routeName = routeName,
                regionID = rOne.regionID,
                region = rOne,
                completed = false,
                routeDate = new DateTime(date.Year + 1, date.Month, date.Day)
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

            Assert.Empty(results);
        }

        /// <summary>
        /// This Test evaulates unsuccessful Route Creation with an Route that already had an RouteID assigned
        /// 
        /// Maybe this test shoudl be moved to an end-to-end test? POST method can perform ID check and return related error. 
        /// Adding error to object causes issues when updating as PUT methods call the validator and the ID validation
        /// causes an unwanted error to throw - KW LO
        /// </summary>
        //[Fact]
        //public void isRouteIDSetInvalid_RouteIDSet_ErrorReturnedRouteNotCreated()
        //{
        //    DateTime date = DateTime.Today;
        //    Route routeOne = new Route()
        //    {
        //        routeID = 1,
        //        routeName = "Harbor Creek",
        //        regionID = rOne.regionID,
        //        region = rOne,
        //        completed = false,
        //        routeDate = new DateTime(date.Year + 1, date.Month, date.Day)
        //    };

        //    List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

        //    Assert.Single(results);
        //    Assert.Equal("Route cannot already have an ID", results[0].ToString());
        //}

        /// <summary>
        /// This test evaulates successful Route Creation with a valid Region
        /// </summary>
        [Fact]
        public void isValidRegionCreated_ValidRegionSet_RouteIsCreated()
        {
            Route routeOne = new Route()
            {
                routeName = "Harbor Creek",
                regionID = 5,
                region = new Region(),
                completed = false,
                routeDate = new DateTime(2021, 01, 01)
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

            Assert.Empty(results);
        }

        /// <summary>
        /// This test evaulates unsuccessfull Route Created with no Region being set
        /// </summary>
        [Fact]
        public void isRegionRequired_NoRegionSet_ErrorReturnedRouteNotCreated()
        {
            Route routeOne = new Route()
            {
                routeName = "Harbor Creek",
                completed = false,
                routeDate = new DateTime(2021, 01, 01)
            };

            List<ValidationResult> results = ClassValidator<Route>.Validate(routeOne);

            Assert.Single(results);
            Assert.Equal("Route must have a Region", results[0].ToString());
        }

        #endregion


    }
}
