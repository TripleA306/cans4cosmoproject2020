<template>
    <b-container>
        <b-jumbotron header="Route Table" class="text-center py-4 mt-2 shadow"></b-jumbotron>
        <b-row>
            <!--Route Table component-->
            <b-col id="rtTableCol"  cols="6">
                <b-button id="btnShowAll" :pressed="showAll" @click="toggleShowAll">Show All</b-button>
                <admin-route-table @show-details="sendDetails" @db-response="handleDBError" :showAll="showAll" :providerUrl="provUrl"></admin-route-table>
            </b-col>

            <!--Route Details component-->
            <b-col id="rtDetailsCol" >
                <admin-route-details :requestResponse="requestResponse" :currentRoute="route" @start-delete="startDelete" @save-route="putRoute"></admin-route-details>
            </b-col>
        </b-row>

        <b-row>
            <p class="networkError" id="networkError" v-if="dbError">A network error occurred when connecting with the database</p>
        </b-row>

        <admin-route-delete-modal :visible="showDeleteModal" :routeName="route.routeName" @delete-route="deleteRoute"></admin-route-delete-modal>

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
    import AdminRouteTable from './AdminRouteTable.vue'
    import AdminRouteDetails from './AdminRouteDetails.vue'
    import AdminRouteDeleteModal from './AdminRouteDeleteModal.vue'
    import axios from 'axios'

    export default {
        name: "AdminRoutesPage",
        components: {
            AdminRouteTable,
            AdminRouteDetails,
            AdminRouteDeleteModal
        },
        data: function(){
            return{
                route: {},                  //Route object to pass to Details
                provUrl: this.apiURLBase,  //provider URL
                dbError: false,
                deleteModalID: "delete-modal",
                showDeleteModal: false,
                errorResponse: {},
                showAll: false,
                requestResponse: {}
            }
        },
        methods:{
            sendDetails: function(route)
            {
                this.route = route;     //Sets this.route to incoming route's properties to send to Details pane
            },
            handleDBError: function(value)          //Checks whether or not a DB error is detected
            {
                value ? this.dbError = false : this.dbError = true;
            },
            // eslint-disable-next-line no-unused-vars
            putRoute: function(route)
            {
                //eslint-disable-next-line no-console

                // eslint-disable-next-line no-undef
                let promise = axios({
                    method: 'PUT',
                    url: this.provUrl + "Routes/" + route.routeID,
                    data: route,
                    headers: {
                        'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                    }
                });  //Targets the /api/Routes directory at the providerUrl

                return promise.then((response) =>{

                    this.requestResponse = response;
                    Object.assign(this.route, response.data);
                    this.$root.$emit('bv::refresh::table','route-table');

                    //if the newly updated route is now completed and the "Show All Routes" option is off
                    if(this.route.completed && !this.showAll)
                    {
                        //clear out the route
                        this.route.routeID = -1;
                    }

                    //this.$refs.routeTableRef.refresh();
                    return response.data;
                }).catch(errors =>{
                    // eslint-disable-next-line no-console

                    if(errors.response.status === 400)
                    {
                        this.requestResponse = errors.response;
                    }
                })
            },
            startDelete:function()
            {
                this.showDeleteModal = true;
                this.$bvModal.show(this.deleteModalID);
            },
            deleteRoute: function()
            {
                this.route.inactive = true;
                let promise = axios({
                        method: "DELETE",
                        url: this.provUrl + "Routes/" + this.route.routeID,
                        data: this.route,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });

                return promise.then((response) =>{
                    this.route.routeID = -1;
                    this.$root.$emit('bv::refresh::table','route-table');
                    return response.data;
                }).catch(errors =>
                {
                    this.errorResponse = errors;
                })
            },
            toggleShowAll:function() {
                this.showAll = !this.showAll;
                this.$root.$emit('bv::refresh::table', 'route-table');
            },
        },
        mounted() {
            if(!sessionStorage.getItem("sessionID"))
            {
                this.$router.push('login');
            }
            else
            {
                this.$emit("login");
            }
        }
    }
</script>

<style scoped>

</style>