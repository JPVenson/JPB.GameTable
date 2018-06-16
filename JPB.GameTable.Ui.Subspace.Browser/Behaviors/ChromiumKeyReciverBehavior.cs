#region

using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using Interceptor;
using JPB.GameTable.Shared.Helper;
using JPB.GameTable.UI.Helper.Win32;
using JPB.GameTable.UI.Resources;
using JPB.GameTable.UI.Resources.Behaviors;
using JPB.GameTable.UI.Unity;

#endregion

namespace JPB.GameTable.Ui.Subspace.Browser.Behaviors
{
	public class ChromiumKeyReciverBehavior : Behavior<ChromiumWebBrowser>, IVirtualInputReciver
	{
		private GameAreaFocusService _gameAreaFocusService;
		IBrowserHost browserHost;

		public ChromiumKeyReciverBehavior()
		{
			_gameAreaFocusService = IoC.Resolve<GameAreaFocusService>();
			WindowInteropHelper = new WindowInteropHelper(Application.Current.MainWindow);
		}

		public WindowInteropHelper WindowInteropHelper { get; set; }

		/// <inheritdoc />
		protected override void OnAttached()
		{
		}

		public void ReciveKeyboardInput(VirtualKeys key, bool shift, bool control, KeyState state)
		{
			browserHost = browserHost ?? AssociatedObject.GetBrowser().GetHost();
			var keyChar = key.GetControlKeyMessages(state);

			foreach (var keyCode in keyChar)
			{
				browserHost.SendKeyEvent(keyCode.message, (int) keyCode.wParam, (int) keyCode.lParam);
			}

			//AssociatedObject.WpfKeyboardHandler.HandleTextInput(new TextCompositionEventArgs(InputManager.Current.MostRecentInputDevice,
			//new TextComposition(InputManager.Current, AssociatedObject, keyToString)));
		}

		/// <inheritdoc />
		public bool ReciveMouseInput(MousePressedEventArgs mouseEvent)
		{
			browserHost = browserHost ?? AssociatedObject.GetBrowser().GetHost();
			if (!_gameAreaFocusService.Conatainer.ContainsKey(mouseEvent.DeviceId))
			{
				return false;
			}

			var dependencyObject = _gameAreaFocusService.Conatainer[mouseEvent.DeviceId];
			if (dependencyObject == null)
			{
				return false;
			}

			return AssociatedObject.Dispatcher.Invoke(() =>
			{
				var translatePoint = dependencyObject.TranslatePoint(new Point(mouseEvent.AbsolutX, mouseEvent.AbsolutY),
				AssociatedObject);
				var chromeMouseEvent = new MouseEvent((int) translatePoint.X, (int) translatePoint.Y, CefEventFlags.None);

				MouseButtonType mouseType = MouseButtonType.Left;
				var mouseUp = false;

				switch (mouseEvent.State)
				{
					case MouseState.Undefined:
						browserHost.SendMouseMoveEvent(chromeMouseEvent, false);
						return true;
					case MouseState.LeftDown:
						mouseType = MouseButtonType.Left;
						break;
					case MouseState.LeftUp:
						mouseUp = true;
						mouseType = MouseButtonType.Left;
						break;
					case MouseState.RightDown:
						mouseType = MouseButtonType.Right;
						break;
					case MouseState.RightUp:
						mouseUp = true;
						mouseType = MouseButtonType.Right;
						break;
					case MouseState.MiddleDown:
						mouseType = MouseButtonType.Middle;
						break;
					case MouseState.MiddleUp:
						mouseUp = true;
						mouseType = MouseButtonType.Middle;
						break;
					case MouseState.LeftExtraDown:
						mouseType = MouseButtonType.Left;
						break;
					case MouseState.LeftExtraUp:
						mouseUp = true;
						mouseType = MouseButtonType.Left;
						break;
					case MouseState.RightExtraDown:
						mouseType = MouseButtonType.Right;
						break;
					case MouseState.RightExtraUp:
						mouseUp = true;
						mouseType = MouseButtonType.Right;
						break;
					default:
						return false;
						throw new ArgumentOutOfRangeException();
				}

				browserHost.SendMouseClickEvent(chromeMouseEvent, mouseType, mouseUp, 1);
				return true;
			}, DispatcherPriority.Send);
		}
	}
}