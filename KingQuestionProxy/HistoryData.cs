using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class HistoryData
    {
        public KingRequest KingRequest { get; set; }

        public KingQuestionData QuestionData { get; set; }

        public SearchResult SearchResult { get; set; }
    }
}
