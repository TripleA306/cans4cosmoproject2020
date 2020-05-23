<!--This class is to represent the subscribers home page.-->
<template>
    <div>
        <b-jumbotron style="margin-bottom: 0" class="text-center">
            <!--Display the users name pthat was passed to this page-->
            <template  v-slot:header>Welcome, {{$store.state.currentSubscriber.firstName}}</template>

        </b-jumbotron>
        <b-navbar toggleable="*"  variant="light">
            <b-navbar-toggle class="ml-auto" target="nav-collapse"></b-navbar-toggle>

            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav >
                    <b-nav-item style="margin: auto" @click="ChangeToDashboard()"><h2>Dashboard</h2></b-nav-item>
                    <b-nav-item style="margin: auto" @click="ChangeToCollectionHistory()"><h2>Collection History</h2></b-nav-item>
                    <b-nav-item style="margin: auto" @click="logOut()"><h2>Log Out</h2></b-nav-item>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>

        <!--Display each child route of the home page here-->
        <router-view></router-view>

        <!--This card will be to display the next collection dates-->

    </div>
</template>

<script>
    const axios = require('axios').default;

    export default {
        name: "HomePage.vue",

        // props: { //These are data items passed down from the parent
        //     subscriberName: { //Name to be displayed
        //         type:String,
        //         default:''
        //     },
        //     subscriberEmail: { //Email passed in so we can find the locationID associated with the account
        //         type:String,
        //         default:''
        //     }
        // },
        // data: function () {
        //     return {
        //         subscriber: {subscriberID: null, firstName: '', lastName: '', phoneNumber: '', email: '', locationID: null, billingLocationID: null},
        //         dates: [], //This will store what the backend sends back as dates
        //         locationID: null, //Will be the users locationID
        //         providerURL: "http://localhost:5002", //The base URL to all API calls
        //         errors: {} //This will hold all errors returned by the back end
        //     }
        // },
        methods:{

            ChangeToCollectionHistory: function () {
                this.$router.push('/home/history');
            },
            //This function will call our API and retrieve the subscriber
            getSubscriber: function() {
                //Call the GET
                // eslint-disable-next-line no-console
                axios({
                    method: 'GET',
                    url: this.providerURL + "/api/Subscribers/" + window.globalSubscriberID,
                    headers: {
                        'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                        this.subscriber = response.data; //Set subscriber
                        this.getSubscriberDates(); //Call the get dates here because it requires the LocationID
                    })
                    .catch(errors => { //Mustve failed
                        let response = errors.response;
                        if (response.status === 400) {
                            this.errors = response.data;
                        }
                    });
            },
            ChangeToDashboard: function() {
                this.$router.push('/home/dashboard');
            },
            //This function will be called by the above method once it successfully gets the locationID
            //It will use this locationID to pass to the back end and in return it will get the dates to be displayed
            getSubscriberDates: function() {
                //Call the GET with the locationID
                axios({
                    method: 'GET',
                    url:this.providerURL + "/api/Locations/locationID-c="+ this.subscriber.locationID,
                    headers: {
                        'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                        //Set dates to the response
                        this.dates = response.data;

                        //For each date call the format method on each date that is retrieved
                        for(let i = 0; i < this.dates.length; i++)
                        {
                            let date = this.dates[i];
                            this.dates[i] = this.formatDate(date);
                        }

                    })
                    .catch(errors => { //We failed so set the errors accordingly
                        let response = errors.response;
                        if (response.status === 400) {
                            this.errors = response.data;
                        }
                    });
            },
            //This method will take in a date and format it to a presentable form
            formatDate: function(date) {

                let parseDate = new Date(date.substring(0,10)); //Substrings and parses the routeDate

                //let days = ['Mon','Tues','Wed','Thu','Fri','Sat','Sun'];        //Array of days
                let months = ['January','February','March','April','May','June','July','August','September','October','November','December'];   //Array of months

                let day = parseDate.getDate();  //Parses routeDate into a date object
                if(day < 10)        //Checking if day is less than 10. need to re-add leading 0 if so
                {
                    day = "0" + day;
                }

                // eslint-disable-next-line no-unused-vars
                let dateStringThree = months[parseDate.getMonth()] + " " + day + ", " + parseDate.getFullYear();

                return dateStringThree;
            },
            logOut: function () {
                sessionStorage.removeItem("sessionID");
                this.$store.commit('setLoggedIn', null);
                this.$router.push('/');
            }
        },
        //Mounted was used instead of computed as the dates will not be dependent on changes made on the page,
        //They will only need to be populated when the page loads

    }
</script>


