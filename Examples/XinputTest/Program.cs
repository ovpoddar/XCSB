using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Models;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Extension.XInput;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Models.TypeInfo;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Response.Event;
using KeyPressEvent = Xcsb.Extension.XInput.Response.Event.KeyPressEvent;

using var con = XcsbClient.Connect();

var x = con.Initialized();
var screen = con.HandshakeSuccessResponseBody.Screens[0];
var wnd = con.NewId();

x.CreateWindowUnchecked(
    0,
    wnd,
    screen.Root,
    0, 0, 400, 300, 0, ClassType.InputOutput,
    screen.RootVisualId, ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)Xcsb.Masks.EventMask.ExposureMask]);
x.MapWindow(wnd);

var ext = con.Extension.XInput();
if (ext is null)
    return;
ext.XiSelectEventsChecked(wnd,
    EventMaskBuilder.Create()
        .AddEventMask(InputDevice.DeviceAllMaster, [XiEventMask.ButtonPress | XiEventMask.KeyPress]));

while (true)
{
    using var evnt = x.GetEvent();
    // var evnt = await x.GetEventAsync();
    if (evnt.Reply.ReplyType == EventType.LastEvent) break;

    if (evnt.Reply.ReplyType == XiInputEventType.KeyPress)
    {
        var keypress = evnt.Reply.As<KeyPressEvent>();
        Console.WriteLine(keypress.ResponseHeader.Reply);
    }

    if (evnt.Reply.ReplyType == EventType.Expose)
    {
        ref readonly var expose = ref evnt.Reply.As<ExposeEvent>();
        Console.WriteLine(expose.X + " " + expose.Y + " " + expose.Width + " " + expose.Height);
    }
    
    Console.WriteLine((string)evnt.Reply.ReplyType + " " + evnt.Buffer.Length);
}

Console.WriteLine("Done");