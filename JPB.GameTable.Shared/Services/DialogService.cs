using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.Extentions.Extensions;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.ViewModel;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Services
{
	public class DialogService
	{
		public DialogService()
		{
			Dialogs = new ThreadSaveObservableCollection<IDialog>();
		}

		public ThreadSaveObservableCollection<IDialog> Dialogs { get; set; }

		public void ShowDialog(IDialog dialog)
		{
			Dialogs.Add(dialog);
		}

		public void CloseDialog(IDialogViewModel dialogViewModelBase)
		{
			var firstOrDefault = Dialogs.FirstOrDefault(e => e.ViewModel == dialogViewModelBase);
			Dialogs.Remove(firstOrDefault);
		}
	}

	public class Dialog : IDialog
	{
		public Dialog(IGameArea forGame, string name, IDialogViewModel viewModel)
		{
			ForGame = forGame;
			Name = name;
			ViewModel = viewModel;
		}

		/// <inheritdoc />
		public IGameArea ForGame { get; }

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public IDialogViewModel ViewModel { get; }
	}
}
