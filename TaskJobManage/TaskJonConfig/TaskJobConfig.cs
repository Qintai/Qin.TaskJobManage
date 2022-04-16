using Microsoft.Extensions.DependencyInjection;
using Qin.TaskJobManage.Hander;
using System;
using System.Collections.Generic;

namespace Qin.TaskJobManage
{
    public class TaskJobConfig
    {
        private readonly IServiceCollection _services;
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        public TaskJobConfig(IServiceCollection services)
        {
            _services = services;
        }
         
        /// <summary>
        /// UI 根目录
        /// </summary>
        public string Route { get; set; } = "/TaskJobUI";

        /// <summary>
        /// 实现类所在程序集名,为空则读取当前程序集
        /// </summary>
        public string LoadAssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// 自启动
        /// </summary>
        public bool SelfTurnOn { get; set; } = true;

        public TaskJobConfig AddJobExecutionLog<JobLog>() where JobLog : class, IJobExecutionLogs
        {
            _services.AddSingleton<IJobExecutionLogs, JobLog>();
            return this;
        }
    }
}
