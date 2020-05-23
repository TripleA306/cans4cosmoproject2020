<template>
    <b-container>
        <h1 id="headerDetails">Details</h1>

        <!-- Show the Entry fields if the region table has been clicked on -->
        <div v-if="region.regionID > -1">

            <!-- Label and Entry field for the Regions Name -->
            <b-form-group id="lblRegionName" label="Region Name" label-for="txtRegionName" label-size="lg" label-cols-lg="5" label-align-lg="left" :invalid-feedback="errorRegionName" :state="states.regionName" >
                <b-form-input id="txtRegionName" size="lg" :disabled="!editMode" v-model="$v.region.regionName.$model">
                </b-form-input>
            </b-form-group>

            <!-- Label and Number entry field for the Regions Frequency -->
            <b-form-group id="lblRegionFrequency" label="Pick Up Frequency" label-for="txtFrequency" label-size="lg" label-cols-lg="5" label-align-lg="left" :invalid-feedback="errorFrequency" :state="states.frequency" >
                <b-form-input  id="txtFrequency" type="number" size="lg" min="1" max="52" :disabled="!editMode" v-model="$v.region.frequency.$model" >
                </b-form-input>
            </b-form-group>

            <!-- Label and Date entry field for displaying the Regions First Collection date
                Only allowed to be changed if it is being created... not edited -->
            <b-form-group id="lblRegionFirst" label="First Collection" label-for="txtfirstCollection" label-size="lg" label-cols-lg="5" label-align-lg="left" :invalid-feedback="errorFirstDate" :state="states.firstDate" >
                <b-form-input id="txtfirstCollection" type="date" size="lg" :disabled="!editMode || region.regionID !== 0"  v-model="$v.region.firstDate.$model" >
                </b-form-input>
            </b-form-group>

            <!-- Label and Value for displaying the Regions next date -->
            <b-form-group id="lblRegionNext" v-if="region.regionID !==0" label="Next Collection Date" label-size="lg" label-cols-lg="5" label-align-lg="left" label-for="txtNextDate">
                <label type="date"  id="txtNextDate">{{this.region.nextDate}}</label>
            </b-form-group>

            <!-- Label and switch for displayed the Regions active status,
                Only allowing this to be changed if the Region is inactive (viewed by clicking the showall button) and in edit mode
                Might seem silly to have these conditions to be met, but deleting an Region has the extra functionality of removing routes or not
                 Thus not allowing to switched in any other condition
                 -->
            <b-form-group id="lblRegionActive" label-for="swtIsActive" label-align-lg="left" label-cols-lg="5" label="Active" label-size="lg" v-if="region.regionID !==0">
                <span id="switchSpan">
                <b-form-checkbox switch size="lg" id="swtIsActive" inline :disabled="!showAll || !editMode || !region.inactive" @change="changeActive" v-bind:checked="!region.inactive" >
                </b-form-checkbox>
                </span>
            </b-form-group>

            <!--Buttons for the page based on whether Edit mode is on or off-->
            <b-form-group>
                <!--Buttons when EDIT MODE is OFF-->
                <b-button-group v-if="!editMode">
                    <!--This is NOT a mistake, the AddRegion button will never exist with the other one, It allows it to be referenced easier in Katalon (no duplicate object) -->
                    <b-button id="btnAddRegion" @click="startAddRegion" variant="outline-primary"><b-icon-plus/>Add Region</b-button>
                    <b-button id="btnEditRegion" @click="startEdit" variant="outline-primary"><b-icon-pencil/>Edit Region</b-button>        <!--EDIT ROUTE button-->
                    <b-button id="btnDeleteRegion" @click="startDelete" variant="outline-danger" ><b-icon-trash/>Delete Region</b-button>    <!--DELETE ROUTE button-->
                </b-button-group>

                <!--Buttons when EDIT MODE is ON-->
                <b-button-group v-else>
                    <b-button id="btnSave" variant="outline-primary" @click="saveRegion"><b-icon-music-player/>Save Changes</b-button>      <!--SAVE CHANGES button-->
                    <b-button id="btnDiscard" variant="outline-warning" @click="cancelEdit"><b-icon-backspace/>Discard Changes</b-button> <!--DISCARD CHANGES button-->
                </b-button-group>
            </b-form-group>
        </div>
        <div v-else> <!-- The default view for the details pane if nothing was selected on the Region Table -->
            <label id="lblSelectRegion">Select a Region for more information</label>
            <!--This is NOT a mistake, the AddRegion button will never exist with the other one, It allows it to be referenced easier in Katalon (no duplicate object) -->
            <b-button id="btnAddRegion" @click="startAddRegion" variant="outline-primary"><b-icon-plus/>Add Region</b-button>
        </div>
    </b-container>
</template>

<script>
    /* eslint-disable no-console */
    const validators = require('vuelidate/lib/validators');
    import { BIconPlus, BIconTrash, BIconPencil, BIconMusicPlayer, BIconBackspace } from 'bootstrap-vue'
    export default {
        name: "AdminRegionDetails",
        props:
            {
                currentRegion:{
                    regionName: "",
                    frequency: "",
                    firstDate: "",
                    nextDate: "",
                    inactive: false,
                },
                requestResponse:    //Blank object to contain the response data from a PUT or DELETE api request
                    {
                        type: Object,
                        default: () => ({data: {}})
                    },
                isDeleting:{
                    type: Boolean,
                    default: false
                },
                showAll:{
                    type: Boolean,
                    default: false
                },
            },
        components: {
            BIconPlus,
            BIconTrash,
            BIconPencil,
            BIconMusicPlayer,
            BIconBackspace
        },

        data: function () {
            return {
                region:{
                    regionName: "",
                    frequency: "",
                    firstDate: "",
                    nextDate: "",
                    inactive: false,
                },
                //Array of errors
                errors: {},
                //Boolean to figure out when an api error has occured
                apiError: false,
                editMode: false,
            }
        },
        methods: {
            /**
             * This method will take in a Date and a boolean parameter to return a mm/dd/yyyy date value to be displayed
             * bool parameter to specify to return an ISO or localeDate string
             * @param regionDate
             * @param localeDate
             * @returns {string}
             */
            isoDate: function(regionDate, localeDate)
            {

                let date = new Date(regionDate);
                if(localeDate)
                {
                    return date.toLocaleDateString();
                }
                else
                {
                    //Setting hours to midnight minus timezone offset to compensate for ISO Date time adjustment
                    //Without the offset, dates are given incorrectly when nearing midnight GMT
                    //With the offset, the dates should always be of the correct day, with times at approximately midnight
                    date.setHours(0 - (date.getTimezoneOffset() / 60));
                    date.setMinutes(0);
                    return date.toISOString();
                }
            },
            /**
             * This method will clear all the errors and region object binded to the detail entry fields
             */
            startAddRegion: function () {
                this.$v.$reset();
                this.editMode = true;
                this.region = {
                    regionID: 0,
                    regionName: "",
                        frequency: "",
                        firstDate: "",
                        nextDate: "",
                        inactive: false,
                };
            },
            /**
             * This helper function sets the editMode boolean to true to display the editmode functions
             * and to unlock the entry fields
             */
            startEdit: function () {
                this.editMode = true;
                this.$emit("edit-mode", this.editMode);
            },
            /**
             * This function will turn off the edit mode fuctions
             * and reset the entry fields back to their orignal values before the edit
             */
            cancelEdit: function () {
                this.editMode = false;
                this.region = JSON.parse(JSON.stringify(this.currentRegion));
                this.region.firstDate = this.isoDate(this.currentRegion.firstDate, false).substring(0, 10);
                this.region.nextDate = this.isoDate(this.currentRegion.nextDate, true).substring(0, 10);
                this.$emit("edit-mode", this.editMode);
            },
            /**
             * This helper function will display the delete modal to confirm deletion
             */
            startDelete: function () {
                this.editMode = false;
                this.$bvModal.show('deleteRegionModal');
            },
            /**
             * This helper function is used to flip the inactive property of the Region object when the active switch is clicked on
             * */
            changeActive: function()
            {
              this.region.inactive = !this.region.inactive;
            },
            /**
             *  This function will first check if any errors are present, if none it will emit the region to the proper method on the Region Page
             *  It will then wait for a response from the api before closing the editmode, (in case client side validation fails)
             * @returns {Promise<void>}
             */
            saveRegion: async function () {
                //Set all the fields to "dirty" to allow them to be flagged for validation
                this.$v.region.regionName.$touch();
                this.$v.region.frequency.$touch();
                this.$v.region.firstDate.$touch();


                if(this.checkForErrors())
                {
                    if(this.region.regionID > 0)
                    {
                        this.$emit("put-region", this.region, this.requestResponse);
                    }
                    else
                    {
                        this.$emit("post-region", this.region, this.requestResponse);
                    }

                    await this.sleep(500);

                    if (this.requestResponse.status < 400) {
                        this.editMode = false;                  //Sets exits edit mode
                        this.$emit("edit-mode", this.editMode);
                    }
                }

            },
            /**
             * This helper function checks if there are any errors
             * @returns {boolean}
             */
            checkForErrors: function(){
                return this.states.regionName === null && this.states.frequency === null && this.states.firstDate === null;
            },
            /**
             * A helper function used to wait for a specified amount of time ... bsaically a thread.sleep()
             * @param ms
             * @returns {Promise<unknown>}
             */
            sleep: function(ms)
            {
                return new Promise(resolve => setTimeout(resolve, ms));
            },
        },
        /**
         * This contains the Vuelidate validations for the RegionName, FirstDate, and Frequency
         * */
        validations: {
            region: {
                regionName: {
                    required: validators.required,
                    minLength: validators.minLength(2),
                    maxLength: validators.maxLength(40)
                },
                frequency: {
                    required: validators.required,
                    minValue: validators.minValue(1),
                    maxValue: validators.maxValue(52)
                },
                firstDate: {
                    required: validators.required
                }
            }
        },

        computed: {

            /**
             * These are used in the b-form groups to signify errors
             * It will also evaluate against the response coming back from the API in case the client side validation failed
             * */
            states: function () {
                return {
                    regionName: this.$v.region.regionName.$error ? false : null || "status" in this.requestResponse && this.requestResponse.status === 400 && "data" in this.requestResponse && this.requestResponse.data.memberNames.includes("regionName") ? false : null,
                    firstDate: this.$v.region.firstDate.$error ? false : null || "status" in this.requestResponse && this.requestResponse.status === 400 && "data" in this.requestResponse && this.requestResponse.data.memberNames.includes("firstDate") ? false : null,
                    frequency: this.$v.region.frequency.$error ? false : null || "status" in this.requestResponse && this.requestResponse.status === 400 && "data" in this.requestResponse && this.requestResponse.data.memberNames.includes("frequency") ? false : null,
                }
            },
            /**
             * This will return the correct validation error string based on region Name
             * */
            errorRegionName: function () {
                let errorString = "";

                //Check the required fields
                if (!this.$v.region.regionName.required) {
                    errorString = "Region name is required";
                }

                //Check the min and max length
                else if (this.$v.region.regionName.minLength || this.$v.region.regionName.maxLength) {
                    errorString = "Region name must be between 2 and 40 characters";
                }

                else if ("regionName" in this.errors) {
                    errorString = this.errors.regionName[0];
                }

                else if ("data" in this.requestResponse && this.requestResponse.status === 400)          //Checking if an error was found in the request response
                {
                    if ("regionName" in this.requestResponse.data)
                    {
                        errorString = this.requestResponse.data.regionName[0];
                    }
                }

                //Return the error
                return errorString;
            },
            /**
             * This will return the correct validation error string based on the region's First Date
             * */
            errorFirstDate: function () {
                let errorString = "";

                //Check the required field
                if (!this.$v.region.firstDate.required) {
                    errorString = "First pickup date is required";
                } else if ("firstDate" in this.errors) {
                    errorString = this.errors.firstDate[0];
                }
                else if ("data" in this.requestResponse && this.requestResponse.status === 400)          //Checking if an error was found in the request response
                {
                    if ("firstDate" in this.requestResponse.data)
                    {
                        errorString = this.requestResponse.data.firstDate[0];
                    }
                }

                //return the error message
                return errorString;
            },
            /**
             *  This will return the correct validation error string based on the regions Frequency
             * */
            errorFrequency: function () {
                let errorString = "";

                if (!this.$v.region.frequency.required) {
                    errorString = "Collection frequency is required";
                } else if (this.$v.region.frequency.minValue || this.$v.region.frequency.maxValue) {
                    errorString = "Collection frequency must be between 1 and 52";
                }
                else if ("frequency" in this.errors) {
                    errorString = this.errors.frequency[0];
                }
                else if ("data" in this.requestResponse && this.requestResponse.status === 400)          //Checking if an error was found in the request response
                {
                    if ("frequency" in this.requestResponse.data)
                    {
                        errorString = this.requestResponse.data.frequency[0];
                    }
                }

                //return the error message
                return errorString;

            }
        },
    watch:
        {
            /**
             * This checks if the passed in Region from the table changes.. if so set the attributes to the details region object.
             * It will also convert the date values to a more readable format
             * Also checks if the region ID changes, if so cancel any editing
             */
            currentRegion:
                {
                    handler : function()
                    {
                        let oldRegionID = this.region.regionID;
                        this.region = JSON.parse(JSON.stringify(this.currentRegion));
                        this.region.firstDate = this.isoDate(this.currentRegion.firstDate, false).substring(0, 10);
                        this.region.nextDate = this.isoDate(this.currentRegion.nextDate, true).substring(0, 10);

                        if(oldRegionID !== this.region.regionID && this.region.regionID !== -1)
                        {
                            this.cancelEdit();
                        }

                    },
                    deep: true
                }
        },
        mounted()
        {
            //Resets the current route's routeID so details do not persist between page openings
            //this.region.regionID = 0;

        }
    }

</script>

<style scoped>
    label
    {
        font-size: 1.5em;
    }

</style>
