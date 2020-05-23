<template>
    <b-card bg-variant="light" class="{ 'form-group--error': $v.name.$error }">
        <h1 style="padding: 2rem" id="lblTitle" class="text-center">General Information</h1>
        <b-form-group
                id="lblFirstName"
                label-cols="12"
                label-cols-md="4"
                label="First Name:"
                label-align="center"
                label-align-md="right"
                label-for="txtFirstName"
                :invalid-feedback="errorFirstName"
                :state="states.firstName"
        > <!--First Name Input-->
            <b-form-input id="txtFirstName" v-model="$v.subscriber.firstName.$model" class="col-12 col-md-6 "></b-form-input>
        </b-form-group>

        <b-form-group
                id="lblLastName"
                label-cols="12"
                label-cols-md="4"
                label="Last Name:"
                label-align="center"
                label-align-md="right"
                label-for="txtLastName"
                :invalid-feedback="errorLastName"
                :state="states.lastName"

        > <!--Last Name Input-->
            <b-form-input id="txtLastName" v-model="$v.subscriber.lastName.$model" class="col-12 col-md-6 "></b-form-input>
        </b-form-group>

        <b-form-group
                id="lblPhoneNumber"
                label-cols="12"
                label-cols-md="4"
                label="Phone Number:"
                label-align="center"
                label-align-md="right"
                label-for="txtPhoneNumber"
                :invalid-feedback="errorPhoneNumber"
                :state="states.phoneNumber"
        > <!--Phone Number Input-->
            <b-form-input id="txtPhoneNumber" v-model="$v.subscriber.phoneNumber.$model" class="col-12 col-md-6 " ></b-form-input>

        </b-form-group>
        <!--Next Button-->
        <b-row>
            <b-button id="btnGenInfoNext" @click="createSubscriber" class="col-md-1 offset-md-7 btn btn-primary">Next</b-button>
        </b-row>

        <b-alert
                v-model="apiError"
                class="position-fixed fixed-top m-0 rounded-0"
                style="z-index: 2000"
                variant="danger"
                dismissible>
        Cannot reach server, please try again later.
        </b-alert>

        <div aria-label="..." class="row d-flex justify-content-center" style="margin-top: 15px">
            <ul class="pagination pagination col-0">
                <li class="page-item active"><p class="page-link">1</p></li>
                <li class="page-item disabled"><p class="page-link">2</p></li>
            </ul>
        </div>
    </b-card>
</template>

<script>
    const axios = require('axios').default;
    const validators = require('vuelidate/lib/validators');

    export default {
        data: function () {
            return {
                //Subscriber Object connected with v-models
                subscriber: {
                    email: '',
                    firstName: '',
                    lastName: '',
                    phoneNumber: ''
                },
                //Array of errors
                errors: {},
                apiError: false
            }
        },
        props: {
            subscriberName: { //Subscriber first name
                type: String,
                default: ""
            },
            email: { //Email passed in from loggedIn page
                type: String,
                default: ""
            },
            loggedIn: {
                type: Boolean,
                default: false,
                googleAuth: {
                    type: String,
                    default: "",
                }
            }
        },
            methods: {
                //Creates the subscriber to send to the API
                createSubscriber: function () {
                    this.errors = {};
                    //Forcefully makes the inputs "dirty", allows error messages to appear before having to press the "next" button
                    this.$v.subscriber.firstName.$touch();
                    this.$v.subscriber.lastName.$touch();
                    this.$v.subscriber.phoneNumber.$touch();

                    //If there are no errors, post the subscriber and go to the next page
                    if (this.checkForErrors()) {
                        axios({
                            method: 'POST',
                            url: this.apiURLBase + 'Subscribers',
                            data: this.subscriber,
                            headers: {
                                'GoogleAuth': sessionStorage.getItem("GoogleAuth"),
                            }
                        }).then(response => {
                            //document.getElementById('txtFirstName').text = "POSTed";
                            if (response.status === 201) {
                                let subscriber = response.data;
                                window.globalSubscriberID = subscriber.subscriberID;
                                axios({
                                    method: 'GET',
                                    url: this.apiURLBase + 'Subscribers/email-d=' + this.email,
                                    headers: {
                                        'GoogleAuth': sessionStorage.getItem("GoogleAuth"),
                                    }
                                }).then(response => {
                                    sessionStorage.setItem("sessionID", response.data);
                                    this.$emit('genInfoDone', response.data);
                                }).catch();
                            }
                        }).catch(errors => {
                            let response = errors.response;
                            if (response === undefined) {
                                this.apiError = true;
                            } else if (response.status === 400) {
                                //document.getElementById('txtFirstName').innerHTML = "Failed";
                                this.errors = response.data;
                            }
                        })
                    }
                },
                //Checks the states of each field
                checkForErrors: function () {
                    return this.states.firstName === null && this.states.lastName === null && this.states.phoneNumber === null;
                },
                //Function to set the variables passed into the page
                setProps: function () {
                    this.subscriber.email = this.email;
                    this.subscriber.firstName = this.subscriberName;
                }
            },
            validations: {
                subscriber: {
                    //First name cannot exceed 60 characters
                    firstName: {
                        minLength: validators.minLength(2),
                        maxLength: validators.maxLength(60)
                    },
                    //Last name cannot exceed 60 characters
                    lastName: {
                        minLength: validators.minLength(1),
                        maxLength: validators.maxLength(60)
                    },
                    //Phone number is required and must be exactly 10 digits
                    phoneNumber: {
                        required: validators.required,
                        minLength: validators.minLength(10),
                        maxLength: validators.maxLength(10)
                    }
                }
            },
            computed: {
                //Ensures that error messages will be displayed if there is any error in the front or back end
                states: function () {
                    return {
                        firstName: this.$v.subscriber.firstName.$error ? false : null || "firstName" in this.errors ? false : null,
                        lastName: this.$v.subscriber.lastName.$error ? false : null || "lastName" in this.errors ? false : null,
                        phoneNumber: this.$v.subscriber.phoneNumber.$error ? false : null || "phoneNumber" in this.errors ? false : null
                    }
                },
                //Custom error messages for the first name
                errorFirstName: function () {
                    let errorString = "";

                    if (this.$v.subscriber.firstName.minLength || this.$v.subscriber.firstName.maxLength) {
                        errorString = "First name must be between 2 and 60 characters";
                    }

                    if ("firstName" in this.errors) {
                        errorString = this.errors.firstName[0];
                    }

                    return errorString;
                },
                errorLastName: function () {
                    let errorString = "";

                    if (this.$v.subscriber.lastName.minLength || this.$v.subscriber.lastName.maxLength) {
                        errorString = "Last name must be between 1 and 60 characters";
                    }

                    if ("lastName" in this.errors) {
                        errorString = this.errors.lastName[0];
                    }

                    return errorString;
                },
                //Custom error messages for the phone number
                errorPhoneNumber: function () {
                    let errorString = "";
                    if (!this.$v.subscriber.phoneNumber.required) {
                        errorString = "Phone number is required";
                    } else if (this.$v.subscriber.phoneNumber.minLength || this.$v.subscriber.phoneNumber.maxLength) {
                        errorString = "Phone number must be 10 digits long";
                    }

                    if ("phoneNumber" in this.errors) {
                        errorString = this.errors.phoneNumber[0];
                    }

                    return errorString;
                }
            },
            //Set the props to the variables on load
            mounted() {
                this.setProps();
                if (this.subscriber.email === '') {
                    this.$router.replace("login");
                }
            }
    }


</script>

<style scoped>

</style>
