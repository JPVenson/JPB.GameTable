using JPB.GameTable.Ui.Contracts.GameArea;

namespace JPB.GameTable.UI.Services
{
	public interface IDialog
	{
		IGameArea ForGame { get; }
		string Name { get; }
		IDialogViewModel ViewModel { get; }
	}

	public interface IDialogViewModel
	{
		string Title { get; }
	}
}