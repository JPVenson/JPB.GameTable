#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Threading;
using Interceptor;
using JPB.GameTable.UI.Helper.Win32;
using JPB.GameTable.UI.Unity;

#endregion

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class VirtualMouseBehavior : Behavior<Canvas>
	{
		public static readonly DependencyProperty MouseDeviceIdProperty = DependencyProperty.Register(
		"MouseDeviceId", typeof(int), typeof(VirtualMouseBehavior), new PropertyMetadata(default(int), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			(
					dependencyObject as VirtualMouseBehavior).SetVirtualMouseId(dependencyPropertyChangedEventArgs.NewValue is int ? (int)dependencyPropertyChangedEventArgs.NewValue : 0);
		}

		private void SetVirtualMouseId(int newId)
		{
			var gameAreaFocusService = IoC.Resolve<GameAreaFocusService>();
			_mouseId = newId;
			gameAreaFocusService.Conatainer.AddOrUpdate(_mouseId, LayoutBounds, (i, o) => LayoutBounds);
		}

		public static readonly DependencyProperty VirtualMouseProperty = DependencyProperty.Register(
		"VirtualMouse", typeof(FrameworkElement), typeof(VirtualMouseBehavior),
		new PropertyMetadata(default(FrameworkElement)));

		public static readonly DependencyProperty LayoutBoundsProperty = DependencyProperty.Register(
		"LayoutBounds", typeof(FrameworkElement), typeof(VirtualMouseBehavior),
		new PropertyMetadata(default(FrameworkElement)));

		public static readonly DependencyProperty RenderCanvasProperty = DependencyProperty.RegisterAttached(
		"RenderCanvas", typeof(string), typeof(Window), new PropertyMetadata(default(string)));

		public static void SetRenderCanvas(DependencyObject element, string value)
		{
			element.SetValue(RenderCanvasProperty, value);
		}

		public static string GetRenderCanvas(DependencyObject element)
		{
			return (string)element.GetValue(RenderCanvasProperty);
		}

		InputInterceptor _inputInterceptor;

		private int _mouseId;
		WindowInteropHelper _wih;
		IntPtr _windowHandle;

		static VirtualMouseBehavior()
		{

		}

		public FrameworkElement LayoutBounds
		{
			get { return (FrameworkElement)GetValue(LayoutBoundsProperty); }
			set { SetValue(LayoutBoundsProperty, value); }
		}

		public FrameworkElement VirtualMouse
		{
			get { return (FrameworkElement)GetValue(VirtualMouseProperty); }
			set { SetValue(VirtualMouseProperty, value); }
		}

		public int MouseDeviceId
		{
			get { return (int)GetValue(MouseDeviceIdProperty); }
			set
			{
				SetValue(MouseDeviceIdProperty, value);
				_mouseId = value;
			}
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			_inputInterceptor = IoC.Resolve<InputInterceptor>();
			_inputInterceptor.VirtualMouseAction += InputInterceptorVirtualMouseAction;
			VirtualMouse.KeyDown += VirtualMouse_KeyDown;
			VirtualMouse.KeyUp += VirtualMouse_KeyDown;
			_wih = new WindowInteropHelper(Application.Current.MainWindow);
			_windowHandle = _wih.EnsureHandle();
		}

		private void VirtualMouse_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = false;
		}

		private void InputInterceptorVirtualMouseAction(object sender, MousePressedEventArgs e)
		{
			if (e.DeviceId != _mouseId)
			{
				return;
			}

			MousePressedEventArgs lastKnown = null;
			Point worldCoodinates = new Point();
			Application.Current.Dispatcher.Invoke(() =>
			{
				lastKnown = _inputInterceptor.Mice[e.DeviceId].LastMouseData;
				var destination = new Point(Canvas.GetLeft(VirtualMouse) + lastKnown.X, Canvas.GetTop(VirtualMouse) + lastKnown.Y);

				if (CheckBounds(lastKnown.X, lastKnown.Y))
				{
					Canvas.SetTop(VirtualMouse, destination.Y);
					Canvas.SetLeft(VirtualMouse, destination.X);
					lastKnown.AbsolutX = (int)destination.X;
					lastKnown.AbsolutY = (int)destination.Y;
				}

				if (lastKnown.State == MouseState.Undefined)
				{
					return;
				}

				worldCoodinates = AssociatedObject.TranslatePoint(destination, Application.Current.MainWindow);
			}, DispatcherPriority.Send);

			bool handeld = false;
			var elementsUnderCursor = new List<DependencyObject>();
			Application.Current.Dispatcher.Invoke(() =>
			{
				var positon = new Point(lastKnown.AbsolutX, lastKnown.AbsolutY);
				if (!GameAreaFocusService.GetExactHitTest(LayoutBounds, LayoutBounds, positon, elementsUnderCursor))
				{
					return;
				}

				var findScrollProvider = elementsUnderCursor
				                         .Select(GameAreaFocusService.GetVisualParentWithKeyHandling)
				                         .FirstOrDefault(f => f != null);
				if (findScrollProvider == null)
				{
					return;
				}

				var firstOrDefault = Interaction.GetBehaviors(findScrollProvider).FirstOrDefault(f => f is IVirtualInputReciver) as IVirtualInputReciver;
				handeld = firstOrDefault.ReciveMouseInput(lastKnown);
			}, DispatcherPriority.Send);
			if (handeld)
			{
				return;
			}
			if (lastKnown.State == MouseState.Undefined)
			{
				return;
			}
			var btnMessages = new MSG[0];

			if (lastKnown.State == MouseState.LeftDown || lastKnown.State == MouseState.LeftUp)
			{
				btnMessages = SendMessageApi.GetControlClickMessages("left", lastKnown.State == MouseState.LeftDown,
				(int) worldCoodinates.X, (int) worldCoodinates.Y);
			}
			else if (lastKnown.State == MouseState.RightDown || lastKnown.State == MouseState.RightUp)
			{
				btnMessages = SendMessageApi.GetControlClickMessages("right", lastKnown.State == MouseState.RightDown,
				(int) worldCoodinates.X, (int) worldCoodinates.Y);
			}
			else if (lastKnown.State == MouseState.MiddleDown || lastKnown.State == MouseState.MiddleUp)
			{
				btnMessages = SendMessageApi.GetControlClickMessages("middle", lastKnown.State == MouseState.MiddleDown,
				(int) worldCoodinates.X, (int) worldCoodinates.Y);
			}
			else if (lastKnown.State == MouseState.ScrollDown)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					var findScrollProvider = GameAreaFocusService.GetVisualParent<ScrollViewer>(elementsUnderCursor.Last());
					if (findScrollProvider != null)
					{
						findScrollProvider.ScrollToVerticalOffset(lastKnown.Rolling);
					}
					else
					{
						var dependencyObject = elementsUnderCursor.Last();
						var mouseWheelEventArgs = new MouseWheelEventArgs(InputManager.Current.PrimaryMouseDevice, 0, lastKnown.Rolling);

						mouseWheelEventArgs.RoutedEvent = UIElement.MouseWheelEvent;
						mouseWheelEventArgs.Source = this;
						(dependencyObject as UIElement).RaiseEvent(mouseWheelEventArgs);
					}
				}, DispatcherPriority.Send);
			}

			foreach (var msg in btnMessages)
			{
				SendMessageApi.SendMessageTo(_windowHandle, msg.message, msg.wParam, msg.lParam);
			}
		}

		private bool CheckBounds(int x, int y)
		{
			var currentPostion = new Point(Canvas.GetLeft(VirtualMouse) + x, Canvas.GetTop(VirtualMouse) + y);
			return new Rect(new Point(0, 0), LayoutBounds.RenderSize).Contains(currentPostion);
		}
	}
}