using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JPB.GameTable.UI.Resources.Controls
{
	/// <summary>
	/// Interaction logic for VirtualButton.xaml
	/// </summary>
	public partial class VirtualButton : Button
	{
		public VirtualButton()
		{
			InitializeComponent();
		}


		public static readonly RoutedEvent VirtualClickEvent =
				EventManager.RegisterRoutedEvent("VirtualClick", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(VirtualButton));

		public void RaiseVirtualClick(Point position)
		{
			RaiseEvent(new VirtualClickEventArgs(VirtualClickEvent, this, position));
		}
	}

	public class VirtualClickEventArgs : RoutedEventArgs
	{
		public VirtualClickEventArgs(Point position)
		{
			Position = position;
		}

		public VirtualClickEventArgs(RoutedEvent routedEvent, Point position) : base(routedEvent)
		{
			Position = position;
		}

		public VirtualClickEventArgs(RoutedEvent routedEvent, object source, Point position) : base(routedEvent, source)
		{
			Position = position;
		}

		public Point Position { get; private set; }
	}
}
