using JPB.DataAccess.ModelsAnotations;
using JPB.GameTable.UI.Database.Entities;

namespace JPB.GameTable.Shared.Services
{
	[ForModel("ChatGroupUser")]
	public class ChatGroupUserMap
	{
		[PrimaryKey]
		[ForModel("ChatGroupUserMap_Id")]
		public int ChatGroupUserMapId { get; set; }

		[ForeignKeyDeclaration(typeof(AppUserEntity))]
		[ForModel("Id_AppUser")]
		public int IdAppUser { get; set; }
		[ForeignKeyDeclaration(typeof(ChatGroupEntity))]
		[ForModel("Id_ChatGroup")]
		public int IdChatGroup { get; set; }
	}
}