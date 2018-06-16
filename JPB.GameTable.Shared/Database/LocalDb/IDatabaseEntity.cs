using JPB.GameTable.Shared.Services;
using JPB.GameTable.UI.Database.Entities;

namespace JPB.GameTable.UI.Database.LocalDb
{
	public interface IDatabaseEntity
	{
		AppUserEntity[] GetUsers();
		void Create();
		void Save();
		UserDataEntity GetUserEntry(int userId, string key);
		UserDataEntity UpdateOrCreateUserData(UserDataEntity dataEntity);
		RoleEntity[] GetRoles();
		void CreateRole(RoleEntity roleEntity);
		void CreateUser(AppUserEntity appUserEntity);
		ChatGroupEntity AddChatGroup(string name);
		void EnterChatGroup(int chatGroupId, int userId);
		ChatMessageEntity AddChatMessage(ChatMessageEntity message);
		void LeaveGroup(int groupId, int appUserId);
		void PurgeChatHistory(int groupId);
		ChatGroupEntity[] GetChatGroups();
		ChatMessageEntity[] GetChatMessages(int entityChatGroupId);
	}
}