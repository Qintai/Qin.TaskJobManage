<script lang="ts" setup>
import { ref, onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
// import {GetBaidu} from '../templates/HttpApi.ts'
import { GetBaidu, request } from '../common/HttpApi.ts';
import { useDark, useToggle } from '@vueuse/core';

const isDark = useDark();
const toggleDark = useToggle(isDark);

const total = ref('0');
const tableData = ref([
  {
    taskName: '美团数据拉取',
    groupName: 'Tom_group',
    status: '运行中',
    cron: '2222',
    des: '每日作业清洗 1点',
    updateTime: '最后执行时间'
  },
  {
    taskName: '美团数据拉取2',
    groupName: 'Tom_group',
    status: '运行中',
    cron: '2222',
    des: '每日作业清洗 1点',
    updateTime: '最后执行时间'
  }
]);

const form = reactive({
  taskName: '',
  groupName: '',
  prams: '',
  pageindex: '',
  pagesize: '',
  delivery: false,
  type: [],
  desc: ''
});

// form.taskName = ref(0);
const router = useRouter();
onMounted(() => {
  console.info('启动了');
});

function DetailClick() {
  ElMessage.error('你要干啥');
}

function onSubmit() {
  ElMessage({ message: '提交成功！', type: 'success' });

  const parms = {
    taskName: form.taskName,
    groupName: form.groupName,
    pageIndex: form.pageindex,
    pageSize: form.pagesize
  };

  request('/TaskJob-Getjobs', parms, function (res) {
    if (res.status) {
      tableData.value = res.data;
      total.value = res.Count;
    }
  });
}

function EditClick() {
  ElMessage({ message: '成功了呀', type: 'success' });
  console.info(router);
  router.push('/as');
}
</script>

<template>
  <div style="">
    <el-form
      :model="form"
      label-width="120px"
      style="display: flex; justify-content: space-around"
    >
      <el-form-item>
        <el-input v-model="form.taskName" placeholder="任务名" clearable style="width: 120px" />
      </el-form-item>
      <el-form-item>
        <el-input v-model="form.groupName" placeholder="分组名" clearable style="width: 120px" />
      </el-form-item>
      <el-form-item>
        <el-input v-model="form.prams" placeholder="执行参数" clearable style="width: 420px" />
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
    </el-form>
  </div>
  <el-table
    :data="tableData"
    border
    size="small"
    highlight-current-row="true"
    style="width: 100%"
  >
    <el-table-column prop="taskName" label="任务名" />
    <el-table-column prop="groupName" label="分组名" />
    <el-table-column prop="status" label="状态" />
    <el-table-column prop="cron" label="Cron" />
    <el-table-column prop="des" label="描述" />
    <el-table-column prop="updateTime" label="最后执行时间" />
    <el-table-column fixed="right" label="操作" width="220">
      <template #default>
        <el-button link type="primary" size="small" @click="DetailClick">
          立即执行
        </el-button>
        <el-button link type="primary" size="small" @click="DetailClick">
          暂停
        </el-button>
      </template>
    </el-table-column>
    <el-table-column fixed="right" label="操作" width="220">
      <template #default>
        <el-button link type="primary" size="small" @click="DetailClick">
          删除
        </el-button>
        <el-button link type="primary" size="small" @click="EditClick">
          记录
        </el-button>
      </template>
    </el-table-column>
  </el-table>

  <router-link to="/ok">去时间组件界面看看</router-link>
  <br />
  <a href="/ok">时间组件界面</a>
</template>

