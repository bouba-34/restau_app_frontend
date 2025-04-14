using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Services.Interfaces;

namespace Client.ViewModels.Base
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        protected readonly INavigationService NavigationService;
        protected readonly IDialogService DialogService;
        
        [ObservableProperty]
        private bool _isBusy;
        
        [ObservableProperty]
        private string _title;
        
        [ObservableProperty]
        private bool _isConnected;
        
        public ViewModelBase(
            INavigationService navigationService,
            IDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
            
            // Set default values
            Title = string.Empty;
            IsBusy = false;
            IsConnected = true;
        }
        
        [RelayCommand]
        public virtual Task GoBackAsync()
        {
            return NavigationService.GoBackAsync();
        }
        
        public virtual Task InitializeAsync(object parameter = null)
        {
            return Task.CompletedTask;
        }
        
        public virtual void OnAppearing()
        {
            // Base implementation does nothing
        }
        
        public virtual void OnDisappearing()
        {
            // Base implementation does nothing
        }
        
        protected async Task ExecuteBusyAction(Func<Task> action, string busyMessage = null)
        {
            if (IsBusy)
                return;
            
            try
            {
                IsBusy = true;
                
                if (!string.IsNullOrEmpty(busyMessage))
                {
                    await DialogService.DisplayToastAsync(busyMessage);
                }
                
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        protected async Task<T> ExecuteBusyAction<T>(Func<Task<T>> action, string busyMessage = null)
        {
            if (IsBusy)
                return default;
            
            try
            {
                IsBusy = true;
                
                if (!string.IsNullOrEmpty(busyMessage))
                {
                    await DialogService.DisplayToastAsync(busyMessage);
                }
                
                return await action();
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        protected async Task ShowErrorAsync(string message, Exception ex = null)
        {
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {message}. Details: {ex.Message}");
            }
            
            await DialogService.DisplayAlertAsync("Error", message, "OK");
        }
        
        protected async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
        {
            return await DialogService.DisplayAlertAsync(title, message, accept, cancel);
        }
        
        protected void ShowToast(string message)
        {
            DialogService.DisplayToastAsync(message);
        }
    }
}
