#region

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JPB.GameTable.UI.Dialogs.ViewModel;
using JPB.GameTable.UI.Models;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

#endregion

namespace JPB.GameTable.UI.ViewModel
{
	public class MainWindowViewModel : AsyncViewModelBase
	{
		public MainWindowViewModel()
		{
			GameAreaViewModels = new ThreadSaveObservableCollection<GameAreaViewModel>();
			CreateNewGameAreaCommand = new NamedDelegateCommand(CommandService.COMMAND_GROUP_ADMIN + ".Create new Area", CreateNewGameAreaExecute, CanCreateNewGameAreaExecute);
			MoveWindowCommand = new NamedDelegateCommand(CommandService.COMMAND_GROUP_ADMIN + ".Move Area", MoveWindowExecute, CanMoveWindowExecute);
			CloseProgramCommand = new NamedDelegateCommand(CommandService.COMMAND_GROUP_ADMIN + ".Close App", CloseProgramExecute, CanCloseProgramExecute);
			var commandService = IoC.Resolve<CommandService>();
			commandService.AddCommand(Roles.Admin, CreateNewGameAreaCommand);
			commandService.AddCommand(Roles.Admin, MoveWindowCommand);
			commandService.AddCommand(Roles.Admin, CloseProgramCommand);

			GameAreaViewModels.Add(new GameAreaViewModel(this));

			commandService.SaveRequested += Handler;
		}

		private async Task Handler()
		{
			foreach (var gameAreaViewModel in GameAreaViewModels)
			{
				await gameAreaViewModel.Save();
			}
		}

		public ThreadSaveObservableCollection<GameAreaViewModel> GameAreaViewModels { get; set; }

		public NamedDelegateCommand CreateNewGameAreaCommand { get; private set; }
		public NamedDelegateCommand MoveWindowCommand { get; private set; }
		public NamedDelegateCommand CloseProgramCommand { get; private set; }

		private void CloseProgramExecute(object sender)
		{
			IoC.Resolve<DialogService>().ShowDialog(new Dialog(Admins(), "Close game", new MessageBoxDialog("Close Game", "Please confirm", MessageBoxButton.OKCancel,
			(dialog, result) =>
			{
				if (result == MessageBoxResult.OK)
				{
					App.Current.MainWindow.Close();
				}

			})));
		}

		private GameAreaViewModel Admins()
		{
			return GameAreaViewModels.First(e => e.AppUserModel.RoleModel == Roles.Admin);
		}

		private bool CanCloseProgramExecute(object sender)
		{
			return true;
		}

		private void MoveWindowExecute(object sender)
		{
			IoC.Resolve<DialogService>().ShowDialog(new Dialog(Admins(), "Postion Dialog", new PositioningDialogViewModel()
			{
				Title = "Position Dialog",
				GameAreas = GameAreaViewModels
			}));
		}

		private bool CanMoveWindowExecute(object sender)
		{
			return true;
		}

		private void CreateNewGameAreaExecute(object sender)
		{
			var newGameArea = new GameAreaViewModel(this)
			{
				GameTablePostion = new Rect(0, 0, 400, 200),
				Orientaion = new RotateTransform(0),
			};
			GameAreaViewModels.Add(newGameArea);
		}

		private bool CanCreateNewGameAreaExecute(object sender)
		{
			return true;
		}

		public void CloseArea(GameAreaViewModel gameAreaViewModel)
		{
			gameAreaViewModel.Save().ContinueWith(e =>
			{
				GameAreaViewModels.Remove(gameAreaViewModel);
			});
		}
	}

	public class MainWindowViewModelDesign : MainWindowViewModel
	{
		public MainWindowViewModelDesign()
		{

		}
	}
}