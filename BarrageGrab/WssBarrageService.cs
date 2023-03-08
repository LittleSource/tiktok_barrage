using System;
using System.Net.WebSockets;
using System.Text;
using BarrageGrab.JsonEntity;
using ColorConsole;
using Newtonsoft.Json;
using System.Threading;

namespace BarrageGrab
{
    /// <summary>
    /// 弹幕服务
    /// </summary>
    internal class WssBarrageService
    {
        ClientWebSocket websocketClient;
        ConsoleWriter console = new ConsoleWriter();
        WssBarrageGrab grab = new WssBarrageGrab();
        Appsetting Appsetting = Appsetting.Get();

        public WssBarrageService()
        {
            this.websocketClient = new ClientWebSocket();

            this.websocketClient.ConnectAsync(new Uri($"ws://127.0.0.1:{Appsetting.WsProt}"), CancellationToken.None);
            this.grab.OnChatMessage += Grab_OnChatMessage;
        }

        private MsgUser GetUser(dynamic obj)
        {
            MsgUser user = new MsgUser()
            {
                DisplayId = obj.User.displayId,
                Gender = obj.User.Gender,
                Id = obj.User.Id,
                Level = obj.User.Level,
                Nickname = obj.User.Nickname
            };
            return user;
        }

        private void Grab_OnChatMessage(object sender, ProtoEntity.ChatMessage e)
        {
            var enty = new Msg()
            {
                Content = e.Content,
                RoomId = e.Common.roomId,
                User = GetUser(e)
            };
            Print($"[弹幕消息] {enty.User.GenderToString()}  {enty.User.Nickname}: {enty.Content}", ConsoleColor.Green);
            var json = JsonConvert.SerializeObject(new SocketMessage(enty.User.Nickname, enty.Content));
            this.Broadcast(json);
        }

        private void Print(string msg, ConsoleColor color)
        {
            if (Appsetting.PrintBarrage)
            {
                console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} " + msg + "\n", color);
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        public void Broadcast(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            this.websocketClient.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void StartListen()
        {
            this.grab.Start(); //启动代理
            console.WriteLine($"{this.websocketClient.State} 弹幕服务已启动...", ConsoleColor.Green);
            Console.Title = $"抖音弹幕监听推送 [{this.websocketClient.SubProtocol}]";
        }

        /// <summary>
        /// 关闭服务器连接
        /// </summary>
        public void Close()
        {
            websocketClient.Dispose();
            grab.Dispose();
            console.WriteLine("服务器已关闭...");
        }
    }


    class SocketMessage
    {
        public string nickname;

        public string content;

        public SocketMessage(string nickname, string content)
        {
            this.content = content;
            this.nickname = nickname;
        }
    }
}
