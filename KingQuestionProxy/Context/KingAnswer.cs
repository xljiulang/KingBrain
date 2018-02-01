using Fiddler;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示响应的答案
    /// </summary>
    public class KingAnswer : KingResponse<KingAnswerData>
    {
        /// <summary>
        /// 从响应获得
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static KingAnswer FromSession(Session session)
        {
            return ConvertAs<KingAnswer>(session);
        }
    }

    /// <summary>
    /// 答案数据
    /// </summary>
    public class KingAnswerData
    {
        public int uid { get; set; }
        public int num { get; set; }
        public int answer { get; set; }
        public int option { get; set; }
        public bool yes { get; set; }
        public int score { get; set; }
        public int baseScore { get; set; }
        public object extraScore { get; set; }
        public int totalScore { get; set; }
        public double rowNum { get; set; }
        public double rowMult { get; set; }
        public int costTime { get; set; }
        public long roomId { get; set; }
        public int enemyScore { get; set; }
        public int enemyAnswer { get; set; }
    }
}
