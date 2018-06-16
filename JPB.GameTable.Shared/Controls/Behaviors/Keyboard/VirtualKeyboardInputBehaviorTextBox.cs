using System.Windows.Input;
using Interceptor;
using JPB.GameTable.Shared.Helper;
using TextBox = System.Windows.Controls.TextBox;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class VirtualKeyboardInputBehaviorTextBox : VirtualKeyboardInputBehaviorBase<TextBox>
	{
		public VirtualKeyboardInputBehaviorTextBox()
		{
		}

		/// <inheritdoc />
		public override void ReciveKeyboardInput(VirtualKeys key, bool shift, bool control, KeyState state)
		{
			if (state != KeyState.Down && state != KeyState.E0)
			{
				return;
			}

			var cursorPosition = AssociatedObject.CaretIndex;
			if (key == VirtualKeys.Back)
			{
				if (AssociatedObject.Text.Length > cursorPosition - 1 && cursorPosition > 0)
				{
					AssociatedObject.Text = AssociatedObject.Text.Remove(cursorPosition - 1, 1);
					AssociatedObject.CaretIndex = cursorPosition - 1;
				}
			}
			else if (key == VirtualKeys.Left)
			{
				if (cursorPosition != 0)
				{
					AssociatedObject.CaretIndex = AssociatedObject.CaretIndex - 1;
				}
			}
			else if (key == VirtualKeys.Right )
			{
				if (cursorPosition < AssociatedObject.Text.Length)
				{
					AssociatedObject.CaretIndex = AssociatedObject.CaretIndex + 1;
				}
			}
			else if (key == VirtualKeys.Delete)
			{
				if (AssociatedObject.Text.Length > cursorPosition)
				{
					AssociatedObject.Text = AssociatedObject.Text.Remove(cursorPosition, 1);
					AssociatedObject.CaretIndex = cursorPosition;
				}
			}
			else
			{
				var toInsert = key.GetCharsFromKeys(shift, control);

				if (toInsert != null)
				{
					AssociatedObject.Text = AssociatedObject.Text.Insert(AssociatedObject.CaretIndex, toInsert);
					AssociatedObject.CaretIndex = cursorPosition + toInsert.Length;
				}
			}
		}
		
		protected override void OnAttached()
		{
			FocusManager.SetIsFocusScope(AssociatedObject, true);
		}

		protected override void OnDetaching()
		{
			FocusManager.SetIsFocusScope(AssociatedObject, false);
		}
	}
}