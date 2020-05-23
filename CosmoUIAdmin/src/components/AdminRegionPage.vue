<template>
    <b-container>

        <!-- The Deletion modal used to remove a Region
            Will ask the admin if they will to remove all the routes or to keep them
            I thought it would be nice to have to choice to delete or keep routes -->
        <b-modal no-close-on-esc no-close-on-backdrop no-stacking hide-header-close id="deleteRegionModal" @ok="prepareDeleteRegion">
            <div v-if="!confirmDeleteRegion">
            Are you sure you want to delete Region {{region.regionName}}?
            </div>
            <div v-else>
                Would you also like to remove all of the routes belonging to {{region.regionName}}?
            </div>
            <template v-slot:modal-footer>
                <div v-if="!confirmDeleteRegion">
                    <b-button  variant="outline-danger" @click="prepareDeleteRegion" >Okay</b-button>
                </div>
                <div v-else>
                    <b-button  variant="outline-danger" @click="deleteRegionAndRoutes">Yes, delete all routes</b-button>
                    <b-button  variant="outline-warning" @click="deleteRegion">No, keep routes</b-button>
                </div>
                    <b-button variant="outline-primary" @click="closeModal">Cancel</b-button>
            </template>
        </b-modal>

        <b-jumbotron header="Region Table" class="text-center py-4 mt-2 shadow"></b-jumbotron>
        <!-- The Region table and Detail Pane
             They're currently sharing equal space on the page   -->
        <b-row>
            <b-col cols="6">
            <b-button variant="outline-primary" @click="showInactive" :pressed="showAll" >Show All</b-button>
            <admin-region-list @show-details="sendDetails" :showAll="showAll" ></admin-region-list>
            </b-col>

            <b-col>
                <admin-region-details :request-response="requestResponse" :currentRegion="region" :showAll="showAll" @put-region="putRegion" @post-region="postRegion" @edit-mode="handleEdit">
                </admin-region-details>
            </b-col>
        </b-row>
        <b-row>
            <admin-region-locations :selected-region="this.region" :edit-mode="this.editMode"></admin-region-locations>
        </b-row>


        <b-alert
                v-model="apiError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible>
            Cannot reach server, please try again later.
        </b-alert>
    </b-container>
</template>

<script>
    /* eslint-disable no-console */
    import AdminRegionList from "./AdminRegionList.vue"
    import AdminRegionDetails from "./AdminRegionDetails"
    import AdminRegionLocations from "./AdminRegionLocations";
    import axios from 'axios'
    export default {
        name: "AdminRegionPage.vue",
        components: {
            AdminRegionLocations,
            AdminRegionDetails,
            AdminRegionList,
        },
        data: function(){
            return {
                //Region object to be populated with our inputted values
                region: {
                    regionID: 0,
                    regionName: "",
                    frequency: 0,
                    firstDate: {},
                    inactive: false
                },
                //Array of errors
                errors: {},
                //Boolean to figure out when an api error has occurred
                apiError: false,
                showAll: false,
                requestResponse: {},
                confirmDeleteRegion: false,
                deleteRoutes: false,
                editMode: false
            }
        },
        methods:{
            handleEdit: function(status)
            {
                this.editMode = status;
            },
            sendDetails: function(region){
                this.region = region;
            },
            //This method will be called when the save button is selected
            //It will send our region object to the api to be processed
            postRegion: function(region)
            {
                //Post the object to the API
                axios({
                    method: 'POST',
                    url: this.apiURLBase + "Regions",
                    data: region,
                    headers: {
                        'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                    this.requestResponse = response;
                    if(this.requestResponse.status === 201)
                    {
                        this.region = response.data;
                        //If we succeeded refresh table and close the modal
                        this.$root.$emit('bv::refresh::table', 'regionTable');
                    }
                }).catch(errors => {
                    //Post failed
                    this.requestResponse = errors.response;
                    if(this.requestResponse === undefined)
                    {
                        //Signify an API error
                        this.apiError = true;
                    }
                    else if (this.requestResponse.status === 400)
                    {
                        //Set the error message to the data returned
                        this.errors = this.requestResponse.data;
                    }
                })

            },
            /**
             * This Function will do a PUT request to the API
             * Will check the response and store it into a variable that is used on the emitting method in Details
             * @param region
             */
            putRegion: function(region)
            {
                axios({
                    method: 'PUT',
                    url: this.apiURLBase + "Regions/" + this.region.regionID,
                    data: region,
                    headers: {
                        'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                    this.requestResponse = response;
                    Object.assign(this.region,response.data);
                    if(this.requestResponse.status === 200)
                    {
                        //If we succeeded refresh table and close the modal
                        this.$root.$emit('bv::refresh::table', 'regionTable');
                    }
                }).catch(errors => {
                    //Post failed
                    this.requestResponse = errors.response;
                    if(this.requestResponse === undefined)
                    {
                        //Signify an API error
                        this.apiError = true;
                    }
                    else if (this.requestResponse.status === 400)
                    {
                        //Set the error message to the data returned
                        this.errors = this.requestResponse.data;
                    }
                })
            },
            /**
             * A Helper function for closing the delete modal
             */
            closeModal: function () {
                this.$bvModal.hide('deleteRegionModal');
                this.confirmDeleteRegion = false;
            },
            /**
             * This helper function will be used when first confirming to delete a Region
             * Will change the confirmDeleteRegion to true to allow the Question to keep or remove associated routes
             */
            prepareDeleteRegion: function () {
                this.confirmDeleteRegion = true;
            },
            /**
             * This helper function is called when choosingto remove all routes, sets the deleteRoutes boolean to true
             * To be evaluated on the API side to remove associted routes
             */
            deleteRegionAndRoutes: function () {
              this.deleteRoutes = true;
              this.deleteRegion();
            },
            /**
             * This function will do an DELETE request to the API
             * Will send the Region ID corresponding to the REgion to delete
             * Will also said a boolean parameter used to determine to delete its associated routes or not
             */
            deleteRegion: function () {
                this.isDeleting = true;
                axios({
                    method: 'DELETE',
                    url: this.apiURLBase + "Regions/" + this.region.regionID + "routes" + this.deleteRoutes,
                    data: this.region.regionID,
                    headers: {
                        'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                    console.log(response);
                    if(response.status === 200)
                    {
                        //If we succeeded refresh table
                        this.$root.$emit('bv::refresh::table', 'regionTable');
                        //Set the regionID to -1 to prevent the details pane from showing the deleted Region
                        this.region.regionID = -1;
                    }
                }).catch(errors => {
                    //Post failed
                    let response = errors.response;
                    console.log(response);
                    if(response === undefined)
                    {
                        //Signify an API error
                        this.apiError = true;
                    }
                    else if (response.status === 400)
                    {
                        //Set the error message to the data returned
                        this.errors = response.data;
                    }
                });
                this.deleteRoutes = false;
                this.$bvModal.hide('deleteRegionModal');
                this.confirmDeleteRegion = false;

            },
            /**
             * A helper function used with the showAll Button to Request all Regions from the API's get request
             * Will refresh the region table to display the changes
             */
            showInactive: function()
            {
                this.showAll = !this.showAll;
                this.$root.$emit('bv::refresh::table', 'regionTable');
            }
        },
    }
</script>

<style scoped>

</style>
