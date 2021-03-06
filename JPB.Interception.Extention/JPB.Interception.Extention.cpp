// JPB.Interception.Extention.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "interception.h"


#ifdef __cplusplus
extern "C" {
#endif

	static int _listKeyboard[20];
	static int _listMice[20];

	__declspec(dllexport) unsigned int interception_get_hardware_id_string(InterceptionContext context, InterceptionDevice device, wchar_t hardware_id_buffer[], unsigned int buffer_size)
	{
		size_t length = interception_get_hardware_id(context, device, hardware_id_buffer, buffer_size);

		if (length > 0 && length < buffer_size) {
			return length;
		}

		return -1;
	}

	__declspec(dllexport) void interception_set_keyboard_list(int listOfDevices[], int length) {
		for (size_t i = 0; i < 20; i++)
		{
			_listKeyboard[i] = 0;
		}

		for (size_t i = 0; i < length; i++)
		{
			_listKeyboard[i] = listOfDevices[i];
		}
	}

	__declspec(dllexport) void interception_set_mice_list(int listOfDevices[], int length) {
		for (size_t i = 0; i < 20; i++)
		{
			_listMice[i] = 0;
		}

		for (size_t i = 0; i < length; i++)
		{
			_listMice[i] = listOfDevices[i];
		}
	}

	__declspec(dllexport) int interception_is_keyboard_whitelist(InterceptionDevice device)
	{
		if (!interception_is_keyboard(device)) {
			return 0;
		}

		for (int i = 0; i < 20; i++) {
			if (_listKeyboard[i] == device)
			{
				return 1;
			}
		}
		return 0;
	}

	__declspec(dllexport) int interception_is_mouse_whitelist(InterceptionDevice device)
	{
		if (!interception_is_mouse(device)) {
			return 0;
		}

		for (int i = 0; i < 20; i++) {
			if (_listMice[i] == device)
			{
				return 1;
			}
		}
		return 0;
	}

	__declspec(dllexport) int interception_is_keyboard_blacklist(InterceptionDevice device)
	{
		if (!interception_is_keyboard(device)) {
			return 0;
		}

		int found = 0;

		for (int i = 0; i < 20; i++) {
			found = found || _listKeyboard[i] != device;
		}
		return found;
	}

	__declspec(dllexport) int interception_is_mouse_blacklist(InterceptionDevice device)
	{
		if (!interception_is_mouse(device)) {
			return 0;
		}

		int found = 0;

		for (int i = 0; i < 20; i++) {
			found = found || _listMice[i] != device;
		}
		return found;
	}

#ifdef __cplusplus
}
#endif

