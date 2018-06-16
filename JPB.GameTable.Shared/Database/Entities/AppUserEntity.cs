using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.DataAccess.ModelsAnotations;

namespace JPB.GameTable.UI.Database.Entities
{
	[ForModel("AppUser")]
	public class AppUserEntity
	{
		[PrimaryKey]
		[ForModel("AppUser_Id")]
		public int AppUserId { get; set; }
		[ForeignKeyDeclaration(typeof(RoleEntity))]
		[ForModel("Id_Role")]
		public int IdRole { get; set; }
		public string Name { get; set; }
	}
}
