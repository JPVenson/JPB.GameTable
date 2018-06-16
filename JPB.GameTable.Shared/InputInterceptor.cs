#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Interceptor;
using JPB.GameTable.Shared.Helper;
using JPB.Tasking.TaskManagement.Threading;

#endregion

namespace JPB.GameTable.UI
{
	public static class InterceptorFilter
	{
		public static int FilterMouse(int device)
		{
			return InterceptionDriver.IsMouse(device) + (device != 13 ? 0 : 1);
		}
	}

	public class InputInterceptor
	{
		private readonly HashSet<Keys> _allowedRanges;

		public InputInterceptor()
		{
			Mice = new Dictionary<int, InputTuple>();
			Keyboards = new Dictionary<int, InputTuple>();
			DeviceMap = new Dictionary<string, PersitantId>();

			DeviceMap.Add(@"HID\VID_1E7D&PID_2E24&REV_0100&MI_00&Col01HID\VID_1E7D&PID_2E24&MI_00&Col01HID\VID_1E7D&UP:0001_U:0002HID_DEVICE_SYSTEM_MOUSEHID_DEVICE_UP:0001_U:0002HID_DEVICE", new PersitantId()
			{
				DeviceName = @"HID\VID_1E7D&PID_2E24&REV_0100&MI_00&Col01HID\VID_1E7D&PID_2E24&MI_00&Col01HID\VID_1E7D&UP:0001_U:0002HID_DEVICE_SYSTEM_MOUSEHID_DEVICE_UP:0001_U:0002HID_DEVICE",
				PersistantId = 1,
				MaybeRealId = 14
			});

			DeviceMap.Add(@"HID\VID_1E7D&PID_2DB4&REV_0101&MI_00&Col01HID\VID_1E7D&PID_2DB4&MI_00&Col01HID\VID_1E7D&UP:0001_U:0002HID_DEVICE_SYSTEM_MOUSEHID_DEVICE_UP:0001_U:0002HID_DEVICE", new PersitantId()
			{
				DeviceName = @"HID\VID_1E7D&PID_2DB4&REV_0101&MI_00&Col01HID\VID_1E7D&PID_2DB4&MI_00&Col01HID\VID_1E7D&UP:0001_U:0002HID_DEVICE_SYSTEM_MOUSEHID_DEVICE_UP:0001_U:0002HID_DEVICE",
				PersistantId = 2,
				MaybeRealId = 12
			});


			DeviceMap.Add(@"HID\VID_1E7D&PID_319C&REV_0100&MI_00HID\VID_1E7D&PID_319C&MI_00HID\VID_1E7D&UP:0001_U:0006HID_DEVICE_SYSTEM_KEYBOARDHID_DEVICE_UP:0001_U:0006HID_DEVICE", new PersitantId()
			{
				DeviceName = @"HID\VID_1E7D&PID_319C&REV_0100&MI_00HID\VID_1E7D&PID_319C&MI_00HID\VID_1E7D&UP:0001_U:0006HID_DEVICE_SYSTEM_KEYBOARDHID_DEVICE_UP:0001_U:0006HID_DEVICE",
				PersistantId = 10,
				MaybeRealId = 3
			});

			DeviceMap.Add(@"HID\VID_1E7D&PID_2FA8&REV_010:&MI_00HID\VID_1E7D&PID_2FA8&MI_00HID\VID_1E7D&UP:0001_U:0006HID_DEVICE_SYSTEM_KEYBOARDHID_DEVICE_UP:0001_U:0006HID_DEVICE", new PersitantId()
			{
				DeviceName = @"HID\VID_1E7D&PID_2FA8&REV_010:&MI_00HID\VID_1E7D&PID_2FA8&MI_00HID\VID_1E7D&UP:0001_U:0006HID_DEVICE_SYSTEM_KEYBOARDHID_DEVICE_UP:0001_U:0006HID_DEVICE",
				PersistantId = 11,
				MaybeRealId = 1
			});

			Mice.Add(1, new InputTuple(true, 1, 10));
			Mice.Add(2, new InputTuple(false, 2, 11));

			foreach (var inputTuple in Mice)
			{
				Keyboards.Add(inputTuple.Value.KeyboardDevice, inputTuple.Value);
			}
			_allowedRanges = new HashSet<Keys>();
			if (!Debugger.IsAttached)
			{
				var values = Enum.GetValues(typeof(AllowedKeys));
				foreach (var value in values)
				{
					_allowedRanges.Add((Keys)value);
				}
			}
		}

		public IDictionary<int, InputTuple> Mice { get; set; }
		public IDictionary<int, InputTuple> Keyboards { get; set; }
		public IDictionary<string, PersitantId> DeviceMap { get; set; }

		public struct PersitantId
		{
			public string DeviceName { get; set; }
			public int PersistantId { get; set; }
			public int MaybeRealId { get; set; }
		}

		public Input Input { get; set; }
		public event EventHandler<MousePressedEventArgs> VirtualMouseAction;
		public event EventHandler<KeyPressedEventArgs> VirtualKeyboardAction;

		public void Start()
		{
			Input = new Input
			{
				MouseFilterMode = MouseFilterMode.All,
				KeyboardFilterMode = KeyboardFilterMode.All,
				FilterType = FilterType.Whitelist
			};

			Input.SetKeyboardDeviceList(Keyboards.Where(e => !e.Value.Natural).Select(e => DeviceMap.FirstOrDefault(f => f.Value.PersistantId == e.Key).Value.MaybeRealId).ToArray());
			Input.SetMiceDeviceList(Mice.Where(e => !e.Value.Natural).Select(e => DeviceMap.FirstOrDefault(f => f.Value.PersistantId == e.Key).Value.MaybeRealId).ToArray());

			//Input.KeyboardFilterMode = KeyboardFilterMode.All;
			Input.OnMouseAction += InputOnMouseAction;
			Input.OnKeyPressed += Input_OnKeyPressed;
			Input.Load();
		}

		private void Input_OnKeyPressed(object sender, KeyPressedEventArgs e)
		{
			e.DeviceId = DeviceMap[e.DeviceName].PersistantId;
			if (Keyboards.ContainsKey(e.DeviceId))
			{
				var inputTuple = Keyboards[e.DeviceId];
				if (!inputTuple.Natural)
				{
					inputTuple.PreKeyData = inputTuple.LastKeyData;
					inputTuple.LastKeyData = e;
					OnVirtualKeyboardAction(e);
					e.Handled = true;
				}
			}
		}

		private void InputOnMouseAction(object sender, MousePressedEventArgs e)
		{
			e.DeviceId = DeviceMap[e.DeviceName].PersistantId;
			if (Mice.ContainsKey(e.DeviceId))
			{
				var inputTuple = Mice[e.DeviceId];
				if (!inputTuple.Natural)
				{
					inputTuple.PreMouseData = inputTuple.LastMouseData;
					inputTuple.LastMouseData = e;
					OnVirtualMouseAction(e);
					e.Handled = true;
				}
			}
		}

		protected virtual void OnVirtualMouseAction(MousePressedEventArgs e)
		{
			VirtualMouseAction?.Invoke(this, e);
		}

		public void Stop()
		{
			Input?.Unload();
		}

	
		public class InputTuple
		{
			public InputTuple(bool natural, int mouseDevice, int keyboardDevice)
			{
				Natural = natural;
				MouseDevice = mouseDevice;
				KeyboardDevice = keyboardDevice;
				LastMouseData = new MousePressedEventArgs(0);
				LastKeyData = new KeyPressedEventArgs();
				PreMouseData = new MousePressedEventArgs(0);
				PreKeyData = new KeyPressedEventArgs();
			}

			public bool InUse { get; set; }
			public bool Natural { get; private set; }
			public int MouseDevice { get; private set; }
			public int KeyboardDevice { get; private set; }
			public MousePressedEventArgs PreMouseData { get; set; }
			public MousePressedEventArgs LastMouseData { get; set; }
			public KeyPressedEventArgs PreKeyData { get; set; }
			public KeyPressedEventArgs LastKeyData { get; set; }
		}

		protected virtual void OnVirtualKeyboardAction(KeyPressedEventArgs e)
		{
			VirtualKeyboardAction?.Invoke(this, e);
		}
	}
}