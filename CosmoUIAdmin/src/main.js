import Vue from 'vue'
import VueRouter from 'vue-router'
import App from './AdminApp.vue'    //Imports the story64 Vue component.
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import Vuelidate from 'vuelidate';
import 'bootstrap/dist/css/bootstrap.css' //import Bootstrap css files
import 'bootstrap-vue/dist/bootstrap-vue.css'
import VueAxios from 'vue-axios'
import axios from 'axios'
//Using vue moment to format dates
import moment from 'moment';
import AdminHomePage from "./components/AdminHomePage";
import AdminLoginPage from "./components/AdminLoginPage";
import AdminRoutePage from "./components/AdminRoutePage"
import AdminSubscriberPage from "./components/AdminSubscriberPage";
import AdminRegionPage from "./components/AdminRegionPage"

Vue.use(moment);
Vue.use(VueAxios,axios);
Vue.use(Vuelidate);
Vue.use(VueRouter);
Vue.use(BootstrapVue);  // Install BootstrapVue
Vue.use(IconsPlugin); // Install IconsPlugin to be used for Bootstrap classes


//Specifying global variables used for axios calls
Vue.store = Vue.prototype.apiURLBase = 'http://localhost:5002/api/';

//Declare a globaly available function to standardize our date functions
Vue.filter('formatDate', function (value) {
  if (value){
    return moment(String(value)).format('MMMM DD, YYYY');
  }
});



//Stops the warning from appearing in the console regarding being in development mode
Vue.config.productionTip = false;

const routes = [
  {path: '/', component: AdminHomePage},
  {path: '/home', component: AdminHomePage},
  {path: '/login', component: AdminLoginPage},
  {path: '/regions', component: AdminRegionPage},
  {path: '/routes', component: AdminRoutePage},
  {path: '/subscribers', component: AdminSubscriberPage},
  {path: '/login', component: AdminLoginPage}
];

const router = new VueRouter({
  routes
});

new Vue({
  render: h => h(App),
  router
}).$mount('#app');