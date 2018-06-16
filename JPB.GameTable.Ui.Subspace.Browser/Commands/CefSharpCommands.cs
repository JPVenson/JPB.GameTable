using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JPB.GameTable.Ui.Subspace.Browser.Commands
{
	public static class CefSharpCommands
	{
		public static RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CefSharpCommands));
		public static RoutedUICommand OpenTabCommand = new RoutedUICommand("OpenTabCommand", "OpenTabCommand", typeof(CefSharpCommands));
		public static RoutedUICommand PrintTabToPdfCommand = new RoutedUICommand("PrintTabToPdfCommand", "PrintTabToPdfCommand", typeof(CefSharpCommands));
		public static RoutedUICommand CustomCommand = new RoutedUICommand("CustomCommand", "CustomCommand", typeof(CefSharpCommands));
	}
}
