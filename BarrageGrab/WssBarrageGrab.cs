using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using BarrageGrab.ProtoEntity;
using ProtoBuf;
using ColorConsole;
using BarrageGrab.Proxy;

namespace BarrageGrab
{
    /// <summary>
    /// 本机Wss弹幕抓取器
    /// </summary>
    public class WssBarrageGrab : IDisposable
    {
        //ISystemProxy proxy = new FiddlerProxy();
        ISystemProxy proxy = new TitaniumProxy();
        Appsetting appsetting = Appsetting.Get();
        ConsoleWriter console = new ConsoleWriter();

        /// <summary>
        /// 聊天
        /// </summary>
        public event EventHandler<ChatMessage> OnChatMessage;


        public WssBarrageGrab()
        {
            proxy.OnWebSocketData += Proxy_OnWebSocketData;
        }

        public void Start()
        {
            proxy.Start();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }

        //gzip解压缩
        private byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

        private void Proxy_OnWebSocketData(object sender, WsMessageEventArgs e)
        {
            if (!appsetting.FilterProcess.Contains(e.ProcessName)) return;
            var buff = e.Payload;
            if (buff.Length == 0) return;
            if (buff[0] != 0x08) return;


            try
            {
                var enty = Serializer.Deserialize<WssResponse>(new ReadOnlyMemory<byte>(buff));
                if (enty == null) return;

                //检测包格式
                if (!enty.Headers.Any(a => a.Key == "compress_type" && a.Value == "gzip")) return;


                //解压gzip
                var odata = enty.Payload;
                var decomp = Decompress(odata);

                var response = Serializer.Deserialize<Response>(new ReadOnlyMemory<byte>(decomp));
                response.Messages.ForEach(f => DoMessage(f));
            }
            catch (Exception) { }
        }

        private void DoMessage(Message msg)
        {
            try
            {
                if (msg.Method == "WebcastChatMessage")
                {
                    var arg = Serializer.Deserialize<ChatMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                    this.OnChatMessage?.Invoke(this, arg);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
