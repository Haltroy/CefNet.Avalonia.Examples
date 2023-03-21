using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CefNet;

namespace AvaloniaApp
{
    internal class Program
    {
        private static CefAppImpl app;
        private static DispatcherTimer messagePump;
        private const int messagePumpDelay = 10;

        [STAThread]
        public static void Main(string[] args)
        {
            string cefPath = GetProjectPath(PlatformInfo.IsMacOS);

            bool externalMessagePump = args.Contains("--external-message-pump");

            var settings = new CefSettings
            {
                MultiThreadedMessageLoop = !externalMessagePump,
                ExternalMessagePump = externalMessagePump,
                NoSandbox = true,
                WindowlessRenderingEnabled = true,
                LocalesDirPath = Path.Combine(cefPath, "locales"),
                ResourcesDirPath = cefPath
            };

            App.FrameworkInitialized += App_FrameworkInitialized;
            App.FrameworkShutdown += App_FrameworkShutdown;

            app = new CefAppImpl();
            app.ScheduleMessagePumpWorkCallback = OnScheduleMessagePumpWork;

            app.Initialize(cefPath, settings);

            AppBuilder.Configure<App>().UsePlatformDetect().StartWithCefNetApplicationLifetime(args);
        }

        private static string GetProjectPath(bool isMacOS)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cefnet", "Release", isMacOS ? Path.Combine("cefclient.app", "Contents", "Frameworks", "Chromium Embedded Framework.framework") : "");
        }

        private static void App_FrameworkInitialized(object sender, EventArgs e)
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

        private static void App_FrameworkShutdown(object sender, EventArgs e)
        {
            messagePump?.Stop();
        }

        private static async void OnScheduleMessagePumpWork(long delayMs)
        {
            await Task.Delay((int)delayMs);
            Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork);
        }

        private static void App_CefProcessMessageReceived(object sender, CefProcessMessageReceivedEventArgs e)
        {
        }
    }
}