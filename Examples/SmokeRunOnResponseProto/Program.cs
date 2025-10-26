using System.Diagnostics;
using Xcsb;
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
    [0x00ff00, (uint)(EventMask.ExposureMask | EventMask.KeyReleaseMask | EventMask.KeyPressMask)]
);
client.MapWindowChecked(window);

client.ChangeActivePointerGrabChecked(0, 0, (ushort)EventMask.ButtonPressMask);

var font = client.NewId();
client.OpenFontChecked("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", font);


var namedColor = client.AllocNamedColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Red"u8);
Console.WriteLine($"{namedColor.Value.ExactBlue} {namedColor.Value.ExactGreen} {namedColor.Value.ExactRed}");

client.CloseFontChecked(font);

Debug.Assert(namedColor.Value.VisualRed == namedColor.Value.ExactRed && namedColor.Value.ExactRed == ushort.MaxValue);
Debug.Assert(namedColor.Value.VisualGreen == namedColor.Value.ExactGreen && namedColor.Value.ExactGreen == 0);
Debug.Assert(namedColor.Value.VisualBlue == namedColor.Value.ExactBlue && namedColor.Value.ExactBlue == 0);

var detailsFont = client.ListFontsWithInfo("*"u8, 5);
foreach (var item in detailsFont)
    Console.WriteLine(item.Name);

var lookUpColor = client.LookupColor(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap, "Light Yellow"u8);
Debug.Assert(lookUpColor.Value.VisualRed == lookUpColor.Value.ExactRed && lookUpColor.Value.ExactRed == ushort.MaxValue);
Debug.Assert(lookUpColor.Value.VisualGreen == lookUpColor.Value.ExactGreen && lookUpColor.Value.ExactGreen == ushort.MaxValue);

var keyboardMapping = client.GetKeyboardMapping(client.HandshakeSuccessResponseBody.MinKeyCode,
    (byte)(client.HandshakeSuccessResponseBody.MaxKeyCode - client.HandshakeSuccessResponseBody.MinKeyCode + 1));

var originalKeySym = keyboardMapping.Value.Keysyms;
Console.WriteLine(string.Join(", ", originalKeySym));
var keysyms_per_keycode = new uint[keyboardMapping.Value.KeyPerKeyCode];
Array.Copy(originalKeySym[0..keyboardMapping.Value.KeyPerKeyCode], keysyms_per_keycode, keyboardMapping.Value.KeyPerKeyCode);
keysyms_per_keycode[0] = 0x0061;
keysyms_per_keycode[1] = 0x0062;

client.ChangeKeyboardMappingChecked(
    1,
    8,
    keyboardMapping.Value.KeyPerKeyCode,
    keysyms_per_keycode
);
Console.WriteLine("ChangeKeyboardMapping: Modified one key (dummy)\n");

var queryColor = client.QueryColors(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
    [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Value.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");



var font_path_reply = client.GetFontPath();
client.SetFontPathChecked(font_path_reply.Value.Paths);

var pattan = "*"u8;
var listFonts = client.ListFonts(pattan, 10);
foreach (var fontName in listFonts.Value.Fonts)
    Console.WriteLine(fontName);

font = client.NewId();
client.OpenFontUnchecked("fixed", font);
var font_info = client.QueryFont(font);
Console.WriteLine($"QueryFont: Max bounds width:  {font_info.Value.MaxBounds.CharacterWidth}\n");
var text = "Hello, XCB!";
var text_extents = client.QueryTextExtents(font, text);
Console.WriteLine($"QueryTextExtents: Width of '{text}': {text_extents.Value.OverallWidth}\n");

var motion = client.GetMotionEvents(window, 0, 0);
Console.WriteLine($"GetMotionEvents: {motion.Value.Events.Length} events");

var screensaver = client.GetScreenSaver();
Console.WriteLine($"GetScreenSaver: Timeout: {screensaver.Value.Timeout}");

queryColor = client.QueryColors(client.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
     [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Value.Colors)
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
        Console.WriteLine($"Best size for 32x32: {getBestWindowSize.Value.Width} {getBestWindowSize.Value.Height}");

        var resultQueryKeymap = client.QueryKeymap();
        Console.WriteLine($"{resultQueryKeymap.Value.keys.Length}");
        var resultGetScreenSaver = client.GetScreenSaver();
        Console.WriteLine($"GetScreenSaver will time out {resultGetScreenSaver.Value.Timeout}");
        var resultGetPointerControl = client.GetPointerControl();
        Console.WriteLine($"{resultGetPointerControl.Value.AccelDenominator} {resultGetPointerControl.Value.AccelNumerator}");
        var resultGetModifierMapping = client.GetModifierMapping();
        Console.WriteLine("GetModifierMapping");
        var resultSetModifierMapping = client.SetModifierMapping(resultGetModifierMapping.Value.Keycodes);
        Console.WriteLine(resultSetModifierMapping.Value.Status);
        var resultGetKeyboardControl = client.GetKeyboardControl();
        Console.WriteLine($"{resultGetKeyboardControl.Value.BellPercent}");
        var resultGetPointerMapping = client.GetPointerMapping();
        var resultSetPointerMapping = client.SetPointerMapping(resultGetPointerMapping.Value.Map);
        Console.WriteLine(resultSetPointerMapping.Value.Status);
    }
    var resultGetMotionEvents = client.GetMotionEvents(window, 0, 1000);
    Console.WriteLine(resultGetMotionEvents.Value.Events.Length);
}