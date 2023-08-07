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
    path: '/as',
    component: () => import('../components/MyHello.vue')
  },
  {
    path: '/ok',
    component: () => import('../components/MyOk.vue')
  },
  {
    path: '/table',
    component: () => import('../components/MyTable.vue')
  }
];

const router = createRouter({
  // history: createWebHistory(), //替代之前的mode，是必须的
  history: createWebHashHistory(),
  routes: constantRoutes
});
export default router;