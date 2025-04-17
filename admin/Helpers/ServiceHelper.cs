using System.Diagnostics;

namespace admin.Helpers;

public static class ServiceHelper
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        Console.WriteLine("ServiceHelper initialized.");
        ServiceProvider = serviceProvider;
    }

    public static T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
