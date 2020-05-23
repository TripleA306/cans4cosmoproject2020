<template>
    <b-container>
<!--        <b-button id="btnShowAll" :pressed="showAll" @click="toggleShowAll">Show All</b-button>-->
        <!--Pagination element for table-->
        <b-pagination
                v-model="currentPage"
                :per-page="perPage"
                :total-rows="totalRows"
                align="fill"
                size="sm"
                class="my-0"
        ></b-pagination>

        <!--Route table element-->
        <b-table
                id="route-table"
                show-empty
                striped hover
                :items="getRoutes"
                :fields="fields"
                no-provider-sorting
                :per-page="perPage"
                no-provider-paging
                :current-page="currentPage"
                selectable
                :select-mode="selectMode"
                @row-selected="onRowSelected"
                :sort-by.sync="sortBy"
                :sort-desc.sync="sortDesc"
                fixed
        >
            <!--Modifying colgroups to apply column styling. Used to make Completed column smaller than the others-->
            <template v-slot:table-colgroup="scope">
                <col
                        v-for="field in scope.fields"
                        :key="field.key"
                        :style="{ width: field.key === 'completed' ? '8em' : '100%'}"
                >
            </template>

            <!--Applying dateFormat method to each routeDate-->
            <template v-slot:cell(routeDate)="row">
                <span>{{row.item.routeDate | formatDate}}</span>
            </template>

            <!--Adding circle-check or circle icons to completed column based on completed status-->
            <template v-slot:cell(completed)="row">
                <span class="complete"  v-if="row.item.completed">
                    <b-icon-check-circle class="h1 mb-2" variant="success"></b-icon-check-circle>
                </span>

                <span class="incomplete" v-else>
                    <b-icon-circle class="h1 mb-2" variant="danger"></b-icon-circle>
                </span>

            </template>
        </b-table>


    </b-container>


</template>

<script>
    /* eslint-disable no-console */

    import axios from 'axios'
    // eslint-disable-next-line no-unused-vars
    import moment from 'moment'
import { BIconCheckCircle, BIconCircle } from 'bootstrap-vue'
   export default
        {
        name: "AdminRoute",
        components:
            {
                BIconCircle,        //Imported bootstrap circle icon
                BIconCheckCircle    //Imported bootstrap check circle icon
            },
        props:
            {
                providerUrl:{
                    type: String,
                    default: 'http://localhost:5002'    //Provider URL property. Defaults to localhost:5002
                },
                showAll:{
                    type:Boolean,
                    default: false
                }
            },
        data: function()
        {
            return {
                fields: //Fields to use for the table
                    [
                        { key:'routeName', label:"Route Name", sortable:true},
                        { key:'routeDate', label:"Date", sortable:true},
                        { key:'completed', label:"Completed", sortable:true, class: 'completedMinWidth'}
                    ],
                items: [],
                perPage: 5, //5 items per page in pagination
                currentPage: 1, //Defaults to page 1 in pagination
                totalRows:0,    //How many items are returned in table
                selectMode: 'single',   //Selection mode set to single item
                selected: {},           //Which object is selected
                sortBy: 'completed',    //Which column to sort by
                sortDesc: false,        //Sorts ascending
                dbError: false,          //Whether dbError is found
            }
        },
        methods:
            {
                // eslint-disable-next-line no-unused-vars
                getRoutes: function()
                {
                    // eslint-disable-next-line no-undef
                    let promise;

                    if(this.showAll)
                    {
                        // eslint-disable-next-line no-unused-vars
                        promise = axios({
                            method: 'GET',
                            url: this.providerUrl + "Routes/showComplete",
                            headers: {
                                'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                            }
                        });
                    }
                    else
                    {
                        promise = axios({
                            method: 'GET',
                            url: this.providerUrl + "Routes/Regions",
                            headers: {
                                'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                            }
                        });  //Targets the /api/Routes directory at the providerUrl
                    }

                    return promise.then((response) =>{
                        this.totalRows = response.data.length;      //Sets totalRows to total items returned
                        this.$emit('db-response', true);         //Emits db-response with true content
                        return response.data;

                    }).catch(errors =>{
                        if(!errors.response)        //If network error occurs, no content is returned in the response
                        {
                            this.$emit('db-response', false);   //Emit db-response with false content
                        }
                    })
                },
                getProvUrl: function()          //Updates the provider url based on possible modfiieres added to URL
                {
                    let provUrl = window.location.href;
                    let paramIndex = provUrl.indexOf('?noApi');
                    //?noApi modifier used to force an invalid API link
                    if (paramIndex !== -1) {
                        this.providerUrl = "http://localhost/:-1000";        //Fake API url
                    }
                },
                expandDetails: function(item)   //prepares data to send to Details pane. Fills region with regionName
                {
                    this.$emit('show-details',item);    //Emits show-details with item
                },

                onRowSelected: function(items)  //Handles preparing an item when a table item is selected
                {
                    Object.assign(this.selected, items);        //Assigns selected item to this.selected object

                    this.expandDetails(this.selected[0]);   //Calls expandDetails to send the item
                }

            },
        computed:
            {

            },
        mounted()
            {
                this.getProvUrl();
            }
        }
</script>

<style scoped>

</style>