# Quartz基础管理页，支持传如动态参数手动触发任务

项目结构
- webapidemo： sample_webapi
- 后端：TaskJobManage
- 前端：TaskJobUI

本地联调时，注意后端接口的 9950 ，需要与前端项目的 代理 一致才可正常联调

```
<PackageReference Include="Qin.TaskJobManage" Version="1.0.6.1" />
```


## 1.配置 
```
 public void ConfigureServices(IServiceCollection services)
 {
    services.AddTaskJob(options=> 
    { 
        // options.Route 主页地址，默认是 /TaskJobUI
        // options.LoadAssemblyName job所在的程序集名称，默认在当前web启动程序集
        // options.SelfTurnOn 程序启动运行任务, 默认true
        // options.AddJobExecutionLog<JobExecutionLog>(); 添加执行日志
   });
 }

public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
{
      lifetime.ApplicationStarted.Register(() => app.ApplicationServices.StartTaskJob());// 定时任务在程序启动后自动运行
      app.UseTaskJob(); // 要在 app.UseRouting(); 之前
}
```
## 2. 新建类，Job1 
```
public class Job1 : TaskModel, IJob 
{
    public override void Config()
    {
        status = 0;
        taskName = "测试任务a";
        groupName = "分组a"; // 分组名不要与其他job重复
        describe = "每10秒执行一次";
        cron = "0/10 * * * * ? ";
        Job = typeof(Job1);
    }

    public override TriggerBuilder ConfigTrigger(TriggerBuilder triggerbuilder)
    {
        return triggerbuilder;
    }

    public Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        Console.WriteLine(dataMap.GetString("parms") ?? "");
        string json = context.Trigger.JobDataMap.GetString("parms") ?? "";
        Console.WriteLine("动态传参数是：" + json);

        return Task.Run(() =>
        {
            //int p = 0;
            //var a = 12 / p;
            ConsoleExcept.WriteLine("Hello-" + DateTime.Now.ToString("HH:mm:ss"), ConsoleColor.Red);
        });
        //return Task.CompletedTask;
    }
}

```

## 3.进入管理页面。进入点击启动

正在运行中的任务：http://localhost:xxx/TaskJobUI/

![正在运行中的任务](https://gitee.com/qintaie/images/raw/master/Images/snipaste_20210923_095148.png "http://localhost:9950/TaskJobUI#/table")

![运行日志](https://gitee.com/qintaie/images/raw/master/Images/snipaste_20210923_095226.png "http://localhost:9950/TaskJobUI#/detail")

## 4.作业执行日志分页列表，请实现 IJobExecutionLogs 
```
    public class JobExecutionLog : IJobExecutionLogs
    {
        public Task<ResultMsg> GetJobExecutionLog(TaskModel parms, int pageIndex, int pageSize)
        {
            List<TaskModel> list = new List<TaskModel>();
            list.Add(new TaskModel() 
            {
                status = 0,
                taskName = "测试任务a",
                groupName = "分组a",
                describe = "每10秒执行一次",
                cron = "0/10 * * * * ? "
            });
            if (!string.IsNullOrWhiteSpace(parms.taskName))
            {
                list = list.Where(a=>a.taskName == parms.taskName).ToList();
            }
            if (!string.IsNullOrWhiteSpace(parms.groupName))
            {
                list = list.Where(a => a.groupName == parms.groupName).ToList();
            }

            return Task.FromResult(new ResultMsg
            {
                msg = $"pageIndex={pageIndex},pageSize={pageSize},taskName={parms.taskName},groupName={parms.groupName}",
                status = true,
                data = new TableList<TaskModel>()
                {
                    Count = list.Count(),
                    TableData = list.Skip(pageIndex == 1 ? 0 : 1).Take(pageSize)
                }
            });
        }
    }
```
## 5.说明
- 有运行任务日志采集，需要自己实现 WorkingLogProvider ，重写 GetLogger()  
- 其他功能，还在继续优化中，暂不支持数据持久化到 DB，但是支持实现灵活接口，复用


## 6.前端初始化：
npm install

npm i element-ui -S

npm install babel-plugin-component -D

npm install axios

运行：npm run dev <

