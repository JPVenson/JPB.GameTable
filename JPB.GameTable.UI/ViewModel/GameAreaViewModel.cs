#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Database.Entities;
using JPB.GameTable.UI.Dialogs.ViewModel;
using JPB.GameTable.UI.Models;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.GameTable.UI.ViewModel.SubSpaces;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

#endregion

namespace JPB.GameTable.UI.ViewModel
{
	public class GameAreaViewModel : AsyncViewModelBase, IGameArea
	{
		private readonly CommandService _commandService;
		private readonly MainWindowViewModel _parent;

		private AppUserModel _appUserModel;
		private SubSpaceBase _currentSubSpace;
		private Rect _gameTablePostion;
		private ThreadSaveObservableCollection<DelegateCommand> _globalCommands;
		private ThreadSaveObservableCollection<NamedDelegateCommand> _localCommands;
		private ThreadSaveObservableCollection<SubSpaceBase> _openSubSpaces;
		private RotateTransform _orientaion;
		private readonly SubSpaceManager _subSpaceManager;

		public GameAreaViewModel(MainWindowViewModel parent)
		{
			_parent = parent;
			GlobalCommands = new ThreadSaveObservableCollection<DelegateCommand>();
			_commandService = IoC.Resolve<CommandService>();
			_subSpaceManager = IoC.Resolve<SubSpaceManager>();
			_commandService.CommandsChanged += (sender, args) => CommandService_CommandsChanged();
			OpenSubSpaces = new ThreadSaveObservableCollection<SubSpaceBase>();
			LocalCommands = new ThreadSaveObservableCollection<NamedDelegateCommand>();
			Orientaion = new RotateTransform();
			GameTablePostion = new Rect(0, 0, 300, 400);
			LocalCommands.Add(new NamedDelegateCommand(CommandService.COMMAND_GROUP_GENERAL + ".Open Notes", f => OpenSubSpace("Notes"), f => CanOpenSubSpace("Notes")));
			LocalCommands.Add(new NamedDelegateCommand(CommandService.COMMAND_GROUP_GENERAL + ".Open Browser", f => OpenSubSpace("Browser"), f => CanOpenSubSpace("Browser")));
			LocalCommands.Add(new NamedDelegateCommand(CommandService.COMMAND_GROUP_GENERAL + ".Open Chat", f => OpenSubSpace("Chat"), f => CanOpenSubSpace("Chat")));
			LocalCommands.Add(new NamedDelegateCommand(CommandService.COMMAND_GROUP_GENERAL + ".Close", f => _parent.CloseArea(this),
			f => AppUserModel?.RoleModel != Roles.Admin));

			var loginViewModel = new SelectionDialogViewModel(f => f is AppUserModel, (d) =>
			{
				Load(d as AppUserModel).ContinueWith(OnTaskDone);
			});
			loginViewModel.AddRange(IoC.Resolve<DbEntities>().DbAccess.GetUsers().Select(e => new AppUserModel
			{
					Name = e.Name,
					RoleModel = Roles.Yield().First(f => f.Id == e.IdRole),
					Id = e.AppUserId
			}));

			IoC.Resolve<DialogService>().ShowDialog(new Dialog(this, "Login", loginViewModel));
		}

		private bool CanOpenSubSpace(string key)
		{
			var hasSubSpace = _subSpaceManager.SubSpaces.FirstOrDefault(e => e.Key.Key == key);

			return hasSubSpace.Key != null &&
			       (!hasSubSpace.Key.AllowMultible || this.OpenSubSpaces.Any(f => f.GetType() == hasSubSpace.Value));
		}

		private SubSpaceBase _selectedSubSpace;

		public SubSpaceBase SelectedSubSpace
		{
			get { return _selectedSubSpace; }
			set
			{
				SendPropertyChanging(() => SelectedSubSpace);
				_selectedSubSpace = value;
				SendPropertyChanged(() => SelectedSubSpace);
			}
		}

		public ThreadSaveObservableCollection<NamedDelegateCommand> LocalCommands
		{
			get { return _localCommands; }
			set
			{
				SendPropertyChanging(() => LocalCommands);
				_localCommands = value;
				SendPropertyChanged(() => LocalCommands);
			}
		}

		/// <inheritdoc />
		public int AppUserId
		{
			get { return AppUserModel.Id; }
		}

		public ThreadSaveObservableCollection<SubSpaceBase> OpenSubSpaces
		{
			get { return _openSubSpaces; }
			set
			{
				SendPropertyChanging(() => OpenSubSpaces);
				_openSubSpaces = value;
				SendPropertyChanged(() => OpenSubSpaces);
			}
		}

		public SubSpaceBase CurrentSubSpace
		{
			get { return _currentSubSpace; }
			set
			{
				SendPropertyChanging(() => CurrentSubSpace);
				_currentSubSpace = value;
				SendPropertyChanged(() => CurrentSubSpace);
			}
		}

		public ThreadSaveObservableCollection<DelegateCommand> GlobalCommands
		{
			get { return _globalCommands; }
			set
			{
				SendPropertyChanging(() => GlobalCommands);
				_globalCommands = value;
				SendPropertyChanged(() => GlobalCommands);
			}
		}

		public AppUserModel AppUserModel
		{
			get { return _appUserModel; }
			set
			{
				SendPropertyChanging(() => AppUserModel);
				_appUserModel = value;
				SendPropertyChanged(() => AppUserModel);
				CommandService_CommandsChanged();
			}
		}

		public RotateTransform Orientaion
		{
			get { return _orientaion; }
			set
			{
				SendPropertyChanging(() => Orientaion);
				_orientaion = value;
				SendPropertyChanged(() => Orientaion);
			}
		}

		public Rect GameTablePostion
		{
			get { return _gameTablePostion; }
			set
			{
				SendPropertyChanging(() => GameTablePostion);
				_gameTablePostion = value;
				SendPropertyChanged(() => GameTablePostion);
			}
		}

		private void OpenSubSpace(string key)
		{
			var subSpaceType = _subSpaceManager.SubSpaces.FirstOrDefault(e => e.Key.Key.Equals(key));

			if (subSpaceType.Key == null)
			{
				return;
			}

			var newSubSpace = Activator.CreateInstance(subSpaceType.Value) as SubSpaceBase;
			newSubSpace.Load(this);
			OpenSubSpaces.Add(newSubSpace);
		}

		private void CommandService_CommandsChanged()
		{
			GlobalCommands.Clear();
			foreach (var delegateCommand in _commandService.GetForRole(AppUserModel.RoleModel))
			{
				GlobalCommands.Add(delegateCommand);
			}

			SendPropertyChanged(() => GlobalCommands);
		}

		public async Task Load(AppUserModel user)
		{
			var inputInterceptor = IoC.Resolve<InputInterceptor>();
			AppUserModel = user;
			var dbEntities = IoC.Resolve<DbEntities>();
			await SimpleWork(() =>
			{
				var rect = dbEntities.DbAccess.GetUserEntry(user.Id, "UI.GAMEAREA.POSITION")?.Value;
				Rect gameTablePostion = Rect.Empty;
				if (rect != null)
				{
					gameTablePostion = Rect.Parse(rect);
				}

				int orientation;
				int.TryParse(dbEntities.DbAccess.GetUserEntry(user.Id, "UI.GAMEAREA.POSITION")?.Value, out orientation);

				ThreadSaveAction(() =>
				{
					Orientaion = new RotateTransform(orientation);
					GameTablePostion = gameTablePostion.IsEmpty ? new Rect(0, 0, 300, 400) : gameTablePostion;
					int mouseDeviceId;
					int keybordDeviceId;
					KeyValuePair<int, InputInterceptor.InputTuple> firstAvailbeMouse;
					if (int.TryParse(dbEntities.DbAccess.GetUserEntry(user.Id, "UI.GAMEAREA.VIRTUAL.MOUSE.DEVICE.ID")?.Value,
					out mouseDeviceId) && int.TryParse(
									   dbEntities.DbAccess.GetUserEntry(user.Id, "UI.GAMEAREA.VIRTUAL.KEYBOARD.DEVICE.ID")?.Value,
									   out keybordDeviceId)
									   && (firstAvailbeMouse =
											   inputInterceptor.Mice.FirstOrDefault(e => !e.Value.InUse && !e.Value.Natural)).Value != null
					)
					{
						AppUserModel.VirtualMouseDiviceId = mouseDeviceId;
						AppUserModel.VirtualKeyboardDeviceId = keybordDeviceId;
						firstAvailbeMouse.Value.InUse = true;
					}
					else
					{
						firstAvailbeMouse = inputInterceptor.Mice.FirstOrDefault(e => !e.Value.InUse && !e.Value.Natural);
						if (firstAvailbeMouse.Value == null)
						{
							if (Debugger.IsAttached)
							{
								firstAvailbeMouse = inputInterceptor.Mice.LastOrDefault();
							}
							else
							{
								return;
							}
						}

						AppUserModel.VirtualMouseDiviceId = firstAvailbeMouse.Value.MouseDevice;
						AppUserModel.VirtualKeyboardDeviceId = firstAvailbeMouse.Value.KeyboardDevice;
						firstAvailbeMouse.Value.InUse = true;
					}
				});
			});
		}

		public async Task Save()
		{
			if (AppUserModel == null)
			{
				return;
			}

			var dbEntities = IoC.Resolve<DbEntities>();
			var orientation = Orientaion.Angle.ToString();
			var postionString = GameTablePostion.ToString();
			await SimpleWork(() =>
			{
				foreach (var openSubSpace in OpenSubSpaces)
				{
					openSubSpace.Save(AppUserId);
				}

				dbEntities.DbAccess.UpdateOrCreateUserData(new UserDataEntity
				{
					IdAppUser = AppUserModel.Id,
					Key = "UI.GAMEAREA.POSITION",
					Value = postionString
				});

				dbEntities.DbAccess.UpdateOrCreateUserData(new UserDataEntity
				{
					IdAppUser = AppUserModel.Id,
					Key = "UI.GAMEAREA.ROTATION",
					Value = orientation
				});
				dbEntities.DbAccess.UpdateOrCreateUserData(new UserDataEntity
				{
					IdAppUser = AppUserModel.Id,
					Key = "UI.GAMEAREA.VIRTUAL.MOUSE.DEVICE.ID",
					Value = AppUserModel.VirtualMouseDiviceId.ToString()
				});
				dbEntities.DbAccess.UpdateOrCreateUserData(new UserDataEntity
				{
					IdAppUser = AppUserModel.Id,
					Key = "UI.GAMEAREA.VIRTUAL.KEYBOARD.DEVICE.ID",
					Value = AppUserModel.VirtualKeyboardDeviceId.ToString()
				});
			});
		}
	}
}