<template>

    <b-container>

        <!-- A jumbotron used for our Subscriber table header -->
        <b-jumbotron header="Subscriber Table" class="text-center py-4 mt-2 shadow"></b-jumbotron>

        <!-- A modal used to Confirm "deletion" of a Subscriber -->
        <b-modal
                id="deleteModal"
                 title="Delete"
                @ok="deleteSubscriber"
                ok-variant="outline-danger"
                cancel-variant="outline-primary"
        >
            <!--The message to display in modal to confirm deletion, will "personalize" the message if the subscriber has a first name -->
            <p v-if="currentSubscriber.firstName !== null" class="my-4">
                Are you sure you want to delete {{currentSubscriber.firstName}}<span v-if="currentSubscriber.lastName !== null"> {{currentSubscriber.lastName}}</span>?
            </p>
            <p v-else class="my-4">Are you sure you want to delete {{currentSubscriber.email}}?</p>
        </b-modal>

        <!--Pagination element for table-->
        <b-pagination
                v-model="currentPage"
                :per-page="perPage"
                :total-rows="totalRows"
                align="fill"
                size="sm"
                class="my-0"
                @change="handleTablePageChange(currentPage)"
        ></b-pagination>

        <!-- table element to used for our Subscriber Table -->
        <b-table
                show-empty
                striped hover
                id="AdminSubscriberPage"
                :fields=fields
                :items="subscriberProvider"

        >
			<!-- this element is used to display the opt out icon in the actions field -->
            <template v-slot:cell(optOut)="row">
                <b-button id="btnOptout" variant="outline-success" size="lg" class="mb-2" @click="optOut(row.item.subscriberID)">
                    <b-icon-house></b-icon-house>
                </b-button>
            </template>
            <!-- this element is used to display the trash can icon in the actions field -->
            <template v-slot:cell(actions)="row">
                <b-button variant="outline-danger" size="lg" class="mb-2" @click="confirmDelete(row.item)">
                    <b-icon-trash></b-icon-trash>
                </b-button>
            </template>
        </b-table>

        <b-alert
                v-model="dbError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible>
            Cannot reach server, please try again later.
        </b-alert>
    </b-container>
</template>

<script>

    import axios from 'axios'
    import {BIconTrash} from 'bootstrap-vue' //Import for the bootstrap trashcan icon
    export default
    {
        name: "AdminSubscriberPage",
        components:
            {
                BIconTrash
            },
        data: function()
        {
            return {

                //used to create our table headers
                fields:[{key:"email", label: "Email", sortable: true},
                        {key:"firstName", label: "First Name"},
                        {key:"lastName", label: "Last Name"},
                        {key:"actions", label: "Actions"}],
                currentSubscriber: {},
                perPage: 5, //5 items per page in pagination
                currentPage: 1, //Defaults to page 1 in pagination
                totalRows:0,    //How many items are returned in table
                selectMode: 'single',   //Selection mode set to single item
                sortDesc: false,        //Sorts ascending
                dbError: false,          //Whether dbError is found
                sortBy: 'asc' //sort by method
            }
        },
        methods:
            {
                /**
                 * This method will be called to update the table whenever the table page has changed
                 * @param page
                 */
                handleTablePageChange: function (page){
                  this.currentPage = page; //set the current page to the new page
                    this.$root.$emit('bv::refresh::table', 'AdminSubscriberPage'); //refresh the table
                },
                /**
                 * This function will handle getting subscriber data form the API
                 * @param ctx
                 * @returns {Promise<AxiosResponse<T> | never>}
                 */
                subscriberProvider: function(ctx){
                    let url = this.apiURLBase +  'subscribers/';
                    if (ctx.sortBy) {
                        this.sortBy = ctx.sortDesc ?  'desc' : 'asc' ;
                    }
                    //append sortby
                    url += "sortBy-s" + this.sortBy;
                    //append pageSize
                    url += "sizePerPage-p" + this.perPage;
                    //append current page index
                    url += "currentPage-c" + this.currentPage;
                    let promise = axios({
                       method: 'GET',
                       url: url,
                       headers: {
                           'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                       }
                    });

                    return promise.then((response) =>{
                        this.$emit('db-response', true);
                        return response.data
                    }).catch((errors) => {
                        if(!errors.response)        //If network error occurs, no content is returned in the response
                        {
                            this.$emit('db-response', false);   //Emit db-response with false content
                        }
                    })
                },
                /**
                 * This function will handle the "deletion" of a Subscriber by changing its status to inactive in the database
                 * @returns {Promise<AxiosResponse<T> | never>}
                 */
                deleteSubscriber: function(){
                    //do after confirming the delete

                    //send a put request to the api with the email of the subscriber to delete
                    let promise = axios({
                        method: 'DELETE',
                        url: this.apiURLBase +  "subscribers/" + this.currentSubscriber.subscriberID,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                    // eslint-disable-next-line no-unused-vars
                    return promise.then((response) => {
                        //hide the modal
                        this.$bvModal.hide('deleteModal');
                        this.getTotalPages(); //get the total pages to update the table with
                        this.$root.$emit('bv::refresh::table', 'AdminSubscriberPage'); //refresh the table.
                        // eslint-disable-next-line no-unused-vars
                    }).catch(errors =>{
                        //if an error occurs, set an error message under the table
                        if(!errors.response)        //If network error occurs, no content is returned in the response
                        {
                            this.dbError = true;   //Emit db-response with false content
                        }
                    })
                },
                /**
                 * This function will bring up a modal for the user to confirm a delete of a subscriber.
                 * @param subscriber
                 */
                confirmDelete: function(subscriber){
                    this.currentSubscriber = subscriber; //set the current subscriber
                    this.$bvModal.show('deleteModal'); //show the modal
                },



                /**
                 * This function will be responsible for making a call to the api to grab the total amount of pages needed based on how many
                 * subscribers are shown on each page
                 * @returns {Promise<AxiosResponse<T> | Promise<AxiosResponse<T> | never>>}
                 */
                getTotalPages: async function(){
                    //send a get request to the api
                    let promise = axios({
                        method: 'GET',
                        url: this.apiURLBase + "subscribers/totalRows-r",
                        headers:{
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                    return promise.then((response) => {
                        this.$emit('db-response', true); //no error message needed
                        this.totalRows = response.data; //set totalRows
                        this.$root.$emit('bv::refresh::table', 'AdminSubscriberPage'); //refresh table
                    }).catch((errors) => {
                        //if an error occurs, show an error message
                        if(!errors.response)        //If network error occurs, no content is returned in the response
                        {
                            this.dbError = true;   //Emit db-response with false content
                        }
                    })
                }

            },
        mounted()
        {
            if(!sessionStorage.getItem("sessionID")) {
                this.$router.push('login');
            }
            else
            {
                this.$emit("login");
            }
            //get the total pages needed from the api.
            this.getTotalPages();
        }
    }
</script>

<style scoped>

</style>
