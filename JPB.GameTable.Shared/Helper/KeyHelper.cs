using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Interceptor;

namespace JPB.GameTable.Shared.Helper
{
	public static class KeyHelper
	{
		public static string KeyToString(this Keys key)
		{
			var toInsert = key.ToString();
			if (toInsert.Length != 1)
			{
				var convertedKey = (AllowedKeys)key;
				if (convertedKey == 0)
				{
					return null;
				}

				toInsert = typeof(AllowedKeys)
				           .GetMember(convertedKey.ToString()).FirstOrDefault()?.GetCustomAttribute<DefaultValueAttribute>()?.Value
				           ?.ToString();

				return toInsert;
			}

			return toInsert;
		}

		[DllImport("user32.dll")]
		static extern int MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll")]
		public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
				byte[] keyboardState,
				[Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
				StringBuilder receivingBuffer,
				int bufferSize, uint flags);

		public static int GetScanCode(this VirtualKeys keys)
		{
			return MapVirtualKey((uint) keys, 2);
		}

		public static string GetCharsFromKeys(this VirtualKeys keys, bool shift, bool altGr)
		{
			var buf = new StringBuilder(256);
			var keyboardState = new byte[256];
			if (shift)
			{
				keyboardState[(int)VirtualKeys.Shift] = 0xff;
			}
			if (altGr)
			{
				keyboardState[(int)VirtualKeys.Control] = 0xff;
				keyboardState[(int)VirtualKeys.Menu] = 0xff;
			}
			ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
			return buf.ToString();
		}

		public static char? KeyToChar(this Keys key)
		{
			var toInsert = key.ToString();
			if (toInsert.Length != 1)
			{
				var convertedKey = (AllowedKeys)key;
				if (convertedKey == 0)
				{
					return null;
				}

				var charCode = typeof(AllowedKeys)
				           .GetMember(convertedKey.ToString()).FirstOrDefault()?.GetCustomAttribute<KeyCharAttribute>()?.CharCode;

				if (charCode != null)
				{
					return (char?) MapVirtualKey((uint) charCode, 2);
				}
			}

			return toInsert[0];
		}
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	sealed class KeyCharAttribute : Attribute
	{
		public int CharCode { get; }

		public KeyCharAttribute(int charCode)
		{
			CharCode = charCode;
		}

		public KeyCharAttribute(char charCode)
		{
			CharCode = charCode;
		}
	}

	public enum AllowedKeys : ushort
	{
		[KeyChar(0x1D)]
		Escape = 1,
		[DefaultValue(1)]
		[KeyChar(0x31)]
		One = 2,
		[DefaultValue(2)]
		[KeyChar(0x32)]
		Two = 3,
		[DefaultValue(3)]
		[KeyChar(0x33)]
		Three = 4,
		[DefaultValue(4)]
		[KeyChar(0x34)]
		Four = 5,
		[DefaultValue(5)]
		[KeyChar(0x35)]
		Five = 6,
		[DefaultValue(6)]
		[KeyChar(0x36)]
		Six = 7,
		[DefaultValue(7)]
		[KeyChar(0x37)]
		Seven = 8,
		[DefaultValue(8)]
		[KeyChar(0x38)]
		Eight = 9,
		[DefaultValue(9)]
		[KeyChar(0x39)]
		Nine = 10,
		[DefaultValue(0)]
		[KeyChar(0x30)]
		Zero = 11,
		[DefaultValue("_")]
		DashUnderscore = 12,
		[KeyChar(0x08)]
		Backspace = 14,
		[KeyChar(0x51)]
		Q = 16,
		[KeyChar(0x57)]
		W = 17,
		[KeyChar(0x45)]
		E = 18,
		[KeyChar(0x52)]
		R = 19,
		[KeyChar(0x54)]
		T = 20,
		[KeyChar(0x59)]
		Y = 21,
		[KeyChar(0x55)]
		U = 22,
		[KeyChar(0x49)]
		I = 23,
		[KeyChar(0x4E)]
		O = 24,
		[KeyChar(0x50)]
		P = 25,
		[DefaultValue("{")]
		OpenBracketBrace = 26,
		[DefaultValue("}")]
		CloseBracketBrace = 27,
		[DefaultValue("\r\n")]
		[KeyChar(0x0D)]
		Enter = 28,
		[KeyChar(0x41)]
		A = 30,
		[KeyChar(0x53)]
		S = 31,
		[KeyChar(0x44)]
		D = 32,
		[KeyChar(0x46)]
		F = 33,
		[KeyChar(0x47)]
		G = 34,
		[KeyChar(0x48)]
		H = 35,
		[KeyChar(0x5A)]
		J = 36,
		[KeyChar(0x4B)]
		K = 37,
		[KeyChar(0x4C)]
		L = 38,
		[DefaultValue(";")]
		SemicolonColon = 39,
		[DefaultValue("\"")]
		SingleDoubleQuote = 40,
		[DefaultValue("~")]
		Tilde = 41,
		LeftShift = 42,
		BackslashPipe = 43,
		[KeyChar(0x5A)]
		Z = 44,
		[KeyChar(0x58)]
		X = 45,
		[KeyChar(0x43)]
		C = 46,
		[KeyChar(0x56)]
		V = 47,
		[KeyChar(0x42)]
		B = 48,
		[KeyChar(0x4E)]
		N = 49,
		[KeyChar(0x4D)]
		M = 50,
		[DefaultValue(" ")]
		[KeyChar(0x20)]
		Space = 57,
		F8 = 66,
		F9 = 67,
		F10 = 68,
		F11 = 87,
		F12 = 88,
		[KeyChar(0x26)]
		Up = 72,
		[KeyChar(0x28)]
		Down = 80,
		[KeyChar(0x27)]
		Right = 77,
		[KeyChar(0x25)]
		Left = 75,
		[KeyChar(0x2E)]
		Delete = 83,
		[KeyChar(0x2D)]
		Insert = 82,
		[DefaultValue("/")]
		[KeyChar(0x6F)]
		NumpadDivide = 53,
		[DefaultValue("*")]
		[KeyChar(0x6A)]
		NumpadAsterisk = 55,
		[DefaultValue("7")]
		[KeyChar(0x37)]
		Numpad7 = 71,
		[DefaultValue("8")]
		[KeyChar(0x38)]
		Numpad8 = 72,
		[DefaultValue("9")]
		[KeyChar(0x39)]
		Numpad9 = 73,
		[DefaultValue("4")]
		[KeyChar(0x35)]
		Numpad4 = 75,
		[DefaultValue("5")]
		[KeyChar(0x36)]
		Numpad5 = 76,
		[DefaultValue("6")]
		[KeyChar(0x37)]
		Numpad6 = 77,
		[DefaultValue("1")]
		[KeyChar(0x32)]
		Numpad1 = 79,
		[DefaultValue("2")]
		[KeyChar(0x33)]
		Numpad2 = 80,
		[DefaultValue("3")]
		[KeyChar(0x34)]
		Numpad3 = 81,
		[DefaultValue("0")]
		[KeyChar(0x31)]
		Numpad0 = 82,
		NumpadDelete = 83,
		NumpadEnter = 28,
		[DefaultValue("+")]
		[KeyChar(0x6B)]
		NumpadPlus = 78,
		[DefaultValue("-")]
		[KeyChar(0x6D)]
		NumpadMinus = 74
	}

}
