using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Database.Entities;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.UI.ViewModel.SubSpaces
{
	[SubSpace("Notes")]
	public class NotesSubSpace : SubSpaceBase
	{
		public NotesSubSpace()
		{
			Title = "Notes";
			base.Commands.Add(new NamedDelegateCommand("Notes.Clear", () => Notes = string.Empty));
		}

		private string _notes;

		public string Notes
		{
			get { return _notes; }
			set
			{
				SendPropertyChanging(() => Notes);
				_notes = value;
				SendPropertyChanged(() => Notes);
			}
		}

		/// <inheritdoc />
		public override void Load(IGameArea gameAreaViewModel)
		{
			base.Load(gameAreaViewModel);
			Notes = IoC.Resolve<DbEntities>().DbAccess.GetUserEntry(gameAreaViewModel.AppUserId, "SUBSPACE.NOTES.TEXT")?.Value;
		}

		/// <inheritdoc />
		public override void Save(int userId)
		{
			IoC.Resolve<DbEntities>().DbAccess.UpdateOrCreateUserData(new UserDataEntity()
			{
					IdAppUser = userId,
					Key = "SUBSPACE.NOTES.TEXT",
					Value = Notes
			});
			base.Save(userId);
		}
	}
}
