// mouse.js
import axios from 'axios';

export function request(url: any, params: any, fun: (arg0: any) => any) {
  if (url.indexOf('Run') > -1) {
    params.dynamicData = encodeURI(params.dynamicData);
  } else {
    params.dynamicData = '';
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
      dynamicData: params.dynamicData
    }
  })
    .then(function (response) {
      fun && fun(response.data);
    })
    .catch(function (error) {
      console.info(error);
    });
}
