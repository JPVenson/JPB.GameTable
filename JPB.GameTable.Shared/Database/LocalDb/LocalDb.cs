#region

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;
using JPB.DataAccess.DbInfoConfig;
using JPB.DataAccess.Helper.LocalDb;
using JPB.DataAccess.Helper.LocalDb.Scopes;
using JPB.GameTable.Shared.Services;
using JPB.GameTable.UI.Database.Entities;

#endregion

namespace JPB.GameTable.UI.Database.LocalDb
{
	public class DatabaseEntity : IDatabaseEntity
	{
		public LocalDbRepository<AppUserEntity> AppUsers { get; set; }
		public LocalDbRepository<RoleEntity> Roles { get; set; }
		public LocalDbRepository<UserDataEntity> UserData { get; set; }
		public LocalDbRepository<ChatGroupEntity> ChatGroups { get; set; }
		public LocalDbRepository<ChatMessageEntity> ChatMessages { get; set; }
		public LocalDbRepository<ChatGroupUserMap> ChatGroupMap { get; set; }

		public AppUserEntity[] GetUsers()
		{
			return AppUsers.ToArray();
		}

		public void Create()
		{
			using (new DatabaseScope())
			{
				var dbConfig = new DbConfig(true);
				AppUsers = new LocalDbRepository<AppUserEntity>(dbConfig);
				UserData = new LocalDbRepository<UserDataEntity>(dbConfig);
				Roles = new LocalDbRepository<RoleEntity>(dbConfig);
				ChatGroups = new LocalDbRepository<ChatGroupEntity>(dbConfig);
				ChatMessages = new LocalDbRepository<ChatMessageEntity>(dbConfig);
				ChatGroupMap = new LocalDbRepository<ChatGroupUserMap>(dbConfig);

				var storeLocation = ConfigurationManager.AppSettings["SaveLocation"];
				if (File.Exists(storeLocation))
				{
					using (var fs = new FileStream(storeLocation, FileMode.Open))
					{
						using (new TransactionScope())
						{
							using (new ReplicationScope())
							{
								new XmlSerializer(typeof(DataContent)).Deserialize(fs);
							}
						}
					}
				}
			}
		}

		public void Save()
		{
			var storeLocation = ConfigurationManager.AppSettings["SaveLocation"];
			//if (File.Exists(storeLocation))
			//{
			//	File.Delete(storeLocation);
			//}

			using (var fs = new FileStream(storeLocation, FileMode.Create))
			{
				new XmlSerializer(typeof(DataContent)).Serialize(fs, AppUsers.Database.GetSerializableContent());
			}
		}

		public UserDataEntity GetUserEntry(int userId, string key)
		{
			return UserData.FirstOrDefault(e => e.IdAppUser == userId && key.ToUpper().Equals(e.Key.ToUpper()));
		}

		public UserDataEntity UpdateOrCreateUserData(UserDataEntity dataEntity)
		{
			lock (dataEntity.IdAppUser + "db")
			{
				var hasUserData =
						UserData.FirstOrDefault(e => e.Key.ToUpper().Equals(dataEntity.Key.ToUpper()) && e.IdAppUser == dataEntity.IdAppUser);

				if (hasUserData == null)
				{
					UserData.Add(dataEntity);
					return UserData.Last(e => e.IdAppUser == dataEntity.IdAppUser);
				}

				hasUserData.Value = dataEntity.Value;

				UserData.Update(hasUserData);
				return dataEntity;
			}
		}

		public RoleEntity[] GetRoles()
		{
			return Roles.ToArray();
		}

		public void CreateRole(RoleEntity roleEntity)
		{
			using (new TransactionScope())
			{
				using (DbReposetoryIdentityInsertScope.CreateOrObtain())
				{
					Roles.Add(roleEntity);
				}
			}
		}

		public void CreateUser(AppUserEntity appUserEntity)
		{
			AppUsers.Add(appUserEntity);
		}

		public ChatGroupEntity AddChatGroup(string name)
		{
			lock (ChatGroups)
			{
				ChatGroups.Add(new ChatGroupEntity()
				{
						Name = name
				});
				return ChatGroups.Last();
			}
		}

		public void EnterChatGroup(int chatGroupId, int userId)
		{
			var isInChatGroup = ChatGroupMap.FirstOrDefault(e => e.IdChatGroup == chatGroupId && e.IdAppUser == userId);
			if (isInChatGroup == null)
			{
				ChatGroupMap.Add(new ChatGroupUserMap()
				{
						IdChatGroup = chatGroupId,
						IdAppUser = userId
				});
			}
		}

		public ChatMessageEntity AddChatMessage(ChatMessageEntity message)
		{
			lock (ChatMessages)
			{
				ChatMessages.Add(message);
				return ChatMessages.Last();
			}
		}

		public void LeaveGroup(int groupId, int appUserId)
		{
			var hasRelation = ChatGroupMap.FirstOrDefault(e => e.IdChatGroup == groupId && e.IdAppUser == appUserId);
			if (hasRelation != null)
			{
				ChatGroupMap.Remove(hasRelation);
			}
		}

		public void PurgeChatHistory(int groupId)
		{
			foreach (var chatMessageEntity in ChatMessages.Where(e => e.IdChatGroup == groupId).ToArray())
			{
				ChatMessages.Remove(chatMessageEntity);
			}
		}

		public ChatGroupEntity[] GetChatGroups()
		{
			return ChatGroups.ToArray();
		}

		public ChatMessageEntity[] GetChatMessages(int entityChatGroupId)
		{
			return ChatMessages.Where(e => e.IdChatGroup == entityChatGroupId).ToArray();
		}
	}
}