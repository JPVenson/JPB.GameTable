using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using JPB.GameTable.Ui.Contracts.GameArea;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class CommandTreeViewBehavior : Behavior<TreeView>
	{
		public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register(
		"Commands", typeof(IEnumerable), typeof(CommandTreeViewBehavior),
		new PropertyMetadata(default(ObservableCollection<NamedDelegateCommand>), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			(dependencyObject as CommandTreeViewBehavior).Reevaluate();
		}

		public static readonly DependencyProperty TreeItemTemplateProperty = DependencyProperty.Register(
		"TreeItemTemplate", typeof(DataTemplate), typeof(CommandTreeViewBehavior), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate TreeItemTemplate
		{
			get { return (DataTemplate) GetValue(TreeItemTemplateProperty); }
			set { SetValue(TreeItemTemplateProperty, value); }
		}
		public IEnumerable Commands
		{
			get { return (IEnumerable) GetValue(CommandsProperty); }
			set { SetValue(CommandsProperty, value); }
		}
		ICollectionView collectionView;

		/// <inheritdoc />
		protected override void OnAttached()
		{
			if (!(Commands is INotifyCollectionChanged))
			{
				throw new InvalidCastException("Cannot attach to a non INotifyCollectionChanged list");
			}

			Tree = new ObservableCollection<TreeItem>();
			collectionView = CollectionViewSource.GetDefaultView(Commands);
			collectionView.CollectionChanged += Commands_CollectionChanged;
			Reevaluate();

			IoC.Resolve<CommandService>().CommandsChanged += CommandTreeViewBehavior_CommandsChanged;
		}

		private void CommandTreeViewBehavior_CommandsChanged(object sender, EventArgs e)
		{
			Reevaluate();
		}

		private void Commands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Reevaluate();
		}

		private ObservableCollection<TreeItem> _tree;

		public ObservableCollection<TreeItem> Tree
		{
			get { return _tree; }
			set
			{
				_tree = value;
			}
		}

		private void Reevaluate()
		{
			if (AssociatedObject == null)
			{
				return;
			}

			AssociatedObject.Items.Clear();

			foreach (var command in collectionView.Cast<INamedDelegateCommand>())
			{
				TreeViewItem nextTreeItem = null;
				var path = command.Path.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var pathPart in path.Take(path.Length - 1))
				{
					nextTreeItem = GetTreeItemUnderPath(pathPart, nextTreeItem?.Items ?? AssociatedObject.Items);
				}

				if (nextTreeItem == null)
				{
					nextTreeItem = new TreeViewItem();
					AssociatedObject.Items.Add(nextTreeItem);
				}
				else
				{
					nextTreeItem = GetTreeItemUnderPath(path.Last(), nextTreeItem.Items);
				}

				if (command is IContextAwareNamedDelegateCommand)
				{
					nextTreeItem.Header = new ContextNamedDelegateCommand(() => AssociatedObject.DataContext as IGameArea, command);
				}
				else
				{
					nextTreeItem.Header = command;
				}

				nextTreeItem.HeaderTemplate = TreeItemTemplate;
			}
		}

		public void Sync()
		{
			var reSyncList = new List<TreeItem>();

			foreach (var namedDelegateCommand in Commands.Cast<INamedDelegateCommand>())
			{
				TreeItem treeItem = null;
				var path = namedDelegateCommand.Path.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var partPart in path.Take(path.Length - 1))
				{
					treeItem = reSyncList.FirstOrDefault(e => e.Title.Equals(partPart));
					if (treeItem == null)
					{
						treeItem = new TreeItem();
						treeItem.Title = partPart;
						reSyncList.Add(treeItem);
					}
				}

				treeItem.Commands.Add(namedDelegateCommand);
			}

			//foreach (var treeItem in reSyncList)
			//{
			//	Resync(treeItem);
			//}
		}

		private void Resync(TreeItem items, TreeViewItem treeItem)
		{
			foreach (var item in treeItem.Items.Cast<TreeViewItem>().ToArray())
			{
				if (item.Header is string)
				{
					if (items.Childs.Any(e => e.Title == item.Header.ToString()))
					{
						continue;
					}
					else
					{
						treeItem.Items.Remove(item);
					}
				}
				else if (item.Header is INamedDelegateCommand)
				{
					if (items.Commands.Any(e => e == item.Header))
					{
						continue;
					}
					else
					{
						treeItem.Items.Remove(item);
					}
				}
			}

			foreach (var namedDelegateCommand in items.Commands.Where(e => treeItem.Items.Cast<TreeViewItem>().Any(f => f.Header == e)).ToArray())
			{
				var nextTreeItem = new TreeViewItem();
				if (namedDelegateCommand is IContextAwareNamedDelegateCommand)
				{
					nextTreeItem.Header = new ContextNamedDelegateCommand(() => AssociatedObject.DataContext as IGameArea, namedDelegateCommand);
				}
				else
				{
					nextTreeItem.Header = namedDelegateCommand;
				}
				nextTreeItem.HeaderTemplate = TreeItemTemplate;
				treeItem.Items.Add(nextTreeItem);
			}

			foreach (var itemsChild in items.Childs)
			{
				TreeViewItem childTreeItem;
				childTreeItem = treeItem.Items.Cast<TreeViewItem>().FirstOrDefault(e => Equals(e.Header, itemsChild.Title));
				if (childTreeItem == null)
				{
					childTreeItem = new TreeViewItem();
					childTreeItem.Header = itemsChild.Title;
					treeItem.Items.Add(childTreeItem);
				}

				Resync(itemsChild, childTreeItem);
			}
		}

		public class TreeItem
		{
			public TreeItem()
			{
				Childs = new List<TreeItem>();
				Commands = new List<INamedDelegateCommand>();
			}
			public string Title { get; set; }
			public ICollection<TreeItem> Childs { get; set; }
			public ICollection<INamedDelegateCommand> Commands { get; set; }
		}

		private TreeViewItem GetTreeItemUnderPath(string pathPart, ItemCollection items)
		{
			TreeViewItem existingItem;
			for (int i = 0; i < items.Count; i++)
			{
				existingItem = (TreeViewItem)items[i];
				if (existingItem.Header.Equals(pathPart))
				{
					return existingItem;
				}
			}

			items.Add(existingItem = new TreeViewItem()
			{
					Header = pathPart
			});
			return existingItem;
		}
	}

	internal class ContextNamedDelegateCommand : NamedDelegateCommand
	{
		private readonly Func<IGameArea> _func;
		private readonly INamedDelegateCommand _command;

		public ContextNamedDelegateCommand(Func<IGameArea> func, INamedDelegateCommand command) : base(command.Path, () => {})
		{
			_func = func;
			_command = command;
			ExecutionAction = (sender) =>
			{
				_command.Execute(_func());
			};
			CanExecutePredicate = (sender) => _command.CanExecute(_func());
		}
	}
}
