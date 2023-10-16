using Microsoft.Extensions.Logging;
using Qin.TaskJobManage;
using Quartz;
using System;
using System.Threading.Tasks;

namespace SampleWebApi
{
    [PersistJobDataAfterExecution] //链式传参，上一次任务的结果当作下一次任务的参数传递
    [DisallowConcurrentExecution] //如果要保证每个任务执行完，才能继续执行下一个任务，就需要用到DisallowConcurrentExecution特性
    public class Job1 : TaskModel, IJob /*,JobExecutionException*/
    {
        ILogger<Job1> _logger;

        public Job1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Job1>();
        }

        public override void Config()
        {
            status = 0;
            taskName = "测试任务a";
            groupName = "分组a";
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
            _logger.LogInformation("Job1 Run");

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string json = context.Trigger.JobDataMap.GetString("parms") ?? "";
            string run_stime = context.Trigger.JobDataMap.GetString("run_stime");
            string run_etime = context.Trigger.JobDataMap.GetString("run_etime");

            ConsoleExcept.WriteLine($"动态传参数[parms]是：{json ?? ""}", ConsoleColor.Blue);
            ConsoleExcept.WriteLine($"动态传参数[run_stime]是：{run_stime ?? ""} ", ConsoleColor.Blue);
            ConsoleExcept.WriteLine($"动态传参数[run_etime]是：{run_etime ?? ""} ", ConsoleColor.Blue);

            return Task.Run(() =>
            {
                //int p = 0;
                //var a = 12 / p;
                ConsoleExcept.WriteLine("Hello-" + DateTime.Now.ToString("HH:mm:ss"), ConsoleColor.Red);
            });
            //return Task.CompletedTask;
        }
    }
}
