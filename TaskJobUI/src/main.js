// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import Axios from 'axios'
import { Button,Table, Select, TableColumn,Input,InputNumber,Pagination } from 'element-ui';
import "element-ui/lib/theme-chalk/index.css";

// axios需要使用prototype将axios挂载到原型上 ，$后面是自己另起的名称，以后就可以使用该名称
Vue.prototype.$axios = Axios

Vue.config.productionTip = false
// https://element.eleme.cn/#/zh-CN/component/quickstart

Vue.prototype.$ELEMENT = { size: 'small', zIndex: 3000 };
Vue.use(Button);
Vue.use(Table);
Vue.use(TableColumn);
Vue.use(Select);
Vue.use(Input);
Vue.use(InputNumber);
Vue.use(Pagination);

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
