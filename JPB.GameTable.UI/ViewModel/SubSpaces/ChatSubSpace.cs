using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using JPB.GameTable.Shared.Services;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.Dialogs.ViewModel;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.WPFBase.MVVM.DelegateCommand;

namespace JPB.GameTable.UI.ViewModel.SubSpaces
{
	[SubSpace("Chat", AllowMultible = false)]
	public class ChatSubSpace : SubSpaceBase
	{
		public ChatSubSpace()
		{
			Title = "Group Chat";
			_chatService = IoC.Resolve<ChatService>();
			SendMessageCommand = new DelegateCommand(SendMessageExecute, CanSendMessageExecute);
			Commands.Add(new NamedDelegateCommand("Chat.Add Group",
			() =>
			{
				IoC.Resolve<DialogService>().ShowDialog(new Dialog(GameArea, "AddGroup",
				new InputDialogViewModel(f => true, (f) => _chatService.AddGroup(f, GameArea.AppUserId))
				{
						Message = "Please enter the Name for the Group",
						Title = "Add Group"
				}));
			}));

			Commands.Add(new NamedDelegateCommand("Chat.Add User",
			() =>
			{
				IoC.Resolve<DialogService>().ShowDialog(new Dialog(GameArea, "AddGroup",
				new InputDialogViewModel(f => true, (f) => _chatService.AddGroup(f, GameArea.AppUserId))));
			}, () => SelectedChatGroup?.Entity.ChatGroupId != 1));

			Commands.Add(new NamedDelegateCommand("Chat.Leave Group",
			() =>
			{
				_chatService.AddMessage(new ChatMessageEntity()
				{
						Color = "Red",
						DateSend = DateTime.Now,
						IdChatGroup = SelectedChatGroup.Entity.ChatGroupId,
						Message = "Has left the Group",
						IdUser = GameArea.AppUserId
				});
				_chatService.LeaveGroup(SelectedChatGroup.Entity.ChatGroupId, GameArea.AppUserId);
			}, () => SelectedChatGroup?.Entity.ChatGroupId != 1));
			UserColor = Colors.Black;
		}

		private Color _userColor;

		public Color UserColor
		{
			get { return _userColor; }
			set
			{
				SendPropertyChanging(() => UserColor);
				_userColor = value;
				SendPropertyChanged(() => UserColor);
			}
		}

		/// <inheritdoc />
		public override void Load(IGameArea gameAreaViewModel)
		{
			base.Load(gameAreaViewModel);
			ChatGroups = _chatService.GetGroupsForUser(gameAreaViewModel.AppUserId);
		}

		public DelegateCommand SendMessageCommand { get; private set; }

		private void SendMessageExecute(object sender)
		{
			_chatService.AddMessage(new ChatMessageEntity()
			{
					IdChatGroup = SelectedChatGroup.Entity.ChatGroupId,
					DateSend = DateTime.Now,
					Message = SendMessageTo,
					IdUser = base.GameArea.AppUserId,
					Color = UserColor.ToString()
			});
			SendMessageTo = string.Empty;
		}

		private bool CanSendMessageExecute(object sender)
		{
			return !string.IsNullOrWhiteSpace(SendMessageTo) && SelectedChatGroup != null;
		}

		private ChatService _chatService;
		private ChatGroupViewModel _selectedChatGroup;

		public ChatGroupViewModel SelectedChatGroup
		{
			get { return _selectedChatGroup; }
			set
			{
				SendPropertyChanging(() => SelectedChatGroup);
				_selectedChatGroup = value;
				SendPropertyChanged(() => SelectedChatGroup);
			}
		}

		private ICollectionView _chatGroups;

		public ICollectionView ChatGroups
		{
			get { return _chatGroups; }
			set
			{
				SendPropertyChanging(() => ChatGroups);
				_chatGroups = value;
				SendPropertyChanged(() => ChatGroups);
			}
		}

		private string _sendMessageTo;

		public string SendMessageTo
		{
			get { return _sendMessageTo; }
			set
			{
				SendPropertyChanging(() => SendMessageTo);
				_sendMessageTo = value;
				SendPropertyChanged(() => SendMessageTo);
			}
		}
	}
}
