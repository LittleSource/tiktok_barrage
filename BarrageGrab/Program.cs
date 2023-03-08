﻿using System;
using System.Runtime.InteropServices;
namespace BarrageGrab
{
    public class Program
    {
        private delegate bool ControlCtrlDelegate(int CtrlType);

        static WssBarrageService server = null;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);//捕获控制台关闭
            Console.Title = "抖音弹幕监听推送";
            server = new WssBarrageService();
            server.StartListen();

            while (true)
            {
                var input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.Escape: goto end; break;
                }
            }

        end:
            server.Close();
            Console.WriteLine("服务器已正常关闭，按任意键结束...");
            Console.ReadKey();
        }


        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    server.Close();
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭
                    server.Close();
                    break;
            }
            return false;
        }
    }
}
