<script lang="ts" setup>
import { onMounted, onUnmounted, ref, reactive } from 'vue';
import { useRoute , useRouter} from 'vue-router';
import { request } from '../common/HttpApi';

const router = useRouter();
const route = useRoute();
const form:any= reactive({
  taskName: '',
  groupName: ''
});
const total = ref(10);
const pageIndex = ref(1);
const pageSize = ref(10);
const fpage = ref(1);

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
onMounted(() => {
  console.info(route.query);
  form.taskName = route.query?.taskName;
  form.groupName = route.query?.groupName;
  onSubmit();
    //绑定监听事件
	window.addEventListener('keydown', keyDown)
});

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

// page-size 改变时触发
const handleSizeChange = (val: number) => {
  console.log(`${val} items per page`);
  pageSize.value = val;
  onSubmit();
};

// current-page 改变时触发
const handleCurrentChange = (val: number) => {
  // console.log(`current page: ${val}`);
  pageIndex.value = val;
  onSubmit();
};

// 上一页
const prevclick = (val: number) => {
  console.log(`current page: ${val}`);
  pageIndex.value = val;
  onSubmit();
};
// 下一页
const nextclick = (val: number) => {
  console.log(`current page: ${val}`);
  pageIndex.value = val;
  onSubmit();
};

function goBack(){
  router.push({
    path: 'jobtable'
  });
}

//点击回车键登录
const keyDown = (e:any) => {
	if (e.keyCode == 13 || e.keyCode == 100) {
		onSubmit()
	}
}

onUnmounted(() => {
	//销毁事件
	window.removeEventListener('keydown', keyDown, false)
});


function onSubmit() {
  const parms = {
    taskName: form.taskName,
    groupName: form.groupName,
    pageIndex: pageIndex.value,
    pageSize: pageSize.value
  };

  request('/TaskJob-Detail', parms, function (res: { status: any; data: { TableData: { taskName: string; groupName: string; status: number; cron: string; des: string; updateTime: string; }[]; Count: number; }; }) {
    if (res.status) {
      tableData.value = res.data.TableData;
      total.value = res.data.Count;
    }
  });
}
</script>

<template>
  <el-form :inline="true" :model="form" label-width="auto" style="">
    <el-form-item>
      <el-input v-model="form.taskName" placeholder="任务名" clearable />
    </el-form-item>
    <el-form-item>
      <el-input v-model="form.groupName" placeholder="分组名" clearable />
    </el-form-item>
    <el-form-item>
      <el-button type="warning" @click="onSubmit">查询</el-button>
    </el-form-item>
    <el-form-item>
      <el-button type="success" @click="goBack">返回</el-button>
    </el-form-item>
  </el-form>
  <el-table
    :data="tableData"
    border
    size="small"
    highlight-current-row="true"
    style="width: 100%"
  >
    <el-table-column prop="taskName" label="任务名" />
    <el-table-column prop="groupName" label="分组名" />
    <el-table-column prop="status" label="状态" #default="scope">
      <span v-html="switchT(scope.row.status)"></span>
    </el-table-column>
    <el-table-column prop="cron" label="Cron" />
    <el-table-column prop="describe" label="描述" />
    <el-table-column prop="lastRunTime" label="最后执行时间" />
  </el-table>

  <div class="example-pagination-block">
    <el-pagination
      layout="prev, pager, next"
      :total="total"
      v-model="fpage"
      @prev-click="prevclick"
      @next-click="nextclick"
      @current-page="handleCurrentChange"
      @page-size="handleSizeChange"
      @current-change="handleCurrentChange"
    />
  </div>
</template>

<!-- 
<script lang="ts">
import { useRouter } from 'vue-router';
import { defineComponent } from 'vue';

// const router = useRouter();

//query
// console.info(router.query.userId);
//
//params
// console.info(router.params.userId);

const tableData = [
  {
    date: '2016-05-03',
    name: 'Tom',
    address: 'No. 189, Grove St, Los Angeles'
  },
  {
    date: '2016-05-02',
    name: 'Tom',
    address: 'No. 189, Grove St, Los Angeles'
  },
  {
    date: '2016-05-04',
    name: 'Tom',
    address: 'No. 189, Grove St, Los Angeles'
  },
  {
    date: '2016-05-01',
    name: 'Tom',
    address: 'No. 189, Grove St, Los Angeles'
  }
];

export default defineComponent({
  components: {},
  setup() {
    return {
      zIndex: 3000,
      size: 'small',
      tableData,
      route : useRouter()
    };
  },
  created: function () {
    // let route = useRouter();
    console.info(useRouter());
    console.info(` query参数如下：${useRouter().query}`);
    console.info(` 参数如下：${this.route.params}`);
  }
});
</script>

<template>
  <el-table :data="tableData" style="width: 100%">
    <el-table-column prop="date" label="Date" width="180" />
    <el-table-column prop="name" label="Name" width="180" />
    <el-table-column prop="address" label="Address" />
  </el-table>
  ---------------------
  <el-card class="box-card">
    <template #header>
      <div class="card-header">
        <span>Card name</span>
        <el-button class="button" text>Operation button</el-button>
      </div>
    </template>
    <div v-for="o in 4" :key="o" class="text item">{{ 'List item ' + o }}</div>
  </el-card>
</template>

<style scoped>
.demo-date-picker {
  display: flex;
  width: 100%;
  padding: 0;
  flex-wrap: wrap;
}

.demo-date-picker .block {
  padding: 30px 0;
  text-align: center;
  border-right: solid 1px var(--el-border-color);
  flex: 1;
}

.demo-date-picker .block:last-child {
  border-right: none;
}

.demo-date-picker .demonstration {
  display: block;
  color: var(--el-text-color-secondary);
  font-size: 14px;
  margin-bottom: 20px;
}
</style> 
-->
