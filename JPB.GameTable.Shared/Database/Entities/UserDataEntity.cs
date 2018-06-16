using JPB.DataAccess.ModelsAnotations;

namespace JPB.GameTable.UI.Database.Entities
{
	[ForModel("UserData")]
	public class UserDataEntity
	{
		[PrimaryKey]
		[ForModel("UserData_Id")]
		public int UserDataId { get; set; }
		[ForModel("[Key]")]
		public string Key { get; set; }
		[ForModel("[Value]")]
		public string Value { get; set; }

		[ForeignKeyDeclaration(typeof(AppUserEntity))]
		[ForModel("Id_AppUser")]
		public int IdAppUser { get; set; }
	}
}