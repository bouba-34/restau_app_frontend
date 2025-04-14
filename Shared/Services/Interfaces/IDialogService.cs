namespace Shared.Services.Interfaces
{
    public interface IDialogService
    {
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
        Task DisplayAlertAsync(string title, string message, string cancel);
        Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
        Task DisplayToastAsync(string message, ToastDuration duration = ToastDuration.Short);
        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1);
    }
    
    public enum ToastDuration
    {
        Short,
        Long
    }
}