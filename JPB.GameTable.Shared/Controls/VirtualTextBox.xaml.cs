using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JPB.GameTable.UI.Resources.Behaviors;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace JPB.GameTable.UI.Resources.Controls
{
	/// <summary>
	/// Interaction logic for VirtualTextBox.xaml
	/// </summary>
	public partial class VirtualTextBox : TextBox
	{
		public VirtualTextBox()
		{
			InitializeComponent();
			SelectionChanged += (sender, e) => MoveCustomCaret();
			
			Interaction.GetBehaviors(this).Add(new VirtualKeyboardInputBehaviorTextBox());
			//LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;
			//GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
		"IsVisible", typeof(bool), typeof(VirtualTextBox), new PropertyMetadata(default(bool), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			(dependencyObject as UIElement).Visibility = dependencyPropertyChangedEventArgs.NewValue.Equals(true)
							? Visibility.Visible : Visibility.Collapsed;
		}

		public bool IsVisible
		{
			get { return (bool) GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		
		/// <summary>
		/// Moves the custom caret on the canvas.
		/// </summary>
		private void MoveCustomCaret()
		{
			var textBox = VisualTreeHelperEx.FindDescendantByName(this, "FakeBox") as TextBox;

			var caretLocation = textBox.GetRectFromCharacterIndex(CaretIndex).Location;
			
			var border = VisualTreeHelperEx.FindDescendantByName(this, "Caret") as UIElement;

			if (!double.IsInfinity(caretLocation.X))
			{
				Canvas.SetLeft(border, caretLocation.X);
			}

			if (!double.IsInfinity(caretLocation.Y))
			{
				Canvas.SetTop(border, caretLocation.Y);
			}
		}
	}
}
