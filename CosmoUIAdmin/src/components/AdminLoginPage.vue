<template>
    <b-container >
        <h1 style="text-align: center; padding: 1em">Cans4Cosmo Admin Login</h1>
        <b-row class="justify-content-center">
            <b-col cols="6">
                <b-form>
                    <b-form-group
                            label="User Name"
                            label-for="userName"
                    >
                        <b-form-input
                                id="userName"
                                v-model="admin.userName"
                        >
                        </b-form-input>
                    </b-form-group>
                    <b-form-group
                            label="Password"
                            label-for="password"
                    >
                        <b-form-input
                                id="password"
                                type="password"
                                v-model="admin.password"
                        >
                        </b-form-input>
                    </b-form-group>
                    <b-button
                     @click="validateUser"
                    >Login</b-button>
                </b-form>
                <p style="color: red" v-if="loginError">Username or password invalid</p>
            </b-col>
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
    import axios from 'axios'
    import sha256 from 'js-sha256'

    export default {
        name: "AdminLoginPage",
        data: function (){
            return {
                //the data tied to the login form
                admin: {
                    userName: "", //username input
                    password: "" //password input
                },

                //this is the data that will be sent to the API
                //data from admin will be copied into this object when the login button is clicked
                preppedAdmin: {
                    userName: "", //the username copied from admin
                    password: "" //the encrypted password which is run through the encryptInfo method
                },
                apiError: false, //This will indicate if an error connecting to the API has occurred
                loginError: false //this will indicate if an error has occurred logging in
            }
        },
        methods: {
            /**
             * This method will take in the username and password from the login form fields and send a request
             * to the API to determine if the credentials entered are valid.
             */
            validateUser: function () {
                //set login error to false
                this.loginError = false;
                //copy data from admin object into preppedAdmin object
                Object.assign(this.preppedAdmin, this.admin);
                //hash the password
                this.preppedAdmin.password = this.encryptInfo(this.preppedAdmin.password);
                //set the api url
                let url = this.apiURLBase + "Admins";

                //send a post request to the API with the url prepared, sending in the preppedAdmin object
                axios.post(url, this.preppedAdmin)
                    .then(response =>{
                        //check if a response with a 200 code was returned
                        if(response.status === 200)
                        {
                            //store a token into the session storage with the response data
                            sessionStorage.setItem("sessionID", response.data);
                            //redirect the user to the home page
                            this.$router.push("home");
                            //emit login to the enable the navigation bar
                            this.$emit("login");
                        }
                    })
                    .catch(errors => {
                        let response = errors.response;
                        //if the response errors are undefined (caused by the API being unreachable)
                        if(response === undefined)
                        {
                            //set apiError to true to display error alert
                            this.apiError = true;
                        }
                        //if the response code is 401
                        else if(response.status === 401)
                        {
                            //set apiError to true to display error alert
                            this.loginError = true;
                        }
                    });
            },

            /**
             * This method will take in a value (password) and hash that value into a SHA256 value and return it.
             * @param value - the value to hash
             */
            encryptInfo: function (value) {
                return sha256(value);
            }
        },
        mounted() {
            //on mount check if the sessionID is stored (then the user is logged in)
            if(sessionStorage.getItem("sessionID"))
            {
                //go to the home page
                this.$router.push('home');
            }
        }
    }
</script>

<style scoped>

</style>