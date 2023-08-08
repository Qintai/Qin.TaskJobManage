<script lang="ts" setup>
import { ref, onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import { GetBaidu, request } from '../common/HttpApi.ts';
import { useDark, useToggle } from '@vueuse/core';

const isDark = useDark();
const toggleDark = useToggle(isDark);
const router = useRouter();
const total = ref('0');
const tableData = ref([
  {
    taskName: '美团数据拉取Demo',
    groupName: 'Tom_group',
    status: 0,
    cron: '2222',
    des: '每日作业清洗 1点',
    updateTime: '最后执行时间'
  }
]);

const form = reactive({
  taskName: '',
  groupName: '',
  dynamicData: '',
  pageindex: '',
  pagesize: ''
});

onMounted(() => {
  console.info('启动了');
  onSubmit()
});

function onSubmit() {
  const parms = {
    taskName: form.taskName,
    groupName: form.groupName,
  };

  request('/TaskJob-Getjobs', parms, function (res) {
    if (res.status) {
      tableData.value = res.data;
      total.value = res.Count;
    }
  });
}

function switchT(status) {
  if (status == 0) {
    return "<span style='color:green;font-weight:bolder'>正常</span>";
  }
  else if (status == 1) {
    return "<span style='color:red;font-weight:bolder'>暂停</span>";
  }
  else if (status == 2) {
    return "<span style='color:red;font-weight:bolder'>完成</span>";
  }
  else if (status == 3) {
    return "<span style='color:yellow;font-weight:bolder'>异常</span>";
  }
  else if (status == 4) {
    return "<span style='color:blue;font-weight:bolder'>阻塞</span>";
  }
  else if (status == 5) {
    return "<span style='color:red;font-weight:bolder'>停止</span>";
  }
  else return "<span style='color:red;font-weight:bolder'>不存在</span>";
}

// 查看详情
function DetailClick(item) {
  // console.info(item);
  // router.push({ path: 'as', params: { datai: item }})
  //router.push({path: 'as', params: item })
  
  router.push({ path: 'as', query: { userId: '123' }})

  ElMessage.error('查看记录');
}

function tiggerAction(action, item) {
  item.dynamicData = form.dynamicData;
  request('/TaskJob-' + action, item, function (res) {
    if (res.status) {
      onSubmit();   // 刷新列表
      ElMessage({ message: res.msg, type: 'success' });
    }
  });
}

</script>

<template>
  <div style="">
    <el-form :inline="true" :model="form" label-width="auto" style="">
      <el-form-item>
        <el-input v-model="form.taskName" placeholder="任务名" clearable />
      </el-form-item>
      <el-form-item>
        <el-input v-model="form.groupName" placeholder="分组名" clearable />
      </el-form-item>
      <el-form-item>
        <el-input v-model="form.dynamicData" placeholder="执行参数" clearable style="width: 400px;" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">查询</el-button>
      </el-form-item>
      <!-- <el-form-item><el-button>Cancel</el-button></el-form-item> -->
      <el-form-item>
        <!--
            main.ts：import 'element-plus/theme-chalk/dark/css-vars.css'
            https://github.com/vueuse/vueuse/blob/main/packages/core/useDark/demo.vue
        -->
        <el-button type="primary" @click="toggleDark()" style="width: 80px">
          <i inline-block align-middle i="dark:carbon-moon carbon-sun" />
          <span class="ml-2">{{ isDark ? 'Dark' : 'Light' }}</span>
        </el-button>
      </el-form-item>
      <el-form-item>
        <a class="el-button el-button--primary " target="_blank" href="https://cron.qqe2.com/">在线cron</a>
      </el-form-item>
    </el-form>
  </div>
  <el-table :data="tableData" border size="small" highlight-current-row="true" style="width: 100%">
    <el-table-column prop="taskName" label="任务名" />
    <el-table-column prop="groupName" label="分组名" />
    <el-table-column prop="status" label="状态" #default="scope">
      <span v-html="switchT(scope.row.status)"></span>
    </el-table-column>
    <el-table-column prop="cron" label="Cron" />
    <el-table-column prop="des" label="描述" />
    <el-table-column prop="updateTime" label="最后执行时间" />
    <el-table-column fixed="right" label="操作" width="240">
      <template #default="scope">
        <el-button link type="primary" size="small" @click="tiggerAction('Run', scope.row)">立即执行</el-button>
        <el-button link type="primary" size="small" @click="tiggerAction('Pause', scope.row)">暂停</el-button>
        <el-button link type="primary" size="small" @click="tiggerAction('Start', scope.row)">开启</el-button>
      </template>
    </el-table-column>
    <el-table-column fixed="right" label="操作" width="220">
      <template #default="scope">
        <el-button link type="primary" size="small" @click="tiggerAction('remove', scope.row)">删除</el-button>
        <el-button link type="primary" size="small" @click="DetailClick(scope.row)">记录</el-button>
      </template>
    </el-table-column>
  </el-table>

  <router-link to="/ok">去时间组件界面看看</router-link>
  <br />
  <a href="/ok">时间组件界面</a>
</template>

