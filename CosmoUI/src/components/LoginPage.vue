<template>
    <div id="login">
        <b-jumbotron header="Login"></b-jumbotron>
        <!-- https://www.npmjs.com/package/vue-google-signin-button -->
        <!--google sign in button-->
        <g-signin-button
                :params="googleSignInParams"
                @success="onSignInSuccess"
                @error="onSignInError">
        </g-signin-button>


    </div>
</template>

<script>
    const axios = require('axios').default;

    export default {
        name: "LoginPage",
        data () {
            return {
                /**
                 * https://developers.google.com/identity/sign-in/web/reference#gapiauth2initparams.
                 * client_id is specified so that the google authentication popup will appear
                 */
                googleSignInParams: {
                    client_id: '314266744370-ln9906n254p0cpqkm4pqb19anqff139q.apps.googleusercontent.com'
                },
                //The email used to authenticate the user in the database
                googleAuthEmail:{},
                //The name that is displayed to the user on the welcome page
                googleAuthFirstName:{},
                //boolean that is used to determine if the user is signed in
                subscriberValues:null,

            }
        },
        methods: {
            //This method will be called on google authentication success
            onSignInSuccess (googleUser) {
                // `googleUser` is the GoogleUser object that represents the just-signed-in user.
                //https://developers.google.com/identity/sign-in/web/reference#users
                //reference used to get the users profile and from that, retrieve the email

                let googleAuth = googleUser.getAuthResponse().id_token;
                sessionStorage.setItem("GoogleAuth", googleAuth);
                //email of the user
                this.googleAuthEmail = googleUser.getBasicProfile().getEmail();
                //first name of the user
                this.googleAuthFirstName = googleUser.getBasicProfile().getGivenName();

                //make a get request to the api for the current user that has logged in
                axios({
                    method: 'get',
                    url: this.apiURLBase+'subscribers/email='+this.googleAuthEmail,
                    headers: {
                        'GoogleAuth': googleAuth
                    }
                }).then(resp => { //direct user to their home page
                    window.globalSubscriberID = resp.data.subscriberID;
                    this.subscriberValues = resp.data; //set signed in to true

                    axios({
                       method: 'GET',
                       url: this.apiURLBase + 'Subscribers/email-d=' + this.googleAuthEmail,
                        headers: {
                           'GoogleAuth': googleAuth
                        }
                    }).then(response => {
                        sessionStorage.setItem("sessionID", response.data);
                        this.$emit('loggedin', this.subscriberValues);
                    }).catch();
                }).catch(() => { //direct user to sign up page if they do not exist in the database
                    this.$emit('signup', this.googleAuthEmail, this.googleAuthFirstName, googleAuth); //emit signup passing email and first name
                });
            },

            //fires when error with signing in
            onSignInError (error) {
                //alert the error
                alert(error.data);
            }
        },


    }
</script>

<style scoped>
    /*normal display*/
    .g-signin-button {
        display: inline-block;
        padding: 4px 8px;
        min-width: 191px;
        min-height: 46px;
        border-radius: 3px;
        background-size: cover;
        background-image: url('../assets/btn_google_signin_dark_normal_web@2x.png');
    }

    /*darken when button is hovered over*/
    .g-signin-button:hover {
         background-image: url('../assets/btn_google_signin_dark_pressed_web@2x.png');
     }

    /*When the button is clicked lighten up*/
    .g-signin-button:active {
        background-image: url('../assets/btn_google_signin_dark_focus_web@2x.png');
    }
    #login{
        text-align: center;
    }
</style>