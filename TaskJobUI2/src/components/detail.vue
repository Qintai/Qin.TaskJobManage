<template>
<div id="div1" style="">
<div id="" style="display:flex;justify-content:center">
    <!-- <el-select v-model="selvalue" placeholder="执行类型" name="v">
        <el-option value="1" name="v1">手动</el-option>
        <el-option value="2" name="v2">自动</el-option>放开报错
    </el-select>
    <select class="el-input el-input--suffix">
        <option value="null" name="v1">请选择</option>
        <option value="1" name="v1">手动</option>
        <option value="2" name="v2">自动</option>
    </select> -->
    <el-input v-model="taskname" placeholder="任务名"></el-input>
    <el-input v-model="groupname" placeholder="分组名"></el-input>
    <el-input v-model="dynamicData" placeholder="动态执行参数"></el-input>
    <el-button type="primary" @click="getlist()">查询</el-button>
    <el-button type="info" @click="back1()">返回</el-button>
</div>
    <el-table
      :data="tableData"
      style="width: 100%">
      <el-table-column
        prop="taskName"
        label="任务名"
        width="180">
      </el-table-column>
      <el-table-column
        prop="groupName"
        label="分组名"
        width="180">
      </el-table-column>
     <el-table-column
        prop="status"
        label="状态">
       <template slot-scope="scope"> <span v-html="switchT(scope.row.status)"></span>  </template>
      </el-table-column>
      <el-table-column
        prop="cron"
        label="cron">
      </el-table-column>
    <el-table-column
        prop="describe"
        label="描述">
      </el-table-column>
          <el-table-column
        prop="lastRunTime"
        label="最后执行时间">
      </el-table-column>  
     <el-table-column
      fixed="right"
      label="操作">
      <!-- <template slot-scope="scope">
        <el-button @click="tiggerAction('Pause',scope.row)" type="text" size="small">暂停</el-button>
        <el-button @click="tiggerAction('Start',scope.row)" type="text" size="small">开启</el-button>
        <el-button @click="tiggerAction('Run',scope.row)" type="text" size="small">立即执行</el-button>
        <el-button @click="tiggerAction('remove',scope.row)" type="text" size="small">删除</el-button>
        <el-button @click="detail(scope.row)" type="text" size="small">记录</el-button>
      </template> -->
    </el-table-column>
    </el-table>
    <el-pagination  small layout="prev, pager, next" :total="total" 
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"></el-pagination>
</div>
</template>
<script> 

export default {
  name: 'App',
  data() {
        return {
           selvalue:"",
            options: [
                { value: '1', label: '手动' },{ value: '2', label: '自动' }
            ],
          groupname:'',
          taskname:'',
          dynamicData:'',
          total:90,
          pageindex:1,
          pagesize:10,
          tableData: []
     }
   },
  methods: {
                handleSizeChange:function(pagesize){
                    this.pagesize = pagesize;
                    this.getlist();
                },
                handleCurrentChange:function(pageindex){
                    this.pageindex = pageindex;
                    this.getlist();
                },
                back1:function(){
                   this.$router.push({ name: 'table', params: { datai: 123 }})
                },
                switchT: function (status) {
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
                },
                getlist: function () {
                    var that = this;
                    var parms = {
                        taskName: that.taskname,
                        groupName: that.groupname,
                        pageIndex: that.pageindex,
                        pageSize: that.pagesize
                    };
                    that.request("/TaskJob-Detail", parms, function (res) {
                       that.tableData = res.data.TableData;
                       that.total = res.data.Count;
                    });
                },
                //记录
                detail: function (item) {
                    console.info(item);
                    this.$router.push({ path: "/" });
                },
                tiggerAction: function (action, item) {
                    var that = this;
                    that.request('/TaskJob-' + action, item, function (res) {
                        if (res.status) {
                            that.getlist();   // 刷新列表
                            alert(res.msg);
                        }
                    });
                },
                request: function (url, params, fun) {
                    var dynamicData = "";
                    if (url.indexOf("Run") > -1) {
                        dynamicData = encodeURI(this.dynamicData);
                    }
                    this.$axios({
                        method: 'post',
                        url: url,
                        //params: params,
                        data: params,
                        headers: { 'X-Requested-With': 'XMLHttpRequest', 'Content-Type': 'application/json', 'taskjob':'now', 'dynamicData': dynamicData }
                    }).then(function (response) {
                        fun && fun(response.data);
                    }).catch(function (error) {
                        console.info(error);
                    });
                }
            },
            created: function (item) {
                if(JSON.stringify(this.$route.params ) !="{}")  {
                    console.info('parms:' + this.$route.params.datai.taskName);
                    this.groupname = this.$route.params.datai.groupName;
                    this.taskname = this.$route.params.datai.taskName;
                }
                this.getlist(); 
            }
}
</script>
<style>
#div1{
   width: 80%;
   margin: 0 auto;
   /* background-color: rgb(22, 228, 22); */
}
</style>