using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CefNet;
using CefNet.Avalonia;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApp
{
	public class WebViewTab : TabItem, IStyleable
	{
		private class ColoredFormattedText : FormattedText
		{
			public IBrush Brush { get; set; }

		}

		private class WebViewTabTitle : TemplatedControl
		{
			private WebViewTab _tab;
			private ColoredFormattedText _xButton;
			private IBrush _xbuttonBrush;
			public WebViewTabTitle(WebViewTab tab)
			{
				_tab = tab;
			}

			public string Text
			{
				get
				{
					return FormattedText?.Text;
				}
				set
				{
					if (string.IsNullOrWhiteSpace(value))
					{
						this.FormattedText = null;
						this.InvalidateMeasure();
						return;
					}
					this.FormattedText = new ColoredFormattedText
					{
						Text = value,
						Typeface = new Typeface(FontFamily, FontStyle, FontWeight),
						FontSize = FontSize,
						Brush = Brushes.Black,
					};
					this.InvalidateMeasure();
				}
			}

			private ColoredFormattedText FormattedText { get; set; }

			private ColoredFormattedText XButton
			{
				get
				{
					if (_xButton == null)
					{
						_xButton = new ColoredFormattedText
						{
							Text = "x",
							Typeface = new Typeface(FontFamily, FontStyle, FontWeight.Bold),
							FontSize = FontSize,
							Brush = Brushes.Gray,
						};
					}
					return _xButton;
				}
			}

			protected override Size MeasureOverride(Size constraint)
			{
				var ft = this.FormattedText;
				if (ft == null)
					return base.MeasureOverride(constraint);
				Rect ftbounds = ft.Bounds;
				return new Size(ftbounds.Width + XButton.Bounds.Width + 4, ftbounds.Height);
			}

			protected override void OnPointerReleased(PointerReleasedEventArgs e)
			{
				if (e.InitialPressMouseButton == MouseButton.Left)
				{
					if (GetXButtonRect().Contains(e.GetPosition(this)))
					{
						_tab.Close();
					}
				}
				base.OnPointerReleased(e);
			}

			private Rect GetXButtonRect()
			{
				Rect xbounds = XButton.Bounds;
				return new Rect(Bounds.Width - xbounds.Width, 0, xbounds.Width, xbounds.Height);
			}

			protected override void OnPointerMoved(PointerEventArgs e)
			{
				SetXButtonBrush(GetXButtonRect().Contains(e.GetPosition(this)) ? Brushes.Black : Brushes.Gray);
				base.OnPointerMoved(e);
			}

			protected override void OnPointerLeave(PointerEventArgs e)
			{
				SetXButtonBrush(Brushes.Gray);
				base.OnPointerLeave(e);
			}

			private void SetXButtonBrush(ISolidColorBrush brush)
			{
				if (brush != _xbuttonBrush)
				{
					_xbuttonBrush = brush;
					XButton.Brush = brush;
					this.InvalidateVisual();
				}
			}

			public override void Render(DrawingContext drawingContext)
			{
				ColoredFormattedText formattedText = this.FormattedText;
				if (formattedText == null)
					return;

				drawingContext.DrawText(formattedText.Brush, new Point(), formattedText);
				drawingContext.DrawText(XButton.Brush, new Point(Bounds.Width - XButton.Bounds.Width, 0), XButton);
			}
		}


		public WebViewTab()
			: this(new CustomWebView())
		{

		}

		//public WebViewTab(CefBrowserSettings settings, CefRequestContext requestContext)
		//	: this(new WebView(settings, requestContext))
		//{

		//}

		private WebViewTab(WebView webview)
		{
			webview.CreateWindow += Webview_CreateWindow;
			webview.DocumentTitleChanged += HandleDocumentTitleChanged;
			this.WebView = webview;
			this.Header = new WebViewTabTitle(this);
		}

		Type IStyleable.StyleKey
		{
			get { return typeof(TabItem); }
		}

		public string Title
		{
			get
			{
				return ((WebViewTabTitle)this.Header).Text;
			}
			set
			{
				((WebViewTabTitle)this.Header).Text = value;
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Content = this.WebView;
		}

		public void Close()
		{
			this.WebView.Close();

			var tabs = this.Parent as TabControl;
			if (tabs == null)
				return;
			((AvaloniaList<object>)tabs.Items).Remove(this);
		}

		private void HandleDocumentTitleChanged(object sender, DocumentTitleChangedEventArgs e)
		{
			this.Title = e.Title;
			//this.ToolTipText = e.Title;
		}

		public IChromiumWebView WebView { get; protected set; }

		public bool PopupHandlingDisabled { get; set; }

		private void Webview_CreateWindow(object sender, CreateWindowEventArgs e)
		{
			if (PopupHandlingDisabled)
				return;

			TabControl tabs = this.FindTabControl();
			if (tabs == null)
			{
				e.Cancel = true;
				return;
			}

			var avaloniaWindow = this.GetVisualRoot() as Window;
			if (avaloniaWindow == null)
				throw new InvalidOperationException("Window not found!");

			var webview = new CustomWebView((WebView)this.WebView);

			IPlatformHandle platformHandle = avaloniaWindow.PlatformImpl.Handle;
			if (platformHandle is IMacOSTopLevelPlatformHandle macOSHandle)
				e.WindowInfo.SetAsWindowless(macOSHandle.GetNSWindowRetained());
			else
				e.WindowInfo.SetAsWindowless(platformHandle.Handle);

			e.Client = webview.Client;
			OnCreateWindow(webview);
		}


		protected void OnCreateWindow(WebView webview)
		{
			var tab = new WebViewTab(webview);
			TabControl tabs = this.FindTabControl();
			((AvaloniaList<object>)tabs.Items).Add(tab);
			tabs.SelectedItem = tab;
		}
	}
}
