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
using Xunit.Abstractions;

namespace CosmoAPITests.EndToEndTests
{
    public class RegionTests : IClassFixture<WebAppFixture<Startup>>
    {
        private readonly ITestOutputHelper _output;

        HttpClient _client; //the HTTP client handles HTTP requests to the API
        private readonly WebAppFixture<Startup> _webApp;    //WebAppFixture to be created using configurations found in Startup.cs
        //private readonly TokenClient _tokenClient;


        public RegionTests(WebAppFixture<Startup> webApp, ITestOutputHelper output)
        {
            _webApp = webApp; //set this _webApp to the WebAppFixture that is passed in
            _output = output;
            _client = _webApp.TestClient;
        }

        #region Story 79 Admin adds/views/removes Locations to/in/from a Region

        [Fact]
        public async void CanGetRegionCount_ValidRegionID_LocationCountReturned()
        {
             _webApp.Load(_webApp.DBContext);;
            Region getRegion = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionName == "The New World").Single();
            int locCount = _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == getRegion.regionID
            && 0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count()).Count();

            HttpResponseMessage response = await _client.GetAsync("api/Locations/regcount=" + getRegion.regionID);

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            int returnCount = JsonConvert.DeserializeObject<int>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(locCount, returnCount);
        }

        [Fact]
        public async void CannotGetInvalidRegionCount_InvalidRegionID_ErrorReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            string expectedMessage = "Invalid Region ID";

            HttpResponseMessage response = await _client.GetAsync("api/Locations/regcount=0");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedMessage, responseString);
        }

        [Fact]
        public async void CanGetUnassignedLocationCount_GetRequestToController_UnassignedLocationCountReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            int locCount = _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == null 
            && 0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count()).Count();

            HttpResponseMessage response = await _client.GetAsync("api/Locations/unassigncount");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            int returnCount = JsonConvert.DeserializeObject<int>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(locCount, returnCount);
        }

        /// <summary>
        /// Tests that the locations associated with a Region can be retrieved from the database via the Locations controller
        /// Test will get a fixture region's ID, and send a request for the locations with the regionID
        /// Once returned as a list of locations, the number of locations, and each location's regionID will be verified against the original Region's ID
        /// </summary>
        [Fact]
        public async void CanGetLocationsForRegion_ValidRegionID_AssociatedLocationsReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            //Getting the Region fixture created for these tests
            Region getRegion = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionName == "The New World").Single();

            //send a GET request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.GetAsync("api/Locations/regid=" + getRegion.regionID + "/1");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Location List
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(responseString);

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 8 Locations were returned
            Assert.Equal(8, locations.Count);

            //Check that each location's region ID is the same as the one we requested
            Assert.All(locations, loc =>
            {
                Assert.Equal(getRegion.regionID, loc.regionID);
                Assert.True(0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => sub.locationID == loc.locationID && !sub.inactive).Count());
            });
        }

        [Fact]
        public async void GetInvalidRegionLocationsFails_InvalidRegionID_ErrorReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            //Getting the Region fixture created for these tests
            int invalidID = -10;
            string expectedError = "Invalid Region ID";

            //send a GET request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.GetAsync("api/Locations/regid=" + invalidID + "/1");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedError, responseString);
        }

        /// <summary>
        /// Tests that unassigned locations can be queried from the database.
        /// Test will send a GET request to the appropriate controller URL
        /// and then confirm that the returned object contains the correct number of location objects
        /// </summary>
        [Fact]
        public async void CanGetUnassignedLocations_CallToUnassignedURL_UnassignedLocationsReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            //send a GET request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.GetAsync("api/Locations/unassigned/1");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Location List
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(responseString);
            
            foreach(Location l in locations)
            {
                _output.WriteLine("City: {0}\t Address: {1}\t Unit: {2}\t\n", l.city, l.address, l.unit);
            }

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 8 Locations were returned
            Assert.Equal(8, locations.Count);

            //Check that each location's region ID is the same as the one we requested
            Assert.All(locations, loc => 
            { 
                Assert.Equal(null, loc.regionID);
                Assert.True(0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => sub.locationID == loc.locationID && !sub.inactive).Count());
            });
        }

        /// <summary>
        /// Tests will confirm that a different value can be passed to the controller's COUNT parameter
        /// to retrieve a different set of locations.
        /// Test will retrieve the first set of unassigned locations, and then a second set, confirming
        /// that the correct count of locations exists in both.
        /// Test will then confirm that the two location sets are not the same
        /// </summary>
        [Fact]
        public async void CanGetDifferentLocationSets_CallForLocationsTwoCounts_DifferentLocationSetsReturned()
        {
             _webApp.Load(_webApp.DBContext);

            int totalLocations = _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == null
            && 0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => !sub.inactive && sub.locationID == loc.locationID).Count()).Count();

            int perPage = 8;

            //send a GET request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.GetAsync("api/Locations/unassigned/1");

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Location List
            List<Location> locationsOne = JsonConvert.DeserializeObject<List<Location>>(responseString);

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 8 Locations were returned
            Assert.Equal(8, locationsOne.Count);

            //Make a GET request for the second subset of Unassigned Locations
            response = await _client.GetAsync("api/Locations/unassigned/2");

            //Read back the content of the response into a string
            responseString = await response.Content.ReadAsStringAsync();

            //Convert second location set into a List
            List<Location> locationsTwo = JsonConvert.DeserializeObject<List<Location>>(responseString);

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 1 Locations were returned
            Assert.Equal(totalLocations - perPage, locationsTwo.Count);

            //Confirm that the second subset of locations is not found within the first subset
            foreach(Location l in locationsTwo)
            {
                Assert.DoesNotContain<Location>(l, locationsOne);
            }
            
        }


        /// <summary>
        /// Tests that Location objects can be updated with an assigned Region.
        /// Test gets the ID of the region fixture for the story, and two locations which have no region ID assigned
        /// The two locations will have their region IDs updated, and then a PUT request will be sent to the API
        /// If successful, a 200 code with the two updated location will be returned
        /// </summary>
        [Fact]
        public async void CanAssignLocationsToRegion_LocationsToAssign_UpdatedLocationsReturned()
        {
             _webApp.Load(_webApp.DBContext);;

            //Get Region object and number of assigned locations in the region
            Region updateRegion = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionName == "The New World").Single();
            int locCount = _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == updateRegion.regionID).Count();

            //Get list of unassigned locations
            List<Location> locations = _webApp.DBContext.Location.Where(loc => loc.regionID == null).ToList();

            //Update locations 5 and 6 (fixture locations) with new region ID
            locations[1].regionID = updateRegion.regionID;
            locations[1].region = updateRegion;
            locations[2].regionID = updateRegion.regionID;
            locations[2].region = updateRegion;

            //Add the changing locations to a list. 
            List<Location> updateLocations = new List<Location>()
            {
                locations[1],
                locations[2]
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updateLocations), Encoding.UTF8, "application/json");

            //send a GET request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.PutAsync("api/Locations/assign/" + updateRegion.regionID, updateContent);

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Location List
            List<Location> responseLocations = JsonConvert.DeserializeObject<List<Location>>(responseString);

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 2 Locations were returned
            Assert.Equal(2, responseLocations.Count);

            //Check that each location's region ID is the same as the one we requested
            Assert.All(responseLocations, loc =>
            {
                Assert.Equal(updateRegion.regionID, loc.regionID);
                Assert.True(0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => sub.locationID == loc.locationID && !sub.inactive).Count());
            });
            Assert.Equal(locCount + 2, _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == updateRegion.regionID).Count());
            Assert.Equal(locations.Count - 2, _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == null).Count());
        }

        /// <summary>
        /// Tests that locations can be unassigned from Regions
        /// Test will get two locations associated with fixture region, and unassign them by changing their regionID to 0
        /// A PUT will be sent to the controller, and then the database will
        /// </summary>
        [Fact]
        public async void CanUnassignLocations_LocationsToUnassign_UpdatedLocationsReturned()
        {
             _webApp.Load(_webApp.DBContext);;
            
            //Get initial count of unassigned locations
            int locCount = _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == null).ToList().Count;

            //Getting fixture region
            Region updateRegion = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionName == "The New World").Single();

            List<Location> locations = _webApp.DBContext.Location.Where(loc => loc.regionID == updateRegion.regionID).ToList();

            //Getting IDs of locations to unassign and assigning them to a list to send to controller
            locations[4].regionID = null;
            locations[4].region = null;
            locations[5].regionID = null;
            locations[5].region = null;

            List<Location> listIDs = new List<Location>()
            {
                locations[4],
                locations[5]
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(listIDs), Encoding.UTF8, "application/json");

            //send a PUT request to the api with location list in JSON form
            HttpResponseMessage response = await _client.PutAsync("api/Locations/assign/0", updateContent);

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Location List
            List<Location> responseLocations = JsonConvert.DeserializeObject<List<Location>>(responseString);

            //created status code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check 2 Locations were returned
            Assert.Equal(2, responseLocations.Count);

            //Check that each location's region ID is 0 (unassigned)
            Assert.All(responseLocations, loc =>
                {
                    Assert.Equal(null, loc.regionID);
                    Assert.True(0 < _webApp.DBContext.Subscriber.AsNoTracking().Where(sub => sub.locationID == loc.locationID && !sub.inactive).Count());
                });

            //Check that total count of unassigned locations increased by two
            Assert.Equal(locCount + 2, _webApp.DBContext.Location.AsNoTracking().Where(loc => loc.regionID == null).ToList().Count);
        }
        #endregion

    }
}
