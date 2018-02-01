using Fiddler;
using Newtonsoft.Json;
using System.Text;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示响应内容
    /// </summary>
    public abstract class KingResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 从sesion响应转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static T ConvertAs<T>(Session session)
        {
            session.utilDecodeResponse();
            var json = Encoding.UTF8.GetString(session.ResponseBody);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    /// <summary>
    /// 表示响应内容
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class KingResponse<TData> : KingResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public TData data { get; set; }

        /// <summary>
        /// 验证是否为Question
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValidate()
        {
            return this.data != null && this.errcode == 0;
        }
    }
}
