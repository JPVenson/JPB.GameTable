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
using CefSharp;
using JPB.GameTable.Ui.Subspace.Browser.Commands;
using JPB.GameTable.Ui.Subspace.Browser.ViewModel;
using JPB.WPFBase.MVVM.DelegateCommand;

namespace JPB.GameTable.Ui.Subspace.Browser.Resources
{
	/// <summary>
	/// Interaction logic for FullBrowser.xaml
	/// </summary>
	public partial class FullBrowser : UserControl
	{
		private const string DefaultUrlForAddedTabs = "https://www.google.com";

		public static readonly DependencyProperty ExecuteGenericCommandProperty = DependencyProperty.Register(
		"ExecuteGenericCommand", typeof(ICommand), typeof(FullBrowser), new PropertyMetadata(default(ICommand)));

		public ICommand ExecuteGenericCommand
		{
			get { return (ICommand) GetValue(ExecuteGenericCommandProperty); }
			set { SetValue(ExecuteGenericCommandProperty, value); }
		}

		public FullBrowser()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));

			CommandBindings.Add(new CommandBinding(CefSharpCommands.Exit, Exit));
			CommandBindings.Add(new CommandBinding(CefSharpCommands.OpenTabCommand, OpenTabCommandBinding));
			CommandBindings.Add(new CommandBinding(CefSharpCommands.CustomCommand, CustomCommandBinding));

			Loaded += MainWindowLoaded;

			var bitness = Environment.Is64BitProcess ? "x64" : "x86";
		}

		private void ExecuteGeneric(object o)
		{
			var param = o.ToString();

			if (param == "New")
			{
				CreateNewTab();
				TabControl.SelectedIndex = TabControl.Items.Count - 1;
			}

			if (DataContext.BrowserTabs.Count > 0)
			{
				var browserViewModel = DataContext.SelectedTab;
				if (param == "Close")
				{
					if (DataContext.BrowserTabs.Count > 0)
					{
						DataContext.BrowserTabs.Remove(browserViewModel);
						browserViewModel.WebBrowser.Dispose();
					}
				}

				if (param == "OpenDevTools")
				{
					browserViewModel.WebBrowser.ShowDevTools();
				}

				if (param == "ZoomIn")
				{
					var cmd = browserViewModel.WebBrowser.ZoomInCommand;
					cmd.Execute(null);
				}

				if (param == "ZoomOut")
				{
					var cmd = browserViewModel.WebBrowser.ZoomOutCommand;
					cmd.Execute(null);
				}

				if (param == "ZoomReset")
				{
					var cmd = browserViewModel.WebBrowser.ZoomResetCommand;
					cmd.Execute(null);
				}

				if (param == "ToggleDownloadInfo")
				{
					browserViewModel.ShowDownloadInfo = !browserViewModel.ShowDownloadInfo;
				}
			}
		}

		public BrowserSubSpaceViewModel DataContext
		{
			get
			{
				return base.DataContext as BrowserSubSpaceViewModel;
			}
		}

		private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
		{
			CreateNewTab();

			TabControl.SelectedIndex = TabControl.Items.Count - 1;
		}

		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			ExecuteGenericCommand = new DelegateCommand(f => ExecuteGeneric(f));
			//this.GetBindingExpression(ExecuteGenericCommandProperty)
			//                  .UpdateTarget();
			CreateNewTab(DefaultUrlForAddedTabs, false);
		}

		private void CreateNewTab(string url = DefaultUrlForAddedTabs, bool showSideBar = false)
		{
			DataContext.BrowserTabs.Add(new BrowserTabViewModel(url) { ShowSidebar = showSideBar });
		}

		private void CustomCommandBinding(object sender, ExecutedRoutedEventArgs e)
		{
			ExecuteGeneric(e.Parameter.ToString());
		}

		private void OpenTabCommandBinding(object sender, ExecutedRoutedEventArgs e)
		{
			var url = e.Parameter.ToString();

			if (string.IsNullOrEmpty(url))
			{
				throw new Exception("Please provide a valid command parameter for binding");
			}

			CreateNewTab(url, true);

			TabControl.SelectedIndex = TabControl.Items.Count - 1;
		}

		private void Exit(object sender, ExecutedRoutedEventArgs e)
		{
		}
	}
}
