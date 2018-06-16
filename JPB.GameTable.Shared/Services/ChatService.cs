using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Database.Entities;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.Shared.Services
{
	public class ChatService
	{
		public ChatService()
		{
			_db = IoC.Resolve<DbEntities>();
			ChatGroups = new ObservableCollection<ChatGroupViewModel>();
			CollectionViews = new ConcurrentDictionary<int, ICollectionView>();
			var users = _db.DbAccess.GetUsers();
			foreach (var chatGroupEntity in _db.DbAccess.GetChatGroups())
			{
				var chatGroupViewModel = new ChatGroupViewModel(chatGroupEntity);
				ChatGroups.Add(chatGroupViewModel);
				
				foreach (var dbAccessChatMessage in _db.DbAccess.GetChatMessages(chatGroupViewModel.Entity.ChatGroupId))
				{
					chatGroupViewModel.Messages.Add(new ChatMessageEntityViewModel()
					{
							Entity = dbAccessChatMessage,
							User = users.FirstOrDefault(e => e.AppUserId == dbAccessChatMessage.IdUser)
					});
				}
			}
		}

		private DbEntities _db;

		public ObservableCollection<ChatGroupViewModel> ChatGroups { get; set; }
		public IDictionary<int, ICollectionView> CollectionViews { get; set; }

		public ICollectionView GetGroupsForUser(int userId)
		{
			if (CollectionViews.ContainsKey(userId))
			{
				return CollectionViews[userId];
			}

			var defaultView = CollectionViewSource.GetDefaultView(ChatGroups);
			defaultView.Filter = o => (o as ChatGroupViewModel).Users.Contains(userId) || (o as ChatGroupViewModel).Entity.ChatGroupId == 1;
			CollectionViews.Add(userId, defaultView);
			return defaultView;
		}

		public void AddGroup(string name, int userId)
		{
			var chatGroupEntity = _db.DbAccess.AddChatGroup(name);
			var chatGroupViewModel = new ChatGroupViewModel(chatGroupEntity);
			ChatGroups.Add(chatGroupViewModel);
			AddUserToGroup(chatGroupEntity.ChatGroupId, userId);
		}

		public void AddUserToGroup(int groupId, int appUserId)
		{
			var group = ChatGroups.FirstOrDefault(e => e.Entity.ChatGroupId == groupId);
			_db.DbAccess.EnterChatGroup(groupId,appUserId);
			group.Users.Add(appUserId);
			if (CollectionViews.ContainsKey(appUserId))
			{
				CollectionViews[appUserId].Refresh();
			}
		}

		public void LeaveGroup(int groupId, int appUserId)
		{
			_db.DbAccess.LeaveGroup(groupId, appUserId);
			var chatGroupViewModel = ChatGroups.FirstOrDefault(e => e.Entity.ChatGroupId == groupId);
			chatGroupViewModel.Users.Remove(appUserId);
			if (!chatGroupViewModel.Users.Any())
			{
				_db.DbAccess.PurgeChatHistory(groupId);
			}
			if (CollectionViews.ContainsKey(appUserId))
			{
				CollectionViews[appUserId].Refresh();
			}
		}

		public void AddMessage(ChatMessageEntity message)
		{
			var group = ChatGroups.FirstOrDefault(e => e.Entity.ChatGroupId == message.IdChatGroup);
			var chatMessageEntity = _db.DbAccess.AddChatMessage(message);
			group.Messages.Add(new ChatMessageEntityViewModel()
			{
					Entity = chatMessageEntity,
					User = _db.DbAccess.GetUsers().FirstOrDefault(e => e.AppUserId == message.IdUser)
			});
		}
	}

	public class ChatGroupViewModel
	{
		public ChatGroupViewModel(ChatGroupEntity entity)
		{
			Messages = new ObservableCollection<ChatMessageEntityViewModel>();
			Users = new ObservableCollection<int>();
			Entity = entity;
		}

		public ChatGroupEntity Entity { get; private set; }
		public ObservableCollection<int> Users { get; set; }
		public ObservableCollection<ChatMessageEntityViewModel> Messages { get; set; }
	}

	public class ChatMessageEntityViewModel
	{
		public ChatMessageEntity Entity { get; set; }
		public AppUserEntity User { get; set; }
	}
}
