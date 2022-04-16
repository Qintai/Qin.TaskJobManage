using System.Web.Mvc;
using System.Web.Routing;

namespace NFXSampleWebMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        { 
            AreaRegistration.RegisterAllAreas();
            RegisterTaskJobUIRoutes(RouteTable.Routes);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
       
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    HttpApplication objApp = (HttpApplication)sender;
        //    if (objApp.Context.Request.HttpMethod != "GET")
        //    {
        //        return;
        //    }
        //    if (objApp.Context.Request.Path == "TaskJobUI" || objApp.Context.Request.Path.Contains("TaskJobUI-static"))
        //    {
        //        new Htmlhandler().ProcessRequest(objApp.Context);
        //    }
        //}

        public void RegisterTaskJobUIRoutes(RouteCollection routes)
        { 
            var a1 =  System.Reflection.Assembly.GetEntryAssembly();

            routes.Add("dhtml_a", new Route("TaskJobUI", new HtmlRoutehander()));
            // 因为末尾有小数点，路由会优先匹配 UrlRoutingModule，请配置 将preCondition设置为空
            routes.Add("dhtml_b", new Route("TaskJobUI-static/{*.a}", new HtmlRoutehander()));
            routes.Add("dhtml_c", new Route("TaskJob-{ppp}", new TaskJobActionRoutehander()));
            routes.RouteExistingFiles = true;
            //RouteTable.Routes.IgnoreRoute("TaskJobUI-static/{*pathInfo}");
        }
    }
}
