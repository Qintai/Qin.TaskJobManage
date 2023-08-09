// import { createRouter, createWebHashHistory, Router } from 'vue-router';
// import avue from '../components/a.vue'

// const About = { template: '<div>About</div>' }

// const router: Router = createRouter({
//   history: createWebHashHistory(),
//   routes: [
//     { path: '/About', component: About },
//     { path: '/a', component: avue },
//   ]
// });

// export default router;

import { createRouter, createWebHistory, RouteRecordRaw , createWebHashHistory} from 'vue-router'

export const constantRoutes: Array<RouteRecordRaw> = [
  {
    path: '/mytable',
    component: () => import('../components/MyTable.vue')
  },
  {
    path: '/helloworld',
    component: () => import('../components/HelloWorld.vue')
  },
  {
    path: '/jobtable',
    component: () => import('../components/JobTable.vue')
  },
  {
    path: '/jobdetail',
    component: () => import('../components/JobDetail.vue')
  }
];

const router = createRouter({
  // history: createWebHistory(), //替代之前的mode，是必须的
  history: createWebHashHistory(),
  routes: constantRoutes
});
export default router;