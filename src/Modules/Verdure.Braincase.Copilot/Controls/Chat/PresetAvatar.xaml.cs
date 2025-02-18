// Copyright (c) Rodel. All rights reserved.

using BotSharp.Abstraction.Agents.Enums;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Verdure.Braincase.Copilot.Controls;

/// <summary>
/// 预设头像.
/// </summary>
public sealed partial class PresetAvatar : UserControl
{
    /// <summary>
    /// <see cref="PresetId"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PresetIdProperty =
        DependencyProperty.Register(nameof(PresetId), typeof(string), typeof(PresetAvatar), new PropertyMetadata(default, new PropertyChangedCallback(OnPresetIdChanged)));

    /// <summary>
    /// Dependency property of <see cref="AvatarUrl"/>.
    /// </summary>
    public static readonly DependencyProperty AvatarUrlProperty =
        DependencyProperty.Register(
            nameof(AvatarUrl),
            typeof(string),
            typeof(PresetAvatar),
            new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="AvatarBitmap"/>.
    /// </summary>
    public static readonly DependencyProperty AvatarBitmapProperty =
        DependencyProperty.Register(
            nameof(AvatarBitmap),
            typeof(ImageSource),
            typeof(PresetAvatar),
            new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="PresetAvatar"/> class.
    /// </summary>
    public PresetAvatar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 是否为聊天预设.
    /// </summary>
    public bool IsChatPreset { get; set; } = true;

    /// <summary>
    /// 预设 ID.
    /// </summary>
    public string PresetId
    {
        get => (string)GetValue(PresetIdProperty);
        set => SetValue(PresetIdProperty, value);
    }

    /// <summary>
    /// AvatarUrl
    /// </summary>
    public string AvatarUrl
    {
        get => (string)GetValue(AvatarUrlProperty);
        set => SetValue(AvatarUrlProperty, value);
    }

    /// <summary>
    /// AvatarBitmap
    /// </summary>
    public ImageSource AvatarBitmap
    {
        get => (ImageSource)GetValue(AvatarBitmapProperty);
        set => SetValue(AvatarBitmapProperty, value);
    }

    private static void OnPresetIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as PresetAvatar;
        instance?.CheckAvatarAsync();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CheckAvatarAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
    }

    private void OnPresetAvatarUpdateRequested(object sender, string e)
    {
        if (PresetId == e)
        {
            CheckAvatarAsync();
        }
    }

    private void CheckAvatarAsync()
    {
        if (PresetId == AgentRole.User)
        {
            AvatarUrl = "ms-appx:///Assets/DefaultIcon.png";
            AvatarBitmap = new BitmapImage(new Uri("ms-appx:///Assets/DefaultIcon.png"));
        }
        else
        {
            AvatarUrl = "ms-appx:///Assets/Square44x44Logo.scale-100.png";
            AvatarBitmap = new BitmapImage(new Uri("ms-appx:///Assets/Square44x44Logo.scale-100.png"));
        }
    }
}
