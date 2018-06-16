using System;
using System.Windows.Media;
using JPB.DataAccess.ModelsAnotations;
using JPB.GameTable.UI.Database.Entities;

namespace JPB.GameTable.Shared.Services
{
	[ForModel("ChatMessage")]
	public class ChatMessageEntity
	{
		[PrimaryKey]
		[ForModel("ChatMessage_Id")]
		public int ChatMessageId { get; set; }
		[ForeignKeyDeclaration(typeof(AppUserEntity))]
		[ForModel("Id_User")]
		public int IdUser { get; set; }
		[ForeignKeyDeclaration(typeof(ChatGroupEntity))]
		[ForModel("Id_ChatGroup")]
		public int IdChatGroup { get; set; }

		public DateTime DateSend { get; set; }
		public string Message { get; set; }
		public string Color { get; set; }
	}
}