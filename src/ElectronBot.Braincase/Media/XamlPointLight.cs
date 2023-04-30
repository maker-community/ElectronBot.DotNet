

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace ElectronBot.Braincase.Media
{
    public class XamlPointLight : XamlLight
    {
        /// <summary>
        /// The Blur value of the associated object
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register(nameof(Distance), typeof(double), typeof(XamlPointLight), new PropertyMetadata(0d));

        /// <summary>
        /// The Color of the spotlight no the associated object.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(XamlPointLight), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the Distance.
        /// </summary>
        /// <value>
        /// The Distance.
        /// </value>
        public double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the spotlight.
        /// </summary>
        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        protected override string GetId()
        {
            return GetType().FullName;
        }

        protected override void OnConnected(UIElement newElement)
        {
            (newElement as FrameworkElement).SizeChanged += XamlPointLight_SizeChanged;
            // 创建灯光
            var compositor = Window.Current.Compositor;
            PointLight light = compositor.CreatePointLight();
            // 设置灯光参数
            light.Color = ((SolidColorBrush)Color).Color;
            CompositionLight = light;
            // 这一句很重要
            XamlLight.AddTargetElement(GetId(), newElement);
        }

        private void XamlPointLight_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newElement = sender as UIElement;
            (CompositionLight as PointLight).Offset = new System.Numerics.Vector3(newElement.ActualSize.X / 2, newElement.ActualSize.Y / 2, (float)Distance);
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            // 这一句是对应的，Add了之后就要Remove
            XamlLight.RemoveTargetElement(GetId(), oldElement);
            // 释放资源
            CompositionLight.Dispose();
            CompositionLight = null;
        }
    }
}
