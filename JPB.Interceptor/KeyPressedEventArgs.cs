using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interceptor
{
    public class KeyPressedEventArgs : EventArgs
    {
        public VirtualKeys Key { get; set; }
        public KeyState State { get; set; }
        public bool Handled { get; set; }
	    public int DeviceId { get; set; }
	    public string DeviceName { get; set; }
    }
}
