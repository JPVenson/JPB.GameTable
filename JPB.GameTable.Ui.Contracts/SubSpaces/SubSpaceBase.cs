using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.Services;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.ViewModel.SubSpaces
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class SubSpaceAttribute : Attribute
	{
		public readonly string Key;
		public bool AllowMultible { get; set; }
		public SubSpaceAttribute(string key)
		{
			Key = key;
		}
	}

	public abstract class SubSpaceBase : AsyncViewModelBase
	{
		public SubSpaceBase()
		{
			CloseSubSpaceCommand = new DelegateCommand(CloseSubSpaceExecute, CanCloseSubSpaceExecute);
			Commands = new ThreadSaveObservableCollection<NamedDelegateCommand>();
			Commands.Add(new NamedDelegateCommand("Close", CloseSubSpaceExecute, CanCloseSubSpaceExecute));
		}

		private string _title;
		public IGameArea GameArea { get; private set; }

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

		public DelegateCommand CloseSubSpaceCommand { get; private set; }

		private ThreadSaveObservableCollection<NamedDelegateCommand> _commands;

		public ThreadSaveObservableCollection<NamedDelegateCommand> Commands
		{
			get { return _commands; }
			set
			{
				SendPropertyChanging(() => Commands);
				_commands = value;
				SendPropertyChanged(() => Commands);
			}
		}

		private void CloseSubSpaceExecute(object sender)
		{
			base.SimpleWork(() =>
			{
				Save(GameArea.AppUserId);
				GameArea.OpenSubSpaces.Remove(this);
			});
		}

		private bool CanCloseSubSpaceExecute(object sender)
		{
			return IsNotWorking;
		}

		public virtual void Save()
		{

		}

		public virtual void Save(int userId)
		{
			Save();
		}

		public virtual void Load(IGameArea gameAreaViewModel)
		{
			GameArea = gameAreaViewModel;
		}
	}
}
