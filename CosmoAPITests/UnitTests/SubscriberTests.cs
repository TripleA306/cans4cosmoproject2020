using System;
using System.Collections.Generic;
using System.Text;
using CosmoAPI.Models;
using Xunit;

namespace CosmoAPITests.UnitTests
{
    /// <summary>
    /// This class is responsible for Story 30 - Backend create a new Subscriber Object - it focuses on the Subscriber object
    /// It will test upper and lower bouddary cases for each attribute - if applicable
    /// </summary>
    public class SubscriberTests
    {
        /// <summary>
        /// Valid subscriber object used to run tests against
        /// </summary>
        Subscriber subscriber = new Subscriber()
        {
            firstName = "John",
            lastName = "Doe",
            email = "JohnDoe@gmail.com",
            phoneNumber = "0123456789",
            locationID = 1,
            billingLocationID = 2,
            isBusiness = false
        };

        Location location = new Location()
        {
            locationID = 1,
            city = "saskatoon",
            locationType = "pickup",
            postalCode = "S0K1M0",
            province = "Saskatchewan",
            address = "123 street"
        };

        Location billingLocation = new Location()
        {
            locationID = 2,
            city = "saskatoon",
            locationType = "billing",
            postalCode = "S0K1M0",
            province = "Saskatchewan",
            address = "123 street"
        };

        /// <summary>
        /// This will be called to reset the subscriber back to a state where it is entirely valid 
        /// </summary>
        private void ResetSubscriber()
        {
            subscriber = new Subscriber()
            {
                firstName = "John",
                lastName = "Doe",
                email = "JohnDoe@gmail.com",
                phoneNumber = "0123456789",
                locationID = 1,
                billingLocationID = 2,
                isBusiness = false
            };

        }

        #region Story30a_Create_Subscriber_Account
        #region LocationID
        /// <summary>
        /// This test will test the valid case for the locationID
        /// </summary>
        /// <param name="locationID">The int that will be assigned to the locationID attribute</param>
        [Theory]
        [InlineData(1)]//valid case
        public void isValidLocationID_ValidLocationID_NoErrorsReturned(int locationID)
        {
            subscriber.locationID = locationID; //Set the locationID to the int passed in
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the returned value from the validator
            Assert.Empty(errors); //Check to make sure no errors are returned
            ResetSubscriber();
        }

        /// <summary>
        /// This test will test the invalid cases for the locationID
        /// </summary>
        /// <param name="locationID">The int that will be assigned to the locationID attribute</param>
        [Theory]
        [InlineData(499)] //not stored ID
        public void isValidLocationID_InvalidLocationID_ErrorReturned(int locationID)
        {
            Location newBilling = new Location()
            {
                locationID = 500,
                city = "saskatoon",
                locationType = "billing",
                postalCode = "S0K 1M0",
                province = "Saskatchewan",
                address = "123 street"
            };

            subscriber.billingLocation = newBilling;
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the returned value from the validator
            Assert.Single(errors); //Check that one error is returned
            Assert.Equal("Must be a valid Location ID", errors[0].ErrorMessage);//check to make sure the error message is correct
            ResetSubscriber();
        }
        #endregion

        #region BillingLocation
        /// <summary>
        /// This test will test the valid billing location entry
        /// </summary>
        /// <param name="locationID"></param>
        [Theory]
        [InlineData(3)] //Valid case
        public void isValidBillingLocation_ValidBillingLocation_NoErrorsReturned(int locationID)
        {
            Location newBilling = new Location()
            {
                locationID = 3,
                city = "saskatoon",
                locationType = "pickup",
                postalCode = "S0K1M0",
                province = "Saskatchewan",
                address = "123 street"
            };
            subscriber.billingLocation = newBilling;
            subscriber.billingLocationID = locationID; //Set the attribute to the int being passed in
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the returned value from the validator
            Assert.Empty(errors); //Check to make sure no errors are returned
            ResetSubscriber();
        }

        /// <summary>
        /// This test will test the invalid billing location entry
        /// </summary>
        /// <param name="locationID"></param>
        [Theory]
        [InlineData(-1)]//invalid
        public void isValidBillingLocation_InvalidBillingLocation_ErrorReturned(int locationID)
        {
            subscriber.billingLocationID = locationID; //Set the attribute to the int being passed in
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the returned value from the validator
            Assert.Single(errors); //Make sure a single error is returned
            Assert.Equal("Must be a valid Location ID", errors[0].ErrorMessage); //Make sure the error returned is the proper error
            ResetSubscriber();
        }
        #endregion

        #region Is Business
        /// <summary>
        /// This test will test both valid values for the IsBuisness attribute
        /// </summary>
        /// <param name="isBusiness"></param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void isValidIsBusiness_ValidIsBusiness_NoErrorsReturned(bool isBusiness)
        {
            subscriber.isBusiness = isBusiness;//Set the attribute to the boolean being passed in
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the returned value from the validator
            Assert.Empty(errors);//Check to make sure no errors are returned
            ResetSubscriber();
        }
        #endregion

        #endregion

        #region story54_Subscriber_Logs_in
        /// <summary>
        /// This test will be for resting the valid cases for the email attribute
        /// </summary>
        /// <param name="email">The string that the emal attribute will be set to</param>
        [Theory]
        [InlineData("AAA@gmail.com")]//A valid email
        [InlineData("123@gmail.com")]//A valid email
        [InlineData("456@gmail.com")]//A valid email
        public void isValidEmail_ValidEmail_NoErrorsReturned(String email)
        {
            subscriber.email = email; //Set the attribute to the string being passed in 
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the return value from the validator
            Assert.Empty(errors); //Check to make sure no errors are returned
            ResetSubscriber(); //reset the subscriber
        }

        /// <summary>
        /// This test will be for resting the invalid cases for the email attribute
        /// </summary>
        /// <param name="email">The string that the emal attribute will be set to</param>
        [Theory]
        [InlineData("AAAGmail.com")] // A invalid format
        [InlineData("AAAG_mail.com")] // A invalid format
        public void isValidEmail_InvalidEmail_ErrorReturned(String email)
        {
            subscriber.email = email; //Set the attribute to the string being passed in 
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the return value from the validator
            Assert.Single(errors); //make sure only a single error is returned
            Assert.Equal("Must be a valid email address", errors[0].ErrorMessage);//Make sure the error message is the correct one
            ResetSubscriber();//reset the subscriber
        }
        #endregion


        #region  story82_Admin_Remove_list_Subscribers
        /// <summary>
        /// This test will be for resting the invalid cases for the email attribute
        /// </summary>
        /// <param name="email">The string that the emal attribute will be set to</param>
        [Theory]
        [InlineData(true)] // A invalid format
        [InlineData(false)] // A invalid format
        public void SetInactive_ValidValues_InactiveValueChanged(bool inactiveState)
        {
            subscriber.inactive = inactiveState; //Set the attribute to the string being passed in 
            var errors = ClassValidator<Subscriber>.Validate(subscriber); //Set errors to the return value from the validator
            Assert.Empty(errors); //make sure only a single error is returned
            ResetSubscriber();//reset the subscriber
        }
        #endregion

    }
}
