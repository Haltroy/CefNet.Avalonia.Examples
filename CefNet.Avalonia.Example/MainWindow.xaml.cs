using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.LogicalTree;
using CefNet;
using CefNet.Avalonia;
using CefNet.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AvaloniaApp
{
	public class MainWindow : Window
	{
		bool isFirstLoad = true;

		private TextBox txtAddress = null;
		private TabControl tabs = null;
		private Menu menu = null;
		private DockPanel controlsPanel = null;
		private SystemDecorations _systemDecorations;

		public MainWindow()
		{
			InitializeComponent();

			this.Opened += MainWindow_Opened;
			this.Closing += MainWindow_Closing;
			CustomWebView.FullscreenEvent.AddClassHandler(typeof(WebView), HandleFullscreenEvent);
			WebView.ScriptDialogOpeningEvent.AddClassHandler(typeof(WebView), HandleScriptDialogOpeningEvent);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);

			txtAddress = this.FindControl<TextBox>(nameof(txtAddress));
			tabs = this.FindControl<TabControl>(nameof(tabs));
			menu = this.FindControl<Menu>(nameof(menu));
			controlsPanel = this.FindControl<DockPanel>(nameof(controlsPanel));
		}


		private void HandleFullscreenEvent(object sender, RoutedEventArgs e)
		{
			WrapPanel tabHeaders = tabs.FindChild<WrapPanel>(null);
			if (((FullscreenModeChangeEventArgs)e).Fullscreen)
			{
				menu.IsVisible = false;
				controlsPanel.IsVisible = false;
				tabHeaders.IsVisible = false;
				_systemDecorations = this.SystemDecorations;
				WindowState = WindowState.Maximized;
				Topmost = true;
			}
			else
			{
				menu.IsVisible = true;
				controlsPanel.IsVisible = true;
				tabHeaders.IsVisible = true;
				this.SystemDecorations = _systemDecorations;
				WindowState = WindowState.Normal;
				Topmost = false;
			}
		}

		private void HandleScriptDialogOpeningEvent(object sender, RoutedEventArgs e)
		{
			var ea = (ScriptDialogOpeningRoutedEventArgs)e;
			if (ea.Kind == ScriptDialogKind.Alert)
			{
				Debug.Print("Alert: " + ea.Message);
			}
		}

		private void MainWindow_Opened(object sender, EventArgs e)
		{
			if (!isFirstLoad)
				return;
			isFirstLoad = false;

			AddTab(true);

			// note: Below demonstrates that WebView cannot navigate immediately after creation.
			// Trying this may cause an InvalidOperationException to be thrown (reproduces on macOS at least).
			// Navigation (and other tasks) require waiting until the BrowserCreated event has fired.
			try
			{
				SelectedView?.Navigate("https://google.com");
			}
			catch (InvalidOperationException ioe)
			{
				Console.WriteLine("Cannot navgiate before browser is initialized:");
				Console.WriteLine(ioe.Message);
			}			
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Close all tabs
			var tablist = new List<WebViewTab>(tabs.ItemCount);
			foreach (WebViewTab tab in tabs.Items)
			{
				tablist.Add(tab);
			}
			foreach (WebViewTab tab in tablist)
			{
				tab.Close();
			}
		}

		private void AddTab(bool useGlobalContext)
		{
			WebViewTab viewTab;
			if (useGlobalContext)
			{
				viewTab = new WebViewTab();
				viewTab.WebView.Navigated += WebView_Navigated;
				((AvaloniaList<object>)tabs.Items).Add(viewTab);
				viewTab.Title = "about:blank";

				tabs.SelectedItem = viewTab;
			}
			else
			{
				//var cx = new CefRequestContext(new CefRequestContextSettings());
				//tabs.Controls.Add(new WebViewTab(new CefBrowserSettings(), cx));
			}
		}

		private void WebView_Navigated(object sender, NavigatedEventArgs e)
		{
			txtAddress.Text = e.Url.ToString();
		}

		private IChromiumWebView SelectedView
		{
			get
			{
				return (tabs.SelectedItem as WebViewTab)?.WebView;
			}
		}



		private void AddTab_Click(object sender, RoutedEventArgs e)
		{
			AddTab(true);
		}

		private void NavigateButton_Click(object sender, RoutedEventArgs e)
		{
			//SelectedView?.Navigate("http://yandex.ru");
			SelectedView?.Navigate("https://cefnet.github.io/winsize.html");
		}

		private void txtAddress_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (Uri.TryCreate(txtAddress.Text, UriKind.Absolute, out Uri url))
				{
					SelectedView?.Navigate(url.AbsoluteUri);
				}
			}
		}

		private void WebView_TextFound(object sender, TextFoundRoutedEventArgs e)
		{
			if (e.FinalUpdate)
			{
				SelectedView?.StopFinding(false);
			}
		}

		private void Find_Click(object sender, RoutedEventArgs e)
		{
			SelectedView?.Find("i", true, true, false);
		}

		private void OpenPopup_Click(object sender, RoutedEventArgs e)
		{
			var tab = tabs.SelectedItem as WebViewTab;
			if (tab == null)
				return;

			IChromiumWebView webView = tab.WebView;
			if (webView == null)
				return;

			tab.PopupHandlingDisabled = true;
			EventHandler<CreateWindowEventArgs> callback = null;
			callback = (a, b) => {
				tab.PopupHandlingDisabled = false;
				Dispatcher.UIThread.Post(() => webView.CreateWindow -= callback, DispatcherPriority.Normal);
			};
			webView.CreateWindow += callback;
			webView.GetMainFrame().ExecuteJavaScript("window.open('http://example.com')", null, 1);
		}

		private void TestV8ValueTypes_Click(object sender, RoutedEventArgs e)
		{
			IChromiumWebView webView = SelectedView;
			if (webView == null)
				return;

			webView.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, new CefProcessMessage("TestV8ValueTypes"));
		}

		private async void Alert_Click(object sender, RoutedEventArgs e)
		{
			IChromiumWebView webView = SelectedView;
			if (webView == null)
				return;

			dynamic scriptableObject = await webView.GetMainFrame().GetScriptableObjectAsync(CancellationToken.None).ConfigureAwait(false);
			scriptableObject.window.alert("hello");
		}

	}
}
