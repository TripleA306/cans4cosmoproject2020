using CosmoAPI;
using CosmoAPI.Models;
using CosmoAPITests.Fixtures;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

using Xunit;
using CosmoAPITests.Utils;

namespace CosmoAPITests.EndToEndTests
{
    public class SubscriberTests : IClassFixture<WebAppFixture<Startup>>
    {
        HttpClient _client; //the HTTP client handles HTTP requests to the API
        private readonly WebAppFixture<Startup> _webApp;    //WebAppFixture to be created using configurations found in Startup.cs
        //private readonly TokenClient _tokenClient;
        Admin admin;

        public SubscriberTests(WebAppFixture<Startup> webApp)
        {
            _webApp = webApp; //set this _webApp to the WebAppFixture that is passed in

            //Builds up a HTTP client object that handles redirects to the API
            //_client = _webApp.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            _client = _webApp.TestClient;

            //This sets up the TokenClient, which handles the authentication tokens that IdentityServer hands out upon user authorization
            //var disco = DiscoveryClient.GetAsync("https://localhost:5000").Result;
            //_tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            //var tokenResponse = _tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1").Result;
            //_client.SetBearerToken(tokenResponse.AccessToken);

            admin = new Admin
            {
                username = "Cans4Cosmo",
                password = "9135f73499f84c7e70bcb5f582e4bf13ed78c899261e6b87c05454e2515c578a", //The SHA-256 hash of Cosmo123
            };
        }

        #region Story54_Subscriber_Logs_In
        //This test will ensure that true is returned from the API when a get request with an email for an exisitng Subscriber is made
        [Theory]
        [InlineData("cans4cosmotest@gmail.com")]
        public async void VerifySubscriberExists_ValidSubscriber_UserLoggedIn(string email)
        {
            _webApp.Load(_webApp.DBContext);

            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=" + email);

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            //make get request to api with email passed in
            response = await _client.GetAsync("api/Subscribers/email=" + email);

            //get the response
            responseString = await response.Content.ReadAsStringAsync();

            Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(responseString);

            Assert.Equal(email, subscriber.email); //ensure expectecd value is returned
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); //ensure sattus code 200 returned

        }

        //This test will ensure that false is returned from the API when a get request with an email for an exisitng Subscriber is made
        [Theory]
        [InlineData("Cans4CosmoTest@gmail.com")]
        public async void VerifySubscriberDoesNotExist_ValidSubscriber_UserLoggedIn(string email)
        {
            _webApp.Load(_webApp.DBContext);
            //await _webApp.loadSubscriberAsync();

            //make a get request to the api with an email passed in
            var response = await _client.GetAsync("api/Subscribers/email=" + email);

            //get the response from the api
            var responseString = await response.Content.ReadAsStringAsync();

            string expectedReturnValue = ""; //expected return value

            //ensure the returned value matches expected value
            Assert.Equal(expectedReturnValue, responseString);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); //ensure status code 404 returned


        }
        #endregion

        #region Story_73 End to end tests
        /// <summary>
        /// This tests will attempt to add a Subscriber object with valid fields to the database.
        /// It will then check to make sure that the Subscriber actually exists in the database
        /// </summary>
        [Fact]
        public async void AddSubscriber_ValidSubscriber_SubscriberAddedToDB()
        {
            //obtained from google

            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Reload the database.
            //SubscriberFixture.Reload(_webApp.DBContext);
            _webApp.Load(_webApp.DBContext);
            //create a new subscriber to add to the database with valid fields
            //Subscriber NewSubscriber = DatabaseSubscriberFixture.GetDerivedSubscribers()[0];

            Subscriber NewSubscriber = new Subscriber
            {
                firstName = "John",
                lastName = "Doe",
                email = "Cans4CosmoTest@gmail.com",
                phoneNumber = "1234567890",
                isBusiness = false
            };

            Location location = _webApp.DBContext.Location.First();

            NewSubscriber.locationID = location.locationID;

            //convert the subsciber to a string for posting to the database
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(NewSubscriber), Encoding.UTF8, "application/json");

            //send a post request to the api with the Subscriber object in JSON form
            var response = await _client.PostAsync("api/Subscribers", stringContent);
            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();
            //deserialize the response into a Subscriber object
            Subscriber deserializedSubscriber = JsonConvert.DeserializeObject<Subscriber>(responseString);

            //created status code is 201
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //try to get the added subscriber form the database using the response data from earlier request.
            response = await _client.GetAsync("api/Subscribers/" + deserializedSubscriber.subscriberID);

            //read the response as a string
            responseString = await response.Content.ReadAsStringAsync();

            //deserialize the object retrieved from the database
            Subscriber GetSubscriber = JsonConvert.DeserializeObject<Subscriber>(responseString);

            //check each field of the retrieved Subscriber and ensure it is equal to the subscriber that was initially send in the POST request 
            Assert.IsType<int>(deserializedSubscriber.subscriberID);
            Assert.Equal(NewSubscriber.firstName, GetSubscriber.firstName);
            Assert.Equal(NewSubscriber.lastName, GetSubscriber.lastName);
            //Assert.Equal(NewSubscriber.Password, GetSubscriber.Password);
            Assert.Equal(NewSubscriber.email, GetSubscriber.email);
            Assert.Equal(NewSubscriber.phoneNumber, GetSubscriber.phoneNumber);
            Assert.Equal(NewSubscriber.isBusiness, GetSubscriber.isBusiness);
            Assert.IsType<int>(GetSubscriber.locationID);

        }

        /// <summary>
        /// This method will attempt to add a Subscriber with invalid attribute values
        /// The database should not add this subscriber and we will expect an HTTP response code of 422
        /// </summary>
        [Fact]
        public async void AddSubscriber_InvalidSubscriber_SubscriberNotAddedToDB()
        {
            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //Reload the database.
            //_webApp.ReloadDBFixtures();
            //SubscriberFixture.Reload(_webApp.DBContext);
            _webApp.Load(_webApp.DBContext);

            //create a new subscriber to add to the database with valid fields
            Subscriber NewSubscriber = _webApp.DBContext.Subscriber.ToList()[0];

            NewSubscriber.phoneNumber = "l";

            //convert the subsciber to a string for posting to the database
            string subscriberString = JsonConvert.SerializeObject(NewSubscriber);

            //Send a post request to the api with the invalid subscriber object
            response = await _client.PostAsync("api/Subscribers", new StringContent(subscriberString, Encoding.UTF8, "application/json"));


            //created status code is 400 - bad request since the object is invalid
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        /// <summary>
        /// This test will attempt to update an Existing subscriber field in the database with a valid new phone number
        /// The API should accept the request and update that existing Subscriber object in the database
        /// </summary>
        [Fact]
        public async void UpdateSubscriber_ValidSubscriber_SubscriberUpdatedInDB()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //get a subscriber form the database
            response = await _client.GetAsync("api/Subscribers");
            //deserialize the retrieved subscribers into a list of Subscriber objects
            string responseJSON = await response.Content.ReadAsStringAsync();
            List<Subscriber> actualSubscribers = JsonConvert.DeserializeObject<List<Subscriber>>(responseJSON);

            //use the first subscriber in the list as the subscriber to update
            //Subscriber UpdateSubscriber = actualSubscribers[100];
            Subscriber UpdateSubscriber = actualSubscribers.FirstOrDefault(sub => sub.email.Equals("cans4cosmotest@gmail.com"));

            //update the phone number
            UpdateSubscriber.phoneNumber = "2222222222";

            //Getting valid Google OAuth token
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            response = await _client.GetAsync("api/Subscribers/email-d=" + UpdateSubscriber.email);

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            //convert the subsciber to a string for posting to the database
            var stringContent = new StringContent(JsonConvert.SerializeObject(UpdateSubscriber), Encoding.UTF8, "application/json");
            response = await _client.PutAsync("api/Subscribers/" + UpdateSubscriber.subscriberID, stringContent);
            //API will send back the update Subscriber object.
            //deserialize that object into a Subscriber object
            responseJSON = await response.Content.ReadAsStringAsync();

            //no content status code is 204
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            _client.SetBearerToken(adminToken);
            //send a get request with the updated subscriber ID to the database and retrieve the updated subscriber
            response = await _client.GetAsync("api/Subscribers/" + UpdateSubscriber.subscriberID);
            responseJSON = await response.Content.ReadAsStringAsync();
            //deserialize the response into a Subscriber object
            Subscriber GetSubscriber = JsonConvert.DeserializeObject<Subscriber>(responseJSON);


            //check each field of the retrieved Subscriber and make sure it matches each field of the initial subscriber after the phone number was modified with the 
            //updated value
            Assert.IsType<int>(UpdateSubscriber.subscriberID);
            Assert.Equal(UpdateSubscriber.firstName, GetSubscriber.firstName);
            Assert.Equal(UpdateSubscriber.lastName, GetSubscriber.lastName);
            //Assert.Equal(UpdateSubscriber.Password, GetSubscriber.Password);
            Assert.Equal(UpdateSubscriber.email, GetSubscriber.email);
            Assert.Equal(UpdateSubscriber.phoneNumber, GetSubscriber.phoneNumber);
            Assert.Equal(UpdateSubscriber.isBusiness, GetSubscriber.isBusiness);
            //Assert.IsType<int>(UpdateSubscriber.locationID);


        }

        /// <summary>
        /// This test will attempt to update an Existing subscriber field in the database with an invalid new phone number
        /// The API should not accept the request and will not update that existing Subscriber object in the database.
        /// </summary>
        [Fact]
        public async void UpdateSubscriber_InvalidSubscriber_SubscriberNotUpdateInDB()
        {
            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //Reload the database.
            //_webApp.ReloadDBFixtures();
            //SubscriberFixture.Reload(_webApp.DBContext);
            _webApp.Load(_webApp.DBContext);

            //send a get request to the API to get all subscriber
            response = await _client.GetAsync("api/Subscribers");
            string responseJSON = await response.Content.ReadAsStringAsync();
            //deserialize all subscribers into a list of subscriber objects
            List<Subscriber> actualSubscribers = JsonConvert.DeserializeObject<List<Subscriber>>(responseJSON);

            //attempt to update the first subscriber in the lsit
            Subscriber UpdateSubscriber = actualSubscribers[0];
            UpdateSubscriber.phoneNumber = "2222";
            int id = UpdateSubscriber.subscriberID;

            //convert the subsciber to a string for posting to the database
            JsonMediaTypeFormatter format = new JsonMediaTypeFormatter();

            var stringContent = new ObjectContent(UpdateSubscriber.GetType(), UpdateSubscriber, format);
            response = await _client.PutAsync("api/Subscribers/" + id, stringContent);

            //created status code is 400 bad request - since the object is invalid
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void GetSubscriber_ValidRequest_AllSubscribersReturned()
        {
            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //_webApp.ReloadDBFixtures();
            //SubscriberFixture.Reload(_webApp.DBContext);
            _webApp.Load(_webApp.DBContext);

            //The subscribers that are expected to be returned
            List<Subscriber> expectedSubscribers = _webApp.DBContext.Subscriber.ToList();
            //Sends a get request to the API
            response = await _client.GetAsync("api/Subscribers");

            //String we expect to get back (in JSON form)
            string responseJSON = await response.Content.ReadAsStringAsync();

            //Deserailize the returned json string of subscribers into a list of subscriber objects
            List<Subscriber> actualSubscribers = JsonConvert.DeserializeObject<List<Subscriber>>(responseJSON);

            //Check subscriber acount returned to ensure the size is X
            //size is 111 instead of the original 10 since 100 subscriebrs are now added 
            //for CSV file checking
            Assert.Equal(134, actualSubscribers.Count);

            //Break each Subscriber up in the actualSubcribers List
            //This assertColleciton will:
            //Ensure the subscriberID is an int
            //And check each attribute that was returned against our hard coded objects from our ficture
            for (int i = 0; i < actualSubscribers.Count; i++)
            {
                Assert.IsType<int>(actualSubscribers[i].subscriberID);
                Assert.Equal(expectedSubscribers[i].locationID, actualSubscribers[i].locationID);
                Assert.Equal(expectedSubscribers[i].isBusiness, actualSubscribers[i].isBusiness);
                //Assert.Equal(expectedCollections[i].description, actualCollections[i].description);
            }

        }
        #endregion

        #region Subscriber Test Fixture Loads
        private void ReloadTestFixtures()
        {
            RegionFixture.Unload(_webApp.DBContext);
            RegionFixture.Load(_webApp.DBContext);
            SubscriberFixture.Reload(_webApp.DBContext);
        }
        #endregion
    }
}

