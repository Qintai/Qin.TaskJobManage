using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qin.TaskJobManage.Hander
{
    internal class TaskJobManageHander
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly TaskJobConfig _taskJonConfig;
        private IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Func<string, Task<ResultMsg>>> _pairs = new Dictionary<string, Func<string, Task<ResultMsg>>>();
        private void Init()
        {
            _pairs.Add("/taskjob-start", async (inputjson) => await Start(GetTaskModel(inputjson)));
            _pairs.Add("/taskjob-stop", async (parms) => await Stop());
            _pairs.Add("/taskjob-pause", async (parms) => await Pause(GetTaskModel(parms))); // suspend
            _pairs.Add("/taskjob-run", async (parms) => await Run(GetTaskModel(parms))); // Pause immediate execution
            _pairs.Add("/taskjob-startup", async (parms) => await Startup());
            _pairs.Add("/taskjob-getjobs", async (parms) => await GetJobs2(GetTaskModel(parms))); // Get running list 
            _pairs.Add("/taskjob-remove", async (parms) => await Remove(GetTaskModel(parms))); // Get running list 
            _pairs.Add("/taskjob-detail", async (parms) =>
            {
                int pageIndex = 0, pageSize = 0;
                using JsonDocument jsondocument = JsonDocument.Parse(parms);
                if (jsondocument.RootElement.TryGetProperty("pageIndex", out JsonElement jpageIndex))
                    jpageIndex.TryGetInt32(out pageIndex);
                if (jsondocument.RootElement.TryGetProperty("pageSize", out JsonElement jpageSize))
                    jpageSize.TryGetInt32(out pageSize);
                return await JobDetail(GetTaskModel(parms), pageIndex, pageSize);
            }); // Execution record
        }

        private TaskModel GetTaskModel(string json) => JsonSerializer.Deserialize<TaskModel>(json) ?? new TaskModel();

        public TaskJobManageHander(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _schedulerFactory = (ISchedulerFactory)serviceProvider.GetService(typeof(ISchedulerFactory));
            _taskJonConfig = (TaskJobConfig)serviceProvider.GetService(typeof(TaskJobConfig));
            _jobFactory = (IJobFactory)serviceProvider.GetService(typeof(IJobFactory));
            Init();
        }

        private async Task<ResultMsg> JobDetail(TaskModel parms, int pageIndex, int pageSize)
        {
            IJobExecutionLogs ijobexecutionlog = (IJobExecutionLogs)_serviceProvider.GetService(typeof(IJobExecutionLogs));
            if (ijobexecutionlog != null)
            {
                ResultMsg resultMsg = await ijobexecutionlog.GetJobExecutionLog(parms, pageIndex, pageSize);
                return resultMsg;
            }
            return ResultMsg.Ok();
        }

        public Task<ResultMsg> ExecHander(string path, string inputjson)
        {
            string lower_path = path.ToLowerInvariant();
            if (_pairs.TryGetValue(lower_path, out var result))
            {
                return result(inputjson);
            }
            return Task.FromResult(ResultMsg.Ok());
        }

        public async Task<ResultMsg> Start(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (var jobkeyItrigger, var msg) = await GetJobKeyByTaskName(scheduler, input);
            if (msg != "")
                return ResultMsg.Fail(msg);

            await scheduler.ResumeTrigger(jobkeyItrigger.Value.Key);
            return ResultMsg.Ok("开启成功");
        }

        // 停止所有
        public async Task<ResultMsg> Stop()
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Shutdown();
            return ResultMsg.Ok("停止所有成功");
        }

        // 暂停
        public async Task<ResultMsg> Pause(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (var jobkeyItrigger, var msg) = await GetJobKeyByTaskName(scheduler, input);
            if (msg != "") return ResultMsg.Fail(msg);
            await scheduler.PauseTrigger(jobkeyItrigger.Value.Key);
            return ResultMsg.Ok("暂停成功");
        }

        //启动
        public async Task<ResultMsg> Startup()
        {
            IEnumerable<TaskModel> taskJobs = (IEnumerable<TaskModel>)_serviceProvider.GetService(typeof(IEnumerable<TaskModel>));
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            scheduler.JobFactory = _jobFactory;
            foreach (TaskModel item in taskJobs)
            {
                item.Config();
                item.Validation();

                IJobDetail job = JobBuilder.Create(item.Job).WithIdentity(item.taskName, item.groupName).Build();
                var builder = TriggerBuilder.Create();
                ITrigger trigger = item.ConfigTrigger(builder)
                       .WithIdentity(item.taskName, item.groupName)
                       .StartNow()
                       .WithDescription(item.describe)
                       .WithCronSchedule(item.cron)
                       .Build();
                try
                {
                    JobKey jobKey = new JobKey(item.taskName, item.groupName);
                    var v = await scheduler.CheckExists(jobKey);
                    if (v)
                    {
                        ConsoleExcept.WriteLine("Repeat start task", ConsoleColor.DarkYellow);
                        return ResultMsg.Fail("重复启动任务");
                    }
                    await scheduler.ScheduleJob(job, trigger);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            await scheduler.Start();
            Quartz.Logging.LogProvider.SetCurrentLogProvider(new WorkingLogProvider());
            ConsoleExcept.WriteLine("Task started successfully", ConsoleColor.Green);
            return ResultMsg.Ok("任务启动成功");
        }

        public async Task<ResultMsg> GetJobs2(TaskModel input)
        {
            IEnumerable<TaskModel> taskJobs = (IEnumerable<TaskModel>)_serviceProvider.GetService(typeof(IEnumerable<TaskModel>));
            if (!input.taskName.IsNullOrWhiteSpace())
                taskJobs = taskJobs.Where(a => a.taskName == input.taskName);
            if (!input.groupName.IsNullOrWhiteSpace())
                taskJobs = taskJobs.Where(a => a.groupName == input.groupName);

            List<TaskModel> taskModels = new List<TaskModel>();
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            if (!scheduler.IsStarted)
                return ResultMsg.Fail("未启动");

            foreach (var item in taskJobs)
            {
                // var jobkey = new JobKey(item.taskName, item.groupName);
                var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(item.groupName));
                if (jobKeys.Count == 0) 
                    continue;

                (var jobkeyItrigger, var msg) = await GetJobKeyByTaskName(scheduler, item, jobKeys);
                if (msg != "") return ResultMsg.Fail(msg);

                TriggerState state = await scheduler.GetTriggerState(jobkeyItrigger.Value.Key);
                DateTimeOffset? dateTimeOffset = jobkeyItrigger.Value.GetPreviousFireTimeUtc();
                if (dateTimeOffset != null)
                {
                    item.SetLastRunTime(Convert.ToDateTime(dateTimeOffset.ToString()));
                }
                item.status = (int)state;
            }
            return ResultMsg.Ok("", taskJobs.ToList());
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
                        if (trigger is Quartz.Impl.Triggers.CronTriggerImpl crontrigger)
                        {
                            model.cron = crontrigger.CronExpressionString ?? "";
                            model.describe = crontrigger.Description ?? "";
                        }
                        else
                        {
                            model.cron = "";
                            model.describe = "Quartz.Impl.Triggers.SimpleTriggerImpl";
                        }
                        DateTimeOffset? dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                        if (dateTimeOffset != null)
                        {
                            model.SetLastRunTime(Convert.ToDateTime(dateTimeOffset.ToString()));
                        }
                    }
                    taskModels.Add(model);
                }
            }
            if (input != null && !string.IsNullOrWhiteSpace(input.taskName))
                taskModels = taskModels.Where(a => a.taskName.Contains(input.taskName)).ToList();
            if (input != null && !string.IsNullOrWhiteSpace(input.groupName))
                taskModels = taskModels.Where(a => a.groupName.Contains(input.groupName)).ToList();

            return ResultMsg.Ok("ok", taskModels);
        }

        // 立即执行
        public async Task<ResultMsg> Run(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (var jobkeyItrigger, var msg) = await GetJobKeyByTaskName(scheduler, input);
            if (msg != "") return ResultMsg.Fail(msg);

            IDictionary<string, object> dat = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(input.dynamicData))
                dat.Add("parms", System.Web.HttpUtility.UrlDecode(input.dynamicData));
            
            if (input.runtimeStr != null && input.runtimeStr.Count > 1)
            {
                dat.Add("run_stime", input.runtimeStr[0]);
                dat.Add("run_etime", input.runtimeStr[1]);
            }

            await scheduler.TriggerJob(jobkeyItrigger.Key, new JobDataMap(dat));
            return ResultMsg.Ok("立即执行成功");
        }

        // 删除
        public async Task<ResultMsg> Remove(TaskModel input)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            (var jobkeyItrigger, var msg) = await GetJobKeyByTaskName(scheduler, input);
            if (msg != "") return ResultMsg.Fail(msg);
            await scheduler.PauseTrigger(jobkeyItrigger.Value.Key);
            await scheduler.UnscheduleJob(jobkeyItrigger.Value.Key);// 移除触发器
            await scheduler.DeleteJob(jobkeyItrigger.Key);
            return ResultMsg.Ok("删除成功");
        }

        public async Task<(KeyValuePair<JobKey, ITrigger> A, string Fail)> GetJobKeyByTaskName(IScheduler scheduler, TaskModel item, IReadOnlyCollection<JobKey> list = null)
        {
            KeyValuePair<JobKey, ITrigger> res = new KeyValuePair<JobKey, ITrigger>();
            if (list == null)
                list = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(item.groupName));

            foreach (JobKey jobkey in list)
            {
                var itriggers = await scheduler.GetTriggersOfJob(jobkey);
                var itrigger = itriggers.FirstOrDefault(x => (x as Quartz.Impl.Triggers.CronTriggerImpl)?.Name == item.taskName);
                if (itrigger != null) 
                    res = new KeyValuePair<JobKey, ITrigger>(jobkey, itrigger); 
            }

            if (res.Key == null || res.Value == null) 
                return (res, "不存在jobkey与itrigger"); 

            return (res, string.Empty);
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
