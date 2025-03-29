using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Messaging.JsonConverters;
using BotSharp.Abstraction.MLTasks;
using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Users;
using BotSharp.Core;
using BotSharp.Core.Crontab.Abstraction;
using BotSharp.Core.Infrastructures;
using BotSharp.Logger;
using Controls;
using Controls.CompactOverlay;
using ElectronBot.DotNet;
using ElectronBot.DotNet.LibUsb;
using ElectronBot.DotNet.WinUsb;
using HelixToolkit.SharpDX.Core;
using HelloWordKeyboard.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using Verdure.Braincase.Activation;
using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Copilot.Hooks;
using Verdure.Braincase.Copilot.Services;
using Verdure.Braincase.Copilot.Services.BotSharp;
using Verdure.Braincase.Copilot.ViewModels;
using Verdure.Braincase.Copilot.Views;
using Verdure.Braincase.Core.Configuration;
using Verdure.Braincase.Core.Contracts.Services.EmojisFile;
using Verdure.Braincase.Core.EbotGrpcService;
using Verdure.Braincase.DataStorage;
using Verdure.Braincase.DataStorage.Services;
using Verdure.Braincase.EBConfiguration.ViewModels;
using Verdure.Braincase.EBConfiguration.Views;
using Verdure.Braincase.EbScreen.Views;
using Verdure.Braincase.Emojis.eShop;
using Verdure.Braincase.Emojis.ViewModels;
using Verdure.Braincase.Notifications;
using Verdure.Braincase.Services;
using Verdure.Braincase.Settings.Services;
using Verdure.Braincase.Settings.ViewModels;
using Verdure.Braincase.Settings.Views;
using Verdure.Braincase.ViewModels;
using Verdure.Braincase.Views;
using Verdure.Braincase.WinUI.Common.Players;
using Verdure.Braincase.WinUI.Common.Services.Picker;
using Verdure.Braincase.WinUI.Common.ViewDataSource;
using Verdure.Braincase.WinUI.Common.ViewModels;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.IoT.Net.Services;
using Verdure.VoiceAssistant.Handlers;
using Verdure.VoiceAssistant.HostedServices;
using ViewModels;
using Views;
using Windows.Media.Playback;
using Windows.Storage;

namespace Verdure.Braincase;
public static class ConfigureServicesExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        var dbSettings = new BotSharpDatabaseSettings();
        config.Bind("Database", dbSettings);

        var destinationFolder = KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot\\data", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

        dbSettings.BotSharpLiteDB = Path.Combine(destinationFolder.Path, "copilot.db");

        var brainSettings = new BraincaseDatabaseSettings();
        config.Bind("Database", brainSettings);

        brainSettings.BraincaseLiteDB = Path.Combine(destinationFolder.Path, "braincase.db");

        var canvasDevice = CanvasDevice.GetSharedDevice();
        services.Configure<LocalSettingsOptions>(config.GetSection(nameof(LocalSettingsOptions)));

        services.Configure<AzureCognitiveServicesOptions>(config.GetSection("AzureCognitiveServices"));
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
            .AddSingleton<ICompositorProvider, CompositorProvider>()
            .AddSingleton<IAppNotificationService, AppNotificationService>()
            //.AddSingleton<ILocalSettingsService, LocalSettingsService>()
            .AddSingleton<ILocalSettingsService, LiteDBLocalSettingsService>()
            .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
            .AddTransient<INavigationViewService, NavigationViewService>()
            .AddSingleton<ISpeechAndTTSService, SpeechAndTTSService>()
            .AddSingleton<IActivationService, ActivationService>()
            .AddSingleton<IPageService, PageService>()
            .AddSingleton<INavigationService, NavigationService>()

            .AddSingleton<IdentityService>()
            .AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>()
            .AddSingleton<IAuthenticationProvider, CustomAuthenticationProvider>()

            .AddSingleton<UserDataService>()

            // Core Services
            .AddSingleton<IFileService, FileService>()

            .AddTransient<IEmojisFileService, EmojisFileService>()
            .AddTransient<IEmojisFileService, LiteDBEmojisFileService>()
            .AddTransient<EmojisSource>()
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
            .AddTransient<HomeViewModel>()
            .AddTransient<HomePage>()
            .AddTransient<ShellPage>()
            .AddTransient<ShellViewModel>()
            .AddTransient<EmojisEditPage>()
            .AddTransient<EmojisEditViewModel>()
            .AddTransient<AddEmojisDialogViewModel>()
            .AddTransient<UploadEmojisDialogViewModel>()
            .AddTransient<UploadEmojisPage>()
            .AddTransient<MarketplacePage>()
            .AddTransient<MarketplaceViewModel>()

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

            .AddSingleton<CompactOverlayWindow>()
            .AddTransient<MiniModePage>()
            .AddTransient<MiniModeViewModel>()

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

            .AddTransient<IClockViewProvider, BubbleClockViewProvider>()

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
            .AddSingleton(brainSettings)
            .AddTransient<BraincaseLiteDBContext>()
            // add botsharp
            .AddTransient<AgentViewModel>()
            .AddTransient<AgentPage>()
            .AddTransient<ChatViewModel>()
            .AddTransient<LingxiSpaceViewModel>()
            .AddTransient<EBLaunchAppPage>()
            .AddTransient<EBLaunchAppViewModel>()
            .AddTransient<ILingxiSpaceService, LiteDBLingxiSpaceService>()
            .AddTransient<EBDebugPage>()
            .AddTransient<EBDebugViewModel>()
            .AddTransient<GamepadActionViewModel>()
            .AddScoped<ConversationHookProvider>()
            .AddTransient<AppSettingsPage>()
            .AddTransient<AppSettingsViewModel>()
            .AddTransient<DialogueSettingsPage>()
            .AddTransient<DialogueSettingsViewModel>()
            .AddTransient<DrawingSettingsPage>()
            .AddTransient<DrawingSettingsViewModel>()
            .AddTransient<VoiceSettingsPage>()
            .AddTransient<VoiceSettingsViewModel>()
            .AddTransient<AboutAppPage>()
            .AddTransient<AboutAppViewModel>()
            .AddBotSharpCore(config, options =>
            {
                options.JsonSerializerOptions.Converters.Add(new RichContentJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TemplateMessageJsonConverter());
            })
            .AddSingleton(dbSettings)
            .AddHttpContextAccessor()
            .AddScoped<IUserIdentity, BotUserIdentity>()
            .AddScoped<IBotToolService, BotToolService>()
            .AddScoped<IBotIotService, BotIotService>()
            .AddScoped<IDialogService, DialogService>()
            .AddBotSharpLogger(config)
            .AddTransient<ILlmProviderService, LocalSettingLlmProviderService>()

            // Add wake phrase listener
            .AddSingleton<IWakeWordListener, AzCognitiveServicesWakeWordListener>()
            //.AddSingleton<IBotSpeech, DefaultBotSpeech>()
            .AddSingleton<IBotSpeech, AzBotSpeech>()
            // Add the primary hosted service to start the loop.
            .AddHostedService<HostedService>()
            .AddMemoryCache()
            .AddScoped<ICrontabHook, NotifyCrontabHook>()
            .AddScoped<IWallpaperService, WallpaperService>()
            .AddScoped<IDataInitService, CopilotDataInitService>()
            .AddScoped<IDataInitService, SettingDataInitService>()
            // Configuration
            .BuildServiceProvider());
    }
}
