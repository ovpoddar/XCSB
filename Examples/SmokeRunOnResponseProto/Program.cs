using System.Diagnostics;
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.TypeInfo;

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


using var namedColor = client.AllocNamedColor(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Red"u8);
Console.WriteLine($"{namedColor.Reply.ExactBlue} {namedColor.Reply.ExactGreen} {namedColor.Reply.ExactRed}");

client.CloseFontChecked(font);

Debug.Assert(namedColor.Reply.VisualRed == namedColor.Reply.ExactRed && namedColor.Reply.ExactRed == ushort.MaxValue);
Debug.Assert(namedColor.Reply.VisualGreen == namedColor.Reply.ExactGreen && namedColor.Reply.ExactGreen == 0);
Debug.Assert(namedColor.Reply.VisualBlue == namedColor.Reply.ExactBlue && namedColor.Reply.ExactBlue == 0);

var detailsFont = client.ListFontsWithInfo("*"u8, 5);
foreach (var item in detailsFont)
    Console.WriteLine(item.Name);

using var lookUpColor = client.LookupColor(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Light Yellow"u8);
Debug.Assert(lookUpColor.Reply.VisualRed == lookUpColor.Reply.ExactRed && lookUpColor.Reply.ExactRed == ushort.MaxValue);
Debug.Assert(lookUpColor.Reply.VisualGreen == lookUpColor.Reply.ExactGreen && lookUpColor.Reply.ExactGreen == ushort.MaxValue);

using var keyboardMapping = client.GetKeyboardMapping(connection.HandshakeSuccessResponseBody.MinKeyCode,
    (byte)(connection.HandshakeSuccessResponseBody.MaxKeyCode - connection.HandshakeSuccessResponseBody.MinKeyCode + 1));

var originalKeySym = keyboardMapping.Reply.Keysyms;
Console.WriteLine(string.Join(", ", originalKeySym));
var keysyms_per_keycode = new uint[keyboardMapping.Reply.KeyPerKeyCode];
Array.Copy(originalKeySym[0..keyboardMapping.Reply.KeyPerKeyCode], keysyms_per_keycode, keyboardMapping.Reply.KeyPerKeyCode);
keysyms_per_keycode[0] = 0x0061;
keysyms_per_keycode[1] = 0x0062;

client.ChangeKeyboardMappingChecked(
    1,
    8,
    keyboardMapping.Reply.KeyPerKeyCode,
    keysyms_per_keycode
);
Console.WriteLine("ChangeKeyboardMapping: Modified one key (dummy)\n");

using var queryColor = client.QueryColors(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
    [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Reply.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");


using var font_path_reply = client.GetFontPath();
client.SetFontPathChecked(font_path_reply.Reply.Paths);

var pattan = "*"u8;
using var listFonts = client.ListFonts(pattan, 10);
foreach (var fontName in listFonts.Reply.Fonts)
    Console.WriteLine(fontName);

font = connection.NewId();
client.OpenFontUnchecked("fixed", font);
using var font_info = client.QueryFont(font);
Console.WriteLine($"QueryFont: Max bounds width:  {font_info.Reply.MaxBounds.CharacterWidth}\n");
var text = "Hello, XCB!";
using var text_extents = client.QueryTextExtents(font, text);
Console.WriteLine($"QueryTextExtents: Width of '{text}': {text_extents.Reply.OverallWidth}\n");

using var motion = client.GetMotionEvents(window, 0, 0);
Console.WriteLine($"GetMotionEvents: {motion.Reply.Events.Length} events");

using var screensaver = client.GetScreenSaver();
Console.WriteLine($"GetScreenSaver: Timeout: {screensaver.Reply.Timeout}");

using var queryColor1 = client.QueryColors(connection.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
     [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor1.Reply.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");

while (true)
{
    using var Event = client.GetEvent();
    if (Event.Reply.ReplyType == EventType.LastEvent)
        break;
    if (Event.Reply.Error.HasValue)
    {
        Console.WriteLine(Event.Reply.Error.Value.Message);
        break;
    }

    if (Event.Reply.ReplyType == EventType.Expose)
    {
        using var getBestWindowSize = client.QueryBestSize(QueryShapeOf.LargestCursor,
            window, 32, 32);
        Console.WriteLine($"Best size for 32x32: {getBestWindowSize.Reply.Width} {getBestWindowSize.Reply.Height}");

        using var resultQueryKeymap = client.QueryKeymap();
        Console.WriteLine($"{resultQueryKeymap.Reply.keys.Length}");
        using var resultGetScreenSaver = client.GetScreenSaver();
        Console.WriteLine($"GetScreenSaver will time out {resultGetScreenSaver.Reply.Timeout}");
        using var resultGetPointerControl = client.GetPointerControl();
        Console.WriteLine($"{resultGetPointerControl.Reply.AccelDenominator} {resultGetPointerControl.Reply.AccelNumerator}");
        using var resultGetModifierMapping = client.GetModifierMapping();
        Console.WriteLine("GetModifierMapping");
        using var resultSetModifierMapping = client.SetModifierMapping(resultGetModifierMapping.Reply.Keycodes);
        Console.WriteLine(resultSetModifierMapping.Reply.Status);
        using var resultGetKeyboardControl = client.GetKeyboardControl();
        Console.WriteLine($"{resultGetKeyboardControl.Reply.BellPercent}");
        using var resultGetPointerMapping = client.GetPointerMapping();
        using var resultSetPointerMapping = client.SetPointerMapping(resultGetPointerMapping.Reply.Map);
        Console.WriteLine(resultSetPointerMapping.Reply.Status);
    }
    using var resultGetMotionEvents = client.GetMotionEvents(window, 0, 1000);
    Console.WriteLine(resultGetMotionEvents.Reply.Events.Length);
}