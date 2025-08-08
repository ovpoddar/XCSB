using Xcsb.Models;
using Xcsb;
using Xcsb.Masks;

var client = XcsbClient.Initialized();
var screen = client.HandshakeSuccessResponseBody.Screens[0];
var window = client.NewId();
client.CreateWindowChecked(
    screen.RootDepth!.DepthValue,
    window,
    screen.Root,
    0, 0, 500, 500, 0, ClassType.InputOutput,
    screen.RootVisualId, ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);
client.MapWindowChecked(window);

var path = client.GetFontPath();


client.ChangeKeyboardMappingChecked(
    1,
    0,
    1,
    [0]
);


for (int i = 1; i < 2; i++)
{
    var ev = client.GetEvent();
    if (ev != null && ev.Value.EventType != Xcsb.Event.EventType.Error) Console.WriteLine("Not working");
    client.WaitForEvent();
}