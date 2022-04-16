using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Qin.TaskJobManage
{
    public class ResultMsg
    {
        /// <summary>
        /// 执行操作是否成功
        /// </summary>
        public bool status { get; set; }

        public string msg { get; set; }

        public object data { get; set; }

        public static ResultMsg Ok(string msg = "", List<TaskModel> taskModels = null)
        {
            return new ResultMsg()
            {
                status = true,
                msg = msg,
                data = taskModels
            };
        }

        public static ResultMsg Fail(string failmsg)
        {
            return new ResultMsg() { status = false, msg = failmsg };
        }
    }

    public class TableList<T>
    {
        public IEnumerable<T>? TableData { get; set; }
        public int Count { get; set; }
    }

}
