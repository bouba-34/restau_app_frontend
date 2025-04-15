namespace Client.Controls
{
    public partial class NotificationBadge : ContentView
    {
        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(NotificationBadge), propertyChanged: OnContentChanged);

        public static readonly BindableProperty BadgeCountProperty =
            BindableProperty.Create(nameof(BadgeCount), typeof(int), typeof(NotificationBadge), 0, propertyChanged: OnBadgeCountChanged);

        public static readonly BindableProperty BadgeColorProperty =
            BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(NotificationBadge), Colors.Red, propertyChanged: OnBadgeColorChanged);

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        
        public int BadgeCount
        {
            get => (int)GetValue(BadgeCountProperty);
            set => SetValue(BadgeCountProperty, value);
        }
        
        public Color BadgeColor
        {
            get => (Color)GetValue(BadgeColorProperty);
            set => SetValue(BadgeColorProperty, value);
        }

        public NotificationBadge()
        {
            InitializeComponent();
        }

        private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NotificationBadge)bindable;
            
            if (newValue is View view)
            {
                control.ContentContainer.Content = view;
            }
        }
        
        private static void OnBadgeCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NotificationBadge)bindable;
            
            if (newValue is int count)
            {
                if (count > 0)
                {
                    control.BadgeLabel.Text = count > 99 ? "99+" : count.ToString();
                    control.Badge.IsVisible = true;
                }
                else
                {
                    control.BadgeLabel.Text = string.Empty;
                    control.Badge.IsVisible = false;
                }
            }
        }
        
        private static void OnBadgeColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NotificationBadge)bindable;
            
            if (newValue is Color color)
            {
                control.Badge.BackgroundColor = color;
            }
        }
    }
}