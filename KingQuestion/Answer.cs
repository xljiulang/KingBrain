using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestion
{
    public class Answer
    {
        /// <summary>
        /// 从json转换为Answer
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Answer Parse(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Answer>(json);
        }

        public AnswerData data { get; set; }
        public int errcode { get; set; }
    }

    public class AnswerData
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
        public int rowNum { get; set; }
        public int rowMult { get; set; }
        public int costTime { get; set; }
        public long roomId { get; set; }
        public int enemyScore { get; set; }
        public int enemyAnswer { get; set; }
    }

    public class Extrascore
    {
    }
}
