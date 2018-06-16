using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Interceptor;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class VirtualKeyboardBehavior : Behavior<Canvas>
	{
		public static readonly DependencyProperty KeyboardDeviceIdProperty = DependencyProperty.Register(
		"KeyboardDeviceId", typeof(int), typeof(VirtualKeyboardBehavior), new PropertyMetadata(default(int)));

		private InputInterceptor _inputInterceptor;
		private GameAreaFocusService _gameAreaFocusService;

		public int KeyboardDeviceId
		{
			get { return (int)GetValue(KeyboardDeviceIdProperty); }
			set { SetValue(KeyboardDeviceIdProperty, value); }
		}

		public VirtualKeyboardBehavior()
		{

		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			_inputInterceptor = IoC.Resolve<InputInterceptor>();
			_gameAreaFocusService = IoC.Resolve<GameAreaFocusService>();
			_inputInterceptor.VirtualKeyboardAction += InputInterceptorOnVirtualKeyboardAction;
		}

		public bool Shift { get; set; }
		public bool Control { get; set; }

		private void InputInterceptorOnVirtualKeyboardAction(object sender, KeyPressedEventArgs keyPressedEventArgs)
		{

			if (keyPressedEventArgs.Key == VirtualKeys.Shift && keyPressedEventArgs.State == KeyState.Down)
			{
				Shift = true;
			}
			else if (keyPressedEventArgs.Key == VirtualKeys.Shift && keyPressedEventArgs.State == KeyState.Up)
			{
				Shift = false;
			}
			else if (keyPressedEventArgs.Key == VirtualKeys.Control && keyPressedEventArgs.State == KeyState.Down)
			{
				Control = true;
			}
			else if (keyPressedEventArgs.Key == VirtualKeys.Control && keyPressedEventArgs.State == KeyState.Up)
			{
				Control = false;
			}

			var mappedMouse = _inputInterceptor.Keyboards[keyPressedEventArgs.DeviceId].MouseDevice;
			if (_gameAreaFocusService.FocusedElements.ContainsKey(mappedMouse))
			{
				var dependencyObject = _gameAreaFocusService.FocusedElements[mappedMouse];
				if (dependencyObject != null)
				{
					Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
				  {
					  var firstOrDefault = Interaction.GetBehaviors(dependencyObject).FirstOrDefault(e => e is IVirtualInputReciver);
					  if (firstOrDefault is IVirtualInputReciver)
					  {
						  (firstOrDefault as IVirtualInputReciver).ReciveKeyboardInput(keyPressedEventArgs.Key, Shift, Control, keyPressedEventArgs.State);
					  }
				  }));
				}
			}
		}
	}
}
