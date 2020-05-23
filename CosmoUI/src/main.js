import Vue from 'vue';
import Vuex from 'vuex';
import VueRouter from 'vue-router'

import App from './App.vue'    //Imports the story64 Vue component.
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

import GSignInButton from 'vue-google-signin-button'

import axios from 'axios';
import Vuelidate from 'vuelidate';
import VueAxios from 'vue-axios'
import VueTheMask from 'vue-the-mask'
import LoginPage from "./components/LoginPage";
import HomePage from "./components/HomePage";
import GenInfoSignUp from "./components/GenInfoSignUp";
import PickUpLocationSignUp from "./components/PickUpLocationSignUp";
import CollectionsPage from "./components/CollectionsPage";
import BillingLocationSignUp from "./components/BillingLocationSignUp";
import NextCollectionDates from "./components/NextCollectionDates";


Vue.use(Vuex);
// Install BootstrapVue
Vue.use(BootstrapVue);
// Install IconsPlugin to be used for Bootstrap classes
Vue.use(IconsPlugin);
Vue.use(VueRouter);

//import the google sign in button to use
Vue.use(GSignInButton);

Vue.use(VueAxios, axios);
Vue.use(Vuelidate);
Vue.use(VueTheMask);

Vue.use(VueAxios,axios);

//Specifying global variables used for axios calls
Vue.store = Vue.prototype.apiURLBase = 'http://localhost:5002/api/';
window.globalSubscriberID = null;
const store = new Vuex.Store({
  state:{
    currentSubscriber: null
  },
  mutations:{
    setLoggedIn(state, subscriber){
      state.currentSubscriber = subscriber;
    }
  }
});





const routes = [
  {
    name: 'app',
    path: '/',
    component:  App,
    beforeEnter:(to, from, next) => {
      if(to.name === 'app' )
      {
        next({name:'login'});
      }

    }
  },
  {name: 'home',
    path: '/home',
    component: HomePage,
    beforeEnter:(to, from, next) => {
      if(store.state.currentSubscriber !== null)
      {

        next();
      }
      else
      {
        next({name: 'login'})
      }
    },
    children:[
      {name: "dashboard", path: 'dashboard', component: NextCollectionDates,
        beforeEnter:(to, from, next) => {
          if(store.state.currentSubscriber !== null)
          {

            next();
          }
          else
          {
            next({name: 'login'})
          }
        }
      },
      {path: 'history', component: CollectionsPage},
    ]

  },
  {
    name: 'login',
    path: '/login',
    component: LoginPage,
    beforeEnter:(to, from, next) => {
      if(to.name === 'login' &&  store.state.currentSubscriber === null)
      {
        next();
      }
      else
      {
        next({name: 'home'})
      }


    }
  },
  {
    name: 'sign-up-general',
    path: '/sign-up-general',
    component: GenInfoSignUp,
  },

  {path: '/sign-up-location', component: PickUpLocationSignUp},

  {path: '/sign-up-billing-location', component: BillingLocationSignUp}

];
const router = new VueRouter({
  routes
});


//Stops the warning from appearing in the console regarding being in development mode
Vue.config.productionTip = false;

new Vue({
  render: h => h(App),
  router,
  store
}).$mount('#app');