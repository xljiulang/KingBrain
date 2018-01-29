using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    /// <summary>
    /// 表示ws客户端
    /// </summary>
    class KingClient : WebSocketClient
    {
        /// <summary>
        /// 是否自动游戏
        /// </summary>
        private readonly bool autoContinue = ConfigurationManager.AppSettings["autoContinue"] == "true";

        /// <summary>
        /// ws客户端
        /// </summary>
        /// <param name="address"></param>
        public KingClient(Uri address)
            : base(address)
        {
        }

        /// <summary>
        /// 绑定手机的ip地址
        /// </summary>
        /// <param name="ipAddress">手机的ip地址</param>
        public void BindIpAddress(IPAddress ipAddress)
        {
            this.SendText(ipAddress.ToString());
        }

        /// <summary>
        /// 收到服务端的数据
        /// </summary>
        /// <param name="frame"></param>
        protected override async void OnText(FrameRequest frame)
        {
            var json = Encoding.UTF8.GetString(frame.Content);
            var notify = JsonConvert.DeserializeObject<WsNotifyData<WsGameAnswer>>(json);

            if (notify.Cmd == WsCmd.GameAnser)
            {
                var searchResult = notify.Data.SearchResult;
                Console.WriteLine(searchResult);

                if (searchResult.Best != null)
                {
                    var index = searchResult.Best.Index;
                    var delay = TimeSpan.FromMilliseconds(notify.Data.GameDelayMSeconds);
                    await this.AutotapOptionsAsync(index, delay, TimeSpan.FromSeconds(0.5d));
                }
                else
                {
                    var index = new Random().Next(0, 3);
                    var delay = TimeSpan.FromMilliseconds(notify.Data.GameDelayMSeconds + 4000);
                    await this.AutotapOptionsAsync(index, delay);
                }
                Console.WriteLine();
            }
            else if (notify.Cmd == WsCmd.GameOver && this.autoContinue == true)
            {
                await this.AutotapOptionsAsync(2, TimeSpan.FromSeconds(6d));
                await this.AutotapOptionsAsync(4, TimeSpan.FromSeconds(4d));
            }
        }

        /// <summary>
        /// 自动点击     
        /// </summary>
        /// <param name="optIndex">选项索引</param>
        /// <param name="delay">延时</param>
        /// <param name="delays">再点击延时</param>
        /// <returns></returns>
        private async Task AutotapOptionsAsync(int optIndex, TimeSpan delay, params TimeSpan[] delays)
        {
            var size = await adb.GetSizeAsync();
            if (size.IsEmpty == true)
            {
                return;
            }

            var x = size.Width / 2f;
            var uY = size.Height * 0.47f;
            var optionH = size.Height * 0.53f / 5f;
            var y = uY + optionH * optIndex + optionH / 2f;

            if (delay > TimeSpan.Zero)
            {
                Console.WriteLine("{0}秒后自动模拟点击...", delay.TotalSeconds.ToString("0.0"));
                await Task.Delay(delay);
            }
            await adb.KeyeventAsync((int)x, (int)y);

            foreach (var item in delays)
            {
                await Task.Delay(item);
                await adb.KeyeventAsync((int)x, (int)y);
            }
        }
    }
}
