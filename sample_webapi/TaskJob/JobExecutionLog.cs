using Qin.TaskJobManage;
using Qin.TaskJobManage.Hander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWebApi.TaskJob
{
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
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务b",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务c",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务d",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务e",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务f",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务g",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });

            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务h",
                groupName = "分组b",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务i",
                groupName = "分组a",
                describe = "每16秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务j",
                groupName = "分组a",
                describe = "每126秒执行一次",
                cron = "0/10 * * * * ? "
            });
            list.Add(new TaskModel()
            {
                status = 0,
                taskName = "测试任务k",
                groupName = "分3组a",
                describe = "每16秒执行一次",
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
}
