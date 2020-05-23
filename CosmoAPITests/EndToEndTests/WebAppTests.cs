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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace CosmoAPITests.EndToEndTests
{
    public class WebAppTests : IClassFixture<WebAppFixture<Startup>>
    {

        HttpClient _client; //the HTTP client handles HTTP requests to the API
        private readonly WebAppFixture<Startup> _webApp;    //WebAppFixture to be created using configurations found in Startup.cs
        //private readonly TokenClient _tokenClient;
        Admin admin;

        public WebAppTests(WebAppFixture<Startup> webApp)
        {
            _webApp = webApp; //set this _webApp to the WebAppFixture that is passed in

            _client = _webApp.TestClient;

            admin = new Admin
            {
                username = "Cans4Cosmo",
                password = "9135f73499f84c7e70bcb5f582e4bf13ed78c899261e6b87c05454e2515c578a", //The SHA-256 hash of Cosmo123
            };
        }

        /// <summary>
        /// Test confirms that a DB Reset is possible through the controller when the application is in a development environment
        /// 
        /// Test checks that the current environment is Development, creates a new Region, adds it to the context.
        /// After being added, it verifies the Region was added. A request is made to the API to reset the database and
        /// then it is verified that the creatd Region no longer exists in the database
        /// </summary>
        [Fact]
        public async void DoesGetResetDBInDev_GetCallSentInDevMode_DBReset()
        {
            _webApp.Load(_webApp.DBContext);

            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //New Region object to add to DB
            Region newRegion = new Region
            {
                regionName = "DB Reset Region",
                frequency = 10,
                firstDate = DateTime.Now.AddDays(1.0),
                inactive = false
            };

            //convert the Region to a string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(newRegion), Encoding.UTF8, "application/json");

            //send a post request to the api with the Subscriber object in JSON form
            response = await _client.PostAsync("api/Regions", StringContent);

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Region object
            Region postRegion = JsonConvert.DeserializeObject<Region>(responseString);

            //created status code is 201
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            int regionID = postRegion.regionID; //Get returned region's ID

            //Attempts to find a single region with the appropriate name and date
            Region dbRegion = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionID == regionID).Single();

            //Confirms only one region is found
            Assert.Equal(newRegion.regionName, dbRegion.regionName);
            Assert.Equal(newRegion.frequency, dbRegion.frequency);
            Assert.Equal(newRegion.firstDate, dbRegion.firstDate);
            Assert.Equal(newRegion.inactive, dbRegion.inactive);

            //Send a PUT request to API to update the Route to completed
            response = await _client.GetAsync("api/WebApp/reloadDB");

            responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Database reset", responseString);

            List<Region> regions = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionID == regionID).ToList();

            Assert.Empty(regions);
        }

        [Fact]
        public async void DoesGetFailInProdEnvironment_GetCallSentInProductionMode_ErrorReturned()
        {
            //Changing environment variable to Production
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            //Send a GET request to API reset the DB
            HttpResponseMessage response = await _client.GetAsync("api/WebApp/reloadDB");

            //Reading the content string returned by response
            string responseString = await response.Content.ReadAsStringAsync();

            //Ensure 400 Status code reutrned with "Invalid Environment" error message
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Invalid Environment", responseString);

            //Resetting environment variable to Development just in case it doesn't after the test is done
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        }

        [Fact]
        public async void DoesResetLoadCorrectOrder_GetCallSentInDevMode_DBResetRecordsCorrect()
        {
            _webApp.Load(_webApp.DBContext);

            List<Admin> origAdmins = _webApp.DBContext.Admin.OrderBy(a => a.adminID).ToList();
            List<Location> origLocations = _webApp.DBContext.Location.OrderBy(l => l.locationID).ToList();
            List<Region> origRegions = _webApp.DBContext.Region.OrderBy(reg => reg.regionID).ToList();
            List<Route> origRoutes = _webApp.DBContext.Route.OrderBy(r => r.routeID).ToList();
            List<Subscriber> origSubscribers = _webApp.DBContext.Subscriber.OrderBy(s => s.subscriberID).ToList();

            //Send a PUT request to API to update the Route to completed
            HttpResponseMessage response = await _client.GetAsync("api/WebApp/reloadDB");

            string responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Database reset", responseString);

            List<Admin> dbAdmins = _webApp.DBContext.Admin.OrderBy(a => a.adminID).ToList();
            List<Location> dbLocations = _webApp.DBContext.Location.OrderBy(l => l.locationID).ToList();
            List<Region> dbRegions = _webApp.DBContext.Region.OrderBy(reg => reg.regionID).ToList();
            List<Route> dbRoutes = _webApp.DBContext.Route.OrderBy(r => r.routeID).ToList();
            List<Subscriber> dbSubscribers = _webApp.DBContext.Subscriber.OrderBy(s => s.subscriberID).ToList();

            Assert.Equal(origAdmins.First().username, dbAdmins.First().username);
            Assert.Equal(origAdmins.Last().username,  dbAdmins.Last().username);

            Assert.Equal(origLocations.First().city, dbLocations.First().city);
            Assert.Equal(origLocations.First().address, dbLocations.First().address);
            Assert.Equal(origLocations.First().postalCode, dbLocations.First().postalCode);
            Assert.Equal(origLocations.Last().city, dbLocations.Last().city);
            Assert.Equal(origLocations.Last().address, dbLocations.Last().address);
            Assert.Equal(origLocations.Last().postalCode, dbLocations.Last().postalCode);

            Assert.Equal(origRegions.First().regionName, dbRegions.First().regionName);
            Assert.Equal(origRegions.First().frequency, dbRegions.First().frequency);
            Assert.Equal(origRegions.First().firstDate, dbRegions.First().firstDate);
            Assert.Equal(origRegions.First().inactive, dbRegions.First().inactive);
            Assert.Equal(origRegions.Last().regionName, dbRegions.Last().regionName);
            Assert.Equal(origRegions.Last().frequency, dbRegions.Last().frequency);
            Assert.Equal(origRegions.Last().firstDate, dbRegions.Last().firstDate);
            Assert.Equal(origRegions.Last().inactive, dbRegions.Last().inactive);

            Assert.Equal(origRoutes.First().routeName,  dbRoutes.First().routeName);
            Assert.Equal(origRoutes.First().completed,  dbRoutes.First().completed);
            Assert.Equal(origRoutes.First().inactive,   dbRoutes.First().inactive);
            Assert.Equal(origRoutes.First().routeDate.ToString().Substring(0,10),  dbRoutes.First().routeDate.ToString().Substring(0, 10));            
            Assert.Equal(origRoutes.Last().routeName,  dbRoutes.Last().routeName);
            Assert.Equal(origRoutes.Last().completed,  dbRoutes.Last().completed);
            Assert.Equal(origRoutes.Last().inactive,   dbRoutes.Last().inactive);
            Assert.Equal(origRoutes.Last().routeDate.ToString().Substring(0, 10),  dbRoutes.Last().routeDate.ToString().Substring(0, 10));

            Assert.Equal(origSubscribers.First().firstName, dbSubscribers.First().firstName);
            Assert.Equal(origSubscribers.First().lastName, dbSubscribers.First().lastName);
            Assert.Equal(origSubscribers.First().email, dbSubscribers.First().email);
            Assert.Equal(origSubscribers.First().phoneNumber, dbSubscribers.First().phoneNumber);
            Assert.Equal(origSubscribers.First().inactive, dbSubscribers.First().inactive);
            Assert.Equal(origSubscribers.First().isBusiness, dbSubscribers.First().isBusiness);
            Assert.Equal(origSubscribers.Last().firstName, dbSubscribers.Last().firstName);
            Assert.Equal(origSubscribers.Last().lastName, dbSubscribers.Last().lastName);
            Assert.Equal(origSubscribers.Last().email, dbSubscribers.Last().email);
            Assert.Equal(origSubscribers.Last().phoneNumber, dbSubscribers.Last().phoneNumber);
            Assert.Equal(origSubscribers.Last().inactive, dbSubscribers.Last().inactive);
            Assert.Equal(origSubscribers.Last().isBusiness, dbSubscribers.Last().isBusiness);
        }

   

    }
}
