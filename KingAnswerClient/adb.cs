using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    public static class adb
    {
        private static readonly string adbFile = ConfigurationManager.AppSettings["adb"];

        public static readonly bool IsSupported = File.Exists(adbFile);

        public async static Task<SizeF> GetSizeAsync()
        {
            var cmd = "shell dumpsys window displays";
            var str = await adb.ExecAsync(cmd);

            var match = Regex.Match(str, @"app=(?<w>\d+)x(?<h>\d+)");
            if (match.Success == true)
            {
                var w = int.Parse(match.Groups["w"].Value);
                var h = int.Parse(match.Groups["h"].Value);
                return new SizeF(w, h);
            }
            return SizeF.Empty;
        }

        public static async Task KeyeventAsync(int x, int y)
        {
            var cmd = $"shell input tap {x} {y}";
            await adb.ExecAsync(cmd);
        }

        private static async Task<string> ExecAsync(string arg)
        {
            if (IsSupported == false)
            {
                return string.Empty;
            }

            var info = new ProcessStartInfo
            {
                FileName = adbFile,
                Arguments = arg,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            return await Process.Start(info).StandardOutput.ReadToEndAsync();
        }
    }
}
