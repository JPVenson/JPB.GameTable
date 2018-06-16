#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Interceptor;
using JPB.GameTable.UI.Resources.Behaviors;
using JPB.GameTable.UI.Unity;

#endregion

namespace JPB.GameTable.UI.Resources
{
	public class GameAreaFocusService
	{
		InputInterceptor _inputInterceptor;

		public GameAreaFocusService()
		{
			FocusedElements = new ConcurrentDictionary<int, DependencyObject>();
			Conatainer = new ConcurrentDictionary<int, UIElement>();
			IoC.ResolveLater<InputInterceptor>().ContinueWith(e =>
			{
				_inputInterceptor = e.Result;
				_inputInterceptor.VirtualMouseAction += InputInterceptor_VirtualMouseAction;
			});
		}

		public ConcurrentDictionary<int, DependencyObject> FocusedElements { get; set; }
		public ConcurrentDictionary<int, UIElement> Conatainer { get; set; }

		private void InputInterceptor_VirtualMouseAction(object sender, MousePressedEventArgs e)
		{
			if (e.State != MouseState.LeftDown)
			{
				return;
			}

			Application.Current.Dispatcher.Invoke(() =>
			{
				var lastKnown = _inputInterceptor.Mice[e.DeviceId].PreMouseData;
				var container = Conatainer[e.DeviceId];

				var positon = new Point(lastKnown.AbsolutX, lastKnown.AbsolutY);
				var elementsUnderCursor = new List<DependencyObject>();
				//var hasHitTest = VisualTreeHelper.HitTest(container, positon);

				if (!GetExactHitTest(container, container, positon, elementsUnderCursor))
				{
					return;
				}

				var findTextBox = GetVisualParentWithKeyHandling(elementsUnderCursor.Last());
				var addOrUpdate = FocusedElements.AddOrUpdate(e.DeviceId, findTextBox,
				(key, old) =>
				{
					RemoveOld(old);
					return findTextBox;
				});
				AddNew(addOrUpdate);
				Console.WriteLine("Current Focused: " + addOrUpdate);
			}, DispatcherPriority.Input);
		}

		private void AddNew(DependencyObject addOrUpdate)
		{
			if (addOrUpdate is UIElement)
			{
				var uiElement = addOrUpdate as UIElement;
				uiElement.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
				{
					RoutedEvent = Mouse.MouseMoveEvent,
					Source = this
				});
			}
		}

		public static bool GetExactHitTest(UIElement reference, UIElement parent, Point position, ICollection<DependencyObject> path, bool findFirst = true)
		{
			var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = childrenCount - 1; i >= 0; i--)
			{
				var child = VisualTreeHelper.GetChild(parent, i) as UIElement;
				if (child == null)
				{
					continue;
				}

				var translated = child.TranslatePoint(position, reference);
				var inputHitTest = child.InputHitTest(translated) as UIElement;
				if (inputHitTest == null)
				{
					continue;
				}

				var hasKeyHandling = GetVisualParentWithKeyHandling(inputHitTest);
				if (hasKeyHandling != null)
				{
					path.Add(hasKeyHandling);
					if (findFirst)
					{
						return true;
					}
				}

				if (!GetExactHitTest(reference, inputHitTest, translated, path))
				{
					continue;
				}

				if (findFirst)
				{
					return true;
				}
			}

			return false;
		}

		public static T GetVisualChild<T>(Visual referenceVisual) where T : Visual
		{
			Visual child = null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
			{
				child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;

				if (child != null && child.GetType() == typeof(T))
				{
					break;
				}

				if (child != null)
				{
					child = GetVisualChild<T>(child);

					if (child != null && child.GetType() == typeof(T))
					{
						break;
					}
				}
			}

			return child as T;
		}

		public static T GetVisualParent<T>(DependencyObject element) where T : DependencyObject
		{
			while (element != null && !(element is T))
			{
				element = VisualTreeHelper.GetParent(element);
			}

			return (T)element;
		}


		public static DependencyObject GetVisualParentWithKeyHandling(DependencyObject element)
		{
			while (element != null && !Interaction.GetBehaviors(element).Any(e => e is IVirtualInputReciver))
			{
				element = VisualTreeHelper.GetParent(element);
			}

			return element;
		}

		private void RemoveOld(DependencyObject old)
		{
		}
	}
}