using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing; 

namespace NFXSampleWebMVC
{
    #region 定时任务UI路由 
    public class HtmlRoutehander : IRouteHandler
    {
        public bool IsReusable => true;

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Htmlhandler();
        }
    }

    public class Htmlhandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html; Language=UTF-8";
            string filename = context.Request.Url.AbsolutePath.Replace("/", "");
            var tooken = Guid.NewGuid();
            // 如果在同一个网站中，可以给 Response 写个 Cookie ，把 tooken 的值写到Cookies中
            // 每当页面刷新时，也就是新一轮请求来临时，重写 Cookie 的值，页面刷新一次 Cookies 的值更改一次，不刷新Cookies 永久存在
            // 当静态页面有接口访问网站时，验证Request的Cookie值是否对的上

            if (context.Request.HttpMethod.Equals("GET") && (context.Request.Path == "/TaskJobUI"
               || context.Request.Path.Contains("TaskJobUI-static"))) // TaskJob-UI
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var names = assembly.GetManifestResourceNames();

                var pp = context.Request.Path.Replace("/", ".").Replace(".TaskJobUI-static.", "");
                var staticitem = names.FirstOrDefault(a =>
                {
                    var name = a.Replace("NFXSampleWebMVC.view.TaskJobUI_static.", "").Replace("/", ".");
                    return name == pp;
                });
                if (staticitem == null && context.Request.Path == "/TaskJobUI")
                {
                    staticitem = names.FirstOrDefault(a =>
                    {
                        var name = a.Replace("NFXSampleWebMVC.view", "").Replace("/", ".");
                        return name.IndexOf("html") > 0;
                    });
                }
                if (staticitem != null)
                {
                    var steam = assembly.GetManifestResourceStream(staticitem); //示例 这样就能拿到每个文件的流
                    var format = Path.GetExtension(pp);
                    var contenttype = "";
                    if (format == ".css") contenttype = "text/css; charset=utf-8";
                    else if (format == ".js") contenttype = "application/javascript; charset=utf-8";
                    else if (format == ".html") contenttype = "text/html; charset=utf-8";
                    context.Response.ContentType = contenttype;
                    steam.CopyTo(context.Response.OutputStream);
                }

                if (context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    //  var hander = new TaskJobManageHander(_schedulerFactory);
                    //  TaskModel input = await GetParms(context);
                    //  var handerresult = await hander.ExecHander(context.Request.Path, input);
                    //  if (handerresult != null)
                    //  {
                    //      await WriteAsync(context, handerresult);
                    //      return;
                    //  }
                    //  await _next(context);
                }

                //  string filepath = context.Server.MapPath($"~/view/index.html");
                //  if (!File.Exists(filepath))
                //      context.Response.Write("没有文件Y！");
                //  else
                //  {
                //      Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //      //stream.CopyToAsync(context.Response.OutputStream).ConfigureAwait(false);
                //      stream.CopyTo(context.Response.OutputStream);
                //
                //      //context.Response.Write(File.ReadAllText(filepath));
                //  }
            }

        }
    }
#endregion
    
    public class TaskJobActionRoutehander : IRouteHandler
    {
        public bool IsReusable => true;
        public IHttpHandler GetHttpHandler(RequestContext requestContext) => new TaskJobActionhandler();
    }
    public class TaskJobActionhandler : IHttpHandler
    {
        public bool IsReusable => true;
        public static ISchedulerFactory _schedulerFactory;

        static TaskJobActionhandler()
        {
            _schedulerFactory = new StdSchedulerFactory();
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html; Language=UTF-8";
            string filename = context.Request.Url.AbsolutePath.Replace("/", "");
            var tooken = Guid.NewGuid();
            if (context.Request.HttpMethod.Equals("POST") && context.Request.Path.Contains("TaskJob-")) // 接口方法
            {
                var hander = new TaskJobManageHander(_schedulerFactory);
                TaskModel input =  GetParms(context);
                var handerresult = hander.ExecHander(context.Request.Path, input).Result;
                if (handerresult != null)
                    context.Response.Write(JsonConvert.SerializeObject(handerresult));

               // 这里使用 task 新开线程会导致，对象 null 引用
               // Task.Run(async () =>
               // { 
               //     // 这里本要引用 TaskJobManage ，.net standard 2.1 不能被 ,netframework 4.8 引用，但是兼容失败 可能的原因是：Standard 库引用的其他库，不太支持 netframework 4.8
               //     var hander = new TaskJobManageHander(_schedulerFactory);
               //     TaskModel input = await GetParms(context);
               //     var handerresult = await hander.ExecHander(context.Request.Path, input);
               //     if (handerresult != null)
               //     {
               //         context.Response.Write(JsonConvert.SerializeObject(handerresult));
               //         await WriteAsync(context, handerresult);
               //         //return;
               //     }
               // }, new System.Threading.CancellationToken());
            }
        }

        private async Task WriteAsync(HttpContext httpContext, object obj, string contentType = "application/json")
        {
            var json = JsonConvert.SerializeObject /*JsonSerializer.Serialize*/(obj/*,new JsonSerializerOptions() {DefaultIgnoreCondition }*/);
            var byte1 = Encoding.UTF8.GetBytes(json);
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.OutputStream.WriteAsync(byte1, 0, byte1.Length);
            httpContext.Response.End();
        }

        private TaskModel GetParms(HttpContext httpContext)
        {
            //httpContext.Request.EnableBuffering();
            using (var reader = new StreamReader(httpContext.Request.InputStream))
            {
                var body = reader.ReadToEnd();
                if (body == null)
                {
                    return null;
                }
                httpContext.Request.InputStream.Position = 0;
                TaskModel model = JsonConvert.DeserializeObject /*JsonSerializer.Deserialize*/<TaskModel>(body);
                return model;
            };
        }

        private async Task<TaskModel> GetParmsAsync(HttpContext httpContext)
        {
            //httpContext.Request.EnableBuffering();
            using (var reader = new StreamReader(httpContext.Request.InputStream))
            {
                var body = await reader.ReadToEndAsync();
                if (body == null)
                {
                    return null;
                }
                httpContext.Request.InputStream.Position = 0;
                TaskModel model = JsonConvert.DeserializeObject /*JsonSerializer.Deserialize*/<TaskModel>(body);
                return model;
            };
        }
    }
}



