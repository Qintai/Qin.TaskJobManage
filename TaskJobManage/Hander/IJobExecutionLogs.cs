using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qin.TaskJobManage.Hander
{
    public interface IJobExecutionLogs
    {
        public Task<ResultMsg> GetJobExecutionLog(TaskModel parms, int pageIndex = 1, int pageSize = 10);
    }
}
