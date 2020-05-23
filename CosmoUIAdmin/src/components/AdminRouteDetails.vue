<template>
    <b-container>
        <!-- h1 element for the Route Details Title -->
        <h1 id="headerDetails">Details</h1>

        <!--A Form group element containing labels for the currently selected Route and its Information-->
        <div>
            <div v-if="currentRoute.routeID > 0">

                <!--Input field for the ROUTE NAME property. Disabled when not in edit mode-->
                <b-form-group
                :invalid-feedback="errorRouteName"
                :state="states.routeName"
                label-cols-lg="4"
                label="Route Name"
                label-for="detInputRouteName"
                label-size="lg"
                label-align-lg="left"
                >
                    <b-form-input id="detInputRouteName" v-model="$v.route.routeName.$model" size="lg" :state="states.routeName" :disabled="!editMode" @update="clearNameError"></b-form-input>
                </b-form-group>

                <!--Label field for ROUTE REGION property. Never directly editable so not set as an input field-->
                <b-form-group
                label-cols-lg="4"
                label="Region"
                label-for="detRegion"
                label-size="lg"
                label-align-lg="left">
                    <label id="detRegion">{{this.currentRoute.region.regionName}}</label>
                </b-form-group>

                <!--Checkbox field for the COMPLETED property. Rendered as a SWITCH element. Disabled when not in edit mode-->
                <b-form-group
                        label-cols-lg="4"
                        label="Completed"
                        label-size="lg"
                        label-for="detCompleteSwitch"
                        label-align-lg="left">

                        <span id="switchSpan">
                            <b-form-checkbox id="detCompleteSwitch" switch size="lg" v-model="route.completed" :disabled="!editMode" inline></b-form-checkbox>
                        </span>
                </b-form-group>

                <!--Date Picker field for the ROUTE DATE property. Disabled when not in Edit Mode-->
                <b-form-group
                        :invalid-feedback="errorRouteDate"
                        :state="states.routeDate"
                        label-cols-lg="4"
                        label="Date"
                        label-size="lg"
                        label-for="detDatePicker"
                        label-align-lg="left">
                    <b-form-input id="detDatePicker" size="lg" :state="states.routeDate" v-model="route.routeDate" @update="clearDateError" :disabled="!editMode" type="date"/>
                </b-form-group>

                <!--Buttons for the page based on whether Edit mode is on or off-->
                <b-form-group>
                    <!--Buttons when EDIT MODE is OFF-->
                    <b-button-group v-if="!editMode">
                        <b-button id="detBtnEditRoute" @click="startEdit" variant="outline-primary"><b-icon-pencil/>Edit Route</b-button>        <!--EDIT ROUTE button-->
                        <b-button id="detBtnDeleteRoute" @click="startDelete" variant="outline-danger" ><b-icon-trash/>Delete Route</b-button> <!--DELETE ROUTE button-->
                        <b-button v-if="!canExport" id="detBtnExport" @click="requestCSVFile" variant="success" disabled>
                            Export to CSV
                        </b-button>
                        <b-button v-else id="detBtnExport" @click="requestCSVFile" variant="success" >
                            <b-spinner v-if="exportPending" variant="white" ></b-spinner>
                            <span v-else>Export to CSV</span>
                        </b-button>
                    </b-button-group>

                    <!--Buttons when EDIT MODE is ON-->
                    <b-button-group v-else>
                        <b-button id="detBtnSaveChanges" variant="outline-primary" @click="saveRoute"><b-icon-music-player/>Save Changes</b-button>      <!--SAVE CHANGES button-->
                        <b-button id="detBtnDiscardChanges" variant="outline-warning" @click="discardEdit"><b-icon-backspace/>Discard Changes</b-button> <!--DISCARD CHANGES button-->
                        <b-button v-if="!canExport" id="detBtnExport" @click="requestCSVFile" variant="success" disabled>
                            Export to CSV
                        </b-button>
                        <b-button v-else id="detBtnExport" @click="requestCSVFile" variant="success" >
                            <b-spinner v-if="exportPending" variant="white" ></b-spinner>
                            <span v-else>Export to CSV</span>
                        </b-button>
                    </b-button-group>
                    <p v-if="!canExport" style="color: red">This region contains no locations</p>
                </b-form-group>

            </div>

            <!-- a label that is only shown on start up, When the currentRoute object matches the default values -->
            <div v-else >
                <label id="lblSelectRoute">Select a Route for more information</label>
            </div>
        </div>
    </b-container>
</template>

<script>
    /* eslint-disable no-console */
    const validators = require('vuelidate/lib/validators');
    import { BIconTrash, BIconPencil, BIconMusicPlayer, BIconBackspace } from 'bootstrap-vue'
    import axios from 'axios'

    export default
        {
            name: "AdminRouteDetails", //The name of the Component
            props:
                {
                    currentRoute: //The Route object that was selected in the Route Table
                    {
                        type: Object,
                        default:()=>( //A default constructor for the currentRoute Object
                            {
                                routeID: 0,
                                routeName: "",
                                regionID: 0,
                                region: {},
                                completed: false,
                                routeDate: {},
                            })
                    },
                    requestResponse:    //Blank object to contain the response data from a PUT or DELETE api request
                        {
                        type: Object,
                        default: () => ({data: {}})
                    }
                },
            components:
                {
                    BIconTrash,
                    BIconPencil,
                    BIconMusicPlayer,
                    BIconBackspace
                },
            data: function()
            {
                return{
                    route: {        //Route object to be copied from the passed in route from the table
                        routeID: 0,
                        routeName: "",
                        regionID: 0,
                        region: {},
                        completed: false,
                        routeDate: "",

                    },
                    editMode: false,
                    errorMessages:{},
                    //This variable is set to true when the route can be exported to CSV file
                    canExport: false,
                    //this variable wil be set to true when a request is made to export the csv file, set back to false when the request has been returned.
                    exportPending: false
                }
            },
            methods:
            {
                startEdit: function()
                {
                    this.editMode = true    //Flips edit mode to TRUE
                },
                discardEdit: function()
                {
                    this.editMode = false;  //Flips edit mode to FALSE

                    this.route = {          //Resets the route to its default state
                        routeID: 0,
                        routeName: "",
                        regionID: 0,
                        region: {},
                        completed: false,
                        routeDate: ""};

                    this.route = JSON.parse(JSON.stringify(this.currentRoute));   //Copies over properties from currentRoute, resetting the displayed data
                    this.route.routeDate = this.isoDate.substring(0,10);
                },
                saveRoute: async function ()   //Starts the PUT request process
                {
                    this.clearResponseErrors();

                    if (this.checkForErrors)     //Checks if any editable fields contain errors
                    {
                        this.$emit("save-route", this.route);     //Emits save-route event if no errors found and the route as content. Handled in parent component

                        await this.sleep(500);

                        if (this.requestResponse.status < 400) {
                            this.editMode = false;                  //Sets exits edit mode
                        }
                    }
                },
                startDelete:function()      //Emits start-delete event to begin DELETE process. Handled in parent component
                {
                    this.$emit("start-delete");
                },

                //Called when fields are updated.
                // If errors were not caught on front end, but returned by API,
                // this will reset the requestResponse data to allow states check to function properly
                clearResponseErrors: function()
                {
                    this.requestResponse = {};
                },
                clearDateError: function()
                {
                    if(this.requestResponse.status === 400 && this.requestResponse.data.memberNames.includes("routeDate"))
                    {
                        this.requestResponse = {};
                    }
                },
                clearNameError: function()
                {
                    if(this.requestResponse.status === 400 && this.requestResponse.data.memberNames.includes("routeName"))
                    {
                        this.requestResponse = {};
                    }
                },
                sleep: function(ms)
                {
                    return new Promise(resolve => setTimeout(resolve, ms));
                },


                /**
                 * This method will be responsible for getting a location count
                 * Location count will be used to determine if a route is able to be exported to
                 * csv file
                 */
                getLocationCount: function(){
                    //send a get request to the API to get the location count for this route
                    //let promise = axios.get( this.apiURLBase +  "routes/count-route-r" + this.route.routeID);

                    let promise = axios({
                        method: 'GET',
                        url: this.apiURLBase +  "routes/count-route-r" + this.route.routeID,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                    return promise.then((response) =>
                    {
                        //if the route has at least one location this route can
                        //be exported. set the button to active
                        if(response.data > 0)
                        {
                            this.canExport = true;
                        }
                        else
                        {
                            this.canExport = false;
                        }

                        return;
                    })
                },

                /**
                 * This method will be used to request a csv file for a particular route
                 */
                requestCSVFile: function(){
                    //set exportPending to true to turn on spinner
                    this.exportPending = true;
                    //send request to API to get get a CSV file back
                   // let promise = axios.get( this.apiURLBase +  "routes/export-r" + this.route.routeID);

                    let promise = axios({
                        method: 'GET',
                        url: this.apiURLBase +  "routes/export-r" + this.route.routeID,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                    return promise.then((response) =>
                    {
                        //create a CSV file from the data returnred
                        const url = window.URL.createObjectURL(new Blob([response.data.cnt], {type:"text/csv"}));
                        //create a link on the DOM, when clicked will download the CSV file
                        const link = document.createElement('a');
                        //the link will be linked to the CSV file created
                        link.href = url;
                        //allow the data to be downloaded to a file named by fileName returned from API
                        link.setAttribute('download', response.data.fileName);
                        //add the link to the page
                        document.body.appendChild(link);
                        //click the link to begin download
                        link.click();

                        //set exportPending to false to stop spinner
                        this.exportPending = false;
                        //show a toast to let the user know that the export has been completed
                        this.$bvToast.toast(`${this.route.routeName} is ready for download`, {
                            id: "exportToast",
                            title: 'Export Success',
                            variant: "success",
                            autoHideDelay: 10000,
                            appendToast: false
                        });
                    })

                }


            },
            validations:{       //Validations for the Route object we're modifying
                route:{
                    //NOTE: We're running trim code to remove trailing spaces on either side of the name, and on the inside of it
                    routeName:{     //Validations for Route Name
                        required: validators.required,      //Route Name field must have values
                        minLength: validators.minLength(2),     //Minimum Length is two characters
                        maxLength: validators.maxLength(40)     //Maximum Length is 40 characters
                    },
                    routeDate:{     //Validations for the Route Date
                        required: validators.required,      //Route Date must be fully filled out
                        // minValue: value => value > new Date().toISOString().substring(0,10)     //Minimum value for the date is TODAY
                        minValue: function(value)
                        {
                            let currentDay = new Date();

                            currentDay.setHours(0);
                            currentDay.setMinutes(0);
                            currentDay.setSeconds(0);

                            //Conditional minValues based on currentRoute's date

                            if(new Date(this.currentRoute.routeDate) < new Date())
                            {
                                //If currentRoute is in the past, ensures that the route date is always set to the same day
                                //CAn't modify the route date eessentially but allows modification of other properties
                                currentDay = new Date(this.currentRoute.routeDate);
                                currentDay.setHours(0 - (currentDay.getTimezoneOffset() / 60));
                                currentDay.setMinutes(0);

                                return value === currentDay.toISOString().substr(0,10);
                            }
                            else
                            {
                                //If currentRoute's route date isn't in the past, ensure that the date can only be set to
                                //today's date or later
                                return value >= currentDay.toISOString().substr(0, 10);
                            }
                        }
                    }
                }
            },
            computed:
                {
                    isoDate: function()
                    {

                        let date = new Date(this.currentRoute.routeDate);

                        //Setting hours to midnight minus timezone offset to compensate for ISO Date time adjustment
                        //Without the offset, dates are given incorrectly when nearing midnight GMT
                        //With the offset, the dates should always be of the correct day, with times at approximately midnight
                        date.setHours(0 - (date.getTimezoneOffset() / 60));
                        date.setMinutes(0);
                       return date.toISOString();
                    },
                    //Checks the states of each field, returns false if anything other than null is found.
                    //Used to prevent sending requests if front-end errors are found
                    checkForErrors: function() {
                        return this.states.routeName === null && this.states.routeDate === null;
                    },
                    states:function()   //Calculates the state of each editable field
                    {
                        return {
                            //First checks to vuelidate status of each editable field, and then checks the requestResponse to determine state
                            routeName: this.$v.route.routeName.$invalid ? false : null || "status" in this.requestResponse && this.requestResponse.status === 400 && "data" in this.requestResponse && this.requestResponse.data.memberNames.includes("routeName") ? false : null,
                            routeDate: this.$v.route.routeDate.$invalid ? false : null || "status" in this.requestResponse && this.requestResponse.status === 400 && "data" in this.requestResponse && this.requestResponse.data.memberNames.includes("routeDate") ? false : null,
                        };
                    },
                    //Custom error messages for the route name
                    errorRouteName: function() {
                        let errorString = "";
                        if (!this.$v.route.routeName.required)      //Route name is a required field
                        {
                            errorString = "Route Name cannot be empty";
                        } else if (!this.$v.route.routeName.minLength)      //Checks if Route Name's minLength is tripped
                        {
                            errorString = "Route Name must be at least 2 characters";
                        } else if (!this.$v.route.routeName.maxLength)      //Checks if Route Name's maxLength is tripped
                        {
                            errorString = "Route Name cannot be greater than 40 characters";
                        } else if ("data" in this.requestResponse && this.requestResponse.status === 400)          //Checking if an error was found in the request response
                        {
                            if ("routeName" in this.requestResponse.data)       //If a route name error was found
                            {
                                errorString = this.requestResponse.data.routeName[0];
                            }
                        }

                        return errorString;
                    },
                    //Custom error messages for the route date
                    errorRouteDate: function() {
                        let errorString = "";
                        if(!this.$v.route.routeDate.required)       //Route Date is a required field
                        {
                            errorString = "Route must have a valid Date";
                        }
                        else if (!this.$v.route.routeDate.minValue)
                        {
                            errorString = "Invalid Date";
                        }
                        else if ("data" in this.requestResponse && this.requestResponse.status === 400)     //checking if an error was found in the request response
                        {
                            if("memberNames" in this.requestResponse.data)
                            {
                                if (this.requestResponse.data.memberNames.includes("routeDate"))
                                {
                                    errorString = this.requestResponse.data.errorMessage;
                                }
                            }
                        }
                        return errorString;
                    }
                },
            watch: {
                /**
                 * This watch function executes when the currentRoute object has changed
                 */
                currentRoute: {
                    handler: function()
                    {
                        this.route = JSON.parse(JSON.stringify(this.currentRoute));
                        this.route.routeDate = this.isoDate.substring(0, 10);
                        //get the location count of this route to check if it can be exported to csv.
                        if(this.route.routeID > 0)
                        {
                            this.getLocationCount();
                        }


                    },
                    deep: true
                }
            },
            mounted()
            {
                //Resets the current route's routeID so details do not persist between page openings
                this.currentRoute.routeID = 0;
            }
        }
</script>

<style scoped>
#switchSpan
{
    object-fit: cover
}
label
{
    font-size: 1.5em;
}
</style>
