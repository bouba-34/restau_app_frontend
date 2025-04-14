namespace Shared.Services.Interfaces
{
    public interface IConnectivityService
    {
        bool IsConnected { get; }
        event EventHandler<bool> ConnectivityChanged;
        Task<bool> CheckConnectivityAsync();
        void StartMonitoring();
        void StopMonitoring();
    }
}