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
        /// 搜索结果
        /// </summary>
        public SearchResult SearchResult { get; set; }

        /// <summary>
        /// 游戏可以点击的延时毫秒数
        /// </summary>
        public int GameDelayMSeconds { get; set; }
    }
}
