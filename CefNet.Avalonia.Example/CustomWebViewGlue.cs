using Avalonia.Media;
using CefNet;
using CefNet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AvaloniaApp
{
	internal sealed class CustomWebViewGlue : AvaloniaWebViewGlue
	{
		private const int SHOW_DEV_TOOLS = (int)CefMenuId.UserFirst + 0;
		private const int INSPECT_ELEMENT = (int)CefMenuId.UserFirst + 1;

		public CustomWebViewGlue(CustomWebView view)
			: base(view)
		{

		}

		private new CustomWebView WebView
		{
			get { return (CustomWebView)base.WebView; }
		}

		protected override bool OnSetFocus(CefBrowser browser, CefFocusSource source)
		{
			if (source == CefFocusSource.Navigation)
				return true;
			return false;
		}

		protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams menuParams, CefMenuModel model)
		{
			model.InsertItemAt(model.Count > 0 ? 1 : 0, (int)CefMenuId.ReloadNocache, "Refresh");
			model.AddSeparator();
			model.AddItem(SHOW_DEV_TOOLS, "&Show DevTools");
			model.AddItem(INSPECT_ELEMENT, "Inspect element");


			CefMenuModel submenu = model.AddSubMenu(0, "Submenu Test");
			submenu.AddItem((int)CefMenuId.Copy, "Copy");
			submenu.AddItem((int)CefMenuId.Paste, "Paste");
			submenu.SetColorAt((int)submenu.Count - 1, CefMenuColorType.Text, CefColor.FromArgb((int)Colors.Blue.ToUint32()));
			submenu.AddCheckItem(0, "Checked Test");
			submenu.SetCheckedAt(submenu.Count - 1, true);
			submenu.AddRadioItem(0, "Radio Off", 0);
			submenu.AddRadioItem(0, "Radio On", 1);
			submenu.SetCheckedAt(submenu.Count - 1, true);
		}

		protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams menuParams, int commandId, CefEventFlags eventFlags)
		{
			if (commandId >= (int)CefMenuId.UserFirst && commandId <= (int)CefMenuId.UserLast)
			{
				switch (commandId)
				{
					case SHOW_DEV_TOOLS:
						((CustomWebView)WebView).ShowDevTools();
						break;
					case INSPECT_ELEMENT:
						((CustomWebView)WebView).ShowDevTools(new CefPoint(menuParams.XCoord, menuParams.YCoord));
						break;
				}
				return true;
			}
			return false; ;
		}

		protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
		{
			WebView.RaiseFullscreenModeChange(fullscreen);
		}

		protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
		{
			Debug.Print("[{0}]: {1} ({2}, line: {3})", level, message, source, line);
			return false;
		}
	}
}
