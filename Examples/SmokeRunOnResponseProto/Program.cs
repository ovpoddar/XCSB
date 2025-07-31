using System.Diagnostics;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;

var client = XcsbClient.Initialized();
var window = client.NewId();
client.CreateWindowChecked(
    0,
    window,
    client.HandshakeSuccessResponseBody.Screens[0].Root,
    0, 0, 600, 800, 0, ClassType.InputOutput,
    client.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [0x00ff00, (uint)(EventMask.ExposureMask)]
);
client.MapWindowChecked(window);

var font = client.NewId();
client.OpenFont("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", font);


var namedColor = client.AllocNamedColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Red"u8);
Console.WriteLine($"{namedColor.ExactBlue} {namedColor.ExactGreen} {namedColor.ExactRed}");

Debug.Assert(namedColor.VisualRed == namedColor.ExactRed && namedColor.ExactRed == ushort.MaxValue);
Debug.Assert(namedColor.VisualGreen == namedColor.ExactGreen && namedColor.ExactGreen == 0);
Debug.Assert(namedColor.VisualBlue == namedColor.ExactBlue && namedColor.ExactBlue == 0);


var lookUpColor = client.LookupColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Light Yellow"u8);

Debug.Assert(lookUpColor.VisualRed == lookUpColor.ExactRed && lookUpColor.ExactRed == ushort.MaxValue);
Debug.Assert(lookUpColor.VisualGreen == lookUpColor.ExactGreen && lookUpColor.ExactGreen == ushort.MaxValue);

while (true)
{
    var Event = client.GetEvent();
    if (!Event.HasValue)
        break;
    if (Event.Value.EventType == EventType.Error)
    {
        Console.WriteLine(Event.Value.ErrorEvent.ErrorCode);
        break;
    }

    if (Event.Value.EventType == EventType.Expose)
    {
        var resultQueryTextExtents = client.QueryTextExtents(font, "Hello World");
        Console.WriteLine("QueryTextExtents");
        var resultGetMotionEvents = client.GetMotionEvents(window, 0, 10000);
        Console.WriteLine(resultGetMotionEvents.Events.Length);

        var resultQueryKeymap = client.QueryKeymap();
        Console.WriteLine("QueryKeymap");
        var resultGetScreenSaver = client.GetScreenSaver();
        Console.WriteLine("GetScreenSaver");
        var resultGetPointerControl = client.GetPointerControl();
        Console.WriteLine("GetPointerControl");
        var resultGetModifierMapping = client.GetModifierMapping();
        Console.WriteLine("GetModifierMapping");
        var resultSetModifierMapping = client.SetModifierMapping(resultGetModifierMapping.Keycodes);
        Console.WriteLine("resultSetModifierMapping");
        var resultGetKeyboardControl = client.GetKeyboardControl();
        Console.WriteLine("GetKeyboardControl");
        var resultGetPointerMapping = client.GetPointerMapping();
        Console.WriteLine("GetPointerMapping");

        //var resultGetFontPath = client.GetFontPath();
        //Console.WriteLine("resultGetFontPath");
        //var resultAllocColorCells = client.AllocColorCells(false,
        //    client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
        //    0xff,
        //    0);
        //Console.WriteLine("resultAllocColorCells");
        //var resultQueryBestSize = client.QueryBestSize(QueryShapeOf.FastestTile, client.HandshakeSuccessResponseBody.Screens[0].Root, 100, 200);
        //Console.WriteLine("resultQueryBestSize");
        //var resultAllocColorPlanes = client.AllocColorPlanes();
        //Console.WriteLine("resultAllocColorPlanes");

        //var resultQueryFont = client.QueryFont(font);
        //Console.WriteLine("resultQueryFont");
    }
}