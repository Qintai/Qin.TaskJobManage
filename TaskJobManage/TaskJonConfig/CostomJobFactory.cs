using Quartz;
using Quartz.Spi;
using System;

namespace Qin.TaskJobManage
{
    /// <summary>
    /// 构造 job 使用 DI容器去构造
    /// </summary>
    public class CostomJobFactory: IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public CostomJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
