// mouse.js
import { ref, onMounted, onUnmounted } from 'vue';
import axios from 'axios';

export function GetBaidu() {
  axios({
    method: 'post',
    url: 'www.baidu.com',
    data: {
      firstName: 'Fred',
      lastName: 'Flintstone'
    }
  });
}

export function request(url, params, fun) {
  if (url.indexOf('Run') > -1) {
    params.DynamicData = encodeURI(params.dynamicData);
  } else {
    params.DynamicData = '';
  }
  axios({
    method: 'post',
    url: url,
    //params: params,
    data: params,
    headers: {
      'X-Requested-With': 'XMLHttpRequest',
      'Content-Type': 'application/json',
      taskjob: 'now',
      dynamicData: params.DynamicData
    }
  })
    .then(function (response) {
      fun && fun(response.data);
    })
    .catch(function (error) {
      console.info(error);
    });
}
