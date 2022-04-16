using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qin.TaskJobManage
{
    public abstract class TaskJobAbstract
    {
        public virtual void Config()
        {
            throw new NotImplementedException("作业未配置");
        }

        public virtual TriggerBuilder ConfigTrigger(TriggerBuilder triggerbuilder) => triggerbuilder;
    }
}
