using Avalonia;
using Avalonia.ReactiveUI;
using System;
using CefNet;
using CefNet.Avalonia;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace CefTest;

internal class Program
{
    internal static CefAppImpl? app;
    private static DispatcherTimer? messagePump;
    private const int messagePumpDelay = 10;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        string cefPath = GetProjectPath(PlatformInfo.IsMacOS);
        bool externalMessagePump = args.Contains("--external-message-pump");

        if (PlatformInfo.IsMacOS)
        {
            externalMessagePump = true;
        }

        var settings = new CefSettings();
        settings.MultiThreadedMessageLoop = !externalMessagePump;
        settings.ExternalMessagePump = externalMessagePump;
        settings.NoSandbox = true;
        settings.WindowlessRenderingEnabled = true;
        settings.LocalesDirPath = Path.Combine(cefPath, PlatformInfo.IsMacOS ? "Resources" : "locales");
        settings.ResourcesDirPath = Path.Combine(cefPath, PlatformInfo.IsMacOS ? "Resources" : "");
        settings.LogSeverity = CefLogSeverity.Warning;
        settings.UncaughtExceptionStackSize = 8;

        App.FrameworkInitialized += App_FrameworkInitialized;
        App.FrameworkShutdown += App_FrameworkShutdown;

        app = new CefAppImpl();
        app.ScheduleMessagePumpWorkCallback = OnScheduleMessagePumpWork;
        app.Initialize(cefPath, settings);
        BuildAvaloniaApp().StartWithCefNetApplicationLifetime(args);
    }

    private static string GetProjectPath(bool isMacOS)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cefnet", "Release", isMacOS ? Path.Combine("cefclient.app", "Contents", "Frameworks", "Chromium Embedded Framework.framework") : "");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void App_FrameworkInitialized(object? sender, EventArgs e)
    {
        if (CefNetApplication.Instance.UsesExternalMessageLoop)
        {
            messagePump = new DispatcherTimer(TimeSpan.FromMilliseconds(messagePumpDelay), DispatcherPriority.Normal, (s, e) =>
            {
                CefApi.DoMessageLoopWork();
                Dispatcher.UIThread.RunJobs();
            });
            messagePump.Start();
        }
    }

    private static void App_FrameworkShutdown(object? sender, EventArgs e)
    {
        messagePump?.Stop();
    }

    private static async void OnScheduleMessagePumpWork(long delayMs)
    {
        await Task.Delay((int)delayMs);
        Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork);
    }
}