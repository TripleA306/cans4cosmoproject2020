<template>
  <b-container>
      <div v-if="this.loggedIn" class="px-0">
          <div>
              <b-button @click="logOut()" class="mt-3 float-right">Log Out</b-button>
          </div>
          <div id="logoHeaderDiv" class="text-center">

              <b-img id="logo_Cans4Cosmo" class=" mt-n3 img-fluid w-25" :src="require('../src/assets/images/cans4cosmo_logo.png')"></b-img>
              <h2>Cans4Cosmo Administration</h2>
          </div>
          <div>
              <!--Navigation Bar-->
              <b-list-group horizontal >
                  <b-list-group-item button variant="primary" @click="openHomePage" class="text-center font-weight-bold ">Home</b-list-group-item>
                  <b-list-group-item button variant="secondary" @click="openRegionPage" class="text-center font-weight-bold ">Regions</b-list-group-item>
                  <b-list-group-item id="buttonRoutesNav" button variant="info" @click="openRoutePage" class="text-center font-weight-bold">Routes</b-list-group-item>
                  <b-list-group-item id="buttonSubscriberNav" button variant="warning" @click="openSubscriberPage"  class="text-center font-weight-bold">Subscribers</b-list-group-item>
              </b-list-group>
          </div>
      </div>

      <router-view @login="setLoggedIn"></router-view>

  </b-container>
</template>

<script>
//import axios from 'axios'
// eslint-disable-next-line no-unused-vars
import {BootstrapVue} from 'bootstrap-vue'

export default {
  name: 'app',
    data:function()
    {
        return {
            currentPage: "Admin - Home", //Current page's title
            loggedIn: false
        }
    },
    methods:{
      openHomePage()
        {
            this.$router.push('home');
        },
        openRegionPage()
        {
            this.$router.push('regions');
        },
        openRoutePage()
        {
          this.$router.push('routes');
        },
        openSubscriberPage()
        {
            this.$router.push('subscribers');
        },
        setLoggedIn()
        {
            this.loggedIn = true;
        },
        logOut()
        {
            sessionStorage.removeItem('sessionID');
            this.loggedIn = false;
            this.$router.push('login')
        }
    },
    computed:{
      states: function(){
          return {loggedIn : sessionStorage.getItem('sessionID') ? true : false};
      }
    },
    watch:
        {
            currentPage: function()
            {
                document.title = this.currentPage;  //Updates document title whenever currentPage changes
            }
        },
    mounted()
    {
        document.title = "Admin - Home";

    }
}
</script>

<style>
#logoHeaderDiv
{
    flex-direction: column;
    display: inline-flex;
    align-items: center;
}

#logo_Cans4Cosmo
{
    flex: 2;
    width: 20%;
}

#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

.networkError
{
    color: red;
    font-size: 1.5rem;
}

#btnShowAll
{

    background-color: white;
    color: black;
}

#btnShowAll.active
{
    background-color: dodgerblue;
    color: white;
}

</style>
