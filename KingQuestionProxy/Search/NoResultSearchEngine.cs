using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    class NoResultSearchEngine : SearchEngineBase
    {
        public override BestOption Search(KingQuestion kingQuestion)
        {
            return null;
        }

        protected override string[] SearchSourceAnswers(string question)
        {
            return new string[0];
        }
    }
}
