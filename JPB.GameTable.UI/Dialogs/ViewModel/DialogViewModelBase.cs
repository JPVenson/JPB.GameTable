using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Dialogs.ViewModel
{
	public class DialogViewModelBase : AsyncViewModelBase, IDialogViewModel
	{
		public DialogViewModelBase()
		{
			CloseCommand = new DelegateCommand(CloseExecute, CanCloseExecute);
		}

		public DelegateCommand CloseCommand { get; private set; }
		private string _title;

		public string Title
		{
			get { return _title; }
			set
			{
				SendPropertyChanging(() => Title);
				_title = value;
				SendPropertyChanged(() => Title);
			}
		}

		protected virtual void CloseExecute(object sender)
		{
			IoC.Resolve<DialogService>().CloseDialog(this);
		}

		protected virtual bool CanCloseExecute(object sender)
		{
			return true;
		}
	}


}