<template>
    <!-- The modal for the Create new Administrator -->
    <b-modal no-close-on-esc no-close-on-backdrop no-stacking hide-header-close
             id="CreateNewAdminModal" @shown="onModalShown" title="Create New Administrator">

        <!--Form group for username-->
        <b-form-group id="lblUsername" label="Username" :invalid-feedback="errorUsername" :state="states.username">
            <div v-if="isCreated">
                <b-form-input :disabled="true" id="eUsername" v-model="$v.administrator.username.$model"></b-form-input>
            </div>
            <div v-else>
                <b-form-input id="eUsername" v-model="$v.administrator.username.$model"></b-form-input>
            </div>
        </b-form-group>

        <!--Form group for Password-->
        <b-form-group id="lblPassword" label="Password" :invalid-feedback="errorPassword" :state="states.password">
            <div v-if="isCreated">
                <b-form-input :disabled="true" id="ePassword" type="password" v-model="$v.administrator.password.$model" ></b-form-input>
            </div>
            <div v-else>
                <b-form-input id="ePassword" type="password" v-model="$v.administrator.password.$model" ></b-form-input>
            </div>
        </b-form-group>

        <!--Form group for the confirmPassword-->
        <b-form-group id="lblConfirmPassword" label="Confirm Password" :invalid-feedback="errorConfirmPassword" :state="states.confirmPassword">
            <div v-if="isCreated">
                <b-form-input :disabled="true" id="eConfirmPassword" type="password" v-model="$v.administrator.confirmPassword.$model"></b-form-input>
            </div>
            <div v-else>
                <b-form-input id="eConfirmPassword" type="password" v-model="$v.administrator.confirmPassword.$model"></b-form-input>
            </div>
        </b-form-group>

        <!--Custom footer to diplay success message, and custom buttons -->
        <template v-slot:modal-footer>
            <b-alert id="lblSuccess" v-if="isCreated" variant="success" show>Successfully added {{administrator.username}}</b-alert>
            <b-button size="sm" @click.stop="closeModal">Close</b-button>
            <div v-if="isCreated">
                <b-button :disabled="true" id="btnSave"  variant="primary" size="sm" @click.stop="postAdmin">Save</b-button>
            </div>
            <div v-else>
                <b-button  id="btnSave"  variant="primary" size="sm" @click.stop="postAdmin">Save</b-button>
            </div>
        </template>
    </b-modal>
</template>

<script>
    const axios = require('axios').default;
    const validators = require('vuelidate/lib/validators');

    import { helpers } from 'vuelidate/lib/validators'
    import sha256 from 'js-sha256'
    const passwordRegex = helpers.regex('passwordRegex', /((?=.*\d)(?=.*[a-z])(?=.*[A-Z]))/);

    export default {
        name: "CreateNewAdminModal",
        data: function(){
            return {
                administrator: {
                    username: "",
                    password: "",
                    confirmPassword: ""

                },
                preppedAdmin: {
                    username: "", //the username copied from admin
                    password: "" //the encrypted password which is run through the encryptInfo method
                },
                errors: {},
                apiError: false,
                adminCreatedMessage: "",
                isCreated: false
            }
        },
        methods:{
            postAdmin:function()
            {
                this.errors = {};

                //Trim all white space from the admin fields
                this.administrator.username = this.administrator.username.trim();
                this.administrator.password = this.administrator.password.trim();
                this.administrator.confirmPassword = this.administrator.confirmPassword.trim();

                //Set all the fields to "dirty" to allow them to be flagged for validation
                this.$v.administrator.username.$touch();
                this.$v.administrator.password.$touch();
                this.$v.administrator.confirmPassword.$touch();

                //check for errors
                if(this.checkForErrors())
                {
                    //if none, copy all the attributes from the admin object to the preppedAdmin
                    //To prevent showing the hashed values on screen
                    Object.assign(this.preppedAdmin, this.administrator);
                    //Remove the confirmPassword property since its no longer needed
                    delete this.preppedAdmin.confirmPassword;
                    //Set the password to the encrypted version
                    this.preppedAdmin.password = this.encryptInfo(this.preppedAdmin.password);


                    axios({
                        method: 'POST',
                        url: this.apiURLBase + 'Admins/username-c=' + this.preppedAdmin.username,
                        data: this.preppedAdmin,
                        headers: {
                            'Authorization': "Bearer " + sessionStorage.getItem('sessionID')
                        }
                    }).then(response => {
                        if(response.status === 201)
                        {
                            //If we succeeded notify the user of the success
                           this.isCreated = true;
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
                            this.checkForErrors();
                        }
                    })
                }

            },
            /**
             * This method will take in a value (password) and hash that value into a SHA256 value and return it.
             * @param value - the value to hash
             */
            encryptInfo: function (value) {
                return sha256(value);
            },
            /**
             * This function will ensure that all the entry fields and validation will be reset upon the modal being opened
             */
            onModalShown: function(){
                this.administrator = {
                    username: "",
                    password: "",
                    confirmPassword: ""
                };
                this.$v.$reset();
            },
            /**
             * This function will close the CreateNewAdminModal and empty all of the errors and reset the isCreated property
             */
            closeModal: function(){
                this.errors = {};
                this.isCreated = false;
                this.$bvModal.hide('CreateNewAdminModal');
            },
            /**
             * This function will check the states for the username, password, and confirmPassword. If they're all NULL it will return true,
             * If any of the states are false indicating that a error exists, it will return false.
             * @returns {boolean}
             */
            checkForErrors: function(){
                return this.states.username === null && this.states.password === null && this.states.confirmPassword === null;
            }
        },
        /**
         * Vuelidate validations for each attribute in the administrator object
         * Password: Is Required, and MUST be between 8 - 40 characters
         * ConfirmPassword: MUST match Password
         * Username: Is Required, and MUST be between 4 - 25 characters
         */
        validations:{
            administrator: {
                password:{
                    required: validators.required,
                    minLength: validators.minLength(8),
                    maxLength: validators.maxLength(40),
                    passwordRegex
                },
                confirmPassword:{
                    sameAs: validators.sameAs('password')
                },
                username:{
                    required: validators.required,
                    minLength: validators.minLength(4),
                    maxLength: validators.maxLength(25),
                }
            }
        },
        computed:{
            //are used in the b-form groups to signify errors
            states: function() {
                return{
                    username: this.$v.administrator.username.$error ? false : null || "username" in this.errors ? false : null,
                    password: this.$v.administrator.password.$error ? false : null || "password" in this.errors ? false : null,
                    confirmPassword: this.$v.administrator.confirmPassword.$error ? false : null || "confirmPassword" in this.errors ? false : null
                }
            },
            /**
             * A Helper method for Vuelidate when validating the username attribute.
             * Will return a errorString with the correct error message
             * @returns {string}
             */
            errorUsername: function() {
                let errorString = "";

                //Check the required fields
                if(!this.$v.administrator.username.required)
                {
                    errorString = "Username is required";
                }

                //Check the min and max length
                else if(!this.$v.administrator.username.minLength || !this.$v.administrator.username.maxLength)
                {
                    errorString = "Username must be between 4 and 25 characters";
                }

                if("username" in this.errors)
                {
                    errorString = this.errors.username[0];
                }

                //Return the error
                return errorString;
            },
            /**
             * A Helper method for Vuelidate when validating the password attribute.
             * Will return a errorString with the correct error message
             * @returns {string}
             */
            errorPassword: function() {
                let errorString = "";

                //Check the required fields
                if(!this.$v.administrator.password.required)
                {
                    errorString = "Password is required";
                }

                //Check the min and max length
                else if(!this.$v.administrator.password.minLength || !this.$v.administrator.password.maxLength || !this.$v.administrator.password.passwordRegex)
                {
                    errorString = "Password must be between 8 and 40 characters, and include at least one Uppercase, and one digit";
                }
                if("password" in this.errors)
                {
                    errorString = this.errors.password[0];
                }

                //Return the error
                return errorString;
            },
            /**
             * A Helper method for Vuelidate when validating the confirmPassword attribute.
             * Will return a errorString with the correct error message
             * @returns {string}
             */
            errorConfirmPassword: function(){
                let errorString = "";

                if(!this.$v.administrator.confirmPassword.sameAs)
                {
                    errorString = "Passwords do not match"
                }

                return errorString;
            }
        }
    }
</script>

<style scoped>

</style>