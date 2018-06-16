using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.UI.Models;
using JPB.WPFBase.MVVM.DelegateCommand;

namespace JPB.GameTable.UI.Services
{
	public class CommandService
	{
		public const string COMMAND_GROUP_GENERAL = "General";
		public const string COMMAND_GROUP_ADMIN = "Admin";

		public CommandService()
		{
			Commands = new ConcurrentDictionary<NamedDelegateCommand, RoleModel>();
		}

		public IDictionary<NamedDelegateCommand, RoleModel> Commands { get; set; }

		public event Func<Task> SaveRequested;

		public void AddCommand(RoleModel roleModel, NamedDelegateCommand command)
		{
			Commands.Add(command, roleModel);
			OnCommandsChanged();
		}

		public event EventHandler CommandsChanged;

		protected virtual void OnCommandsChanged()
		{
			CommandsChanged?.Invoke(this, EventArgs.Empty);
		}

		public IEnumerable<DelegateCommand> GetForRole(RoleModel appUserRoleModel)
		{
			return Commands.Where(e => e.Value.Id <= appUserRoleModel.Id).Select(e => e.Key);
		}

		public async Task OnSaveRequested(object sender)
		{
			var hander = SaveRequested;
			if (hander == null)
			{
				return;
			}

			await Task.WhenAll(hander.GetInvocationList().Select(e => e.DynamicInvoke() as Task).ToArray()).ConfigureAwait(false);
		}
	}
}
