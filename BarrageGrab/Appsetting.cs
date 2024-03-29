﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Configuration.ConfigurationManager;
namespace BarrageGrab
{
    internal class Appsetting
    {
        private static readonly Appsetting ins = new Appsetting();

        public static Appsetting Instanse { get => Get(); }

        public Appsetting()
        {
            FilterProcess = AppSettings["filterProcess"].Trim().Split(',');
            WsProt = int.Parse(AppSettings["wsListenPort"]);
            PrintBarrage = AppSettings["printBarrage"].ToLower() == "true";
            ProxyPort = int.Parse(AppSettings["proxPort"]);
        }

        public static Appsetting Get()
        {
            return ins;
        }

        /// <summary>
        /// 过滤的进程
        /// </summary>
        public string[] FilterProcess { get; private set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int WsProt { get; private set; }

        /// <summary>
        /// 控制台打印消息开关
        /// </summary>
        public bool PrintBarrage { get; private set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get; private set; } = 8827;
    }
}
