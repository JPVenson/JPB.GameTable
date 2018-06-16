using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JPB.GameTable.Ui.Subspace.Browser.Commands;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.ViewModel.SubSpaces;
using JPB.WPFBase.MVVM.DelegateCommand;

namespace JPB.GameTable.Ui.Subspace.Browser.ViewModel
{
	public class BrowserSubSpaceViewModel : SubSpaceBase
	{
		public BrowserSubSpaceViewModel()
		{
			Title = "Browser";
			BrowserTabs = new ObservableCollection<BrowserTabViewModel>();
			Commands.Add(new NamedDelegateCommand("Browser.File.New", (f) => ExecuteCommandCommand.Execute("New"),
			f => ExecuteCommandCommand.CanExecute("New")));
			Commands.Add(new NamedDelegateCommand("Browser.File.Close", (f) => ExecuteCommandCommand.Execute("Close"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("Close")));

			Commands.Add(new NamedDelegateCommand("Browser.File.Zoom In", (f) => ExecuteCommandCommand.Execute("ZoomIn"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("ZoomIn")));
			Commands.Add(new NamedDelegateCommand("Browser.File.Zoom Out", (f) => ExecuteCommandCommand.Execute("ZoomOut"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("ZoomOut")));
			Commands.Add(new NamedDelegateCommand("Browser.File.Zoom Reset", (f) => ExecuteCommandCommand.Execute("ZoomReset"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("ZoomReset")));
			Commands.Add(new NamedDelegateCommand("Browser.View.Toggle Sidebar", (f) => ExecuteCommandCommand.Execute("ToggleSidebar"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("ToggleSidebar")));
			Commands.Add(new NamedDelegateCommand("Browser.View.Downlods", (f) => ExecuteCommandCommand.Execute("ToggleDownloadInfo"),
			f => SelectedTab != null && ExecuteCommandCommand.CanExecute("ToggleDownloadInfo")));
		}

		public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }

		private ICommand _executeCommandCommand;

		public ICommand ExecuteCommandCommand
		{
			get { return _executeCommandCommand; }
			set
			{
				SendPropertyChanging(() => ExecuteCommandCommand);
				_executeCommandCommand = value;
				SendPropertyChanged(() => ExecuteCommandCommand);
			}
		}

		private BrowserTabViewModel _selectedTab;

		public BrowserTabViewModel SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				SendPropertyChanging(() => SelectedTab);
				_selectedTab = value;
				SendPropertyChanged(() => SelectedTab);
			}
		}
	}
}
