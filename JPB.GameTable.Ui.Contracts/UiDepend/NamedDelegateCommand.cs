using System;
using System.Linq;
using System.Windows.Input;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.WPFBase.MVVM.DelegateCommand;

namespace JPB.GameTable.UI.Services
{
	public interface INamedDelegateCommand : ICommand
	{
		string Path { get; set; }
		string Name { get; }
	}

	public interface IContextAwareNamedDelegateCommand : INamedDelegateCommand
	{
	}

	public class NamedDelegateCommand : DelegateCommand, INamedDelegateCommand
	{
		public NamedDelegateCommand(string path, Action execute) : base(execute)
		{
			Path = path;
		}

		public NamedDelegateCommand(string path, Action<object> execute) : base(execute)
		{
			Path = path;
		}

		public NamedDelegateCommand(string path, Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
		{
			Path = path;
		}

		public NamedDelegateCommand(string path, Action execute, Func<bool> canExecute) : base(execute, canExecute)
		{
			Path = path;
		}

		public string Path { get; set; }

		public string Name
		{
			get { return Path.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(); }
		}
	}

	public class GameAreaContextDelegateCommandImpl : DelegateCommand<IGameArea>, IContextAwareNamedDelegateCommand
	{
		/// <inheritdoc />
		public GameAreaContextDelegateCommandImpl(Action execute) : base(execute)
		{
		}

		/// <inheritdoc />
		public GameAreaContextDelegateCommandImpl(Action<IGameArea> execute) : base(execute)
		{
		}

		/// <inheritdoc />
		public GameAreaContextDelegateCommandImpl(Action<IGameArea> execute, Func<IGameArea, bool> canExecute) : base(execute, canExecute)
		{
		}

		/// <inheritdoc />
		public GameAreaContextDelegateCommandImpl(Action execute, Func<bool> canExecute) : base(execute, canExecute)
		{
		}

		/// <inheritdoc />
		public string Path { get; set; }

		public string Name
		{
			get { return Path.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(); }
		}
	}
}