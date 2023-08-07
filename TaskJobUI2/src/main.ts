import { createApp } from 'vue';
import App from './App.vue';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';
import { setupStore } from '@/stores';
import './styles/index.scss';
import router from './router/index';

// 导入暗黑模式的样式
import 'element-plus/theme-chalk/dark/css-vars.css'

// build 之后 消息弹窗的样式丢失了。所以手动引入样式 https://juejin.cn/post/7114295680339804173
import 'element-plus/theme-chalk/src/message-box.scss'
import 'element-plus/theme-chalk/src/message.scss'

// import axios from 'axios'
// import VueAxios from 'vue-axios'

const app = createApp(App);

dayjs.locale('zh-ch');
setupStore(app);

app.use(router)
    // .use(VueAxios,axios)
    .mount('#app');
