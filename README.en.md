<h1 align="center">Electronic Braincase</h1>
<div align="center">

<a href="https://github.com/maker-community/ElectronBot.DotNet/stargazers"><img src="https://img.shields.io/github/stars/maker-community/ElectronBot.DotNet" alt="Stars Badge"/></a>
<a href="https://github.com/maker-community/ElectronBot.DotNet/network/members"><img src="https://img.shields.io/github/forks/maker-community/ElectronBot.DotNet" alt="Forks Badge"/></a>
<a href="https://github.com/maker-community/ElectronBot.DotNet/pulls"><img src="https://img.shields.io/github/issues-pr/maker-community/ElectronBot.DotNet" alt="Pull Requests Badge"/></a>
<a href="https://github.com/maker-community/ElectronBot.DotNet/issues"><img src="https://img.shields.io/github/issues/maker-community/ElectronBot.DotNet" alt="Issues Badge"/></a>
<a href="https://github.com/maker-community/ElectronBot.DotNet/graphs/contributors"><img alt="GitHub contributors" src="https://img.shields.io/github/contributors/maker-community/ElectronBot.DotNet?color=2b9348"></a>
<a href="https://github.com/maker-community/ElectronBot.DotNet/blob/master/LICENSE.txt"><img src="https://img.shields.io/github/license/maker-community/ElectronBot.DotNet?color=2b9348" alt="License Badge"/></a>

<a href="https://github.com/maker-community/ElectronBot.DotNet"><img src="https://img.shields.io/static/v1?label=&labelColor=505050&message=Chinese 中文自述文件&color=%230076D6&style=flat&logo=google-chrome&logoColor=green" alt="website"/></a>


<i>Like this project? Please consider giving a Star ⭐️ to help improve!</i>

<br/><a href="https://www.microsoft.com/store/productId/9NQWDB4MQV0C"><img src="https://cdn.jsdelivr.net/gh/qishibo/img/microsoft-store.png" height="58" width="180" alt="get from microsoft store"></a>
</div>

---

Electronic Braincase is a desktop program project that provides some software functions for the open-source desktop robot [ElectronBot](https://github.com/peng-zhihui/ElectronBot) and Hanwen Keyboard ([HelloWord-Keyboard](https://github.com/peng-zhihui/HelloWord-Keyboard)) by Zhihuijun. It was developed by Green Shade Aguang using Microsoft's WASDK framework and C# language.

![Electronic Braincase Screenshot](/Images/home1.png)

Electronic control interface

![Electronic Braincase Interface](/Images/HomePage.png)

Hanwen keyboard control
![Hanwen Interface](/Images/helloworld-%20keyboard.PNG)
## Features

- Colorful dial: Can display time and custom text, and there is a dial that can specifically display the current computer resource usage.
![Clock](/Images/clock.png)
- Gesture recognition voice interaction: Gesture recognition combined with voice recognition for voice dialogue, can ask about weather conditions, simple joke replies or intelligent chat through Turing Robot API or ChatGPT dialogue API. (Note: You need to solve the network problem yourself)
[![Bilibili video demonstration link](/Images/chatgpt-talk.JPG)](https://www.bilibili.com/video/BV1FX4y1S7hA/?share_source=copy_web&vd_source=dbfa7a452a337f924e60d4da2715b6eb)


- Quantum entanglement: This function can recognize the user's expression, and can also play the user's facial data on the robot's face.
![Quantum entanglement page](/Images/face.png)

- Expression list: Users can customize their favorite expression data, and then specify the corresponding action file, which can be played in combination with expressions and actions. You can also export, share with others or import others' shared expressions.
![emojis](/Images/emojis.png)

- Joystick control: After the user connects the xbox joystick, you can control the electronics on the joystick controller page, and you can operate the bottom rotation, arm rotation, single arm expansion and head total five servos at the same time.
![xbox](/Images/xbox-controller.png)

- Electronic simulation: You can randomly play expressions without a robot to show the effect.
![ElectronBotModelLoad](/Images/ElectronBotModelLoad.gif)


## Installation

1. Install Visual Studio 2022 and choose to install the WASDK development component.
2. Clone or download this project locally.
3. Open the ElectronBot.Braincase.sln file and compile and run.
4. The key is to set the startup project to ElectronBot.Braincase otherwise it will not run.

[Click here for detailed steps to run the project](https://github.com/maker-community/ElectronBot.Braincase)

## Usage

1. Select the function module you want to use on the homepage and click to enter.
2. Operate or set parameters according to the prompts of different modules.
3. Click the return button to return to the homepage or exit the program.

## Configuration

On the settings page, you can configure the following parameters:

- Serial port number: Select the serial port number connected to the ElectronBot hardware device.
- Turing Robot API Key: Enter the Turing Robot API key you applied for to implement the voice interaction function.
- ChatGPT dialogue API address: Enter the ChatGPT dialogue API address you built or accessed to implement the intelligent chat function.

## Reference Projects and Dependencies

The projects and dependencies referenced by this project:

+ [Project template - TemplateStudio](https://github.com/microsoft/TemplateStudio)

+ [Dial reference project - A Pomodoro](https://github.com/DinoChan/OnePomodoro)
+ [Community Toolkit - CommunityToolkit](https://github.com/CommunityToolkit/WindowsCommunityToolkit)

+ [Control library demo - WinUI-Gallery](https://github.com/microsoft/WinUI-Gallery)

+ [Image processing library - opencvsharp](https://github.com/shimat/opencvsharp)

+ [Emoji8 Expression Recognition Example](https://github.com/microsoft/Windows-Machine-Learning/tree/master/Samples/Emoji8/UWP/cs)

+ [ChatGPTSharp](https://github.com/aiqinxuancai/ChatGPTSharp)

+ [MediaPipe.NET](https://github.com/vignetteapp/MediaPipe.NET)

+ [helix-toolkit](https://github.com/helix-toolkit/helix-toolkit)

+ [Semantic Kernel](https://github.com/microsoft/semantic-kernel)

## License

This project is released under the MIT license, please see the [LICENSE](https://github.com/maker-community/ElectronBot.DotNet/blob/master/LICENSE.txt) file.

Thanks to [JetBrains](https://www.jetbrains.com/?from=maker-community) for providing a license to develop **Electronic Braincase** <a href="https://www.jetbrains.com/?from=maker-community"><img src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" width="94" align="center" /></a>

[Introduction to the Raspberry Pi connection ElectronBot project](https://github.com/maker-community/ElectronBot.DotNet/tree/master/src/Verdure.ElectronBot.GrpcService)

![Cover](/Images/videoCar.jpg)

## Contact

If you have any questions, suggestions or feedback, please contact me:

- Email: gil.zhang.dev@outlook.com



Electronic Braincase exchange group (924558003), if you have a complete electronic, you can search for Electronic Braincase in the windows store, or join the group for discussion.

![Exchange Group](/Images/QQ.jpg)

roadmap
