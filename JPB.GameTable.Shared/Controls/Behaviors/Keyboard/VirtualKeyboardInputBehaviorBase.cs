using System.Windows;
using System.Windows.Interactivity;
using Interceptor;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public abstract class VirtualKeyboardInputBehaviorBase<T> : Behavior<T>, IVirtualInputReciver where T : DependencyObject
	{
		public virtual void ReciveKeyboardInput(VirtualKeys key, bool shift, bool control, KeyState state)
		{

		}

		/// <inheritdoc />
		public virtual bool ReciveMouseInput(MousePressedEventArgs mouseEvent)
		{
			return false;
		}
	}
}
