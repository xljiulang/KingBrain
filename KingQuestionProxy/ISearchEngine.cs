using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 定义搜索引擎的接口
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// 获取下一个引擎
        /// </summary>
        ISearchEngine Next { set; }

        /// <summary>
        /// 获取或设置匹配模式
        /// </summary>
        MatchMode MatchMode { get; set; }

        /// <summary>
        /// 搜索最佳选项
        /// 搜索不到找回null
        /// </summary>
        /// <param name="kingQuestion">问题</param>   
        /// <returns></returns>
        BestOption Search(KingQuestion kingQuestion);
    }

    /// <summary>
    /// 匹配方式
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// 精确
        /// </summary>
        Accurate = 0,

        /// <summary>
        /// 模糊
        /// </summary>
        Fuzzy = 1,
    }
}
