using CosmoAPI;
using CosmoAPI.Models;
using CosmoAPITests.Fixtures;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using Xunit;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using CosmoAPITests.Utils;

namespace CosmoAPITests.EndToEndTests
{
    public class UnauthorizedAccessTests : IClassFixture<WebAppFixture<Startup>>
    {
        HttpClient _client;
        private readonly WebAppFixture<Startup> _webApp;
        Region region;
        Route route;
        Location location;
        Subscriber subscriber;
        Admin admin;

        string subscriberEmail = "Cans4Cosmo@gmail.com";

        public UnauthorizedAccessTests(WebAppFixture<Startup> webApp)
        {
            _webApp = webApp;

            _client = _webApp.TestClient;

            region = new Region
            {
                regionID = 1,
                regionName = "Test",
                frequency = 7,
                firstDate = DateTime.Now,
                inactive = false
            };

            route = new Route
            {
                routeID = 1,
                routeName = "TestRoute",
                region = region,
                completed = false,
                inactive = false,
                routeDate = DateTime.Now
            };

            location = new Location
            {
                locationID = 1,
                city = "Saskatoon",
                address = "123 Main Street",
                postalCode = "A1A 1A1",
                province = "SK",
                locationType = "PickUp",
                region = region
            };

            subscriber = new Subscriber
            {
                subscriberID = 1,
                firstName = "Brett",
                lastName = "Hickie",
                email = "Cans4Cosmo@gmail.com",
                phoneNumber = "1234567890",
                locationID = 1,
                location = location
            };

            admin = new Admin
            {
                username = "Cans4CosmoTest",
                password = "9135f73499f84c7e70bcb5f582e4bf13ed78c899261e6b87c05454e2515c578a", //The SHA-256 hash of Cosmo123
            };

            _webApp.Load(_webApp.DBContext);
            //SubscriberFixture.Reload(_webApp.DBContext);
            //RegionFixture.Load(_webApp.DBContext);
            //LocationFixture.Reload(_webApp.DBContext);
            //AdminFixture.Load(_webApp.DBContext);
        }

        #region Story93_Unauthorized_Persons_Cannot_Access_API

        #region Unauthorized User Tests
        [Fact]
        public async void VerifyGetSubscribers_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSubscriberCount_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/totalRows-r");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSortedSubscribers_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/sortBy-sascsizePerPage-p5currentPage-c1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSpecificSubscriber_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSpecificSubscriberLocation_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/email-c=Cans4CosmoTest@gmail.com");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPostSubscriber_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(subscriber), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("api/Subscribers", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPutSubscriber_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(subscriber), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync("api/Subscribers/1", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyDeleteSubscriber_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.DeleteAsync("api/Subscribers/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetRoutes_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Routes/Regions");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSpecificRoute_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Routes/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetCompletedRoutes_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Routes/showComplete");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPutRoute_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync("api/Routes/1", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyDeleteRoute_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.DeleteAsync("api/Routes/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetRegions_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Regions");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSpecificRegion_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Regions/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetPickUpDateForRegion_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Regions/regionID-c=1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPutRegion_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(region), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync("api/Regions/1", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPostRegion_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(region), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("api/Regions", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetLocations_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Locations");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSpecificLocation_UnauthorizedUser_UserIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Locations/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetNextPickupDatesForLocation_UnauthorizedUser_UserIsUnauthorized()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            HttpResponseMessage response = await _client.GetAsync("api/Locations/locationID-c=1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPostLocations_UnauthorizedUser_UserIsUnauthorized()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var StringContent = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("api/Locations", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyCreateAdmin_UnauthorizedUser_UserIsUnauthorized()
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        #endregion

        #region Role Tests

        [Fact]
        public async void VerifyGetSubscriberCount_ValidSubscriber_SubscriberIsUnauthorized()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/email=" + subscriberEmail);

            string subscriberToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(subscriberToken);

            response = await _client.GetAsync("api/Subscribers/totalRows-r");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetSortedSubscribers_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.GetAsync("api/Subscribers/sortBy-sascsizePerPage-p5currentPage-c1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetOtherSubscriberLocation_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.GetAsync("api/Subscribers/5");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPutOtherSubscriber_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            var StringContent = new StringContent(JsonConvert.SerializeObject(subscriber), Encoding.UTF8, "application/json");

            response = await _client.PutAsync("api/Subscribers/3", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyDeleteSubscriber_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.DeleteAsync("api/Subscribers/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetRoutes_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.GetAsync("api/Routes/Regions");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPutRoute_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            var StringContent = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            response = await _client.PutAsync("api/Routes/1", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyDeleteRoute_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.DeleteAsync("api/Routes/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetRegions_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.GetAsync("api/Regions");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPostRegion_ValidSubscriber_SubscriberIsUnauthorized()
        {

            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            var StringContent = new StringContent(JsonConvert.SerializeObject(region), Encoding.UTF8, "application/json");

            response = await _client.PostAsync("api/Regions", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyGetPickupDatesForOtherLocation_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4Cosmo@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            response = await _client.GetAsync("api/Regions/regionID-c=1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void VerifyPostAdmin_ValidSubscriber_SubscriberIsUnauthorized()
        {
            string googleAuth = GoogleAuth.getGoogleAuth(_client);

            _client.DefaultRequestHeaders.Add("GoogleAuth", googleAuth);

            //Getting subscriber token from api
            var response = await _client.GetAsync("api/subscribers/email-d=Cans4CosmoTest@gmail.com");

            var responseString = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseString);

            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

            response = await _client.PostAsync("api/Admins", StringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #endregion
    }
}
