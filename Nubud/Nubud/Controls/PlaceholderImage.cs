using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nubud.Controls
{
    public class PlaceholderImage : ContentControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(PlaceholderImage), new PropertyMetadata(OnSourceChanged));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PlaceholderImage), null);

        public PlaceholderImage()
        {
            DefaultStyleKey = typeof(PlaceholderImage);
        }

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public Stretch Stretch
        {
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, "Content", false);
        }

        private void OnSourceChanged(object oldValue, object newValue)
        {
            VisualStateManager.GoToState(this, "Content", false);

            var oldBitmapSource = oldValue as BitmapImage;
            var newBitmapSource = newValue as BitmapImage;

            if (oldBitmapSource != null)
            {
                oldBitmapSource.ImageOpened -= OnImageOpened;
            }

            if (newBitmapSource != null)
            {
                //VisualStateManager.GoToState(this, "Image", true);
                newBitmapSource.ImageOpened += OnImageOpened;
            }
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Image", true);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placeholderImage = (PlaceholderImage)d;

            placeholderImage.OnSourceChanged(e.OldValue, e.NewValue);
        }

        public BitmapImage ImageSource { get; set; }
    }

    public class ProgressRing : Control
    {
        bool hasAppliedTemplate = false;

        public ProgressRing()
        {
            this.DefaultStyleKey = typeof(ProgressRing);
            TemplateSettings = new TemplateSettingValues(60);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            hasAppliedTemplate = true;
            UpdateState(this.IsActive);
        }

        void UpdateState(bool isActive)
        {
            if (hasAppliedTemplate)
            {
                string state = isActive ? "Active" : "Inactive";
                System.Windows.VisualStateManager.GoToState(this, state, true);
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            var width = 100D;
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                width = this.Width != double.NaN ? this.Width : availableSize.Width;
            TemplateSettings = new TemplateSettingValues(width);
            return base.MeasureOverride(availableSize);
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new PropertyMetadata(false, new PropertyChangedCallback(IsActiveChanged)));

        private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var pr = (ProgressRing)d;
            var isActive = (bool)args.NewValue;
            pr.UpdateState(isActive);
        }

        public TemplateSettingValues TemplateSettings
        {
            get { return (TemplateSettingValues)GetValue(TemplateSettingsProperty); }
            set { SetValue(TemplateSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TemplateSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register("TemplateSettings", typeof(TemplateSettingValues), typeof(ProgressRing), new PropertyMetadata(new TemplateSettingValues(100)));


        public class TemplateSettingValues : System.Windows.DependencyObject
        {
            public TemplateSettingValues(double width)
            {
                MaxSideLength = 400;
                EllipseDiameter = width / 10;
                EllipseOffset = new System.Windows.Thickness(EllipseDiameter);
            }

            public double MaxSideLength
            {
                get { return (double)GetValue(MaxSideLengthProperty); }
                set { SetValue(MaxSideLengthProperty, value); }
            }

            // Using a DependencyProperty as the backing store for MaxSideLength.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MaxSideLengthProperty =
                DependencyProperty.Register("MaxSideLength", typeof(double), typeof(TemplateSettingValues), new PropertyMetadata(0D));

            public double EllipseDiameter
            {
                get { return (double)GetValue(EllipseDiameterProperty); }
                set { SetValue(EllipseDiameterProperty, value); }
            }

            // Using a DependencyProperty as the backing store for EllipseDiameter.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty EllipseDiameterProperty =
                DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(TemplateSettingValues), new PropertyMetadata(0D));

            public Thickness EllipseOffset
            {
                get { return (Thickness)GetValue(EllipseOffsetProperty); }
                set { SetValue(EllipseOffsetProperty, value); }
            }

            // Using a DependencyProperty as the backing store for EllipseOffset.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty EllipseOffsetProperty =
                DependencyProperty.Register("EllipseOffset", typeof(Thickness), typeof(TemplateSettingValues), new PropertyMetadata(new Thickness()));
        }
    }

}
