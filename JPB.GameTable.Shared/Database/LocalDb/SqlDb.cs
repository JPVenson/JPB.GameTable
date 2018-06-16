using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.DataAccess.Manager;
using JPB.GameTable.Shared.Services;
using JPB.GameTable.UI.Database.Entities;
using JPB.GameTable.UI.Database.LocalDb;

namespace JPB.GameTable.Shared.Database.LocalDb
{
	public class SqlDb : DbAccessLayer, IDatabaseEntity
	{
		public SqlDb() : base(DbAccessType.MsSql, ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString)
		{
			Async = false;
			//ThreadSave = true;
		}

		/// <inheritdoc />
		public AppUserEntity[] GetUsers()
		{
			return Select<AppUserEntity>();
		}

		/// <inheritdoc />
		public void Create()
		{
		}

		/// <inheritdoc />
		public void Save()
		{
		}

		/// <inheritdoc />
		public UserDataEntity GetUserEntry(int userId, string key)
		{
			return Query().Select.Table<UserDataEntity>().Where.Column(f => f.IdAppUser).Is.EqualsTo(userId).And
			              .Column(f => f.Key).Is.EqualsTo(key).FirstOrDefault();
		}

		/// <inheritdoc />
		public UserDataEntity UpdateOrCreateUserData(UserDataEntity dataEntity)
		{
			var hasUserData = GetUserEntry(dataEntity.IdAppUser, dataEntity.Key);
			if (hasUserData == null)
			{
				return InsertWithSelect(dataEntity);
			}

			Update(dataEntity);
			return GetUserEntry(dataEntity.IdAppUser, dataEntity.Key);
		}

		/// <inheritdoc />
		public RoleEntity[] GetRoles()
		{
			return Select<RoleEntity>();
		}

		/// <inheritdoc />
		public void CreateRole(RoleEntity roleEntity)
		{
			Insert(roleEntity);
		}

		/// <inheritdoc />
		public void CreateUser(AppUserEntity appUserEntity)
		{
			Insert(appUserEntity);
		}

		/// <inheritdoc />
		public ChatGroupEntity AddChatGroup(string name)
		{
			return InsertWithSelect(new ChatGroupEntity() {Name = name});
		}

		/// <inheritdoc />
		public void EnterChatGroup(int chatGroupId, int userId)
		{
			Insert(new ChatGroupUserMap()
			{
					IdAppUser = userId,
					IdChatGroup = chatGroupId
			});
		}

		/// <inheritdoc />
		public ChatMessageEntity AddChatMessage(ChatMessageEntity message)
		{
			return InsertWithSelect(message);
		}

		/// <inheritdoc />
		public void LeaveGroup(int groupId, int appUserId)
		{
			Query().Delete<ChatGroupUserMap>().Where.Column(f => f.IdAppUser).Is.EqualsTo(appUserId).And
			       .Column(f => f.IdChatGroup).Is.EqualsTo(groupId).ExecuteNonQuery();
		}

		/// <inheritdoc />
		public void PurgeChatHistory(int groupId)
		{
			Query().Delete<ChatMessageEntity>().Where.Column(f => f.IdChatGroup).Is.EqualsTo(groupId).ExecuteNonQuery();
		}

		/// <inheritdoc />
		public ChatGroupEntity[] GetChatGroups()
		{
			return Select<ChatGroupEntity>();
		}

		/// <inheritdoc />
		public ChatMessageEntity[] GetChatMessages(int entityChatGroupId)
		{
			return Query().Select.Table<ChatMessageEntity>().Where.Column(f => f.IdChatGroup).Is.EqualsTo(entityChatGroupId)
			              .ToArray();
		}
	}
}
