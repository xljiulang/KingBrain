using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示游戏答案
    /// </summary>
    public class WsGameAnswer
    {
        /// <summary>
        /// 最佳选项的索引
        /// -1表示无答案
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 提问内容
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
    }
}
