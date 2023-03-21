using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApp
{
	public class FullscreenModeChangeEventArgs : RoutedEventArgs
	{
		public FullscreenModeChangeEventArgs(IInteractive source, bool fullscreen)
			: base(CustomWebView.FullscreenEvent, source)
		{
			this.Fullscreen = fullscreen;
		}

		public bool Fullscreen { get; }
	}
}
