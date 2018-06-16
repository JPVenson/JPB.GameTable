#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using JPB.DataAccess.Query.Operators;
using JPB.GameTable.Ui.Subspace.Browser;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.GameTable.UI.ViewModel;
using Application = System.Windows.Application;

#endregion

namespace JPB.GameTable.UI
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool _closingOK;

		public MainWindow()
		{
			AcceptedDevices = new Dictionary<int, string>();
			var screenNo = int.Parse(ConfigurationManager.AppSettings["Screen"]);
			ShowOnMonitor(screenNo, this);

			InitializeComponent();
			DataContext = new MainWindowViewModel();
			//GetMice();
		}

		public IDictionary<int, string> AcceptedDevices { get; set; }
		
		/// <inheritdoc />
		protected override void OnClosing(CancelEventArgs e)
		{
			if (_closingOK)
			{
				base.OnClosing(e);
				return;
			}

			e.Cancel = true;
			var vm = DataContext;
			DataContext = "Please wait ...";
			IoC.Resolve<CommandService>().OnSaveRequested(this).ContinueWith(f =>
			{
				Console.WriteLine(vm);
				IoC.Resolve<DbEntities>().DbAccess.Save();
				e.Cancel = false;
				_closingOK = true;
				Application.Current.Dispatcher.Invoke(Close);
			});
		}

		private void ShowOnMonitor(int monitor, Window window)
		{
			var screen = ScreenHandler.GetScreen(monitor);
			var currentScreen = ScreenHandler.GetCurrentScreen(this);
			window.WindowState = WindowState.Normal;
			window.Left = screen.WorkingArea.Left;
			window.Top = screen.WorkingArea.Top;
			window.Width = screen.WorkingArea.Width;
			window.Height = screen.WorkingArea.Height;
			window.Loaded += Window_Loaded;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var senderWindow = sender as Window;
			senderWindow.WindowState = WindowState.Maximized;



			new ChromeBrowserModule().OnStart();
		}
	}
}