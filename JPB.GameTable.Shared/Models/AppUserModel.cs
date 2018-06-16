using System.Collections.Generic;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Models
{
	public class AppUserModel : ViewModelBase
	{
		public RoleModel RoleModel { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
		private int _virtualMouseDiviceId;
		private int _virtualKeyboardDeviceId;

		public int VirtualKeyboardDeviceId
		{
			get { return _virtualKeyboardDeviceId; }
			set
			{
				SendPropertyChanging(() => VirtualKeyboardDeviceId);
				_virtualKeyboardDeviceId = value;
				SendPropertyChanged(() => VirtualKeyboardDeviceId);
			}
		}

		public int VirtualMouseDiviceId
		{
			get { return _virtualMouseDiviceId; }
			set
			{
				SendPropertyChanging(() => VirtualMouseDiviceId);
				_virtualMouseDiviceId = value;
				SendPropertyChanged(() => VirtualMouseDiviceId);
			}
		}
	}

	public static class Roles
	{
		public static RoleModel Admin { get; private set; } = new RoleModel() { Id = 1, Name = "Admin" };
		public static RoleModel GameMaster { get; private set; } = new RoleModel() { Id = 2, Name = "GM" };
		public static RoleModel Player { get; private set; } = new RoleModel() { Id = 3, Name = "Player" };

		public static IEnumerable<RoleModel> Yield()
		{
			yield return Admin;
			yield return GameMaster;
			yield return Player;
		}
	}

	public class RoleModel
	{
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
