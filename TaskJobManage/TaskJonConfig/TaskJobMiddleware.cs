using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Qin.TaskJobManage.Hander;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly TaskJobConfig _taskJonConfig;

        public TaskJobMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _taskJonConfig = serviceProvider.GetRequiredService<TaskJobConfig>();
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

            if (httpContext.Request.Method.Equals("GET"))
            {
                bool ismainRoute = requestPath == _taskJonConfig.Route.Replace("/", "").ToLower();
                string staticFilePath = null;
                if (ismainRoute)
                {
                    staticFilePath = CurrentAssembly.GetManifestResourceNames().FirstOrDefault(p => p.EndsWith(".html"));
                }
                else if (requestPath.EndsWith(".js") || requestPath.EndsWith(".css") || requestPath.EndsWith(".ico"))
                {
                    staticFilePath = CurrentAssembly.GetManifestResourceNames().FirstOrDefault(p => p.EndsWith(requestPath.Replace("/", "."), StringComparison.OrdinalIgnoreCase));
                }

                if (staticFilePath != null)
                {
                    var steam = CurrentAssembly.GetManifestResourceStream(staticFilePath); //Get the static file stream first
                    var staticfile = httpContext.Request.Path.Value.Replace("/", ".");
                    var format = Path.GetExtension(staticfile);
                    var contenttype = "";
                    if (format == ".css") contenttype = "text/css; charset=utf-8";
                    else if (format == ".js") contenttype = "application/javascript; charset=utf-8";
                    else if (format == ".html") contenttype = "text/html; charset=utf-8";
                    httpContext.Response.ContentType = contenttype;
                    await steam.CopyToAsync(httpContext.Response.Body).ConfigureAwait(false);
                    return;
                }
            }

            if (httpContext.Request.Headers.TryGetValue("taskjob", out _))
            {
                if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) && httpContext.Request.Path.Value.Contains("TaskJob-"))
                {
                    var hander = new TaskJobManageHander(_serviceProvider);
                    string inputjson = await GetParms(httpContext);
                    var handerresult = await hander.ExecHander(httpContext.Request.Path, inputjson);
                    await WriteAsync(httpContext, handerresult);
                    return;
                }
                else await _next(httpContext);
            }
            else await _next(httpContext);
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
