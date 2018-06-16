using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class DialogBehavior : Behavior<Grid>
	{
		private DialogService _dialogService;

		public static readonly DependencyProperty DialogDisplayProperty = DependencyProperty.Register(
		"DialogDisplay", typeof(ContentControl), typeof(DialogBehavior), new PropertyMetadata(default(ContentControl)));

		public ContentControl DialogDisplay
		{
			get { return (ContentControl) GetValue(DialogDisplayProperty); }
			set { SetValue(DialogDisplayProperty, value); }
		}

		private IDialog _dialog;

		public IDialog Dialog
		{
			get { return _dialog; }
			set
			{
				_dialog = value;
			}
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			_dialogService = IoC.Resolve<DialogService>();
			WeakEventManager<ThreadSaveObservableCollection<IDialog>, NotifyCollectionChangedEventArgs>.AddHandler(_dialogService.Dialogs, nameof(INotifyCollectionChanged.CollectionChanged), DialogsChanged);
			var hasDialog = _dialogService.Dialogs.FirstOrDefault(e => e.ForGame == AssociatedObject.DataContext);
			if (hasDialog != null)
			{
				SetDialog(hasDialog);
			}
		}

		private void DialogsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			var dialogForScope = _dialogService.Dialogs.FirstOrDefault(e => e.ForGame == AssociatedObject.DataContext);

			if (dialogForScope != null && Dialog != dialogForScope)
			{
				SetDialog(dialogForScope);
			}

			if (dialogForScope == null)
			{
				RemoveDialog();
			}
		}

		private void RemoveDialog()
		{
			DialogDisplay.Content = null;
			DialogDisplay.Visibility = Visibility.Collapsed;
		}

		private void SetDialog(IDialog dialogForScope)
		{
			Dialog = dialogForScope;
			DialogDisplay.Content = dialogForScope.ViewModel;
			DialogDisplay.Visibility = Visibility.Visible;
		}

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			
		}
	}


}
