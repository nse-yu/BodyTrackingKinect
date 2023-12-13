namespace KinectDemo2.Custom.Control.navigation
{
    public class NavigationTab : View
    {
        public static readonly BindableProperty PageProperty = 
            BindableProperty.Create(nameof(Page), typeof(Type), typeof(NavigationTab));

        public static readonly BindableProperty IconProperty = 
            BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(NavigationTab), propertyChanged: OnIconChanged);

        public static readonly BindableProperty IconBackgroundColorProperty = 
            BindableProperty.Create(nameof(IconBackgroundColor), typeof(Color), typeof(NavigationTab));

        public Type Page
        {
            get => GetValue(PageProperty) as Type;
            set => SetValue(PageProperty, value);
        }
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public Color IconBackgroundColor
        {
            get => (Color)GetValue(IconBackgroundColorProperty);
            set => SetValue(IconBackgroundColorProperty, value);
        }


        private static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as NavigationTab;
        }
    }
}
