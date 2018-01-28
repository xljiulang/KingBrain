using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class KingAnswer
    {
        /// <summary>
        /// 从json转换为Answer
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static KingAnswer Parse(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<KingAnswer>(json);
        }

        public KingAnswerData data { get; set; }
        public int errcode { get; set; }
    }

    public class KingAnswerData
    {
        public int uid { get; set; }
        public int num { get; set; }
        public int answer { get; set; }
        public int option { get; set; }
        public bool yes { get; set; }
        public int score { get; set; }
        public int baseScore { get; set; }
        public Extrascore extraScore { get; set; }
        public int totalScore { get; set; }
        public double rowNum { get; set; }
        public double rowMult { get; set; }
        public int costTime { get; set; }
        public long roomId { get; set; }
        public int enemyScore { get; set; }
        public int enemyAnswer { get; set; }
    }

    public class Extrascore
    {
    }
}
