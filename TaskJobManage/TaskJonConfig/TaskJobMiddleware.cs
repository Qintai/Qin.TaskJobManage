using Microsoft.AspNetCore.Http;
using Qin.TaskJobManage.Hander;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Qin.TaskJobManage
{
    internal class TaskJobMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly TaskJobConfig _taskJonConfig;

        public TaskJobMiddleware(RequestDelegate next, ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider, TaskJobConfig taskJonConfig)
        {
            _next = next;
            _schedulerFactory = schedulerFactory;
            _serviceProvider = serviceProvider;
            _taskJonConfig = taskJonConfig;
        }

        private async Task<string> GetParms(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            using var reader = new StreamReader(httpContext.Request.Body);
            var body = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;
            return body ?? string.Empty;
        }

        private static System.Reflection.Assembly CurrentAssembly => System.Reflection.Assembly.GetExecutingAssembly();

        public async Task Invoke(HttpContext httpContext)
        {
            var requestPath = httpContext.Request.Path.Value.Trim('/').ToLower();
            bool ismainRoute = requestPath == _taskJonConfig.Route.Replace("/", "").ToLower();

            if (httpContext.Request.Method.Equals("GET") && (ismainRoute || httpContext.Request.Path.Value.Contains("TaskJobUI-static")))
            {
                var names = CurrentAssembly.GetManifestResourceNames();

                // Find static files
                var staticfile = httpContext.Request.Path.Value.Replace("/", ".").Replace(".TaskJobUI-static.", "");
                var staticitem = names.FirstOrDefault(a =>
                {
                    //var name = a.Replace($"{nameof(Qin.TaskJobManage)}.view.TaskJobUI_static.", "").Replace("/", ".");
                    var name = a.Replace($"Qin.TaskJobManage.view.TaskJobUI_static.", "").Replace("/", ".");
                    return name == staticfile;
                });
                // Find HTML
                if (staticitem == null && ismainRoute)
                {
                    staticitem = names.FirstOrDefault(a =>
                    {
                        var name = a.Replace($"{nameof(Qin.TaskJobManage)}.view", "").Replace("/", ".");
                        return name.IndexOf("html") > 0;
                    });
                }
                if (staticitem != null)
                {
                    var steam = CurrentAssembly.GetManifestResourceStream(staticitem); //Get the static file stream first
                    var format = Path.GetExtension(staticfile);
                    var contenttype = "";
                    if (format == ".css") contenttype = "text/css; charset=utf-8";
                    else if (format == ".js") contenttype = "application/javascript; charset=utf-8";
                    else if (format == ".html") contenttype = "text/html; charset=utf-8";
                    httpContext.Response.ContentType = contenttype;
                    await steam.CopyToAsync(httpContext.Response.Body).ConfigureAwait(false);
                }
                return;
            }

            if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) && httpContext.Request.Path.Value.Contains("TaskJob-"))
                {
                var hander = new TaskJobManageHander(_serviceProvider);
                string inputjson = await GetParms(httpContext);
                var handerresult = await hander.ExecHander(httpContext.Request.Path, inputjson);
                await WriteAsync(httpContext, handerresult);
                return;
            }
            else  await _next(httpContext);
        }

        private async Task WriteAsync(HttpContext httpContext, object obj, string contentType = "application/json")
        {
            var json = JsonSerializer.Serialize(obj);
            var byte1 = Encoding.UTF8.GetBytes(json);
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.Body.WriteAsync(byte1);
        }
    }
}
