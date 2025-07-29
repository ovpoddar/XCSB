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



while (true)
{
    var Event = client.GetEvent();
    if (!Event.HasValue) 
        break;
    if (Event.Value.EventType == EventType.Error)
    {
        Console.WriteLine(Event.Value.ErrorEvent.ErrorCode);
        // break;
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
        var resultGetKeyboardControl = client.GetKeyboardControl();
        Console.WriteLine("GetKeyboardControl");
        var resultGetPointerMapping = client.GetPointerMapping();
        Console.WriteLine("GetPointerMapping");
        var resultAllocNamedColor = client.AllocNamedColor();
        var resultSetModifierMapping = client.SetModifierMapping();
        var resultLookupColor = client.LookupColor();
        // var resultGetFontPath = client.GetFontPath();


        // var resultAllocColorCells = client.AllocColorCells();
        // var resultQueryFont = client.QueryFont();
        // var resultListInstalledColormaps = client.ListInstalledColormaps();
        // var resultAllocColorPlanes = client.AllocColorPlanes();
        // var resultQueryBestSize = client.QueryBestSize();
        // var resultSetPointerMapping = client.SetPointerMapping();

    }
}