<template>
    <b-card bg-variant="light" class="{ 'form-group--error': $v.name.$error }">
        <h1 style="padding: 2rem" id="lblTitle" class="text-center">Mailing Location</h1>

        <b-row id="contentRow">
            <b-col id="contentCol">
                <b-row align-h="end">
                    <b-col cols="3">
                        <!--Unit Input-->
                        <b-form-group
                                id="lblUnit"
                                label="Unit:"
                                label-cols-md="4"
                                label-align="center"
                                label-align-md="right"
                                label-for="txtUnit"
                                :invalid-feedback="errorUnit"
                                :state="states.unit"
                        >
                            <b-form-input id="txtUnit" v-model="$v.location.unit.$model" ></b-form-input>
                        </b-form-group>
                    </b-col>

                    <b-col cols="8">
                        <b-form-group
                                id="lblAddress"
                                label-cols-md="2"
                                label="Address:"
                                label-align="center"
                                label-align-md="right"
                                label-for="txtAddress"
                                :invalid-feedback="errorAddress"
                                :state="states.address"

                        >
                            <!--Address Input-->
                            <b-form-input id="txtAddress" v-model="$v.location.address.$model" ></b-form-input>
                        </b-form-group>
                    </b-col>
                </b-row>

                <b-row>
                    <b-col cols="12">
                        <b-form-group
                                id="lblCity"
                                label-cols="12"
                                label-cols-md="2"
                                label="City:"
                                label-align="center"
                                label-align-md="right"
                                label-for="txtCity"
                                :invalid-feedback="errorCity"
                                :state="states.city"
                        >
                            <!--City Input-->
                            <b-form-input id="txtCity" v-model="$v.location.city.$model"></b-form-input>
                        </b-form-group>
                    </b-col>
                </b-row>

                <b-row>
                    <b-col cols="12">
                        <b-form-group
                                id="lblPostalCode"
                                label-cols="12"
                                label-cols-md="2"
                                label="Postal Code:"
                                label-align="center"
                                label-align-md="right"
                                label-for="txtPostalCode"
                                :invalid-feedback="errorPostalCode"
                                :state="states.postalCode"
                        >
                            <!--Postal Code Input-->
                            <b-form-input id="txtPostalCode" v-model="$v.location.postalCode.$model" v-mask="'A#A #A#'" masked="true"></b-form-input>

                        </b-form-group>
                    </b-col>
                </b-row>

                <b-row>
                    <b-col cols="12">
                        <b-form-group
                                id="lblProvince"
                                label-cols="12"
                                label-cols-md="2"
                                label="Province:"
                                label-align="center"
                                label-align-md="right"
                                label-for="txtProvince"
                                :invalid-feedback="errorProvince"
                                :state="states.province"
                        >
                            <!--Province Combo Box. Forced to Saskatchewan-->
                            <b-form-select id="cboProvince" v-model="$v.location.province.$model" :options="provOptions" :disabled="true"></b-form-select>
                        </b-form-group>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>

        <!--Next Button-->
        <b-row align-h="center">
                <b-button id="btnPickUpLocationNext" @click="skipSubscriber" class="btn btn-primary col-md-1">Skip</b-button>
                <b-button id="btnPickUpLocationNext" @click="updateSubscriber" class="btn btn-primary col-md-1">Save</b-button>
        </b-row>


        <b-alert
                v-model="apiError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible
                @click="apiError=false"
        >
            Cannot reach server, please try again later.
        </b-alert>
        <div aria-label="..." class="row d-flex justify-content-center" style="margin-top: 15px">
            <ul class="pagination pagination col-0">
                <li class="page-item disabled"><p class="page-link">1</p></li>
                <li class="page-item disabled"><p class="page-link">2</p></li>
                <li class="page-item active"><p class="page-link">3</p></li>
            </ul>
        </div>
    </b-card>
</template>

<script>
    const axios = require('axios').default;
    const validators = require('vuelidate/lib/validators');
    //const TheMask = require('vue-the-mask').default;

    export default {
        data: function() {
            return {
                //Location Object
                location: {
                    address: '',
                    unit: '',
                    city: '',
                    postalCode: '',
                    province: 'SK',
                    locationType: 'Billing'
                },
                //Errors array
                errors: {},
                //Options for the Province
                provOptions: [
                    { value: "AB", text: "Alberta" },
                    { value: "BC", text: "British Columbia" },
                    { value: "MB", text: "Manitoba" },
                    { value: "NB", text: "New Brunswick" },
                    { value: "NL", text: "Newfoundland and Labrador" },
                    { value: "NS", text: "Nova Scotia" },
                    { value: "NT", text: "Northwest Territories" },
                    { value: "NU", text: "Nunavut" },
                    { value: "ON", text: "Ontario" },
                    { value: "PE", text: "Prince Edward Island" },
                    { value: "QC", text: "Quebec" },
                    { value: "SK", text: "Saskatchewan" },
                    { value: "YT", text: "Yukon" },
                ],
                apiError: false
            }
        },
        props: {
            //Subscriber object passed in
            subscriber: {
                type: Object,
                required: true
            }
        },
        methods:{
            //Updating the subscriber passed in
            updateSubscriber: function (){
                this.errors = {};
                // eslint-disable-next-line no-console
                console.log(this.location);
                // this.location.postalCode = this.location.postalCode.toUpperCase(); //Force to upper case

                //Check to ensure that the fields were used before sending
                this.$v.location.unit.$touch();
                this.$v.location.address.$touch();
                this.$v.location.city.$touch();
                this.$v.location.postalCode.$touch();

                //Check to see if any errors exist before sending to API
                if (this.checkForErrors()) {
                    axios({
                        method: 'POST',
                        url: this.apiURLBase + "Locations",
                        data: this.location,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    }).then(response => {
                        if(response.status === 201)
                        {
                            let data = response.data;
                            this.subscriber.billingLocationID = data.locationID; //Set the locationID to the actual ID
                            //Update the subscriber
                            axios({
                                method: 'PUT',
                                url: this.apiURLBase + 'Subscribers/' + this.subscriber.subscriberID,
                                data: this.subscriber,
                                headers: {
                                    'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                                }
                            }).then(response => {
                                if (response.status === 204) {

                                    this.$emit('billingLocationSignUpDone', this.subscriber);
                                }
                            }).catch(err => {
                                let response = err.response;
                                if(response === undefined)
                                {
                                    this.apiError = true;
                                }
                                else if (response.status === 400) {
                                    //Set the errors
                                    Object.assign(this.errors, response.data);
                                    this.checkForErrors();
                                }
                            })
                        }
                    }).catch(err =>{
                        let response = err.response;
                        if(response.status === 400){
                            //Set the errors
                            this.errors = response.data;
                        }
                    });
                }
            },
            skipSubscriber: function() {
                //Set the billingLocationID to the locationID
                this.subscriber.billingLocationID = this.subscriber.locationID;

                axios({
                    method: 'PUT',
                    url: this.apiURLBase + 'Subscribers/' + this.subscriber.subscriberID,
                    data: this.subscriber,
                    headers: {
                        'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                    }
                }).then(response => {
                    if (response.status === 204) {
                        this.$emit('billingLocationSkipped', response.data);
                    }
                }).catch(err => {
                    let response = err.response;
                    if(response === undefined)
                    {
                        this.apiError = true;
                    }
                    else if (response.status === 400) {
                        //Set the errors
                        Object.assign(this.errors, response.data);
                        this.checkForErrors();
                    }
                });


            },
            //Check to make sure that no errors exist
            checkForErrors: function() {
                return this.states.address === null && this.states.city === null && this.states.postalCode === null && this.states.province === null;
            }
        },
        validations: {
            location: {
                unit: {
                    maxLength: validators.maxLength(10)
                },
                //Address is required and must be between 2 and 60 characters
                address: {
                    required: validators.required,
                    minLength: validators.minLength(2),
                    maxLength: validators.maxLength(60)
                },
                //City is required and must be between 2 and 60 characters
                city: {
                    required: validators.required,
                    minLength: validators.minLength(2),
                    maxLength: validators.maxLength(60)
                },
                //Postal Code is required and must be 7 characters (with/without a space)
                postalCode: {
                    required: validators.required,
                    minLength: validators.minLength(7),
                    maxLength: validators.maxLength(7)
                },
                //Province is required (Set by default)
                province: {
                    required: validators.required
                },
                //Is business is required (Is a boolean)
                isBusiness: {
                    required: validators.required
                }
            }
        },
        computed: {
            //Ensure that the errors are displayed if they exist
            states: function() {
                return {
                    address: this.$v.location.address.$error ? false : null || "address" in this.errors ? false : null,
                    unit: this.$v.location.unit.$error ? false : null || "unit" in this.errors ? false: null,
                    city: this.$v.location.city.$error ? false : null || "city" in this.errors ? false : null,
                    postalCode: this.$v.location.postalCode.$error ? false : null || "postalCode" in this.errors ? false : null,
                    province: this.$v.location.province.$error ? false : null || "province" in this.errors ? false : null
                }
            },
            //Custom error messages for the address
            errorAddress: function () {
                let errorString = null;

                //Frontend errors
                if (!this.$v.location.address.required) {
                    errorString = "Address is required";
                } else if (this.$v.location.address.minLength || this.$v.location.address.maxLength) {
                    errorString = "Address must be between 2 and 60 characters";
                }
                if ("address" in this.errors) {
                    errorString = this.errors.address[0];
                }

                return errorString;
            },
            errorUnit: function()
            {
                let errorString = null;

                //Frontend Errors
                if(!this.$v.location.unit.maxLength)
                {
                    errorString = "Unit/Apt. cannot exceed 10 characters in length";
                }
                if("address" in this.errors)
                {
                    errorString = this.errors.address[0];
                }

                return errorString;
            },
            //Custom error messages for the city
            errorCity: function () {
                let errorString = null;

                if (!this.$v.location.city.required) {
                    errorString = "City is required";
                } else if (this.$v.location.city.minLength || this.$v.location.city.maxLength) {
                    errorString = "City must be between 2 and 60 characters";
                }
                if ("city" in this.errors) {
                    errorString = this.errors.city[0];
                }

                return errorString;
            },
            //Custom error messages for the postal code
            errorPostalCode: function () {
                let errorString = null;

                if (!this.$v.location.postalCode.required) {
                    errorString = "Postal code is required";
                } else if (this.$v.location.postalCode.minLength || this.$v.location.postalCode.maxLength) {
                    errorString = "Postal code must be in the format A1A 1A1";
                }
                if (this.errors.postalCode) {
                    errorString = this.errors.postalCode[0];
                }

                return errorString;
            },
            //Custom error messages for the province
            errorProvince: function () {
                let errorString = null;

                if ("province" in this.errors) {
                    errorString = this.errors.province[0];
                }

                return errorString;
            }
        }
    }
</script>

<style scoped>
    #contentRow
    {
        width: 50%;
        margin: auto;
    }
</style>
