using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

var conn = XcsbClient.Initialized();
var screen = conn.HandshakeSuccessResponseBody.Screens[0];
var window = conn.NewId();
conn.CreateWindow(
    0,
    window,
    screen.Root,
    0, 0, 800, 600, 10,
    ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.KeyPressMask | EventMask.ExposureMask)]);

conn.MapWindow(window);

var font_path_reply = conn.GetFontPath();
conn.SetFontPathChecked(font_path_reply.Paths);

var pattan = "*"u8;
var listFonts = conn.ListFonts(pattan, 10);
foreach (var fontName in listFonts.Fonts)
    Console.WriteLine(fontName);

var font = conn.NewId();
conn.OpenFont("fixed", font);
var font_info = conn.QueryFont(font);
Console.WriteLine($"QueryFont: Max bounds width:  {font_info.MaxBounds.CharacterWidth}\n");
var text = "Hello, XCB!";
var text_extents = conn.QueryTextExtents(font, text);
Console.WriteLine($"QueryTextExtents: Width of '{text}': {text_extents.OverallWidth}\n");

var motion = conn.GetMotionEvents(window, 0, 0);
Console.WriteLine($"GetMotionEvents: {motion.Events.Length} events");

var screensaver = conn.GetScreenSaver();
Console.WriteLine($"GetScreenSaver: Timeout: {screensaver.Timeout}");

var queryColor = conn.QueryColors(conn.HandshakeSuccessResponseBody.Screens[0].DefaultColormap,
     [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
foreach (var color in queryColor.Colors)
    Console.WriteLine($"Blue: {color.Blue} Green: {color.Green} Red: {color.Red} Reserved: {color.Reserved}");
while (true)
{
    var Event = conn.GetEvent();
    // todo: fix this
    // if (!Event.HasValue || Event.Value.EventType == EventType.Error)
    //     break;
    if (Event.Value.Reply == EventType.KeyPress)
        break;
}

conn.DestroyWindow(window);