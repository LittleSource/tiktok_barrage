# 抖音弹幕监听器

## ⛳近期更新

1. 【重大更新】更换了底层代理框架，从而解决了原先版本随着系统请求总数增加而导致内存溢出的情况
2.  过滤了无关业务的域名，从而提升代理后的请求响应速度


## 😎介绍及配置

### 介绍

基于系统代理抓包打造的抖音弹幕服务推送程序，它能够抓取电脑上所有抖音弹幕来源数据，主要包括两种来源：**浏览器进程** ，**抖音直播伴侣**。它可以监听**弹幕**，**点赞**，**关注**，**送礼**，**进入直播间**，**直播间统计**，**粉丝团**系列消息，你可使用它做直播间数据分析，以及弹幕互动游戏，语音播报等。

### 配置

程序中有基本的配置可以过滤弹幕进程，弹幕数据通过Websocket服务推送，其他程序只需接入ws服务器就能接收到到弹幕数据消息

``` xml
	<appSettings>
		<!--过滤Websocket数据源进程,可用','进行分隔，程序将会监听以下进程的弹幕信息-->
		<add key="filterProcess" value="直播伴侣,chrome,msedge"/>
		<!--Websocket监听端口-->
		<add key="wsListenPort" value="8888"/>
		<!--在控制台输出弹幕-->
		<add key="printBarrage" value="true"/>
		<!--系统代理端口-->
		<add key="proxPort" value="8827"/>
	</appSettings>
```

### 推送数据格式

弹幕数据由WebSocket服务进行分发，使用Json格式进行推送，见项目  [BarrageMessages.cs](./BarrageGrab/JsonEntity/BarrageMessages.cs)，如需调整请克隆项目后参照 [message.proto](./BarrageGrab/proto/message.proto) 进行源码修改调整，文件包含所有弹幕相关数据结构，可前往[ws在线测试](http://wstool.jackxiang.com/)网站，连接 ws://127.0.0.1:8888 进行测试

## 🖼️控制台截图

[![控制台截图](https://s1.ax1x.com/2022/11/10/z9YYPU.png)](https://imgse.com/i/z9YYPU)



## 🐳主要依赖项

+ [Titanium.Web.Proxy](https://www.nuget.org/packages/Titanium.Web.Proxy)
+ [Protobuf-net](https://www.nuget.org/packages/protobuf-net/)



## ⚠️特别注意

1. 程序只能监听到握手之后的ws数据包，例如先进入直播间或开启直播再打开本程序是无法监听到的，所以可以保持程序后台运行

2. 由于打开系统代理需要自动检查/生成证书，所以程序需要管理员身份运行

3. 只有到达客户端的弹幕数据才能被接收，被抖音服务器过滤的弹幕是抓不到的

   

## 📢鸣谢

+ 特别鸣谢 [douyin_web_live](https://github.com/gll19920817/douyin_web_live) 提供的部分proto文件
