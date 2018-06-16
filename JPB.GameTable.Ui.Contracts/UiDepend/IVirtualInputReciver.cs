using Interceptor;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public interface IVirtualInputReciver
	{
		void ReciveKeyboardInput(VirtualKeys key, bool shift, bool control, KeyState state);
		bool ReciveMouseInput(MousePressedEventArgs mouseEvent);
	}
}