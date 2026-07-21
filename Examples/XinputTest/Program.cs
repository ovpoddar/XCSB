using Xcsb;
using Xcsb.Connection;
using Xcsb.Extension.XInput;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models.TypeInfo;
using Xcsb.Extension.XInput.Models.Writers;

using var con = XcsbClient.Connect();
var x = con.Initialized();
var screen = con.HandshakeSuccessResponseBody.Screens[0];
var wnd = con.NewId();

x.CreateWindowUnchecked(
		0,
		wnd,
		screen.Root,
		0, 0, 400,300, 0, ClassType.InputOutput,
		screen.RootVisualId, ValueMask.BackgroundPixel | ValueMask.EventMask,
		[screen.WhitePixel, (uint)Xcsb.Masks.EventMask.ExposureMask]);
x.MapWindow(wnd);

var ext = con.Extension.XInput();
if (ext is null)
	return;
ext.XiSelectEventsChecked(wnd, 	EventMaskBuilder.Create().AddEventMask(InputDevice.DeviceAllMaster, [XiEventMask.RawButtonRelease]));
while(true)
{
	var evnt = x.GetEvent();

	if (evnt.ReplyType ==EventType.LastEvent) break;

	Console.WriteLine(evnt.ReplyType);
}

