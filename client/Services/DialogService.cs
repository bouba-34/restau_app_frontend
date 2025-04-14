using Shared.Services.Interfaces;

namespace Client.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            return await MainThread.InvokeOnMainThreadAsync(() => 
                Shell.Current.DisplayAlert(title, message, accept, cancel));
        }

        public async Task DisplayAlertAsync(string title, string message, string cancel)
        {
            await MainThread.InvokeOnMainThreadAsync(() => 
                Shell.Current.DisplayAlert(title, message, cancel));
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return await MainThread.InvokeOnMainThreadAsync(() => 
                Shell.Current.DisplayActionSheet(title, cancel, destruction, buttons));
        }

        public async Task DisplayToastAsync(string message, ToastDuration duration = ToastDuration.Short)
        {
            try
            {
                var toast = CommunityToolkit.Maui.Alerts.Toast.Make(message, duration == ToastDuration.Long ? 
                    CommunityToolkit.Maui.Core.ToastDuration.Long : 
                    CommunityToolkit.Maui.Core.ToastDuration.Short);
                
                await toast.Show();
            }
            catch
            {
                // Fallback if toast fails
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Shell.Current.DisplayAlert("Notification", message, "OK"));
            }
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1)
        {
            return await MainThread.InvokeOnMainThreadAsync(() => 
                Shell.Current.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength));
        }
    }
}