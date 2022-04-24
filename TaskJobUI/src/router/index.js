import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '@/components/HelloWorld'
import table from '@/page/table'
import detail from '@/page/detail'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'HelloWorld',
      component: HelloWorld
    },
    {
      path: '/table',
      name: 'table',
      component: table
    },
    {
      path: '/detail',
      name: 'detail',
      component: detail
    }
  ]
})

//https://www.jianshu.com/p/6d5c25a88818
const originalPush = Router.prototype.push;
Router.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err)
}
