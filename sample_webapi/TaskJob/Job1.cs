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
}
