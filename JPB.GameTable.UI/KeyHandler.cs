﻿//using System;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;
//using System.Windows.Input;

//namespace JPB.GameTable.UI
//{
//	public static class KeyHandler
//	{
//		/// <summary>Determines whether the specified keyboard input should be allowed to be processed by the system.</summary>
//		/// <remarks>Helps block unwanted keys and key combinations that could exit the app, make system changes, etc.</remarks>
//		public static bool AllowKeyboardInput(bool alt, bool control, Keys key)
//		{
//			// Disallow various special keys.
//			if (key <= Keys.Back || key == Keys.None ||
//				key == Keys.Menu || key == Keys.Pause ||
//				key == Keys.Help)
//			{
//				return false;
//			}

//			// Disallow ranges of special keys.
//			// Currently leaves volume controls enabled; consider if this makes sense.
//			// Disables non-existing Keys up to 65534, to err on the side of caution for future keyboard expansion.
//			if ((key >= Keys.LWin && key <= Keys.Sleep) ||
//				(key >= Keys.KanaMode && key <= Keys.HanjaMode) ||
//				(key >= Keys.IMEConvert && key <= Keys.IMEModeChange) ||
//				(key >= Keys.BrowserBack && key <= Keys.BrowserHome) ||
//				(key >= Keys.MediaNextTrack && key <= Keys.LaunchApplication2) ||
//				(key >= Keys.ProcessKey && key <= (Keys)65534))
//			{
//				return false;
//			}

//			// Disallow specific key combinations. (These component keys would be OK on their own.)
//			if ((alt && key == Keys.Tab) ||
//				(alt && key == Keys.Space) ||
//				(control && key == Keys.Escape) ||
//				(control && key == Keys.Delete))
//			{
//				return false;
//			}

//			// Allow anything else (like letters, numbers, spacebar, braces, and so on).
//			return true;
//		}

//		private static IntPtr _hookID = IntPtr.Zero;


//		/// <summary>
//		/// Detach the keyboard hook; call during shutdown to prevent calls as we unload
//		/// </summary>
//		public static void DetachKeyboardHook()
//		{
//			if (_hookID != IntPtr.Zero)
//			{
//				InterceptKeys.UnhookWindowsHookEx(_hookID);
//			}
//		}

//		public static void RegisterKeyHandler()
//		{
//			_hookID = InterceptKeys.SetHook(_proc);
//		}


//		private static readonly InterceptKeys.LowLevelKeyboardProc _proc = KeyboardHook;

//		private static IntPtr KeyboardHook(int nCode, IntPtr wParam, IntPtr lParam)
//		{
//			if (nCode >= 0)
//			{
//				bool alt = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
//				bool control = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

//				int vkCode = Marshal.ReadInt32(lParam);
//				Keys key = (Keys)vkCode;

//				if (alt && key == Keys.F4)
//				{
//					return (IntPtr)1; // Handled.
//				}

//				if (!AllowKeyboardInput(alt, control, key))
//				{
//					return (IntPtr)1; // Handled.
//				}
//			}

//			return InterceptKeys.CallNextHookEx(_hookID, nCode, wParam, lParam);
//		}
//	}
//}