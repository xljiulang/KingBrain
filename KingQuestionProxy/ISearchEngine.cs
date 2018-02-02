using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public interface ISearchEngine
    {
        ISearchEngine Next { set; }

        BestOption Search(KingQuestion kingQuestion);
    }
}
