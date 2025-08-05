using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

var conn = XcsbClient.Initialized();
var screen = conn.HandshakeSuccessResponseBody.Screens[0];
// Create a window
var window = conn.NewId();
conn.CreateWindow(
    0,
    window,
    screen.Root,
    0, 0, 800, 600, 10,
    ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)EventMask.KeyPressMask]);

conn.MapWindow(window);

var font_path_reply = conn.GetFontPath();
conn.SetFontPathChecked(font_path_reply.Paths);

// 2. AllocColorCells and use pixels for 
//todo: has calling issue
/*
var color_cells = conn.AllocColorCells(false, screen.DefaultColormap, 1, 1);
Console.WriteLine($"AllocColorCells: {color_cells.Pixels.Length} pixels allocated");
if (color_cells.Pixels.Length > 0)
{
    var color = new ColorItem[1]
    {
        new ColorItem()
        {
            Pixel = color_cells.Pixels[0],
            Red = 65535,
            Green = 32768,
            Blue = 0,
            ColorFlag = ColorFlag.Red
        }
    };
    conn.StoreColorsChecked(screen.DefaultColormap, color.AsSpan());
}
// todo calling issue
// 4. AllocColorPlanes and use results for StoreNamedColor
xcb_alloc_color_planes_reply_t* color_planes = xcb_alloc_color_planes_reply(conn,
    xcb_alloc_color_planes(conn, screen->default_colormap, 0, 1, 1, 1, 1), NULL);
if (color_planes && color_planes->pixels_len > 0)
{
    printf("AllocColorPlanes: %d pixels allocated\n", color_planes->pixels_len);
    uint32_t* plane_pixels = xcb_alloc_color_planes_pixels(color_planes);
    // Use the first pixel for StoreNamedColor
    xcb_store_named_color(conn, 0, screen->default_colormap, plane_pixels[0],
        strlen("blue"), "blue");
    printf("StoreNamedColor: Stored 'blue' for pixel %u\n", plane_pixels[0]);
    free(color_planes);
}
Console.WriteLine();
*/
// 5. QueryFont and use font for QueryTextExtents
var font = conn.NewId();
conn.OpenFont("fixed", font);
var font_info = conn.QueryFont(font);
Console.WriteLine($"QueryFont: Max bounds width:  {font_info.MaxBounds.CharacterWidth}\n");
// Use font in QueryTextExtents
var text = "Hello, XCB!";
var text_extents = conn.QueryTextExtents(font, text);
Console.WriteLine($"QueryTextExtents: Width of '{text}': {text_extents.OverallWidth}\n");


// 6. ChangeSaveSet
// conn.ChangeSaveSetChecked(ChangeSaveSetMode.Insert, window);

/*
 todo depends on old todo
// 7. ChangeActivePointerGrab with allocated pixel
if (pixels && color_cells && color_cells->pixels_len > 0)
{
    xcb_change_active_pointer_grab(conn, XCB_CURSOR_NONE, XCB_CURRENT_TIME,
        XCB_EVENT_MASK_BUTTON_PRESS);
    printf("ChangeActivePointerGrab: Grab set with allocated pixel\n");
}
*/

// 8. GetMotionEvents
var motion = conn.GetMotionEvents(window, 0, 0);
Console.WriteLine($"GetMotionEvents: {motion.Events.Length} events");

// 10. GetScreenSaver
var screensaver = conn.GetScreenSaver();
Console.WriteLine($"GetScreenSaver: Timeout: {screensaver.Timeout}");

// 14. ChangeKeyboardMapping (using minimal example to avoid breaking keyboard)
conn.ChangeKeyboardMapping(
    1,
    0,
    1,
    [0]
    );
Console.WriteLine("ChangeKeyboardMapping: Modified one key (dummy)\n");

while (true)
{
    var Event = conn.GetEvent();
    if (!Event.HasValue)
        break;
    if (Event.Value.EventType == EventType.KeyPress)
        break;
}
conn.DestroyWindow(window);