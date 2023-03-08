# 电子脑壳

电子脑壳是一个为稚晖君开源的桌面机器人[ElectronBot](https://github.com/peng-zhihui/ElectronBot)提供一些软件功能的桌面程序项目。它是由绿荫阿广开发的，使用了微软的WASDK框架和C#语言。

![电子脑壳截图](/Images/home.png)

## 功能

- 多彩表盘：可以显示时间和自定义文字，也有表盘可以专门显示当前电脑资源占用情况。

![Clock](/Images/clock.png)
- 手势识别语音交互：手势识别结合语音识别进行语音对话交流，可以通过图灵机器人API或ChatGPT对话API进行天气状况询问、简单的笑话回复或智能聊天。（注：需要自己解决网络问题）

[![B站视频演示链接](/Images/chatgpt-talk.JPG)](https://www.bilibili.com/video/BV1FX4y1S7hA/?share_source=copy_web&vd_source=dbfa7a452a337f924e60d4da2715b6eb)


- 量子纠缠：此功能可以识别出用户的表情，也可以将用户的面部数据同步到机器人的面部进行播放。

![量子纠缠页](/Images/EmojiPage.png)

- 表情列表：用户可以自定义自己喜欢的表情数据，然后指定对应的动作文件，可以结合表情和动作进行播放。也可以将制作好的表情进行导出、分享给别人或导入别人分享的表情。
![emojis](/Images/emojis.png)

- 手柄控制：用户连接xbox手柄之后，可以在手柄控制器页面进行电子的控制，可以同时操作底部旋转、手臂旋转、手臂单个展开和头部总共五个舵机。
![xbox](/Images/xbox-controller.png)


## 安装

1. 安装Visual Studio 2022，并选择安装WASDK开发组件。
2. 克隆或下载本项目到本地。
3. 打开ElectronBot.Braincase.sln文件，并编译运行。
4. 连接ElectronBot硬件设备，并在设置页面选择相应的端口号。

## 使用

1. 在首页选择想要使用的功能模块，并点击进入。
2. 根据不同模块的提示操作或设置参数。
3. 点击返回按钮返回首页或退出程序。

## 配置

在设置页面，你可以配置以下参数：

- 串口号：选择与ElectronBot硬件设备连接的串口号。
- 图灵机器人API密钥：输入你申请到的图灵机器人API密钥，用于实现语音交互功能。
- ChatGPT对话API地址：输入你搭建或访问到的ChatGPT对话API地址，用于实现智能聊天功能。

## 依赖

本项目依赖以下资源：

- [Windows App SDK](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/)
- [TemplateStudio](https://github.com/microsoft/WindowsTemplateStudio)
- [TOMATO CLOCK](https://github.com/zhenghaoz/TomatoClock)
- [Turing Robot API](http://www.tuling123.com/)
- [ChatGPT API](https://github.com/polakowo/gpt2bot)

## 许可证

本项目采用MIT许可证发布，请参见[LICENSE](https://example.com/LICENSE)文件。

[树莓派连接ElectronBot项目介绍](https://github.com/maker-community/ElectronBot.DotNet/tree/master/src/Verdure.ElectronBot.GrpcService)

![封面](/Images/videoCar.jpg)

## 联系方式

如果你有任何问题、建议或反馈，请联系我：

- 邮箱：gil.zhang.dev@outlook.com



电子脑壳 交流群（924558003），有完整电子的可以windows商店搜索 电子脑壳，或者进群交流。

![交流群](/Images/QQ.jpg)
