using CosmoAPI;
using CosmoAPI.Models;
using CosmoAPITests.Fixtures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CosmoAPI.Controllers;
using Newtonsoft.Json.Linq;
using CosmoAPITests.Utils;

namespace CosmoAPITests.EndToEndTests
{
    public class RouteTests : IClassFixture<WebAppFixture<Startup>>
    {
        HttpClient _client; //the HTTP client handles HTTP requests to the API
        private readonly WebAppFixture<Startup> _webApp;    //WebAppFixture to be created using configurations found in Startup.cs
        DateCalculation dateCalculation;
        //private readonly TokenClient _tokenClient;

        Admin admin;

        public RouteTests(WebAppFixture<Startup> webApp)
        {
            _webApp = webApp; //set this _webApp to the WebAppFixture that is passed in

            //Builds up a HTTP client object that handles redirects to the API
            //_client = _webApp.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            _client = _webApp.TestClient;
            dateCalculation = new DateCalculation();

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

        #region story14_subscriber_cancels_upcoming_pickup

        [Fact]
        public async void SubscriberIsOptedOut_ValidSubscriberID_SubscriberIDValid()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);
            Subscriber dbSubscriber = _webApp.DBContext.Subscriber.Where(s => s.email.Equals("hunter2@thenewworld.com")).FirstOrDefault();


            Assert.NotNull(dbSubscriber);

            response = await _client.GetAsync("api/Subscribers/optoutID=" + dbSubscriber.subscriberID);
            string responseString = await response.Content.ReadAsStringAsync();
            Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(responseString);
            
            //Check to ensure that the subscriberID and locationID are both ints
            Assert.IsType<int>(subscriber.subscriberID);
            Assert.IsType<int>(subscriber.locationID);

            Assert.True(subscriber.subscriberID == dbSubscriber.subscriberID);
        }
        [Fact]
        public async void SubscriberIsNotOptedOut_InvalidSubscriberID_SubscriberIDInvalid()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //A mock to the passed in subscriber ID
            int subscriberID = 0;

            response = await _client.GetAsync("api/Subscribers/optoutID=" + subscriberID);

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }
        [Fact]
        public async void SubscriberIsNotOptedOut_StringSubscriberID_SubscriberIDInvalid()
        {
            _webApp.Load(_webApp.DBContext);
            //A mock to the passed in subscriber ID
            string subscriberID = "";

            HttpResponseMessage response = await _client.GetAsync("api/Subscribers/optoutID=" + subscriberID);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void LocationRouteIsAddedIntoOptedOutLocationRouteList_ValidSubscriberID_LocationRouteIsAdded()
        {
            _webApp.Load(_webApp.DBContext);
            
            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            Subscriber dbSubscriber = _webApp.DBContext.Subscriber.Where(s => s.email.Contains("hunter2@")).FirstOrDefault();

            Assert.NotNull(dbSubscriber);

            response = await _client.GetAsync("api/Subscribers/optoutID=" + dbSubscriber.subscriberID);

            string responseString = await response.Content.ReadAsStringAsync();
            //Subscriber
            Subscriber subscriber = JsonConvert.DeserializeObject<Subscriber>(responseString);
            //Location
            Location location = _webApp.DBContext.Location.FirstOrDefault(x => x.locationID == subscriber.locationID);
            //Region
            Region region = _webApp.DBContext.Region.FirstOrDefault(x => x.regionID == location.regionID);
            //Route
            Route route = _webApp.DBContext.Route.FirstOrDefault(x => x.regionID == region.regionID && !x.completed && !x.inactive);
            //LocationRoute
            LocationRoute locationRoute = _webApp.DBContext.LocationRoute.FirstOrDefault(x => x.routeID == route.routeID && x.locationID == location.locationID);
            //Ensure locationRoute is added to optoutLocationRouteList
            Assert.Contains<LocationRoute>(locationRoute, route.optoutLocationRouteList);
        }
        #endregion

        #region Story 62 End to End Tests - KYLE AND LOGAN YOU MADE THESE

        [Fact]
        public async void DoValidRouteUpdatesSucceed_ValidRoute_RouteIsUpdated()
        {
            
            _webApp.Load(_webApp.DBContext);
            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            string changeName = "Test Changing Name";
            Route updateRoute = _webApp.DBContext.Route.ToList<Route>()[0];     //Gets the first route from the Context's Route table and assigns it to updateRoute

            //Updating the routes name
            updateRoute.routeName = changeName;

            //convert the Route to a JSON string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //Send PUT request to API with updated route. API should send back the updated Route object.
            response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();
            Route returnRoute = JsonConvert.DeserializeObject<Route>(responseString);

            //Making sure that all of the route's information is correct
            Assert.Equal(updateRoute.routeID, returnRoute.routeID);
            Assert.Equal(changeName, returnRoute.routeName);
            Assert.Equal(updateRoute.regionID, returnRoute.regionID);
            Assert.Equal(updateRoute.routeDate, returnRoute.routeDate);
            Assert.Equal(updateRoute.completed, returnRoute.completed);

            //Verifying that the status code returned is the "204 No Content" status code
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void DoInvalidRouteUpdatesFail_InvalidShortRouteName_ErrorReturnedRouteIsNotUpdated()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            string changeName = "A";
            Route updateRoute = _webApp.DBContext.Route.ToList<Route>()[0];     //Gets the first route from the Context's Route table and assigns it to updateRoute

            updateRoute.routeName = changeName;

            //convert the Route to a JSON string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //Send PUT request to API with updated route. API should send back the updated Route object.
            response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();

            Dictionary<string, string[]> results = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseString);

            //gets a list of validation results
            //IEnumerable<ValidationResult> results = JsonConvert.DeserializeObject<IEnumerable<ValidationResult>>(responseString);

            ////making sure only one error is returned
            Assert.Single(results);
            ////ensuring that the error returned has the correct message
            Assert.Equal("Route Name must be at least 2 characters", results.ElementAt(0).Value.ElementAt(0));

            ////Verifying that the status code returned is the "400 Bad Request" status code
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void DoInvalidRouteUpdatesFail_InvalidLongRouteName_ErrorReturnedRouteIsNotUpdated()
        {
            _webApp.Load(_webApp.DBContext);
            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            string changeName = "This name is way toooooooooooooooooo long";
            Route updateRoute = _webApp.DBContext.Route.FirstOrDefault();     //Gets the first route from the Context's Route table and assigns it to updateRoute

            updateRoute.routeName = changeName;

            //convert the Route to a JSON string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //Send PUT request to API with updated route. API should send back the updated Route object.
            response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();

            Dictionary<string, string[]> results = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseString);

            //gets a list of validation results
            //IEnumerable<ValidationResult> results = JsonConvert.DeserializeObject<IEnumerable<ValidationResult>>(responseString);

            ////making sure only one error is returned
            Assert.Single(results);
            ////ensuring that the error returned has the correct message
            Assert.Equal("Route Name cannot be greater than 40 characters", results.ElementAt(0).Value.ElementAt(0));

            ////Verifying that the status code returned is the "400 Bad Request" status code
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void DoInvalidRouteUpdatesFail_EmptyRouteName_ErrorReturnedRouteIsNotUpdated()
        {
            _webApp.Load(_webApp.DBContext);
            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            string changeName = "";
            Route updateRoute = _webApp.DBContext.Route.FirstOrDefault();     //Gets the first route from the Context's Route table and assigns it to updateRoute

            updateRoute.routeName = changeName;

            //convert the Route to a JSON string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //Send PUT request to API with updated route. API should send back the updated Route object.
            response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();

            Dictionary<string, string[]> results = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseString);

            //gets a list of validation results
            //IEnumerable<ValidationResult> results = JsonConvert.DeserializeObject<IEnumerable<ValidationResult>>(responseString);

            ////making sure only one error is returned
            Assert.Single(results);
            ////ensuring that the error returned has the correct message
            Assert.Equal("Route Name cannot be empty", results.ElementAt(0).Value.ElementAt(0));

            ////Verifying that the status code returned is the "400 Bad Request" status code
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //[Fact]
        //public async void DoInvalidRouteUpdatesFail_NoRouteID_ErrorReturnedRouteIsNotUpdated()
        //{

        //    int newID = -100;
        //    Route updateRoute = _webApp.DBContext.Route.ToList<Route>()[0];     //Gets the first route from the Context's Route table and assigns it to updateRoute

        //    updateRoute.routeID = newID;

        //    //convert the Route to a JSON string for posting to the database
        //    var StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

        //    //Send PUT request to API with updated route. API should send back the updated Route object.
        //    HttpResponseMessage response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

        //    //Convert the response content into a Route object for comparing against what we sent
        //    string responseString = await response.Content.ReadAsStringAsync();

        //    Dictionary<string, string[]> results = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseString);

        //    //gets a list of validation results
        //    //IEnumerable<ValidationResult> results = JsonConvert.DeserializeObject<IEnumerable<ValidationResult>>(responseString);

        //    ////making sure only one error is returned
        //    Assert.Single(results);
        //    ////ensuring that the error returned has the correct message
        //    Assert.Equal("Route was not found", results.ElementAt(0).Value.ElementAt(0));

        //    ////Verifying that the status code returned is the "400 Bad Request" status code
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        [Fact]
        public async void DoValidRouteRemovalsSucceed_ValidRouteID_RouteIsRemoved()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            Route deleteRoute = _webApp.DBContext.Route.ToList<Route>()[0];     //Gets the first route from the Context's Route table and assigns it to updateRoute

            //convert the Route to a JSON string for posting to the database
            StringContent = new StringContent(JsonConvert.SerializeObject(deleteRoute), Encoding.UTF8, "application/json");

            //Send DELETE request to API with updated route. API should send back status code
            response = await _client.DeleteAsync("api/Routes/" + deleteRoute.routeID);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();

            //Verifying that the status code returned is the "204 No Content" status code
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Verify that the correct message is returned
            Assert.Equal("Route " + deleteRoute.routeName + " was archived", responseString);
        }

        [Fact]
        public async void DoInvalidRouteRemovalsFail_InvalidRouteID_ErrorReturnedRouteIsNotRemoved()
        {
            _webApp.Load(_webApp.DBContext);
            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            //Send DELETE request to API with updated route. API should send back status code
            response = await _client.DeleteAsync("api/Routes/" + -100);

            //Convert the response content into a Route object for comparing against what we sent
            string responseString = await response.Content.ReadAsStringAsync();

            //Verifying that the status code returned is the "400 Bad Request" status code
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            //Verify that the correct message is returned
            Assert.Equal("Archiving Failed - Route ID was not found", responseString);
        }



        #endregion

        #region Story90 Admin creates a Route by creating a Region/completing a Route - Kyle Wei

        /// <summary>
        /// Test checks that a new Route object is created and added to DB when 
        /// a new Region object is created and added to the DB
        /// 
        /// Newly created Route attributes should be as follows:
        ///     routeID: Autogenerated by DB
        ///     routeName: "RegionName - FirstPickupDate"
        ///     regionID: ID of new Region
        ///     region: Newly created Region Object
        ///     completed: false
        ///     routeDate: return date from Region.GenerateNextPickupDate()
        /// </summary>
        [Fact]
        public async void IsRouteCreatedForRegion_RegionIsCreated_RouteIsCreated()
        {
            _webApp.Load(_webApp.DBContext);

            DateTime today = DateTime.Now;  //Get Today's date 

            //New Region object to add to DB
            Region newRegion = new Region
            {
                regionName = "Candyland",
                frequency = 10,
                firstDate = today.AddDays(1.0),
                inactive = false
            };

            //convert the Region to a string for posting to the database
            var StringContent = new StringContent(JsonConvert.SerializeObject(newRegion), Encoding.UTF8, "application/json");

            //send a post request to the api with the Subscriber object in JSON form
            HttpResponseMessage response = await _client.PostAsync("api/Regions", StringContent);

            //get the response from the api and read it into a string
            string responseString = await response.Content.ReadAsStringAsync();

            //deserialize the response into a Region object
            Region postRegion = JsonConvert.DeserializeObject<Region>(responseString);

            //created status code is 201
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            
            int regionID = postRegion.regionID; //Get returned region's ID
            string dateString = dateCalculation.GetOneDate(postRegion).ToString("MMMM dd, yyyy");  //Get returned region's next pickup date in format MMMM dd, yyyy
            string expectedRouteName = postRegion.regionName + " - " + dateString;  //Concatenate region name and date string togethe "RegionName - NextPickupDate"

            Route newRoute = _webApp.DBContext.Route.Where(r => r.regionID == regionID && r.completed == false).Single();

            Assert.Equal(expectedRouteName, newRoute.routeName);    //Check Route Name is as expected "RegionName - NextPickupDate"
            Assert.Equal(regionID, newRoute.regionID);              //Check Route's Region ID matches the added Region's
            Assert.False(newRoute.completed);                       //Check Route's completed status is false
            Assert.False(newRoute.inactive);                        //Check Route's inactive status is false

        }

        /// <summary>
        /// Test checks that a new Route object is created and added to DB 
        /// when an existing Route is set to Complete
        /// 
        /// Newly created Route attributes should be as follows:
        ///     routeID: Autogenerated by DB
        ///     routeName:"Completed Route's Name"
        ///     regionID: ID of completed Route's Region
        ///     region: Completed Route's Region
        ///     completed: false
        ///     routeDate: return date from Region.GenerateNextPickupDate()
        /// </summary>
        [Fact]
        public async void IsRouteCreatedAfterRouteCompletion_RouteSetToComplete_RouteIsCreated()
        {
            _webApp.Load(_webApp.DBContext);

            Route updateRoute = _webApp.DBContext.Route.FirstOrDefault(r => !r.completed);   //Getting the first Incomplete Route
            Assert.NotNull(updateRoute);
            //Region region = _webApp.DBContext.Region.Where(reg => reg.regionID == updateRoute.regionID).First();    //Get the Region the previous Route corresponds to
            Region region = updateRoute.region;
            updateRoute.completed = true;       //Set the Route to Completed

            //convert the Route to a string for posting to the database
            var StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //send a PUT request to API to update the Route to completed
            HttpResponseMessage response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            List<Route> newRoute = _webApp.DBContext.Route.Where(r => r.regionID == region.regionID && !r.completed).ToList();

            DateCalculation dateCalculation = new DateCalculation();

            Assert.Single(newRoute);
            Assert.Equal(updateRoute.routeName, newRoute[0].routeName);
            Assert.Equal(updateRoute.regionID, newRoute[0].regionID);
            Assert.False(newRoute[0].completed);
            Assert.False(newRoute[0].inactive);
            Assert.Equal(dateCalculation.GetOneDate(region), newRoute[0].routeDate);
        }

        /// <summary>
        /// Test checks that a new Route object IS NOT created if an existing Route
        /// is set to Complete, but the associated Region has been archived/is inactive
        /// </summary>
        [Fact]
        public async void DoesInactiveRegionStopRouteCompletedCreation_RegionInactiveRouteCompleted_RouteIsNotCreated()
        {
            _webApp.Load(_webApp.DBContext);

            int routeCount = _webApp.DBContext.Route.Count();

            Route updateRoute = _webApp.DBContext.Route.First(r => !r.completed);   //Getting the first Incomplete Route
            Region updateRegion = _webApp.DBContext.Region.Where(reg => reg.regionID == updateRoute.regionID).First();    //Get the Region the previous Route corresponds to
            updateRegion.inactive = true;       //Set the Region to Inactive
            updateRoute.completed = true;       //Set the Route to Completed

            //Update the Region Table with the archived region
            _webApp.DBContext.Region.Update(updateRegion);

            //Convert the Route to a JSON String and update it in the DB
            var StringContent = new StringContent(JsonConvert.SerializeObject(updateRoute), Encoding.UTF8, "application/json");

            //Send a PUT request to API to update the Route to completed
            HttpResponseMessage response = await _client.PutAsync("api/Routes/" + updateRoute.routeID, StringContent);

            Route newRoute = _webApp.DBContext.Route.Where(r => !r.completed && r.regionID == updateRegion.regionID).FirstOrDefault();

            Assert.Equal(routeCount, _webApp.DBContext.Route.Count());
            Assert.Null(newRoute);
        }

        /// <summary>
        /// Test checks that a new Route object is created if an existing Route
        /// is archived/deleted and the associated Region is active
        /// Newly created Route attributes should be as follows:
        ///     routeID: Autogenerated by DB
        ///     routeName: "RegionName - FirstPickupDate"
        ///     regionID: ID of archived Route's Region
        ///     region: Archived Route's Region
        ///     completed: false
        ///     routeDate: return date from Region.GenerateNextPickupDate()
        /// </summary>
        [Fact]
        public async void IsNewRouteCreatedAfterRouteArchive_RouteArchived_RouteIsCreated()
        {
            _webApp.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);

            Route archiveRoute = _webApp.DBContext.Route.AsNoTracking().First(r => !r.completed && !r.inactive);  //Get an incomplete Route from the DB
            Region region = _webApp.DBContext.Region.AsNoTracking().Where(reg => reg.regionID == archiveRoute.regionID).Single();   //Get that Route's region

            //send a DELETE request to API to archive the Route to completed
            response = await _client.DeleteAsync("api/Routes/" + archiveRoute.routeID);

            //Get the single Route associated with the original Region which are incomplete
            //This doubles as a test as the "Single" method throws an exception if more than one element is found
            Route newRoute = _webApp.DBContext.Route.AsNoTracking().Where(r => r.regionID == archiveRoute.regionID && !r.inactive && !r.completed).Single();

            Assert.Equal(archiveRoute.routeName, newRoute.routeName);    //Check that new Route has same name as the archived Route
            Assert.Equal(archiveRoute.regionID, newRoute.regionID);      //Check that the new Route's Region ID matches archived Route's Region ID
            Assert.False(newRoute.completed);           //Check that the new Route is incomplete
            Assert.False(newRoute.inactive);            //Check that the new Route is active
            Assert.Equal(dateCalculation.GetOneDate(region), newRoute.routeDate);      //Check that the new Route's routeDate is the Region's next pickup date

        }

        /// <summary>
        /// Test checks that a new Route object IS NOT created if an existing Route
        /// is archived/deleted and the associated Region is archived/inactive
        /// </summary>
        [Fact]
        public async void DoesInactiveRegionStopRouteArchivedCreation_RegionInactiveRouteArchived_RouteIsNotCreated()
        {
            _webApp.Load(_webApp.DBContext);

            int routeCount = _webApp.DBContext.Route.Count();
            Region updateRegion = _webApp.DBContext.Region.Where(reg => !reg.inactive && reg.regionName == "Harbor Creek").First();    //Get the Region the previous Route corresponds to

            updateRegion.inactive = true;       //Set the Region to Inactive

            //Update the Region Table with the archived region
            _webApp.DBContext.Region.Update(updateRegion);
            await _webApp.DBContext.SaveChangesAsync();

            Route updateRoute = _webApp.DBContext.Route.First(r => r.regionID == updateRegion.regionID && !r.completed);   //Getting the first Incomplete Route

            updateRoute.completed = true;       //Set the Route to Completed

            //Send a DELETE request to API to archive the Route
            HttpResponseMessage response = await _client.DeleteAsync("api/Routes/" + updateRoute.routeID);

            Route newRoute = _webApp.DBContext.Route.Where(r => r.regionID == updateRegion.regionID && !r.inactive && !r.completed).FirstOrDefault();

            Assert.Equal(routeCount, _webApp.DBContext.Route.Count());
            Assert.Null(newRoute);
        }
        #endregion

        #region Story 95 Admin exports route locations and subscriber information into CSV file.
        [Theory]
        [InlineData("S90 A Past Route")]
        [InlineData("Logan's Loop")]
        public async void RoutesInfoReturnedInCSVFormat_RequestForLargeRoute_RouteInfoReturnedInCSVFormatAsync(string routeName)
        {
            AdminFixture.Load(_webApp.DBContext);

            //setting up login token
            var StringContent = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("api/Admins", StringContent);

            string adminToken = await response.Content.ReadAsStringAsync();

            _client.SetBearerToken(adminToken);
            //reload the db
            _webApp.Load(_webApp.DBContext);

           
            //get the route that is expected to contain all of the locations and subscribers
            Route r = _webApp.DBContext.Route.Where(e => e.routeName.Equals(routeName)).FirstOrDefault();
            //get all locations in the same region as this route
            List<Location> locations = _webApp.DBContext.Location.AsQueryable().Where(e => e.regionID == r.regionID).ToList<Location>();
            //instantiate list of subscribers
            List<Subscriber> subscribers = new List<Subscriber>();
            //for each location in the list get its associated subscriber only if the subscriber is currently active. Inactive subscribers will not have their locations be completed on this route.
            foreach (Location l in locations)
            {
                subscribers.Add(_webApp.DBContext.Subscriber.AsQueryable().Where(e => e.locationID == l.locationID).FirstOrDefault());
            }

            //from each subscriber, create a new object with data from both the subscriber and their location as one single object
            var rows = subscribers.AsQueryable().Select(s => new
            {
                s.location.address,
                s.location.city,
                s.location.province,
                s.location.postalCode,
                s.firstName,
                s.lastName,
                s.phoneNumber,
                s.email
            }).ToList();
            //create a string with headers for csv file of the expected outcome
            string expected = "Address,City,Province,Postal Code,First Name,Last Name,Phone Number,Email\n";

            //for each object (subscriber-location) in the rows variable, add its data in csv format to the expected string.
            foreach (var row in rows)
            {
                expected += row.address + ',' +
                row.city + ',' +
                row.province + ',' +
                row.postalCode + ',' +
                row.firstName + ',' +
                row.lastName + ',' +
                row.phoneNumber + ',' +
                row.email + "\n";
            }
            //get the CSV data back from the API in a response
            response = await _client.GetAsync("api/Routes/" + "export-r" + r.routeID);
            //parse the data into a json object and get the value of the content
            string responseString = JObject.Parse(response.Content.ReadAsStringAsync().Result).First.Next.First.ToString();
            //ensure that the CSV content retrieved back from the API matches what our expected string
            Assert.Equal(expected, responseString);
        }


        
        #endregion

        /// <summary>
        /// Helper method to unload and load fixtures in required order
        ///     UNLOAD ORDER
        ///     1) Routes
        ///     2) Regions
        ///     3) Locations
        ///     4) Subscribers
        ///     Routes have foreign keys which rely on Regions, must remove any reliances before removing Regions
        ///     
        ///     LOAD ORDER
        ///     1) Regions
        ///     2) Routes
        ///     3) Locations
        ///     4) Subscribers
        ///     Regions must exist so they can be loaded into Routes
        ///     
        /// </summary>
    }
}
