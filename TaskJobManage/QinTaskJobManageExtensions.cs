using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Qin.TaskJobManage;
using Qin.TaskJobManage.Hander;
using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QinTaskJobManageExtensions
    {
        public static IApplicationBuilder UseTaskJob(this IApplicationBuilder builder)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "view");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            builder.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(path),
                // RequestPath = ""
            });
            return builder.UseMiddleware<TaskJobMiddleware>();
        }
         
        /// <summary>
        /// 自启
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void StartTaskJob(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var taskJonConfig = serviceProvider.GetRequiredService<TaskJobConfig>();
            if (taskJonConfig.SelfTurnOn)
            {
                var hander = new TaskJobManageHander(serviceProvider);
                hander.ExecHander("/taskjob-startup", "");
            }
        }

        public static IServiceCollection AddTaskJob(this IServiceCollection services)
        {
            services.AddTaskJob(options => { });
            return services;
        }

        public static IServiceCollection AddTaskJob(this IServiceCollection services, Action<TaskJobConfig> config)
        {
            //services.AddSingleton<IPathProvider, PathProvider>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<Quartz.Spi.IJobFactory, CostomJobFactory>();

            TaskJobConfig configModel = new TaskJobConfig(services);
            config(configModel);

            var IJobtype = typeof(IJob);
            var TaskModeltype = typeof(TaskModel);
            var typeList = CurrentTypes(configModel.LoadAssemblyName)
                .Where(t => t.GetInterfaces().Contains(IJobtype) && t.IsSubclassOf(TaskModeltype))
                .ToList();

            foreach (var item in typeList)
            {
                services.AddSingleton(TaskModeltype, item); //TaskJobManageHander -> Startup 批量扫描并执行
                services.AddSingleton(item); // CostomJobFactory -> bundle.JobDetail.JobType 使用DI构造Job，不使用Quartz直接去new
            }
      
            services.AddSingleton(configModel);
            return services;
        }

        private static Type[] CurrentTypes(string loadAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(loadAssemblyName))
                return Assembly.GetEntryAssembly().GetTypes();
            else
            {
                var servicesDllFile = Path.Combine(AppContext.BaseDirectory, loadAssemblyName + ".dll");
                if (File.Exists(servicesDllFile))
                {
                    var assemblysServices = Assembly.LoadFrom(servicesDllFile);
                    if (assemblysServices != null)
                    {
                        return assemblysServices.GetTypes();
                    }
                }
                return Enumerable.Empty<Type>().ToArray();
            }
        }
    }
}
