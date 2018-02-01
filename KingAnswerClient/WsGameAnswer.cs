using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    /// <summary>
    /// 表示游戏答案
    /// </summary>
    public class WsGameAnswer
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 提问
        /// </summary>
        public string Quiz { get; set; }

        /// <summary>
        /// 选项
        /// </summary>
        public string[] Options { get; set; }

        /// <summary>
        /// 游戏可以点击的延时毫秒数
        /// </summary>
        public int DelayMilliseconds { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(this.Quiz);

            var index = 0;
            foreach (var opt in this.Options)
            {
                builder.AppendLine($"{++index}、{opt}");
            }

            if (this.Index >= 0)
            {
                builder.AppendLine($"参考答案：{this.Options[this.Index]}");
            }
            else
            {
                builder.AppendLine("找不到答案");
            }
            return builder.ToString();
        }
    }
}
