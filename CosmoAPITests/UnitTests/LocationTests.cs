using System;
using System.Collections.Generic;
using System.Text;
using CosmoAPI.Models;
using Xunit;
using System.Linq;

namespace CosmoAPITests.UnitTests
{
    /// <summary>
    /// Class will handle
    /// </summary>
    public class LocationTests
    {
        [Theory]
        [InlineData("1A")]
        [InlineData("10")]
        [InlineData("123")]
        [InlineData("A")]
        [InlineData("A-B")]
        [InlineData("110-123")]
        [InlineData("100-A")]
        public void IsLocationWithUnitCreated_ValidLocationWithUnit_LocationCreated(string sUnit)
        {
            Location loc = new Location
            {
                locationID = 3,
                city = "Flavortown",
                unit = sUnit,
                locationType = "pickup",
                postalCode = "S0K 1M0",
                province = "Saskatchewan",
                address = "123 street"
            };

            var errors = ClassValidator<Location>.Validate(loc); //Set errors to the returned value from the validator
            Assert.Empty(errors); //Check to make sure no errors are returned
        }

        [Theory]
        [InlineData("Unit 1")]
        [InlineData("Un!7 #1")]
        [InlineData("House #1")]
        [InlineData("!@#$%^&*()")]
        [InlineData("Unit_1")]
        public void IsUnitWithSpecialCharactersCreated_UnitWithSpecialCharacters_ErrorReturned(string sUnit)
        {
            string sCharError = "Unit/Apt. can only contain letters, numbers, and the - symbol";

            Location loc = new Location
            {
                locationID = 3,
                city = "Flavortown",
                unit = sUnit,
                locationType = "pickup",
                postalCode = "S0K 1M0",
                province = "Saskatchewan",
                address = "123 street"
            };

            var errors = ClassValidator<Location>.Validate(loc); //Set errors to the returned value from the validator
            Assert.Equal(sCharError, errors[0].ErrorMessage); //Check to make sure no errors are returned
        }

        [Fact]
        public void IsLocationWithoutUnitCreated_ValidLocationNoUnit_LocationCreated()
        {
            Location loc = new Location
            {
                locationID = 3,
                city = "Flavortown",
                locationType = "pickup",
                postalCode = "S0K 1M0",
                province = "Saskatchewan",
                address = "123 street"
            };

            var errors = ClassValidator<Location>.Validate(loc); //Set errors to the returned value from the validator
            Assert.Empty(errors); //Check to make sure no errors are returned
        }

        [Theory]
        [InlineData("SuperValidLocationThatIsNotTooLong")]
        [InlineData("16CharacterPlace")]
        public void IsUnitAddressLengthRestricted_UnitAddressTooLong_ErrorReturned(string sUnit)
        {
            string sCharError = "Unit/Apt. cannot exceed 15 characters in length";
            Location loc = new Location
            {
                locationID = 3,
                city = "Flavortown",
                unit = sUnit,
                locationType = "pickup",
                postalCode = "S0K1M0",
                province = "Saskatchewan",
                address = "123 street"
            };

            var errors = ClassValidator<Location>.Validate(loc); //Set errors to the returned value from the validator
            Assert.Equal(sCharError, errors[0].ErrorMessage); //Check to make sure no errors are returned
        }
    }
}
