<template>
<div id="div1" style="">
  <!-- <el-form ref="form" :model="form" label-width="80px">
    <el-form-item label="活动名称">
      <el-input v-model="form.name"></el-input>
    </el-form-item> 
  </el-form> -->
<div id="" style="display:flex;justify-content:center">
    <el-input v-model="taskname" placeholder="任务名"></el-input>
    <el-input v-model="groupname" placeholder="分组名"></el-input>
    <el-input v-model="dynamicData" placeholder="动态执行参数"></el-input>
    <el-button type="primary" @click="getlist()">查询</el-button>
    <el-button type="info">重置</el-button>
    <el-button type="success" @click="tiggerAction('Startup',{})">启动</el-button>
    <a class="el-button el-button--primary el-button--small" target="_blank" href="https://cron.qqe2.com/">在线cron</a>
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
      <template slot-scope="scope">
        <el-button @click="tiggerAction('Pause',scope.row)" type="text" size="small">暂停</el-button>
        <el-button @click="tiggerAction('Start',scope.row)" type="text" size="small">开启</el-button>
        <el-button @click="tiggerAction('Run',scope.row)" type="text" size="small">立即执行</el-button>
        <el-button @click="tiggerAction('remove',scope.row)" type="text" size="small">删除</el-button>
        <el-button @click="detail(scope.row)" type="text" size="small">记录</el-button>
      </template>
    </el-table-column>
    </el-table>
</div>
</template>

<script> 
export default {
  name: 'App', 
   data() {
        return {
          groupname:'',
          taskname:'',
          dynamicData:'',
          tableData: []
     }
   },
      methods: {
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
                    };
                    that.request("/TaskJob-GetJobs", parms, function (res) {
                        if (res.data.length > 0) { }
                        that.tableData = res.data;
                    });
                },
                //记录
                detail: function (item) {
                    console.info(item);
                    this.$router.push({ name: 'detail', params: { datai: item }})
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
                    if (url.indexOf("Run") > -1) {
                        params.DynamicData = encodeURI(this.dynamicData);
                    }
                    this.$axios({
                        method: 'post',
                        url: url,
                        //params: params,
                        data: params,
                        headers: { 'X-Requested-With': 'XMLHttpRequest', 'Content-Type': 'application/json','taskjob':'now' }
                    }).then(function (response) {
                        fun && fun(response.data);
                    }).catch(function (error) {
                        console.info(error);
                    });
                }
            },
            created: function (item) {
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