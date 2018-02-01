using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    class Logger : TextWriter
    {
        private readonly TextWriter[] writer;

        public override Encoding Encoding => Encoding.UTF8;

        public Logger(params TextWriter[] writer)
        {
            this.writer = writer;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            foreach (var item in this.writer)
            {
                item.Write(buffer, index, count);
            }
        }
    }

    class DebugWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char[] buffer, int index, int count)
        {
            var log = new string(buffer, index, count);
            System.Diagnostics.Debugger.Log(0, null, log);
        }
    }
}
