using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JPB.GameTable.UI.Models;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Dialogs.ViewModel
{
	public class ItemWrapperViewModel : ViewModelBase
	{
		public object Entity { get; set; }
		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				SendPropertyChanging(() => IsSelected);
				_isSelected = value;
				SendPropertyChanged(() => IsSelected);
			}
		}
	}

	public class SelectionDialogViewModel : DialogViewModelBase
	{
		private readonly Action<IEnumerable<object>> _result;
		private readonly Func<IEnumerable<object>, bool> _validateResult;

		private SelectionDialogViewModel()
		{
			AcceptCommand = new DelegateCommand(AcceptExecute, CanAcceptExecute);
			Items = new ThreadSaveObservableCollection<ItemWrapperViewModel>();
			ItemSelectionChangedCommand = new DelegateCommand<ItemWrapperViewModel>(ItemSelectionChangedExecute, CanItemSelectionChangedExecute);
		}

		public SelectionDialogViewModel(Func<object, bool> validateResult, Action<object> result) : this()
		{
			SingleSelection = true;
			_validateResult = (e) => e.Any() && validateResult(e.FirstOrDefault());
			_result = (e) => result(e.FirstOrDefault());
		}

		public SelectionDialogViewModel(Action<IEnumerable<object>> result, Func<IEnumerable<object>, bool> validateResult) : this()
		{
			_validateResult = (e) => e.Any() && validateResult(e);
			_result = result;
		}

		public DelegateCommand AcceptCommand { get; private set; }
		private string _message;
		public ThreadSaveObservableCollection<ItemWrapperViewModel> Items { get; set; }
		public bool SingleSelection { get; set; }

		public DelegateCommand ItemSelectionChangedCommand { get; private set; }

		private void ItemSelectionChangedExecute(ItemWrapperViewModel sender)
		{
			if (SingleSelection)
			{
				foreach (var itemWrapperViewModel in Items.Where(e => e != sender))
				{
					itemWrapperViewModel.IsSelected = false;
				}
			}
		}

		private bool CanItemSelectionChangedExecute(ItemWrapperViewModel sender)
		{
			return true;
		}

		public string Message
		{
			get { return _message; }
			set
			{
				SendPropertyChanging(() => Message);
				_message = value;
				SendPropertyChanged(() => Message);
			}
		}

		private void AcceptExecute(object sender)
		{
			_result(Items.Where(e => e.IsSelected).Select(e => e.Entity));
			CloseExecute(sender);
		}

		private bool CanAcceptExecute(object sender)
		{
			return _validateResult(Items.Where(e => e.IsSelected).Select(e => e.Entity));
		}

		public void SwapSelection(ItemWrapperViewModel dataContext)
		{
			dataContext.IsSelected = dataContext.IsSelected;
		}

		public void AddRange<T>(IEnumerable<T> items)
		{
			this.Items.AddRange(items.Select(e => new ItemWrapperViewModel() { Entity = e }));
		}
	}

	public class InputDialogViewModel : DialogViewModelBase
	{
		private readonly Action<string> _result;
		private readonly Func<string, bool> _validateResult;

		public InputDialogViewModel(Func<string, bool> validateResult, Action<string> result)
		{
			AcceptCommand = new DelegateCommand(AcceptExecute, CanAcceptExecute);
			_validateResult = validateResult;
			_result = result;
		}

		public DelegateCommand AcceptCommand { get; private set; }
		private string _message;
		private string _input;

		public string Input
		{
			get { return _input; }
			set
			{
				SendPropertyChanging(() => Input);
				_input = value;
				SendPropertyChanged(() => Input);
			}
		}

		public string Message
		{
			get { return _message; }
			set
			{
				SendPropertyChanging(() => Message);
				_message = value;
				SendPropertyChanged(() => Message);
			}
		}

		private void AcceptExecute(object sender)
		{
			_result(Input);
			CloseExecute(sender);
		}

		private bool CanAcceptExecute(object sender)
		{
			return _validateResult(Input);
		}
	}

	public class MessageBoxDialog : DialogViewModelBase
	{
		private readonly Action<MessageBoxDialog, MessageBoxResult> _result;

		public MessageBoxDialog(string title, string message, MessageBoxButton buttons, Action<MessageBoxDialog, MessageBoxResult> result)
		{
			_result = result;
			Title = title;
			Message = message;
			Buttons = new ThreadSaveObservableCollection<NamedDelegateCommand>();
			switch (buttons)
			{
				case MessageBoxButton.OK:
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.OK.ToString(), SetResult(MessageBoxResult.OK)));
					break;
				case MessageBoxButton.OKCancel:
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.OK.ToString(), SetResult(MessageBoxResult.OK)));
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.Cancel.ToString(), SetResult(MessageBoxResult.Cancel)));
					break;
				case MessageBoxButton.YesNoCancel:
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.Yes.ToString(), SetResult(MessageBoxResult.Yes)));
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.No.ToString(), SetResult(MessageBoxResult.No)));
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.Cancel.ToString(), SetResult(MessageBoxResult.Cancel)));
					break;
				case MessageBoxButton.YesNo:
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.Yes.ToString(), SetResult(MessageBoxResult.Yes)));
					Buttons.Add(new NamedDelegateCommand(MessageBoxResult.No.ToString(), SetResult(MessageBoxResult.No)));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
			}
		}

		private Action SetResult(MessageBoxResult result)
		{
			return () =>
			{
				IoC.Resolve<DialogService>().CloseDialog(this);
				_result(this, result);
			};
		}

		private string _message;

		public string Message
		{
			get { return _message; }
			set
			{
				SendPropertyChanging(() => Message);
				_message = value;
				SendPropertyChanged(() => Message);
			}
		}

		private ThreadSaveObservableCollection<NamedDelegateCommand> _buttons;

		public ThreadSaveObservableCollection<NamedDelegateCommand> Buttons
		{
			get { return _buttons; }
			set
			{
				SendPropertyChanging(() => Buttons);
				_buttons = value;
				SendPropertyChanged(() => Buttons);
			}
		}
	}
}
