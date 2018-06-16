using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.Shared.Database.LocalDb;
using JPB.GameTable.UI.Database.Entities;
using JPB.GameTable.UI.Database.LocalDb;
using JPB.GameTable.UI.Models;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.UI.Database
{
	public class DbEntities
	{
		public DbEntities()
		{
			DbAccess = new SqlDb();
			DbAccess.Create();
			try
			{
				Migrade();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private void Migrade()
		{
			if (!DbAccess.GetRoles().Any())
			{
				foreach (var roleModel in Roles.Yield())
				{
					DbAccess.CreateRole(new RoleEntity()
					{
							Name = roleModel.Name,
							RoleId = roleModel.Id
					});
				}
			}

			if (!DbAccess.GetUsers().Any())
			{
				DbAccess.CreateUser(new AppUserEntity()
				{
						Name = "Admin",
						IdRole = Roles.Admin.Id,
				});
			}

			if (!DbAccess.GetChatGroups().Any())
			{
				DbAccess.AddChatGroup("General");
			}
		}

		public IDatabaseEntity DbAccess { get; set; }
	}
}
