using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerServer
{
    class KingWebsocketClient : WebSocketClient
    {
        public KingWebsocketClient(Uri address)
            : base(address)
        {
        }

        public void BindIpAddress(IPAddress ipAddress)
        {
            this.SendText(ipAddress.ToString());
        }

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
                Console.WriteLine();
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
