using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CefTest.ViewModels;
using CefTest.Views;

namespace CefTest;

public partial class App : Application
{
    public static event EventHandler? FrameworkInitialized;
    public static event EventHandler? FrameworkShutdown;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.Startup += (s, e) => FrameworkInitialized?.Invoke(this, EventArgs.Empty);
            desktop.Exit += (s, e) => FrameworkShutdown?.Invoke(this, EventArgs.Empty);
        }

        base.OnFrameworkInitializationCompleted();
    }
}