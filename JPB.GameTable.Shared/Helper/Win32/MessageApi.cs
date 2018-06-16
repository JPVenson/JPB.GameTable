using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Interceptor;
using JPB.GameTable.Shared.Helper;

namespace JPB.GameTable.UI.Helper.Win32
{
	///summary>
	/// Virtual Messages
	/// </summary>
	public enum WMessages : int
	{
		WM_LBUTTONDOWN = 0x201, //Left mousebutton down
		WM_LBUTTONUP = 0x202, //Left mousebutton up
		WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
		WM_RBUTTONDOWN = 0x204, //Right mousebutton down
		WM_RBUTTONUP = 0x205,  //Right mousebutton up
		WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
		WM_KEYDOWN = 0x100, //Key down
		WM_KEYUP = 0x101,  //Key up
		WM_CHAR = 0x102,
		WM_SCROLL = 0x020A,
		WM_MBUTTONDOWN = 0x0207,
		WM_MBUTTONUP = 0x0208,
	}

	public static class SendMessageApi
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		static extern IntPtr PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// Sends a message to the specified handle
		/// </summary>
		public static void SendMessageTo(IntPtr handle, int Msg, IntPtr wParam, IntPtr lParam)
		{
			PostMessage(handle, Msg, wParam, lParam);
		}

		/// <summary>
		/// MakeLParam Macro
		/// </summary>
		public static IntPtr MakeLParam(int LoWord, int HiWord)
		{
			return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
		}

		/// <summary>
		/// MakeLParam Macro
		/// </summary>
		public static IntPtr MakeWParam(int LoWord, int HiWord)
		{
			return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
		}

		/// <summary>
		/// returns handle of specified window name
		/// </summary>
		public static IntPtr FindWindow(string wndName)
		{
			return FindWindow(null, wndName);
		}

		public static MSG[] GetControlClickMessages(string button, int x, int y, bool doubleklick)
		{
			IntPtr LParam = MakeLParam(x, y);

			int btnDown = 0;
			int btnUp = 0;

			if (button == "left")
			{
				btnDown = (int)WMessages.WM_LBUTTONDOWN;
				btnUp = (int)WMessages.WM_LBUTTONUP;
			}

			if (button == "right")
			{
				btnDown = (int)WMessages.WM_RBUTTONDOWN;
				btnUp = (int)WMessages.WM_RBUTTONUP;
			}

			if (button == "middle")
			{
				btnDown = (int)WMessages.WM_MBUTTONDOWN;
				btnUp = (int)WMessages.WM_MBUTTONUP;
			}

			MSG[] result;
			if (doubleklick == true)
			{
				result = new MSG[4];
				result[0] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnDown, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
				result[1] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnUp, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
				result[2] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnDown, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
				result[3] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnUp, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
				return result;
			}

			result = new MSG[2];
			result[0] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnDown, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
			result[1] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnUp, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
			return result;
		}

		public static uint GetLParamForKey(int scanCode, uint followUp)
		{
			return (uint)(0
						   | (scanCode << 16)
						   | (0 << 24)
						   | (0 << 29)
						   | (followUp << 30)
						   | (followUp << 31));
		}

		public static MSG[] GetControlKeyMessages(this VirtualKeys key, KeyState state)
		{
			if (state == KeyState.Down || (state != KeyState.Up && state == KeyState.E0))
			{
				return new[]
				{
						new MSG(){message = (int) WMessages.WM_KEYDOWN, wParam = (IntPtr) key, lParam = unchecked((IntPtr)(int) GetLParamForKey(key.GetScanCode(),0))},
						new MSG(){message = (int) WMessages.WM_CHAR, wParam = (IntPtr) key, lParam = unchecked((IntPtr)(int) GetLParamForKey(key.GetScanCode(),0))},
				};
			}
			else if (state == KeyState.Up)
			{
				return new[]
				{
						new MSG(){message = (int) WMessages.WM_KEYUP, wParam = (IntPtr) key, lParam = unchecked((IntPtr)(int) GetLParamForKey(key.GetScanCode(),1))},
				};
			}
			return new MSG[0];

		}

		public static MSG[] GetControlClickMessages(string button, bool down, int x, int y)
		{
			IntPtr LParam = MakeLParam(x, y);

			int btnKey = 0;

			if (button == "left")
			{
				if (down)
				{
					btnKey = (int)WMessages.WM_LBUTTONDOWN;
				}
				else
				{
					btnKey = (int)WMessages.WM_LBUTTONUP;
				}
			}

			if (button == "right")
			{
				if (down)
				{
					btnKey = (int)WMessages.WM_RBUTTONDOWN;
				}
				else
				{
					btnKey = (int)WMessages.WM_RBUTTONUP;
				}
			}

			if (button == "middle")
			{
				if (down)
				{
					btnKey = (int)WMessages.WM_MBUTTONDOWN;
				}
				else
				{
					btnKey = (int)WMessages.WM_MBUTTONUP;
				}
			}

			MSG[] result;

			result = new MSG[1];
			result[0] = new MSG() { lParam = LParam, hwnd = IntPtr.Zero, message = btnKey, pt_x = x, pt_y = y, wParam = IntPtr.Zero };
			return result;
		}

		public static void ControlClickWindow(IntPtr hWnd, string button, int x, int y, bool doubleklick)
		{
			foreach (var controlClickMessage in GetControlClickMessages(button, x, y, doubleklick))
			{
				SendMessageTo(hWnd, controlClickMessage.message, controlClickMessage.wParam, controlClickMessage.lParam);
			}
		}
	}
}
