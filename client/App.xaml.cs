using client;
using Client.ViewModels.Base;
using Client.Views.Menu;
using Shared.Services.Interfaces;

namespace Client
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private readonly IAuthService _authService;
        private readonly ViewModelLocator _viewModelLocator;
        
        public App(ISettingsService settingsService, 
                  IAuthService authService,
                  ViewModelLocator viewModelLocator)
        {
            InitializeComponent();

            _settingsService = settingsService;
            _authService = authService;
            _viewModelLocator = viewModelLocator;

            MainPage = new AppShell(viewModelLocator);
            //MainPage = new MenuPage(viewModelLocator);
            
            // Register for window metrics changes
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                #if WINDOWS
                var nativeWindow = handler.PlatformView;
                nativeWindow.Activated += (s, e) =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Refresh data when app is activated
                        (_viewModelLocator.Main as MainViewModel)?.OnAppearing();
                    });
                };
                #endif
            });
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            
            // Set title and size for desktop
            window.Title = "Restaurant App";
            
            #if WINDOWS
            window.Width = 1280;
            window.Height = 720;
            window.MinimumWidth = 800;
            window.MinimumHeight = 600;
            #endif
            
            return window;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}