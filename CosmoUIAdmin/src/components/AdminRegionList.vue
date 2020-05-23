<template>
    <b-container id="regionTableContainer">


        <b-pagination
                v-model="currentPage"
                :per-page="perPage"
                :total-rows="totalRows"
                align="fill"
                size="sm"
                class="my-0"
        ></b-pagination>

        <b-table
                id="regionTable"
                show-empty
                striped hover
                :items="regionsProvider"
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
        >
            <template v-slot:cell(firstDate)="row">
                <span>{{row.item.firstDate | formatDate}}</span>
            </template>

            <template v-slot:cell(nextDate)="row">
                <span>{{row.item.nextDate[0] | formatDate}}</span>
            </template>

        </b-table>
        <b-alert
                v-model="apiError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible>
            Cannot reach server, please try again later.
        </b-alert>

<!--        <admin-region-locations :selectedRegion="this.selectedRegion"></admin-region-locations>-->
    </b-container>
</template>

<script>
    /* eslint-disable no-console */
   //import {AxiosInstance as axios} from "axios";
   // import AdminRegionLocations from "./AdminRegionLocations";
    const axios = require('axios').default;

    export default {
        name: "AdminRegionList",
        components: {},
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
        data: function(){
          return{
              fields:
                  [
                      {key: 'regionName', label:"Region Name", sortable:true},
                      // {key: 'frequency', label:"Collection Frequency", sortable: true},
                      // {key: 'firstDate', label:"First Pickup Date", sortable: true},
                      {key:'nextDate', label:"Next Pickup Date", sortable: true}
                  ],
              items: [],
              perPage: 5,
              currentPage: 1,
              totalRows:0,
              selectMode: 'single',
              selected: {},
              sortBy: 'regionName',
              sortDesc: false,
              dbError: false,
              apiError: false,

              selectedRegion:  //A default constructor for the selectedRegion Object
                  {
                      regionID: 0,
                      regionName: "",
                      frequency: 0,
                      firstDate: {},
                      inactive: false
                  },
          }
        },
        methods:{
            regionsProvider: function ()
            {
                let promise = null;

                if(this.showAll)
                {
                   promise = axios({
                    method: 'GET',
                    url: this.apiURLBase + "Regions/showInactive",
                    headers: {
                        'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                    }
                   });
                }
                else
                {
                    promise = axios({
                        method: 'GET',
                        url: this.apiURLBase + "Regions",
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    });
                }

                return promise.then((response) => {
                    this.totalRows = response.data.length;
                    return response.data;
                }).catch((errors) =>
                {
                    let response = errors.response;
                    if(response === undefined)
                    {
                        this.apiError = true;
                    }
                })
            },
            expandDetails: function(item)   //prepares data to send to Details pane. Fills region with regionName
            {
                this.$emit('show-details',item);    //Emits show-details with item
            },
            onRowSelected: function (items){
                Object.assign(this.selected, items); //Assigns selected item to this.selected object
                this.expandDetails(this.selected[0]);   //Calls expandDetails to send the item
                this.selectedRegion = items[0];
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
