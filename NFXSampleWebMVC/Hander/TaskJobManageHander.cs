using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NFXSampleWebMVC
{
    internal class TaskJobManageHander
    {
        Dictionary<string, Func<TaskModel, Task<ResultMsg>>> _pairs = new Dictionary<string, Func<TaskModel, Task<ResultMsg>>>();
        ISchedulerFactory _schedulerFactory;

        public TaskJobManageHander(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            _pairs.Add("/TaskJob-Start", async (parms) => await Start(parms)); // 开始
            _pairs.Add("/TaskJob-Stop", async (parms) => await Stop());// 停止
            _pairs.Add("/TaskJob-Pause", async (parms) => await Pause(parms)); // 暂停
            _pairs.Add("/TaskJob-Run", async (parms) => await Run(parms)); // 立即执行
            _pairs.Add("/TaskJob-Startup", async (parms) => await Startup()); // 启动
            _pairs.Add("/TaskJob-GetJobs", async (parms) => await GetJobs(parms)); // 获取运行中的列表 
            _pairs.Add("/TaskJob-Detail",  (parms) => { return Task.FromResult(new ResultMsg()); }); // 执行记录：暂未实现
        }

        public Task<ResultMsg> ExecHander(string path, TaskModel input) 
        {
            if (_pairs.TryGetValue(path, out var result))
            {
                return result(input);
            }
            return null;
        }

        public async Task<ResultMsg> Start(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (ITrigger trigger, _, string msg) = await Verification(scheduler, input.groupName, input.taskName);
            if (msg != null)
                return ResultMsg.Fail(msg);

            await scheduler.ResumeTrigger(trigger.Key);
            // 通知改 _taskModelLists
            return ResultMsg.Ok("开启成功");
        }

        // 停止所有
        public async Task<ResultMsg> Stop()
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Shutdown();
            // 通知改 _taskModelLists
            return ResultMsg.Ok("停止所有成功");
        }

        // 暂停
        public async Task<ResultMsg> Pause(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (ITrigger trigger, _, string msg) = await Verification(scheduler, input.groupName, input.taskName);
            if (msg != null)
                return ResultMsg.Fail(msg);

            await scheduler.PauseTrigger(trigger.Key);
            // 通知改 _taskModelLists
            return ResultMsg.Ok("暂停成功");
        }

        static Assembly[] _assemblies;

        static TaskJobManageHander() 
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        //启动
        public async Task<ResultMsg> Startup()
        {
            Assembly p1 = System.Reflection.Assembly.GetEntryAssembly();
            Assembly[] p2 = AppDomain.CurrentDomain.GetAssemblies();

            List<TaskModel> tasksList = new List<TaskModel>();
            var IJobtype = typeof(IJob);
            var TaskModeltype = typeof(TaskModel);

            // 这里偶尔会报空 null，因为执行现成的不同，"无法加载一个或多个请求的类型。有关更多信息，请检索 LoaderExceptions 属性。"

            var types = /*AppDomain.CurrentDomain.GetAssemblies()*/ _assemblies
                       .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(IJobtype) && t.IsSubclassOf(TaskModeltype)))
                       ?.ToArray();// types 可能为空

            //var types1 = System.Reflection.Assembly.GetEntryAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(IJobtype) && t.IsSubclassOf(TaskModeltype))
            //       ?.ToArray();// types1 可能为空

            foreach (Type item in types)
            {
                var taskmodel = Activator.CreateInstance(item) as TaskModel;
                taskmodel.Config();
                taskmodel.Validation();
                tasksList.Add(taskmodel);
            }

            foreach (TaskModel item in tasksList)
            {
                IJobDetail job = JobBuilder.Create(item.Job).WithIdentity(item.taskName, item.groupName).Build();
                var builder = TriggerBuilder.Create();
                ITrigger trigger = item.ConfigTrigger(builder)
                       .WithIdentity(item.taskName, item.groupName)
                       .StartNow()
                       .WithDescription(item.describe)
                       .WithCronSchedule(item.cron)
                       .Build();
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                JobKey jobKey = new JobKey(item.taskName, item.groupName);
                var v = await scheduler.CheckExists(jobKey);
                if (v)
                {
                    return ResultMsg.Fail("重复启动");
                }
                await scheduler.ScheduleJob(job, trigger);
                await scheduler.Start();
            }
            Quartz.Logging.LogProvider.SetCurrentLogProvider(new WorkingLogProvider());
            return ResultMsg.Ok("启动成功");
        }

        public async Task<ResultMsg> GetJobs(TaskModel input)
        {
            List<TaskModel> taskModels = new List<TaskModel>();
            IScheduler _scheduler = await _schedulerFactory.GetScheduler();

            var groups = await _scheduler.GetJobGroupNames();
            foreach (var groupName in groups)
            {
                TaskModel model = new TaskModel
                {
                    groupName = groupName
                };
                foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)))
                {
                    model.taskName = jobKey.Name;
                    var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                    foreach (ITrigger trigger in triggers)
                    {
                        TriggerState state = await _scheduler.GetTriggerState(trigger.Key);
                        model.status = (int)state; //这里需要从内存中读取，判断任务的执行状态
                        model.cron = ((Quartz.Impl.Triggers.CronTriggerImpl)trigger).CronExpressionString;
                        model.describe = ((Quartz.Impl.Triggers.CronTriggerImpl)trigger).Description;
                        DateTimeOffset? dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                        if (dateTimeOffset != null)
                        {
                            model.SetLastRunTime(Convert.ToDateTime(dateTimeOffset.ToString()));
                        }
                        // 发起执行时间的事件，记录日志
                    }
                    taskModels.Add(model);
                }
            }
            if (input != null && !string.IsNullOrWhiteSpace(input.taskName))
                taskModels = taskModels.Where(a => a.taskName.Contains(input.taskName)).ToList();
            if (input != null && !string.IsNullOrWhiteSpace(input.groupName))
                taskModels = taskModels.Where(a => a.taskName.Contains(input.groupName)).ToList();

            return ResultMsg.Ok("ok", taskModels);
        }

        // 立即执行
        public async Task<ResultMsg> Run(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (ITrigger trigger, JobKey jobKey, string msg) = await Verification(scheduler, input.groupName, input.taskName);
            if (msg != null)
                return ResultMsg.Fail(msg);

            // IJobDetail jobdetail = await scheduler.GetJobDetail(jobKey);
            IDictionary<string, object> dat = new Dictionary<string, object>();
            dat.Add("parms", System.Web.HttpUtility.UrlDecode(input.DynamicData));
            await scheduler.TriggerJob(jobKey, new JobDataMap(dat));
            return ResultMsg.Ok("立即执行成功");
        }

        // 删除
        public async Task<ResultMsg> Remove(TaskModel model)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (ITrigger trigger, JobKey jobKey, string msg) = await Verification(scheduler, model.groupName, model.taskName);
            if (msg != null)
                return ResultMsg.Fail(msg);

            await scheduler.PauseTrigger(trigger.Key);
            await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
            await scheduler.DeleteJob(jobKey);
            return ResultMsg.Ok("删除成功");
        }

        private async Task<(ITrigger, JobKey jobKey, string)> Verification(IScheduler scheduler, string groupName, string taskName)
        {
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName));
            if (jobKeys == null || jobKeys.Count == 0)
                return (null, null, $"未找到分组[{groupName}]");

            JobKey jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as Quartz.Impl.Triggers.CronTriggerImpl).Name == taskName)).FirstOrDefault();
            if (jobKey == null)
                return (null, null, $"未找到JobKey[{taskName}]");

            var triggers = await scheduler.GetTriggersOfJob(jobKey);
            ITrigger trigger = triggers?.Where(x => (x as Quartz.Impl.Triggers.CronTriggerImpl).Name == taskName).FirstOrDefault();

            if (trigger == null)
                return (null, null, $"未找到触发器[{taskName}]");

            return (trigger, jobKey, null);
        }
    }
}
