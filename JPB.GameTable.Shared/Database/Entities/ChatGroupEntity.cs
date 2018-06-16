using JPB.DataAccess.ModelsAnotations;

namespace JPB.GameTable.Shared.Services
{
	[ForModel("ChatGroup")]
	public class ChatGroupEntity
	{
		[PrimaryKey]
		[ForModel("ChatGroup_Id")]
		public int ChatGroupId { get; set; }
		public string Name { get; set; }
	}
}