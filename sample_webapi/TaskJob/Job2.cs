using Quartz;
using System;
using System.Threading.Tasks;
using Qin.TaskJobManage;
using Microsoft.Extensions.Logging;

namespace SampleWebApi
{
    public class Job2 : TaskModel,IJob
    {
        ILogger<Job2> _logger;

        public Job2(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Job2>();
        }

        public override void Config()
        {
            status = 0;
            taskName = "测试任务b";
            groupName = "分组b";
            describe = "每7秒执行一次";
            cron = "0/7 * * * * ? ";
            Job = typeof(Job2);
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job2 Run");

            return Task.Run(() =>
            {
                ConsoleExcept.WriteLine("你好-" + DateTime.Now.ToString("HH:mm:ss"), ConsoleColor.Yellow);
            });
        }
    }
}
