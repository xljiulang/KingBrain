using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 王者的问题
    /// </summary>
    public class KingQuestion
    {
        /// <summary>
        /// 从json转换为Question
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static KingQuestion Parse(string json)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<KingQuestion>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>

        public KingQuestionData data { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 验证是否为Question
        /// </summary>
        /// <returns></returns>
        public bool IsValidate()
        {
            return this.data != null && this.errcode == 0;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.data?.ToString();
        }
    }

    /// <summary>
    /// 数据
    /// </summary>
    public class KingQuestionData
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string quiz { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public string[] options { get; set; }
        /// <summary>
        /// 题号
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string school { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        public string contributor { get; set; }
        public int endTime { get; set; }
        public int curTime { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.options != null)
            {
                var builder = new StringBuilder().AppendLine(this.quiz);
                foreach (var item in this.options)
                {
                    builder.AppendLine(item);
                }
                return builder.ToString();
            }
            return null;
        }
    }
}
