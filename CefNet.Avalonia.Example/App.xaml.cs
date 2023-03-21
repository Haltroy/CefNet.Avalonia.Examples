using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CefNet;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApp
{
    public class App : Application
    {
        public static event EventHandler FrameworkInitialized;

        public static event EventHandler FrameworkShutdown;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();

                desktop.Startup += (sender, e) => FrameworkInitialized?.Invoke(this, e);
                desktop.Exit += (sender, e) => FrameworkShutdown?.Invoke(this, e);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}