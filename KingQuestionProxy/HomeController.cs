using NetworkSocket.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class HomeController : NetworkSocket.Http.HttpController
    {
        [Route("/")]
        public ActionResult Index()
        {
            var html = System.IO.File.ReadAllText("index.html", Encoding.UTF8);
            return Content(html);
        }
    }
}
