using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JPB.GameTable.UI.ViewModel;
using JPB.WPFBase.MVVM.DelegateCommand;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.UI.Dialogs.ViewModel
{
	public class PositioningDialogViewModel : DialogViewModelBase
	{
		public PositioningDialogViewModel()
		{
			MoveDirectionCommand = new DelegateCommand<string>(MoveDirectionExecute, CanMoveDirectionExecute);
			RoatateCommand = new DelegateCommand<string>(RoatateExecute, CanRoatateExecute);
		}

		private GameAreaViewModel _gameArea;
		private ThreadSaveObservableCollection<GameAreaViewModel> _gameAreas;

		public ThreadSaveObservableCollection<GameAreaViewModel> GameAreas
		{
			get { return _gameAreas; }
			set
			{
				SendPropertyChanging(() => GameAreas);
				_gameAreas = value;
				SendPropertyChanged(() => GameAreas);
			}
		}
		public GameAreaViewModel GameArea
		{
			get { return _gameArea; }
			set
			{
				SendPropertyChanging(() => GameArea);
				_gameArea = value;
				SendPropertyChanged(() => GameArea);
			}
		}

		public DelegateCommand<string> MoveDirectionCommand { get; private set; }
		public DelegateCommand<string> RoatateCommand { get; private set; }

		private void RoatateExecute(string sender)
		{
			GameArea.Orientaion = Transform(GameArea.Orientaion, GameArea.GameTablePostion, sender);
		}

		private RotateTransform Transform(RotateTransform transformation, Rect bounds, string orientation)
		{
			switch (orientation)
			{
				case "+":
					return new RotateTransform(transformation.Angle + 90, bounds.X / 2, bounds.Y / 2);
					break;
				case "-":
					return new RotateTransform(transformation.Angle - 90, bounds.X / 2, bounds.Y / 2);
					break;
			}

			return null;
		}

		private bool CanRoatateExecute(string sender)
		{
			if (GameArea == null)
			{
				return false;
			}
			return IsInFullBounds(sender, null);
		}

		private void MoveDirectionExecute(string sender)
		{
			GameArea.GameTablePostion = Resize(GameArea.GameTablePostion, sender);
		}

		private Rect Resize(Rect point, string sender)
		{
			switch (sender)
			{
				case "Right":
					return new Rect(GameArea.GameTablePostion.X + 10, GameArea.GameTablePostion.Y, point.Width, point.Height);
					break;
				case "Top":
					return new Rect(GameArea.GameTablePostion.X, GameArea.GameTablePostion.Y - 10, point.Width, point.Height);
					break;
				case "Bottom":
					return new Rect(GameArea.GameTablePostion.X, GameArea.GameTablePostion.Y + 10, point.Width, point.Height);
					break;
				case "Left":
					return new Rect(GameArea.GameTablePostion.X - 10, GameArea.GameTablePostion.Y, point.Width, point.Height);
					break;
				default:
					return point;
			}
		}

		private bool IsInFullBounds(string rotate, string move)
		{
			var transformation = GameArea.Orientaion;
			var position = GameArea.GameTablePostion;
			if (rotate != null)
			{
				transformation = Transform(transformation, position, rotate);
			}

			if (move != null)
			{
				position = Resize(position, move);
			}
			var child = Application.Current.MainWindow.Content as FrameworkElement;
			position = new Rect(transformation.Transform(new Point(position.X, position.Y)), position.Size);
			return new Rect(new Point(0, 0), child.RenderSize).Contains(position);
		}

		private bool CanMoveDirectionExecute(string sender)
		{
			if (GameArea == null)
			{
				return false;
			}

			return IsInFullBounds(null, sender);
		}
	}
}
