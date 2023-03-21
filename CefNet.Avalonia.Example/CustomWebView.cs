using Avalonia.Interactivity;
using CefNet.Avalonia;
using CefNet.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApp
{
	sealed class CustomWebView : WebView
	{
		public static RoutedEvent<FullscreenModeChangeEventArgs> FullscreenEvent = RoutedEvent.Register<WebView, FullscreenModeChangeEventArgs>("Fullscreen", RoutingStrategies.Bubble);

		public event EventHandler<FullscreenModeChangeEventArgs> Fullscreen
		{
			add { AddHandler(FullscreenEvent, value); }
			remove { RemoveHandler(FullscreenEvent, value); }
		}

		public CustomWebView()
		{

		}

		public CustomWebView(WebView opener)
			: base(opener)
		{

		}

		protected override WebViewGlue CreateWebViewGlue()
		{
			return new CustomWebViewGlue(this);
		}



		internal void RaiseFullscreenModeChange(bool fullscreen)
		{
			RaiseCrossThreadEvent(OnFullScreenModeChange, new FullscreenModeChangeEventArgs(this, fullscreen), false);
		}

		private void OnFullScreenModeChange(FullscreenModeChangeEventArgs e)
		{
			RaiseEvent(e);
		}


	}
}
