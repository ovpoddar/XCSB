using System.Diagnostics;
using Xcsb;
using Xcsb.Extension.Generic.Event;
using Xcsb.Masks;
using Xcsb.Models;

using var connection = XcsbClient.Connect();
var client = connection.Initialized();
var window = connection.NewId();
client.CreateWindowChecked(
    0,
    window,
    connection.HandshakeSuccessResponseBody.Screens[0].Root,
    0, 0, 600, 800, 0, ClassType.InputOutput,
    connection.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [0x00ff00, (uint)(EventMask.ExposureMask | EventMask.KeyReleaseMask | EventMask.KeyPressMask)]
);
client.MapWindowChecked(window);

client.ChangeActivePointerGrabChecked(0, 0, (ushort)EventMask.ButtonPressMask);

var font = connection.NewId();
client.OpenFontChecked("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", font);


var namedColor = client.AllocNamedColor(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Red"u8);
Console.WriteLine($"{namedColor.ExactBlue} {namedColor.ExactGreen} {namedColor.ExactRed}");

client.CloseFontChecked(font);

Debug.Assert(namedColor.VisualRed == namedColor.ExactRed && namedColor.ExactRed == ushort.MaxValue);
Debug.Assert(namedColor.VisualGreen == namedColor.ExactGreen && namedColor.ExactGreen == 0);
Debug.Assert(namedColor.VisualBlue == namedColor.ExactBlue && namedColor.ExactBlue == 0);

var detailsFont = client.ListFontsWithInfo("*"u8, 5);
foreach (var item in detailsFont)
    Console.WriteLine(item.Name);

var lookUpColor = client.LookupColor(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Light Yellow"u8);
Debug.Assert(lookUpColor.VisualRed == lookUpColor.ExactRed && lookUpColor.ExactRed == ushort.MaxValue);
Debug.Assert(lookUpColor.VisualGreen == lookUpColor.ExactGreen && lookUpColor.ExactGreen == ushort.MaxValue);

var keyboardMapping = client.GetKeyboardMapping(connection.HandshakeSuccessResponseBody.MinKeyCode,
    (byte)(connection.HandshakeSuccessResponseBody.MaxKeyCode - connection.HandshakeSuccessResponseBody.MinKeyCode + 1));

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

var queryColor = client.QueryColors(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
    [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");



var font_path_reply = client.GetFontPath();
client.SetFontPathChecked(font_path_reply.Paths);

var pattan = "*"u8;
var listFonts = client.ListFonts(pattan, 10);
foreach (var fontName in listFonts.Fonts)
    Console.WriteLine(fontName);

font = connection.NewId();
client.OpenFontUnchecked("fixed", font);
var font_info = client.QueryFont(font);
Console.WriteLine($"QueryFont: Max bounds width:  {font_info.MaxBounds.CharacterWidth}\n");
var text = "Hello, XCB!";
var text_extents = client.QueryTextExtents(font, text);
Console.WriteLine($"QueryTextExtents: Width of '{text}': {text_extents.OverallWidth}\n");

var motion = client.GetMotionEvents(window, 0, 0);
Console.WriteLine($"GetMotionEvents: {motion.Events.Length} events");

var screensaver = client.GetScreenSaver();
Console.WriteLine($"GetScreenSaver: Timeout: {screensaver.Timeout}");

queryColor = client.QueryColors(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
     [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");

while (true)
{
    var Event = client.GetEvent();
    if (Event.ReplyType == XEventType.LastEvent)
        break;
    if (Event.Error.HasValue)
    {
        Console.WriteLine(Event.Error.Value.ResponseHeader.Reply);
        break;
    }

    if (Event.ReplyType == XEventType.Expose)
    {
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
    var resultGetMotionEvents = client.GetMotionEvents(window, 0, 1000);
    Console.WriteLine(resultGetMotionEvents.Events.Length);
}