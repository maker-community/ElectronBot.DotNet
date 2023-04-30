// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI;
using System.Numerics;
using Microsoft.UI.Composition;
using Windows.UI;
using ElectronBot.Braincase.ViewModels;
using ElectronBot.Braincase;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ClockViews
{
    public sealed partial class GradientsWithBlend : UserControl
    {
        public ClockViewModel ViewModel
        {
            get;
        }

        private readonly CompositionLinearGradientBrush _foregroundBrush;
        private readonly CompositionLinearGradientBrush _backgroundBrush;
        private readonly SpriteVisual _backgroundVisual;
        private static readonly Color Blue = Color.FromArgb(255, 43, 210, 255);
        private static readonly Color Green = Color.FromArgb(255, 43, 255, 136);
        private static readonly Color Red = Colors.Red;

        //private static readonly Color Pink = Color.FromArgb(255, 255, 43, 212);
        private static readonly Color Pink = Color.FromArgb(255, 142, 211, 255);

        private static readonly Color Black = Colors.Black;

        private readonly Compositor _compositor;
        private readonly CompositionColorGradientStop _topLeftradientStop;
        private readonly CompositionColorGradientStop _bottomRightGradientStop;

        private readonly CompositionColorGradientStop _bottomLeftGradientStop;
        private readonly CompositionColorGradientStop _topRightGradientStop;

        public GradientsWithBlend()
        {
            InitializeComponent();
            ViewModel = App.GetService<ClockViewModel>();

            Visual hostVisual = ElementCompositionPreview.GetElementVisual(this);
            _compositor = hostVisual.Compositor;

            _foregroundBrush = _compositor.CreateLinearGradientBrush();
            _foregroundBrush.StartPoint = Vector2.Zero;
            _foregroundBrush.EndPoint = new Vector2(1.0f);

            _bottomRightGradientStop = _compositor.CreateColorGradientStop();
            _bottomRightGradientStop.Offset = 0.5f;
            _bottomRightGradientStop.Color = Green;
            _topLeftradientStop = _compositor.CreateColorGradientStop();
            _topLeftradientStop.Offset = 0.5f;
            _topLeftradientStop.Color = Blue;
            _foregroundBrush.ColorStops.Add(_bottomRightGradientStop);
            _foregroundBrush.ColorStops.Add(_topLeftradientStop);

            _backgroundBrush = _compositor.CreateLinearGradientBrush();
            _backgroundBrush.StartPoint = new Vector2(1.0f, 0);
            _backgroundBrush.EndPoint = new Vector2(0, 1.0f);

            _topRightGradientStop = _compositor.CreateColorGradientStop();
            _topRightGradientStop.Offset = 0.25f;
            _topRightGradientStop.Color = Black;
            _bottomLeftGradientStop = _compositor.CreateColorGradientStop();
            _bottomLeftGradientStop.Offset = 1.0f;
            _bottomLeftGradientStop.Color = Black;
            _backgroundBrush.ColorStops.Add(_topRightGradientStop);
            _backgroundBrush.ColorStops.Add(_bottomLeftGradientStop);

            var graphicsEffect = new BlendEffect()
            {
                Mode = BlendEffectMode.Screen,
                Foreground = new CompositionEffectSourceParameter("Main"),
                Background = new CompositionEffectSourceParameter("Tint"),
            };

            var effectFactory = _compositor.CreateEffectFactory(graphicsEffect);
            var brush = effectFactory.CreateBrush();
            brush.SetSourceParameter("Main", _foregroundBrush);
            brush.SetSourceParameter("Tint", _backgroundBrush);

            _backgroundVisual = _compositor.CreateSpriteVisual();
            _backgroundVisual.Brush = brush;

            ElementCompositionPreview.SetElementChildVisual(Gradient, _backgroundVisual);

            Loaded += async (s, e) =>
            {
                await Task.Delay(2000);
                UpdateGradients();
                await Task.Delay(2000);
                UpdateText();
            };

            Gradient.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;

                _backgroundVisual.Size = e.NewSize.ToVector2();
                _foregroundBrush.CenterPoint = _backgroundVisual.Size / 2;
                _backgroundBrush.CenterPoint = _backgroundVisual.Size / 2;
            };

            //ViewModel.IsInPomodoroChanged += (s, e) =>
            //{
            //    UpdateGradients();
            //    UpdateText();
            //};
        }

        private void UpdateText()
        {
            FocusTextCompact.Visibility = Visibility.Visible;
            //RelaxTextCompact.Visibility = Visibility.Visible;
        }

        private void UpdateGradients()
        {
            if (true)
            {
                StartOffsetAnimation(_bottomRightGradientStop, 0.6f);
                StartColorAnimation(_bottomRightGradientStop, Blue);

                StartOffsetAnimation(_topLeftradientStop, 0f);
                StartColorAnimation(_topLeftradientStop, Green);

                StartOffsetAnimation(_topRightGradientStop, 0.25f);
                StartColorAnimation(_topRightGradientStop, Red);

                StartOffsetAnimation(_bottomLeftGradientStop, 1f);
                StartColorAnimation(_bottomLeftGradientStop, Black);
            }
        }

        private void StartOffsetAnimation(CompositionColorGradientStop gradientOffset, float offset)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.Duration = TimeSpan.FromSeconds(1);
            offsetAnimation.InsertKeyFrame(1.0f, offset);
            gradientOffset.StartAnimation(nameof(CompositionColorGradientStop.Offset), offsetAnimation);
        }

        private void StartColorAnimation(CompositionColorGradientStop gradientOffset, Color color)
        {
            var colorAnimation = _compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromSeconds(2);
            colorAnimation.Direction = Microsoft.UI.Composition.AnimationDirection.Alternate;
            colorAnimation.InsertKeyFrame(1.0f, color);
            gradientOffset.StartAnimation(nameof(CompositionColorGradientStop.Color), colorAnimation);
        }

    }
}
