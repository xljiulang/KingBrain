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
    /// <summary>
    /// 提供adb的包装
    /// </summary>
    public static class adb
    {
        private static readonly string adbFile = ConfigurationManager.AppSettings["adb"];

        /// <summary>
        /// 获取是否支持
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                return File.Exists(adbFile);
            }
        }

        /// <summary>
        /// 获取游戏画面大小
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 模拟点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static async Task KeyeventAsync(int x, int y)
        {
            var cmd = $"shell input tap {x} {y}";
            await adb.ExecAsync(cmd);
        }

        /// <summary>
        /// 执行shell
        /// </summary>
        /// <param name="arg">参数</param>
        /// <returns></returns>
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
