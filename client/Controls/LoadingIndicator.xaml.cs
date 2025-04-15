namespace Client.Controls
{
    public partial class LoadingIndicator : ContentView
    {
        public static readonly BindableProperty IsRunningProperty =
            BindableProperty.Create(nameof(IsRunning), typeof(bool), typeof(LoadingIndicator), false, propertyChanged: OnIsRunningChanged);

        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(nameof(Message), typeof(string), typeof(LoadingIndicator), "Loading...", propertyChanged: OnMessageChanged);

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public LoadingIndicator()
        {
            InitializeComponent();
        }

        private static void OnIsRunningChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (LoadingIndicator)bindable;
            
            if (newValue is bool isRunning)
            {
                control.Root.IsVisible = isRunning;
                control.ActivityIndicator.IsRunning = isRunning;
            }
        }

        private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (LoadingIndicator)bindable;
            
            if (newValue is string message)
            {
                control.MessageLabel.Text = message;
            }
        }
    }
}