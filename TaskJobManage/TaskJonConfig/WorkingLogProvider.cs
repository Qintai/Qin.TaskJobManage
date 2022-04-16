using Quartz.Logging;
using System;
using System.Linq;
using LogLevel = Quartz.Logging.LogLevel;

namespace Qin.TaskJobManage
{
    public class WorkingLogProvider : ILogProvider
    {
        public virtual Logger GetLogger(string name)
        {
            return new Logger((level, func, exception, parameters) =>
            {
                if (exception != null)
                {
                    Console.WriteLine("异常了" + exception.Message);
                }
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine($"[{ DateTime.Now.ToLongTimeString()}] [{ level}] { func()} {string.Join(";", parameters.Select(p => p == null ? " " : p.ToString()))}  自定义日志{name}");
                }
                return true;
            });
        }

        public virtual IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public virtual IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }
    }
}
