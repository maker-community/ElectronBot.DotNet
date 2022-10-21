# [树莓派Zero 2 W（ubuntu-22.04）通过.NET6和libusb操作USB读写](https://www.cnblogs.com/GreenShade/p/16795988.html)


# ElectronBot.DotNet

# 此项目为[雉晖君的ElectronBot](https://github.com/peng-zhihui/ElectronBot)提供了.Net版本上位机传输数据的SDK。


# ElectronBot.BraincasePreview
# 电子脑壳项目为WinUI项目 最好使用VS2022最新版本 并安装Windows App SDK 插件运行

 电子脑壳全部代码已经开源，有兴趣的可以拿来进行学习。

![首页](/Images/HomePage.png)

表情编辑页包含表情视频的上传，表情的播放。

![表情编辑页](/Images/EmojisEdit.png)

![量子纠缠页](/Images/EmojiPage.png)

目前的一些表盘如下图：

![Clock](/Images/clock.png)

## [Verdure.ElectronBot.GrpcService项目介绍](https://github.com/maker-community/ElectronBot.DotNet/tree/master/src/Verdure.ElectronBot.GrpcService)
    此项目是一个用.net6编写的web服务，主要承载了grpc服务，此服务部署到树莓派上，通过和libusb互操作，将电子脑壳传输的表情数据进行接收写入到电子本体里进行显示。

![EbGrpc](/Images/EbGrpc.png)

测试效果如下

![electronbot](/Images/electron-raspberrypi.jpg)

## 相关信息
+ 树莓派Zero 2 W 一个 并安装ubuntu 2022
+ .net 6 sdk和运行时安装
+ libusb 软连接的创建


### ***[电子脑壳详细介绍](https://github.com/maker-community/ElectronBot.Braincase)***

# [下载 Windows App SDK](https://docs.microsoft.com/zh-cn/windows/apps/windows-app-sdk/downloads)

## 本人根据这个sdk开发了一个上位机，效果如下图，主打表盘显示功能，交流群（924558003）如下，有完整电子的可以windows商店搜索 电子脑壳，或者进群交流。

![交流群](/Images/QQ.jpg)
