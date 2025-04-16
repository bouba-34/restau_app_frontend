using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace admin.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string subtitle = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private bool isInitialized;

        public ICommand RefreshCommand { get; set; }

        public BaseViewModel()
        {
            RefreshCommand = new Command(async () => await RefreshAsync());
        }

        public virtual Task InitializeAsync()
        {
            IsInitialized = true;
            return Task.CompletedTask;
        }

        public virtual Task RefreshAsync()
        {
            IsRefreshing = false;
            return Task.CompletedTask;
        }

        public void ShowError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}