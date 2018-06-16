using JPB.DataAccess.ModelsAnotations;

namespace JPB.GameTable.UI.Database.Entities
{
	public class RoleEntity
	{
		[PrimaryKey]
		[ForModel("RoleEntity_Id")]
		public int RoleId { get; set; }
		public string Name { get; set; }
	}
}