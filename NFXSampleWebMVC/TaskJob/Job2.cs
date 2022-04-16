using Quartz;
using System;
using System.Threading.Tasks; 

namespace NFXSampleWebMVC
{
    public class Job2 : TaskModel,IJob
    {
        public override void Config()
        {
            status = 0;
            taskName = "NFXSampleWebMVC测试任务b";
            groupName = "分组b";
            describe = "每7秒执行一次";
            cron = "0/7 * * * * ? ";
            Job = typeof(Job2);
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                ConsoleExcept.WriteLine("你好-" + DateTime.Now.ToString("HH:mm:ss"), ConsoleColor.Yellow);
            });
        }
    }
}
