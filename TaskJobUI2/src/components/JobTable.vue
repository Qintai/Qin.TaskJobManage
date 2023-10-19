<script lang="ts" setup>
import { ref, onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import { request } from '../common/HttpApi';
import { useDark, useToggle } from '@vueuse/core';

const isDark = useDark();
const toggleDark = useToggle(isDark);
const router = useRouter();
const total = ref('0');
const tableData = ref([
  {
    taskName: 'Demo1',
    groupName: 'Demo_group',
    status: 0,
    cron: '0 0 12 * * ?',
    des: '每天中午12点触发',
    updateTime: '2023-08-09 01:01:11'
  }
]);

const form = reactive({
  taskName: '',
  groupName: '',
  dynamicData: '',
  runtimeStr: null,
  pageindex: '',
  pagesize: ''
});

onMounted(() => {
  onSubmit();
});

function onSubmit() {
  const parms = {
    taskName: form.taskName,
    groupName: form.groupName
  };

  request('/TaskJob-Getjobs', parms, function (res: { msg: string; status: any; data: { taskName: string; groupName: string; status: number; cron: string; des: string; updateTime: string; }[]; Count: string; }) {
    if (res.status) {
      if (res.msg == "未启动") {
        isStartAndStop.value = false;
      }
      else if (res.msg == "ok" || res.msg == "") {
        isStartAndStop.value = true;
      }

      tableData.value = res.data;
      total.value = res.Count;
    }
  });
}

function switchT(status: number) {
  if (status == 0) {
    return "<span style='color:green;font-weight:bolder'>正常</span>";
  } else if (status == 1) {
    return "<span style='color:red;font-weight:bolder'>暂停</span>";
  } else if (status == 2) {
    return "<span style='color:red;font-weight:bolder'>完成</span>";
  } else if (status == 3) {
    return "<span style='color:yellow;font-weight:bolder'>异常</span>";
  } else if (status == 4) {
    return "<span style='color:blue;font-weight:bolder'>阻塞</span>";
  } else if (status == 5) {
    return "<span style='color:red;font-weight:bolder'>停止</span>";
  } else return "<span style='color:red;font-weight:bolder'>不存在</span>";
}

// 查看详情
function DetailClick(item: { taskName: any; groupName: any; }) {
  router.push({
    path: 'jobdetail',
    query: { taskName: item.taskName, groupName: item.groupName }
  });
  // ElMessage.error('查看记录');
}

function tiggerAction(action: string, item: { dynamicData: string; runtimeStr: any }) {
  item.dynamicData = form.dynamicData;
  item.runtimeStr = form.runtimeStr;

  request('/TaskJob-' + action, item, function (res: { status: any; msg: any; }) {
    if (res.status && action != "Stop") {
      onSubmit(); // 刷新列表
      ElMessage({ message: res.msg, type: 'success' });
    }
  });
}

const isStartAndStop = ref(false);
function StartAndStop(ischeck: boolean) {
  if (isStartAndStop.value) {
    tiggerAction('Startup', { dynamicData: '', runtimeStr: null });
  }
  else {
    tiggerAction('Stop', { dynamicData: '', runtimeStr: null });
  }
}
</script>

<style scoped>
.demo-datetime-picker {
  display: flex;
  width: 100%;
  padding: 0;
  flex-wrap: wrap;
  justify-content: space-around;
  align-items: stretch;
}

.demo-datetime-picker .block {
  padding: 15px 0;
  text-align: center;
}
</style>

<template>
  <div style="margin-bottom: 15px">
    <a class="el-button el-button--primary" target="_blank" href="https://cron.qqe2.com/">
      在线cron
    </a>

    <!--
            main.ts：import 'element-plus/theme-chalk/dark/css-vars.css'
            https://github.com/vueuse/vueuse/blob/main/packages/core/useDark/demo.vue
    -->
    <el-button type="success" @click="toggleDark()" style="width: 80px">
      <i inline-block align-middle i="dark:carbon-moon carbon-sun" />
      <span class="ml-2">{{ isDark ? 'Dark' : 'Light' }}</span>
    </el-button>
    <el-button>
      <el-switch v-model="isStartAndStop" class="ml-2" inline-prompt @change="StartAndStop"
        style="--el-switch-on-color: #13ce66; --el-switch-off-color: #4c4d4f" active-text="已开启调度" inactive-text="已关闭调度" />
    </el-button>

  </div>
  <el-form :inline="true" :model="form" label-width="auto" style="" v-show="isStartAndStop">
    <el-form-item>
      <el-input v-model="form.taskName" placeholder="任务名" clearable />
    </el-form-item>
    <el-form-item>
      <el-input v-model="form.groupName" placeholder="分组名" clearable />
    </el-form-item>
    <el-form-item>
      <el-button type="warning" @click="onSubmit">查询</el-button>
    </el-form-item>

    <!-- 动态执行参数 Start-->
    <el-form-item>
      <el-input v-model="form.dynamicData" placeholder="执行参数" clearable style="width: 400px" />
    </el-form-item>

    <el-form-item>
      <div class="demo-datetime-picker">
        <div class="block">
          <el-date-picker v-model="form.runtimeStr" type="datetimerange" start-placeholder="Start date"
            end-placeholder="End date" format="YYYY-MM-DD HH:mm:ss" date-format="YYYY/MM/DD ddd" time-format="A hh:mm:ss"
            el-date-picker value-format="YYYY-MM-DD HH:mm:ss" />
        </div>
      </div>
    </el-form-item>
    <!-- 动态执行参数 End-->
  </el-form>

  <el-table :data="tableData" border size="small" highlight-current-row="true" style="width: 100%"
    v-show="isStartAndStop">
    <el-table-column prop="taskName" label="任务名" />
    <el-table-column prop="groupName" label="分组名" />
    <el-table-column prop="status" label="状态" #default="scope">
      <span v-html="switchT(scope.row.status)"></span>
    </el-table-column>
    <el-table-column prop="cron" label="Cron" />
    <el-table-column prop="describe" label="描述" />
    <el-table-column prop="lastRunTime" label="最后执行时间" />
    <el-table-column fixed="right" label="操作" width="240">
      <template #default="scope">
        <el-button link type="primary" size="small" @click="tiggerAction('Run', scope.row)">
          立即执行
        </el-button>
        <el-button link type="primary" size="small" @click="tiggerAction('Pause', scope.row)">
          暂停
        </el-button>
        <el-button link type="primary" size="small" @click="tiggerAction('Start', scope.row)">
          开启
        </el-button>
      </template>
    </el-table-column>
    <el-table-column fixed="right" label="操作" width="220">
      <template #default="scope">
        <!-- <el-button
          link
          type="primary"
          size="small"
          @click="tiggerAction('remove', scope.row)"
        >
          删除
        </el-button> -->
        <el-button link type="primary" size="small" @click="DetailClick(scope.row)">
          记录
        </el-button>
      </template>
    </el-table-column>
  </el-table>

  <!-- <router-link to="/ok">去时间组件界面看看</router-link>
  <br />
  <a href="/ok">时间组件界面</a> -->
</template>

