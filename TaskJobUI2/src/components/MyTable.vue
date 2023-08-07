<script lang="ts">
import { ref, defineComponent } from 'vue';
import { ElMessage } from "element-plus"
import { useRouter } from "vue-router"

// const axios = require('axios');

const tableData = [
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
];

const taskName = ref('')

export default defineComponent({
  components: {
  },
  setup() {
    return {
      zIndex: 3000,
      size: 'small',
      tableData: tableData
    }
  },
  // 启用了类型推导
  props: {
    name: String,
    msg: { type: String, required: true }
  },
  data() {
    return {
      form: {
        taskName,
      },
      router: useRouter(),
      count: 1,
      DetailClick() {
        ElMessage.error('Oops, this is a error message.')
      }
    }
  },
  methods: {
    onSubmit() {
      ElMessage({ message: '提交成功！', type: 'success' });

      // this.axios({
      //   method: 'post',
      //   url: '/user/12345',
      //   data: {
      //     firstName: 'Fred',
      //     lastName: 'Flintstone'
      //   }
      // });
    },
    EditClick() {
      ElMessage({ message: '成功了呀', type: 'success' });
      this.router.push('/ok');
      // this.$route.push('/ok');
    },
  },
  computed: {
    username() {
      // 我们很快就会看到 `params` 是什么
      return this.$route.params.username
    },
  },
  mounted() {
    this.name // 类型：string | undefined
    this.msg // 类型：string
    this.count // 类型：number
  }
});
</script>
  
<template>
  <div style="">
    <el-form :model="form" label-width="120px" style="display: flex;justify-content: flex-start;">
      <el-form-item><el-input v-model="form.taskName" placeholder="任务名" clearable /></el-form-item>
      <el-form-item><el-input v-model="form.taskName" placeholder="分组名" clearable /></el-form-item>
      <el-form-item><el-input v-model="form.taskName" placeholder="执行参数" clearable /></el-form-item>
      <el-form-item><el-button type="primary" @click="onSubmit">查询</el-button></el-form-item>
      <el-form-item><el-button>Cancel</el-button></el-form-item>
    </el-form>
  </div>
  <el-table :data="tableData" border size="small" highlight-current-row="true" style="width: 100%">
    <el-table-column prop="taskName" label="任务名" />
    <el-table-column prop="groupName" label="分组名" />
    <el-table-column prop="status" label="状态" />
    <el-table-column prop="cron" label="Cron" />
    <el-table-column prop="des" label="描述" />
    <el-table-column prop="updateTime" label="最后执行时间" />
    <el-table-column fixed="right" label="操作" width="220">
      <template #default>
        <el-button link type="primary" size="small" @click="DetailClick">立即执行</el-button>
        <el-button link type="primary" size="small" @click="DetailClick">暂停</el-button>
      </template>
    </el-table-column>
    <el-table-column fixed="right" label="操作" width="220">
      <template #default>
        <el-button link type="primary" size="small" @click="DetailClick">删除</el-button>
        <el-button link type="primary" size="small" @click="EditClick">记录</el-button>
      </template>
    </el-table-column>
  </el-table>

  <router-link to="/ok">去时间组件界面看看</router-link>
  <br />
  <a href="/ok">时间组件界面</a>
</template>