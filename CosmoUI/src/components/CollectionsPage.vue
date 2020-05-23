<template>
    <b-container>
        <!--Pagination element for table-->
        <b-pagination
                v-model="currentPage"
                :per-page="perPage"
                :total-rows="totalRows"
                align="fill"
                size="md"
                class="my-0"
                @change="handleCollectionPageChange(currentPage)"
        ></b-pagination>

        <!-- table element to used for our Subscriber Table -->
        <b-table
                show-empty
                striped hover
                id="CollectionHistory"
                :fields=fields
                :items="collectionProvider"
        >
        </b-table>

        <b-alert
                id="apiErrorAlert"
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

    export default {
        name: "CollectionsPage.vue",
        data: function(){
            return{
                fields:[{key:"routeDate", label: "Date Completed", sortable: true}],

                totalRows: 0, //total completed routes (used to calculate how many pages appear in the table

                //The amount of rows per table page
                perPage: 3,
                //the current table page being viewed
                currentPage: 1,
                //is the current sort in descending order
                sortDesc: true,
                //if there currently a db error
                dbError: false,
                //the sortBy value to send to the API
                sortBy: 'desc'
            }
        },
        methods:{
            /**
             * This method will handle a page change for tbe collection table
             * @param page
             */
            handleCollectionPageChange: function(page){

                this.currentPage = page;
                this.$root.$emit('bv::refresh::table', 'CollectionHistory'); //refresh the table
            },

            /**
             * This function will handle getting subscriber data from the API
             * @param ctx
             */
            collectionProvider: function(ctx){
                //create the axios get url
                let url = this.apiURLBase +  'routes/';
                //set the sort arrow
                if (ctx.sortBy) {
                    this.sortBy = ctx.sortDesc ?  'desc' : 'asc' ;
                }

                //append sortby
                url += "sortBy-s" + this.sortBy;
                url += "subscriberID-i" + this.$store.state.currentSubscriber.subscriberID;

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
                    //get the routes returned from the response
                    let routes = response.data;

                    //For each date call the format method on each date that is retrieved
                    for(let i = 0; i < routes.length; i++)
                    {
                        let route = routes[i];
                        route.routeDate = this.formatDate(route.routeDate);
                    }
                    //return the routes with the formatted dates and place then into the table
                    return routes;
                }).catch((errors) => {
                    if(!errors.response)        //If network error occurs, no content is returned in the response
                    {
                        this.$emit('db-response', false);   //Emit db-response with false content
                    }
                })
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
            /**
             * This function wil be responsible for making a call to the api to grab the total amount of pages needed based on how many collections are shown on each
             * table page
             * @returns {Promise<void>}
             */
            getTotalPages: function(){
                let promise = axios({
                        method: 'GET',
                        url: this.apiURLBase + "routes/totalRows-rsubscriberID-s" + this.$store.state.currentSubscriber.subscriberID,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                return promise.then((response) => {
                    this.$emit('db-response', true); //no error message needed
                    this.totalRows = response.data; //set totalRows
                    this.$root.$emit('bv::refresh::table', 'CollectionHistory'); //refresh table
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
            //get the total pages when page first loads
            this.getTotalPages();
        }

    }
</script>

<style scoped>

</style>