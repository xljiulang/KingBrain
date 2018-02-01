using Fiddler;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示响应的问题
    /// </summary>
    public class KingQuestion : KingResponse<KingQuestionData>
    {
        /// <summary>
        /// 从响应获得
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static KingQuestion FromSession(Session session)
        {
            return ConvertAs<KingQuestion>(session);
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
        /// <summary>
        /// 贡献者
        /// </summary>
        public string contributor { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public int endTime { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public int curTime { get; set; }
    }
}
