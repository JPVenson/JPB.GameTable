using System.Linq;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Models;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.GameTable.UI.ViewModel;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Dialogs.ViewModel
{
	public class LoginDialogViewModel : DialogViewModelBase
	{
		private readonly GameAreaViewModel _gameAreaViewModel;

		public LoginDialogViewModel(GameAreaViewModel gameAreaViewModel)
		{
			_gameAreaViewModel = gameAreaViewModel;
			AppUsers = new ThreadSaveObservableCollection<AppUserModel>();
			LoginCommand = new DelegateCommand(LoginExecute, CanLoginExecute);

			base.SimpleWork(() =>
			{
				var dbEntities = IoC.Resolve<DbEntities>();
				var users = dbEntities.DbAccess.GetUsers();
				foreach (var user in users.Select(e => new AppUserModel()
				{
						Name = e.Name,
						RoleModel = Roles.Yield().First(f => f.Id == e.IdRole),
						Id = e.AppUserId
				}))
				{
					AppUsers.Add(user);
				}
			});
		}

		private ThreadSaveObservableCollection<AppUserModel> _appUsers;
		private AppUserModel _selectedUser;

		public AppUserModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				SendPropertyChanging(() => SelectedUser);
				_selectedUser = value;
				SendPropertyChanged(() => SelectedUser);
			}
		}

		public DelegateCommand LoginCommand { get; private set; }

		/// <inheritdoc />
		protected override bool CanCloseExecute(object sender)
		{
			return false;
		}

		private void LoginExecute(object sender)
		{
			_gameAreaViewModel.Load(SelectedUser).ContinueWith(e =>
			{
				IoC.Resolve<DialogService>().CloseDialog(this);
			});
		}

		private bool CanLoginExecute(object sender)
		{
			return true;
		}

		public ThreadSaveObservableCollection<AppUserModel> AppUsers
		{
			get { return _appUsers; }
			set
			{
				SendPropertyChanging(() => AppUsers);
				_appUsers = value;
				SendPropertyChanged(() => AppUsers);
			}
		}
	}
}