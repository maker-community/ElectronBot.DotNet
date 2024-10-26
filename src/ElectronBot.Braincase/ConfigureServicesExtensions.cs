using BotSharp.Abstraction.Messaging.JsonConverters;
using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Users;
using BotSharp.Core;
using BotSharp.Logger;
using Contracts.Services;
using Controls;
using Controls.CompactOverlay;
using ElectronBot.Braincase.Activation;
using ElectronBot.Braincase.ClockViews;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Notifications;
using ElectronBot.Braincase.Picker;
using ElectronBot.Braincase.Services;
using ElectronBot.Braincase.ViewModels;
using ElectronBot.Braincase.Views;
using ElectronBot.Copilot.Services.BotSharp;
using ElectronBot.Copilot.ViewModels;
using ElectronBot.Copilot.Views.Agents;
using ElectronBot.DotNet;
using ElectronBot.DotNet.LibUsb;
using ElectronBot.DotNet.WinUsb;
using HelixToolkit.SharpDX.Core;
using HelloWordKeyboard.DotNet;
using Hw75Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;

using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Services;
using Verdure.IoT.Net.Services;
using Verdure.WinUI.Common;
using Verdure.WinUI.Common.Players;
using Verdure.WinUI.Common.Services;
using ViewModels;
using Views;
using Windows.Media.Playback;
using Windows.Storage;

namespace ElectronBot.Braincase;
public static class ConfigureServicesExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        var dbSettings = new BotSharpDatabaseSettings();
        config.Bind("Database", dbSettings);

        var destinationFolder = KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

        dbSettings.BotSharpLiteDB = Path.Combine(destinationFolder.Path, "ElectronBot.db");


        var canvasDevice = CanvasDevice.GetSharedDevice();
        services.Configure<LocalSettingsOptions>(config.GetSection(nameof(LocalSettingsOptions)));
        // Register 
        Ioc.Default.ConfigureServices(
            services.AddSingleton(canvasDevice)
            // Default Activation Handler
            .AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>()

            // Other Activation Handlers
            .AddTransient<IActivationHandler, AppNotificationActivationHandler>()

            .AddTransient<IActivationHandler, StartupTaskActivationHandler>()

            .AddHttpClient()
            // Services
            .AddSingleton<IAppNotificationService, AppNotificationService>()
            .AddSingleton<ILocalSettingsService, LocalSettingsService>()
            .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
            .AddTransient<INavigationViewService, NavigationViewService>()
            .AddSingleton<ISpeechAndTTSService, SpeechAndTTSService>()
            .AddSingleton<IActivationService, ActivationService>()
            .AddSingleton<IPageService, PageService>()
            .AddSingleton<INavigationService, NavigationService>()

            .AddSingleton<IdentityService>()
            .AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>()

            .AddSingleton<UserDataService>()

            // Core Services
            .AddSingleton<IFileService, FileService>()

            .AddTransient<IEmojisFileService, EmojisFileService>()

            .AddTransient<IEmojiseShopService, EmojiseShopService>()

            .AddTransient<IElectronLowLevel, WinUsbElectronLowLevel>()

            .AddTransient<IElectronLowLevel, LibUsbElectronLowLevel>()

            .AddSingleton<IElectronBotPlayer, ElectronBotPlayer>()

            .AddSingleton<IEmoticonActionFrameService, EmoticonActionFrameService>()

            .AddTransient<MediaPlayer>()

            .AddTransient<ObjectPicker<WriteableBitmap>>()

            .AddSingleton<ObjectPickerService>()

            .AddSingleton<ClockDiagnosticService>()

            // Views and ViewModels
            .AddTransient<CameraEmojisViewModel>()
            .AddTransient<CameraEmojisPage>()
            .AddTransient<SettingsViewModel>()
            .AddTransient<SettingsPage>()
            .AddTransient<TodoViewModel>()
            .AddTransient<TodoPage>()
            .AddTransient<BlankViewModel>()
            .AddTransient<BlankPage>()
            .AddTransient<MainViewModel>()
            .AddTransient<MainPage>()
            .AddTransient<ShellPage>()
            .AddTransient<ShellViewModel>()
            .AddTransient<EmojisEditPage>()
            .AddTransient<EmojisEditViewModel>()
            .AddTransient<AddEmojisDialogViewModel>()
            .AddTransient<UploadEmojisDialogViewModel>()
            .AddTransient<UploadEmojisPage>()
            .AddTransient<MarketplacePage>()
            .AddTransient<MarketplaceViewModel>()

            .AddTransient<GestureClassificationPage>()
            .AddTransient<GestureClassificationViewModel>()

            .AddTransient<GestureInteractionPage>()
            .AddTransient<GestureInteractionViewModel>()

            .AddTransient<PoseRecognitionPage>()
            .AddTransient<PoseRecognitionViewModel>()


            .AddTransient<VisionPage>()
            .AddTransient<VisionViewModel>()


            .AddTransient<ElectronBot3D>()
            .AddTransient<ElectronBot3DViewModel>()

            .AddTransient<MoviePage>()
            .AddTransient<MovieViewModel>()

            .AddTransient<RandomContentPage>()
            .AddTransient<RandomContentViewModel>()

            .AddTransient<LaunchAppPage>()
            .AddTransient<LaunchAppViewModel>()

            .AddTransient<EmojisInfoDialogViewModel>()

            .AddTransient<GamepadViewModel>()
            .AddTransient<GamepadPage>()

            .AddTransient<GestureAppConfigPage>()
            .AddTransient<GestureAppConfigViewModel>()

            .AddTransient<LongShadow>()

            .AddTransient<HiddenTextView>()
            .AddSingleton<ClockViewModel>()

            .AddSingleton<ComboxDataService>()

            .AddTransient<DispatcherTimer>()

            .AddTransient<ImageCropperPickerViewModel>()

            .AddTransient<ImageCropperPage>()

            .AddTransient<TodoCompactOverlayViewModel>()
            .AddTransient<ModelLoadCompactOverlayViewModel>()
            .AddTransient<ModelLoadCompactOverlayPage>()
            .AddTransient<IEffectsManager, DefaultEffectsManager>()


            .AddTransient<Hw75ViewModel>()

            .AddTransient<Hw75Page>()

            .AddTransient<Hw75ShellPage>()
            .AddTransient<Hw75ShellViewModel>()
            .AddTransient<Hw75CustomView>()
            .AddTransient<Hw75WeatherView>()
            .AddTransient<Hw75YellowCalendarView>()

            .AddTransient<Hw75CustomViewModel>()

            .AddTransient<Hw75WeatherViewModel>()

            .AddTransient<Hw75YellowCalendarViewModel>()

            .AddSingleton<IClockViewProviderFactory, ClockViewProviderFactory>()

            .AddTransient<IClockViewProvider, DefaultClockViewProvider>()

            .AddTransient<IClockViewProvider, CustomClockViewProvider>()

            .AddTransient<IClockViewProvider, GrooveClockViewProvider>()

            .AddTransient<IClockViewProvider, LongShadowClockViewProvider>()

            .AddTransient<IClockViewProvider, GooeyFooterClockViewProvider>()

            .AddTransient<IClockViewProvider, GradientsWithBlendClockViewProvider>()

            .AddSingleton<IActionExpressionProvider, DefaultActionExpressionProvider>()

            .AddSingleton<IActionExpressionProviderFactory, ActionExpressionProviderFactory>()

            .AddTransient<TodoView>()
            .AddTransient<Hw75DynamicViewModel>()

            .AddSingleton<EmoticonActionFrameService>()

            .AddSingleton<GestureClassificationService>()

            .AddSingleton<PoseRecognitionService>()


            .AddTransient<IChatbotClient, ChatGPTChatbotCustomClient>()

            .AddTransient<IChatbotClient, ChatGPTChatbotClient>()

            .AddTransient<IChatbotClient, TuringChatbotClient>()

            .AddTransient<IChatbotClient, SparkDeskChatbotClient>()

            .AddTransient<IChatbotClientFactory, ChatbotClientFactory>()

            .AddTransient<IHw75DynamicViewProvider, Hw75DynamicTodoViewProvider>()


            .AddTransient<IHw75DynamicViewProvider, Hw75DynamicCustomViewProvider>()

            .AddTransient<IHw75DynamicViewProvider, Hw75DynamicWeatherViewProvider>()
            .AddTransient<IHw75DynamicViewProvider, Hw75DynamicYellowCalendarViewProvider>()

            .AddTransient<IHw75DynamicViewProviderFactory, Hw75DynamicViewProviderFactory>()

            .AddTransient<IHw75DynamicDevice, Hw75DynamicDevice>()

            //.AddGrpcClient<ElectronBotActionGrpc.ElectronBotActionGrpcClient>(o =>
            //{
            //    o.Address = new Uri("http://192.168.3.239:5241")
            //    //o.Address = new Uri("http://localhost:5241")
            //})

            .AddSingleton<EbGrpcService>()

            // add botsharp
            .AddTransient<AgentViewModel>()
            .AddTransient<AgentPage>()
            .AddTransient<ChatViewModel>()
            .AddTransient<ChatPage>()
            .AddSingleton(dbSettings)
            .AddBotSharpCore(config, options =>
            {
                options.JsonSerializerOptions.Converters.Add(new RichContentJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TemplateMessageJsonConverter());
            })
            .AddHttpContextAccessor()
            .AddScoped<IUserIdentity, BotUserIdentity>()
            .AddScoped<IBotToolService, BotToolService>()
            .AddScoped<IBotIotService, BotIotService>()
            .AddBotSharpLogger(config)
            // Configuration
            .BuildServiceProvider());
    }
}
