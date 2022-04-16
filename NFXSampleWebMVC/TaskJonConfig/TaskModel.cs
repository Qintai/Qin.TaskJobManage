using Quartz;
using Quartz.Impl.Triggers;
using System;

namespace NFXSampleWebMVC
{
    public class TaskModel
    {
        /// <summary>
        /// 0：正常 1=暂停 2：完成 3：错误 4：堵塞 5：没有
        /// <see cref="TriggerState"/>
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string taskName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string groupName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// cron表达式
        /// </summary>
        public string cron { get; set; }
        private DateTime _LastRunTime = DateTime.Now;

        /// <summary>
        /// 最后一次执行时间
        /// </summary>
        public string lastRunTime { get => _LastRunTime.ToString("yyyy-MM-dd HH:mm:ss"); }

        public void SetLastRunTime(DateTime a) { this._LastRunTime = a; }

        /// <summary>
        /// 作业操作
        /// </summary>
        public Type Job;

        /// <summary>
        /// 动态参数
        /// </summary>
        public string DynamicData { get; set; }

        public virtual void Config()
        {
            throw new NotImplementedException("子类未重写此方法");
        }

        public virtual TriggerBuilder ConfigTrigger(TriggerBuilder triggerbuilder) => triggerbuilder;

        /// <summary>
        /// 验证Cron表达式
        /// </summary>
        /// <returns></returns>
        public void Validation()
        {
            try
            {
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cron;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            //throw new ArgumentException(date == null ? $"请确认表达式{cron}是否正确!" : "");
        }
    }
}
