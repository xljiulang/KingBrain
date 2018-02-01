using Fiddler;
using System;
using System.Linq;
using System.Text;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示请求内容
    /// </summary>
    public class KingRequest
    {
        /// <summary>
        /// 房间id
        /// </summary>
        public string roomID { get; set; }

        /// <summary>
        /// 题号
        /// </summary>
        public string quizNum { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        /// 获取哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.roomID + this.quizNum + this.uid).GetHashCode();
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public override bool Equals(object obj)
        {
            var request = obj as KingRequest;
            return request != null && request.GetHashCode() == this.GetHashCode();
        }

        /// <summary>
        /// 从会话获得
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static KingRequest FromSession(Session session)
        {
            session.utilDecodeRequest();
            var requestBody = Encoding.UTF8.GetString(session.RequestBody);
            return Parse(requestBody);
        }

        /// <summary>
        /// roomID=4224224371&quizNum=1&uid=194682676
        /// </summary>
        /// <param name="requestBody">请求体内容</param>
        /// <returns></returns>
        private static KingRequest Parse(string requestBody)
        {
            var request = new KingRequest();

            var kvs = from item in requestBody.Split('&')
                      let kv = item.Split('=')
                      where (kv.Length == 2)
                      select new { key = kv.First(), value = kv.Last() };

            foreach (var kv in kvs)
            {
                if (kv.key.Equals("roomID", StringComparison.OrdinalIgnoreCase))
                {
                    request.roomID = kv.value;
                }
                else if (kv.key.Equals("quizNum", StringComparison.OrdinalIgnoreCase))
                {
                    request.quizNum = kv.value;
                }
                else if (kv.key.Equals("uid", StringComparison.OrdinalIgnoreCase))
                {
                    request.uid = kv.value;
                }
            }
            return request;
        }
    }
}
