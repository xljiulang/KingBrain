using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    /// <summary>
    /// 表示ws指令
    /// </summary>
    public enum WsCmd
    {
        /// <summary>
        /// 游戏答案
        /// </summary>
        GameAnser = 0,

        /// <summary>
        /// 游戏结束
        /// </summary>
        GameOver = 1
    }
}
