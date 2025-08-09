using System.Diagnostics;
using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

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

client.ChangeActivePointerGrabChecked(0, 0, (ushort)EventMask.ButtonPressMask);

var font = client.NewId();
client.OpenFont("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", font);


var namedColor = client.AllocNamedColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Red"u8);
Console.WriteLine($"{namedColor.ExactBlue} {namedColor.ExactGreen} {namedColor.ExactRed}");

client.CloseFont(font);

Debug.Assert(namedColor.VisualRed == namedColor.ExactRed && namedColor.ExactRed == ushort.MaxValue);
Debug.Assert(namedColor.VisualGreen == namedColor.ExactGreen && namedColor.ExactGreen == 0);
Debug.Assert(namedColor.VisualBlue == namedColor.ExactBlue && namedColor.ExactBlue == 0);


var lookUpColor = client.LookupColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Light Yellow"u8);

Debug.Assert(lookUpColor.VisualRed == lookUpColor.ExactRed && lookUpColor.ExactRed == ushort.MaxValue);
Debug.Assert(lookUpColor.VisualGreen == lookUpColor.ExactGreen && lookUpColor.ExactGreen == ushort.MaxValue);

var keyboardMapping = client.GetKeyboardMapping(client.HandshakeSuccessResponseBody.MinKeyCode,
    (byte)(client.HandshakeSuccessResponseBody.MaxKeyCode - client.HandshakeSuccessResponseBody.MinKeyCode + 1));

var originalKeySym = keyboardMapping.Keysyms;
Console.WriteLine(string.Join(", ", originalKeySym));
var keysyms_per_keycode = new uint[keyboardMapping.KeyPerKeyCode];
Array.Copy(originalKeySym[0..keyboardMapping.KeyPerKeyCode], keysyms_per_keycode, keyboardMapping.KeyPerKeyCode);
keysyms_per_keycode[0] = 0x0061;
keysyms_per_keycode[1] = 0x0062;

client.ChangeKeyboardMappingChecked(
    1,
    8,
    keyboardMapping.KeyPerKeyCode,
    keysyms_per_keycode
);
Console.WriteLine("ChangeKeyboardMapping: Modified one key (dummy)\n");

var queryColor = client.QueryColors(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
    [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");


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
        var resultGetMotionEvents = client.GetMotionEvents(window, 0, 10000);
        Console.WriteLine(resultGetMotionEvents.Events.Length);

        var getBestWindowSize = client.QueryBestSize(QueryShapeOf.LargestCursor,
            window, 32, 32);
        Console.WriteLine($"Best size for 32x32: {getBestWindowSize.Width} {getBestWindowSize.Height}");

        var resultQueryKeymap = client.QueryKeymap();
        Console.WriteLine($"{resultQueryKeymap.keys.Length}");
        var resultGetScreenSaver = client.GetScreenSaver();
        Console.WriteLine($"GetScreenSaver will time out {resultGetScreenSaver.Timeout}");
        var resultGetPointerControl = client.GetPointerControl();
        Console.WriteLine($"{resultGetPointerControl.AccelDenominator} {resultGetPointerControl.AccelNumerator}");
        var resultGetModifierMapping = client.GetModifierMapping();
        Console.WriteLine("GetModifierMapping");
        var resultSetModifierMapping = client.SetModifierMapping(resultGetModifierMapping.Keycodes);
        Console.WriteLine(resultSetModifierMapping.Status);
        var resultGetKeyboardControl = client.GetKeyboardControl();
        Console.WriteLine($"{resultGetKeyboardControl.BellPercent}");
        var resultGetPointerMapping = client.GetPointerMapping();
        var resultSetPointerMapping = client.SetPointerMapping(resultGetPointerMapping.Map);
        Console.WriteLine(resultSetPointerMapping.Status);
    }
}