<template>
  <div id="app">
    <router-view :subscriberName="subscriberName" :subscriber="subscriber" :email="subscriberEmail"
                 @signup="setGenInfoSignUpPage"
                 @loggedin="setLoggedIn"
                 @pickUpLocationSignUpDone="setLoggedIn"
                 @billingLocationSignUpDone="setLoggedIn"
                 @billingLocationSkipped="setLoggedIn"
                 @genInfoDone="setPickUpSignUp"></router-view>
  </div>
</template>
        <!-- Sign up page is only shown after the subscriber successfully logs in but their email is not recorded in the database -->
        <GenInfoSignUp v-if="genInfoSignUp" @genInfoDone="setPickUpSignUp" :subscriberName="subscriber.firstName" :email="subscriber.email" :googleAuth="googleAuth"></GenInfoSignUp>

<script>


export default {
    name: 'app',
    data: function () {
        return {

            subscriberID: '',
            subscriberEmail: '', //the logged in subscriber's email
            subscriberName: '', //the logged in subscribers name
            subscriber: {}
        }
    },
    methods: {

        setGenInfoSignUpPage: function (subscriberEmail, subscriberName) {
            //this.genInfoSignUp = true; //set to true to bring up sign in page
            this.$router.push("sign-up-general");
            this.subscriberEmail = subscriberEmail;
            this.subscriberName = subscriberName;
        },
        setPickUpSignUp: function (subscriber) {
            this.subscriber = subscriber;
            this.$router.push("sign-up-location")
        },
        setBillingLocationSignUp: function (subscriberObject) {
            Object.assign(this.subscriber, subscriberObject);
            this.$router.push("sign-up-billing-location");
        },
        //this method will take in an email and a subscriber name and direct the subscriber to their home page
        setLoggedIn: function (subscriberObject) {
            //This is for when the subscriber has a name and email, but does not have a phone number
            if (subscriberObject.phoneNumber === null || subscriberObject.phoneNumber === "") {
                this.setGenInfoSignUpPage(subscriberObject.email, subscriberObject.firstName);
            } else if (subscriberObject.locationID === null) { //This is for when the subscriber has an email and phone number, but does not have a location ID
                this.setPickUpSignUp(subscriberObject);
            } else if (subscriberObject.billingLocationID === null) {
                this.setBillingLocationSignUp(subscriberObject);
            } else { //When they have all the required fields
                this.subscriberEmail = subscriberObject.email;
                this.subscriberName = subscriberObject.firstName;
                this.subscriber = subscriberObject;
                this.$store.commit('setLoggedIn', subscriberObject);
                this.$router.replace("home");
            }
        }
    }
}

   
</script>

<style>
    #app {
        font-family: 'Avenir', Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        color: #2c3e50;
        margin-top: 60px;
    }
</style>


