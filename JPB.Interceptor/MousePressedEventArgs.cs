using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Interceptor
{
    public class MousePressedEventArgs : EventArgs
    {
	    public MousePressedEventArgs(int deviceId)
	    {
		    DeviceId = deviceId;
		    RealDeviceId = deviceId;
	    }

	    public int RealDeviceId { get; set; }

	    public MouseState State { get; set; }
        public bool Handled { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public short Rolling { get; set; }
	    public string DeviceName { get; set; }
	    public int DeviceId { get; set; }
	    public int AbsolutX { get; set; }
	    public int AbsolutY { get; set; }
    }
}
