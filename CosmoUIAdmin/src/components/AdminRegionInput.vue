<template>
    <!-- NO LONGER BEING USED... REPLACED BY ADMIN REGION DETAILS.vue -->
    <!--Modal for showing the add region inputs-->
    <b-modal id="region-input-modal" title="Add Region" @shown="onModalShown">
        <!--Form group for regionName, includes error messages and states, and links to the data region object below-->
        <b-form-group :invalid-feedback="errorRegionName" :state="states.regionName" label="Region Name" label-for="txtRegionName">
            <b-form-input id="txtRegionName" v-model="$v.region.regionName.$model">
            </b-form-input>
        </b-form-group>

        <!--Form group for firstDate, includes error messages and states, and links to the data region object below-->
        <b-form-group :invalid-feedback="errorFirstDate" :state="states.firstDate" label="First Collection Date" label-for="txtFirstDate">
            <b-form-input id="txtFirstDate" type="date" v-model="$v.region.firstDate.$model">
            </b-form-input>
        </b-form-group>

        <!--Form group for frequency, includes error messages and states, and links to the data region object below-->
        <b-form-group :invalid-feedback="errorFrequency" :state="states.frequency" label="Pick Up Frequency" label-for="txtFrequency">
            <!--Validation added for min and max frequency allows by the ticker -->
            <b-form-input  id="txtFrequency" type="number" min="1" max="52" v-model="$v.region.frequency.$model">
            </b-form-input>
        </b-form-group>

        <!--New footer containing our custom buttons-->
        <template v-slot:modal-footer>
            <b-button size="sm" @click.stop="closeModal">Cancel</b-button>
            <b-button variant="primary" size="sm" @click.stop="postRegion">Save</b-button>
        </template>

        <!--Display error upon API error-->
        <b-alert
                v-model="apiError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible>
            Cannot reach server, please try again later.
        </b-alert>
    </b-modal>
</template>

<script>
    //Add our constants
    const axios = require('axios').default;
    const validators = require('vuelidate/lib/validators');

    export default {
        //Name of the page
        name: "AdminRegionCreate",
        data: function(){
          return {
              //Region object to be populated with our inputed values
              region:{
                  regionName: "",
                  frequency: "",
                  firstDate: ""
              },
              //Array of errors
              errors: {},
              //Boolean to figure out when an api error has occured
              apiError: false
          }
        },
        methods:{
            //This method will be called when the save button is selected
            //It will send our region object to the api to be processed
            postRegion: function()
            {
                this.errors = {};

                //Touch is to mark the fields as modified so the required error message will show
                this.$v.region.frequency.$touch();
                this.$v.region.regionName.$touch();
                this.$v.region.firstDate.$touch();

                //If there are no errors
                if(this.checkForErrors())
                {
                    //Post the object to the API
                    axios({
                        method: 'POST',
                        url: this.apiURLBase + "Regions",
                        data: this.region,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    }).then(response => {
                        if(response.status === 201)
                        {
                            //If we succeeded refresh table and close the modal
                            this.$root.$emit('bv::refresh::table', 'regionTable');
                            this.closeModal();
                        }
                    }).catch(errors => {
                        //Post failed
                        let response = errors.response;
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
                    })
                }
            },
            //This method will be called when the modal needs to be closed
            closeModal: function() {
                //Close the modal
                this.$bvModal.hide('region-input-modal');
            },
            //This method will check the input fields for errors and will be used to know if we can send the object to the API
            checkForErrors: function() {
                return this.states.regionName === null && this.states.firstDate === null && this.states.frequency === null;
            },
            //When the modal is shown make sure text boxes are empty
            onModalShown: function () {
                this.region= {
                    regionName: "",
                    frequency: "",
                    firstDate: ""
                };
                this.$v.$reset();
            }
        },
        //Vuelidate each field of the region
        validations:{
            region: {
                regionName:{
                    required: validators.required,
                    minLength: validators.minLength(2),
                    maxLength: validators.maxLength(40)
                },
                frequency:{
                    required: validators.required,
                    minValue: validators.minValue(1),
                    maxValue: validators.maxValue(52)
                },
                firstDate:{
                    required: validators.required
                }
            }
        },
        computed:{
            //are used in the b-form groups to signify errors
            states: function() {
                return{
                    regionName: this.$v.region.regionName.$error ? false : null || "regionName" in this.errors ? false : null ,
                    firstDate: this.$v.region.firstDate.$error ? false : null || "firstDate" in this.errors ? false : null,
                    frequency: this.$v.region.frequency.$error ? false : null || "frequency" in this.errors ? false : null
                }
            },
            errorRegionName: function() {
                let errorString = "";

                //Check the required fields
                if(!this.$v.region.regionName.required)
                {
                    errorString = "Region name is required";
                }

                //Check the min and max length
                else if(this.$v.region.regionName.minLength || this.$v.region.regionName.maxLength)
                {
                    errorString = "Region name must be between 2 and 40 characters";
                }

                if("regionName" in this.errors)
                {
                    errorString = this.errors.regionName[0];
                }

                //Return the error
                return errorString;


            },
            errorFirstDate: function() {
                let errorString = "";

                //Check the required field
                if(!this.$v.region.firstDate.required)
                {
                    errorString = "First pickup date is required";
                }

                else if("firstDate" in this.errors)
                {
                    errorString = this.errors.firstDate[0];
                }

                //return the error message
                return errorString;
            },
            errorFrequency: function() {
                let errorString = "";

                if(!this.$v.region.frequency.required)
                {
                    errorString = "Collection frequency is required";
                }

                else if(this.$v.region.frequency.minValue || this.$v.region.frequency.maxValue)
                {
                    errorString = "Collection frequency must be between 1 and 52";
                }

                if("frequency" in this.errors)
                {
                    errorString = this.errors.frequency[0];
                }

                //return the error message
                return errorString;

            }
        }
    }
</script>

<style scoped>

</style>