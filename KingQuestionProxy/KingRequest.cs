using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class KingRequest
    {
        public string roomID { get; set; }

        public string quizNum { get; set; }

        public string uid { get; set; }

        public override int GetHashCode()
        {
            return (this.roomID + this.quizNum + this.uid).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var request = obj as KingRequest;
            return request != null && request.GetHashCode() == this.GetHashCode();
        }

        /// <summary>
        /// roomID=4224224371&quizNum=1&uid=194682676
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static KingRequest Parse(string requestBody)
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
