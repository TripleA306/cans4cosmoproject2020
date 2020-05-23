<template>
    <b-container class="bv-example-row">
        <!--Location List Rows-->
        <b-row id="locationLists" align-h="between" align-v="center">
            <!--Region Locations-->
            <b-col class="locTableCol">
                <h3 id="regionLocHeader" v-if="selectedRegion.regionID <= 0">Region Locations</h3>
                <h3 id="regionLocHeader" v-else>{{selectedRegion.regionName}} Locations</h3>
                <b-pagination
                        id="assignedPagination"
                        v-model="currentAssignPage"
                        :per-page="perPage"
                        :total-rows="totalAssignRows"
                        align="fill"
                        size="sm"
                        class="my-0"
                        @change="handleAssignPageChange"
                ></b-pagination>
                <b-table
                        id="assigned-locations-table"
                        ref="assignedLocationsTable"
                        show-empty
                        striped hover
                        :items="regionLocs"
                        :fields="locationFields"
                        no-provider-sorting
                        :per-page="perPage"
                        no-provider-paging
                        selectable
                        :select-mode="selectMode"
                        :empty-text=emptyRegionMessage
                        @row-selected="onAssignRowSelected"
                >
                    <template v-slot:head(aptUnit)="data">
                        <span>{{data.label}}</span>
                    </template>

                    <template v-slot:cell(aptUnit)="row">
                        <span :style="{ width: '25%' }">{{row.value}}</span>
                    </template>

                    <template v-slot:cell(selected)="row">
                        <b-form-checkbox size="lg" @change="regionLocSelect(row.item)" :checked="checkRow(row.item)"></b-form-checkbox>
                    </template>
                </b-table>
            </b-col>

            <!--Add/Remove Locations button column-->
            <b-col id="locBtnCol" md="auto">
                <b-button-group id="locationButtons" vertical>
                    <b-button id="assignLocBtn" @click="assignLocation" :disabled="!editMode"><b-icon-arrow-left/></b-button>
                    <b-button id="removeLocBtn" @click="unassignLocation" :disabled="!editMode"><b-icon-arrow-right/></b-button>
                </b-button-group>
            </b-col>

            <!--Unassigned Locations Table-->
            <b-col class="locTableCol">
                <h3 id="unassignedLocHeader">Unassigned Locations</h3>
                <b-pagination
                        id="unassignedPagination"
                        v-model="currentUnassignPage"
                        :per-page="perPage"
                        :total-rows="totalUnassignRows"
                        align="fill"
                        size="sm"
                        class="my-0"
                        @change="handleUnassignPageChange"
                ></b-pagination>
                <b-table
                        id="unassigned-locations-table"
                        ref="unassignedLocationsTable"
                        show-empty
                        striped hover
                        :items="unassignedLocs"
                        :fields="locationFields"
                        no-provider-sorting
                        :per-page="perPage"
                        no-provider-paging
                        selectable
                        :select-mode="selectMode"
                        @row-selected="onUnassignRowSelected">
                    <template v-slot:head(aptUnit)="data">
                        <span>{{data.label}}</span>
                    </template>

                    <template v-slot:cell(aptUnit)="row">
                        <span :style="{ width: '25%' }">{{row.value}}</span>
                    </template>

                    <template v-slot:cell(selected)="row">
                        <b-form-checkbox size="lg" @change="unassignedLocSelect(row.item)" :checked="checkRow(row.item)"></b-form-checkbox>
                    </template>
                </b-table>
            </b-col>
        </b-row>
    </b-container>
</template>

<script>
    /* eslint-disable no-console */
    const axios = require('axios').default;
    import { BIconArrowLeft,  BIconArrowRight } from 'bootstrap-vue'
    export default {
        name: "AdminRegionLocations",
        props:
            {
                selectedRegion:
                    {
                        type: Object,
                        default: ()=>( //A default constructor for the selectedRegion Object
                            {
                                regionID: 0,
                                regionName: "",
                                frequency: 0,
                                firstDate: {},
                                inactive: false
                            })
                    },
                editMode:
                    {
                        type: Boolean,
                        default: false
                    },
            },
        components:
            {
              BIconArrowLeft,
              BIconArrowRight,
            },
        data: function()
        {
            return {
                selectMode: 'range',
                currentAssignPage: 1,
                totalAssignRows: 0,
                currentUnassignPage: 1,
                totalUnassignRows: 0,
                perPage: 8,
                //Fields to be displayed in Locations tables
                locationFields:
                    [
                        {key: 'address', label: "Street", sortable: true},
                        {key: 'unit', label: "Unit #", sortable: false, thStyle: {width: '15%'}},
                        {key: 'selected', label: "Select", sortable: false, thStyle: {width: '10%'}},
                    ],
                regionLocs: [],
                unassignedLocs: [],
                //"Master" array of locations assigned to the region
                //Temporary array to store locations that are assigned to the Region before Save/Discard
                selectedRegionLocations: [],
                //Temporary array to store locations that are unassigned before Save/Discard
                selectedUnassignedLocations: [],

                viewMode: false,
                emptyRegionMessage: "",
                emptyUnassignMessage: "No unassigned locations found",
            }
       },
        methods:
            {
                handleAssignPageChange: function (page){
                    this.currentAssignPage = page; //set the current page to the new page
                    this.getAssignedLocations();
                    this.$root.$emit('bv::refresh::table', 'assigned-locations-table');

                },
                handleUnassignPageChange: function (page){
                    this.currentUnassignPage = page; //set the current page to the new page
                    this.getUnassignedLocations();
                    this.$root.$emit('bv::refresh::table', 'unassigned-locations-table');
                },
                //Enables edit mode, which enables to arrow buttons
                startEdit: function()
                {
                    this.editMode = true;
                },
                //Discards any changes made and disables edit mode
                endEdit: function()
                {
                    this.editMode = false;
                },
                getAssignCount: function()
                {
                    axios({
                        method: 'GET',
                        url: this.apiURLBase + "Locations/regcount=" + this.selectedRegion.regionID,
                        headers:
                            {
                                'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                            }
                    }).then((response) => {
                        this.totalAssignRows = response.data;
                    }).catch((errors) =>
                    {
                        let response = errors.response;

                        if(response === undefined)
                        {
                            this.apiError = true;
                        }
                    });

                },
                getUnassignCount: function()
                {
                    axios({
                        method: 'GET',
                        url: this.apiURLBase + "Locations/unassigncount",
                        headers:
                            {
                                'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                            }
                    }).then((response) => {
                        this.totalUnassignRows = response.data;
                    }).catch((errors) =>
                    {
                        let response = errors.response;

                        if(response === undefined)
                        {
                            this.apiError = true;
                        }
                    });

                },
                //Sends a GET request to API to retrieve an array of all locations associated with the currently selected region ID
                getAssignedLocations: function()
                {
                    axios({
                        method: 'GET',
                        url: this.apiURLBase + "Locations/regid=" + this.selectedRegion.regionID + "/" + this.currentAssignPage,
                        headers:
                            {
                                'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                            }
                    }).then((response) => {
                        if(Array.isArray(response.data))
                        {
                            this.regionLocs = response.data;
                        }
                        else
                        {
                            this.totalAssignRows = 1;
                            this.regionLocs = [];
                        }
                    }).catch((errors) =>
                    {
                        let response = errors.response;

                        if(response === undefined)
                        {
                            this.apiError = true;
                        }
                    });

                },
                //Sends a GET request to API to retrieve an array of all locations which are NOT associated with any regions
                getUnassignedLocations: function()
                {
                    axios({
                        method: 'GET',
                        url: this.apiURLBase + "Locations/unassigned/" + this.currentUnassignPage,
                        headers:
                            {
                                'Authorization': 'Bearer ' + sessionStorage.getItem('sessionID')
                            }
                    }).then((response) => {
                        if(Array.isArray(response.data))
                        {
                            this.unassignedLocs = response.data;
                        }
                        else
                        {
                            this.totalUnassignRows = 1;
                            this.unassignedLocs = [];
                        }

                    }).catch((errors) =>
                    {
                        let response = errors.response;
                        if(response === undefined)
                        {
                            this.apiError = true;
                        }
                    })

                },
                onUnassignRowSelected(items)
                {
                    this.selectedUnassignedLocations = items;
                },
                onAssignRowSelected(items)
                {
                    this.selectedRegionLocations = items;
                },
                regionLocSelect: function(item)
                {
                    if(this.selectedRegionLocations.includes(item))
                    {
                        let index = this.selectedRegionLocations.indexOf(item);
                        this.selectedRegionLocations.splice(index, 1);
                    }
                    else
                    {
                        this.selectedRegionLocations.push(item);
                    }
                },
                unassignedLocSelect: function(item)
                {
                    if(item.value === "select")
                    {
                        let index = this.selectedUnassignedLocations.indexOf(item);
                        this.selectedUnassignedLocations.splice(index, 1);
                    }
                    else
                    {
                        this.selectedUnassignedLocations.push(item);
                    }
                },
                //Called when <-- button clicked. Moves any selected locations to the Assigned table
                assignLocation: async function()
                {
                    if(this.selectedUnassignedLocations.length > 0 && this.selectedRegion.regionID > 0)
                    {
                        let promise = axios.put(this.apiURLBase + "Locations/assign/" + this.selectedRegion.regionID, this.selectedUnassignedLocations);

                        return promise.then((response) => {
                            if(Array.isArray(response.data) && response.data.length === this.selectedUnassignedLocations.length)
                            {
                                this.getUnassignCount();
                                this.getAssignCount();
                                this.getUnassignedLocations();
                                this.getAssignedLocations();
                                this.$root.$emit('bv::refresh::table', 'assigned-locations-table');
                                this.$root.$emit('bv::refresh::table', 'unassigned-locations-table');

                                this.selectedUnassignedLocations = [];
                            }
                        }).catch((errors) =>
                        {
                            let response = errors.response;
                            if(response === undefined)
                            {
                                this.apiError = true;
                            }
                        })
                    }
                },
                //Called when --> button clicked. Moves any selected locations to the Unassigned table
                unassignLocation: async function()
                {
                    if(this.selectedRegionLocations.length > 0)
                    {
                        let promise = axios.put(this.apiURLBase + "Locations/assign/0", this.selectedRegionLocations);

                        return promise.then((response) => {
                            if(Array.isArray(response.data) && response.data.length === this.selectedRegionLocations.length)
                            {
                                this.getUnassignCount();
                                this.getAssignCount();
                                this.getUnassignedLocations();
                                this.getAssignedLocations();
                                this.$root.$emit('bv::refresh::table', 'assigned-locations-table');
                                this.$root.$emit('bv::refresh::table', 'unassigned-locations-table');

                                this.selectedRegionLocations = [];
                            }
                        }).catch((errors) =>
                        {
                            let response = errors.response;
                            if(response === undefined)
                            {
                                this.apiError = true;
                            }
                        })
                    }
                },
                getEmptyMessage: function()
                {
                    if(this.selectedRegion.regionID === 0)
                    {
                        this.emptyRegionMessage = "No region selected";
                    }
                    else
                    {
                        this.emptyRegionMessage = "No locations assigned to " + this.selectedRegion.regionName;
                    }
                },
                checkRow: function(item)
                {
                    if(item.regionID === null)
                    {
                        return this.selectedUnassignedLocations.includes(item);
                    }
                    else
                    {
                        return this.selectedRegionLocations.includes(item);
                    }
                },
                sleep: function(ms)
                {
                    return new Promise(resolve => setTimeout(resolve, ms));
                }
            },
        computed:
            {

            },
        watch: {
            /**
             * This watch function executes when the currentRoute object has changed
             */
            selectedRegion: {
                handler: function()
                {
                    this.getAssignCount();
                    this.getAssignedLocations();
                    this.getEmptyMessage();
                },
                deep: true
            }
        },
        mounted()
        {
            this.getUnassignCount();
            this.getUnassignedLocations();
            this.getEmptyMessage();
        },

    }
</script>

<style scoped>
    .container
    {
        margin-top: 1%;
        margin-bottom: 5%;
        padding: 0;
    }
    #locationButtons
    {
        align-self: center;
    }

    #locationButtons > button
    {
        margin: 15% 0;
    }

    #locationLists
    {
        flex: 2;
        display: inline-flex;
        justify-content: space-between;
        align-items: flex-start;
        width:100%;
        margin: 0;
        height: 550px;
    }

    .locTableCol
    {
        align-self: flex-start;
    }

    #locBtnCol
    {
        align-self: center;
    }
</style>
