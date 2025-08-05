using Xcsb;
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

/*
// 6. ChangeSaveSet
xcb_change_save_set(conn, XCB_SET_MODE_INSERT, window);
printf("ChangeSaveSet: Added window to save set\n");

// 7. ChangeActivePointerGrab with allocated pixel
if (pixels && color_cells && color_cells->pixels_len > 0)
{
    xcb_change_active_pointer_grab(conn, XCB_CURSOR_NONE, XCB_CURRENT_TIME,
        XCB_EVENT_MASK_BUTTON_PRESS);
    printf("ChangeActivePointerGrab: Grab set with allocated pixel\n");
}

// 8. GetMotionEvents
xcb_get_motion_events_reply_t* motion = xcb_get_motion_events_reply(conn,
    xcb_get_motion_events(conn, window, XCB_TIME_CURRENT_TIME, XCB_TIME_CURRENT_TIME), NULL);
if (motion)
{
    printf("GetMotionEvents: %d events\n", motion->events_len);
    free(motion);
}

// 9. QueryKeymap
xcb_query_keymap_reply_t* keymap = xcb_query_keymap_reply(conn,
    xcb_query_keymap(conn), NULL);
if (keymap)
{
    printf("QueryKeymap: Keymap retrieved\n");
    free(keymap);
}

printf("\t %hhd", keymap->keys);

// 10. GetScreenSaver
xcb_get_screen_saver_reply_t* screensaver = xcb_get_screen_saver_reply(conn,
    xcb_get_screen_saver(conn), NULL);
if (screensaver)
{
    printf("GetScreenSaver: Timeout: %d\n", screensaver->timeout);
    free(screensaver);
}

// 11. GetPointerControl
xcb_get_pointer_control_reply_t* pointer = xcb_get_pointer_control_reply(conn,
    xcb_get_pointer_control(conn), NULL);
if (pointer)
{
    printf("GetPointerControl: Acceleration: %d/%d\n",
        pointer->acceleration_numerator, pointer->acceleration_denominator);
    free(pointer);
}

// 12. GetModifierMapping and use for SetModifierMapping
xcb_get_modifier_mapping_reply_t* modmap = xcb_get_modifier_mapping_reply(conn,
    xcb_get_modifier_mapping(conn), NULL);
if (modmap)
{
    printf("GetModifierMapping: Keycodes per modifier: %d\n", modmap->keycodes_per_modifier);
    // Use the retrieved mapping to reset it
    xcb_keycode_t* mod_keycodes = xcb_get_modifier_mapping_keycodes(modmap);
    xcb_set_modifier_mapping_reply_t* set_modmap = xcb_set_modifier_mapping_reply(conn,
        xcb_set_modifier_mapping(conn, modmap->keycodes_per_modifier * 8, mod_keycodes), NULL);
    if (set_modmap)
    {
        printf("SetModifierMapping: Status: %d\n", set_modmap->status);
        free(set_modmap);
    }

    free(modmap);
}

// 13. GetKeyboardControl
xcb_get_keyboard_control_reply_t* kb_control = xcb_get_keyboard_control_reply(conn,
    xcb_get_keyboard_control(conn), NULL);
if (kb_control)
{
    printf("GetKeyboardControl: Bell percent: %d\n", kb_control->bell_percent);
    free(kb_control);
}

// 14. ChangeKeyboardMapping (using minimal example to avoid breaking keyboard)
xcb_keycode_t keycodes[] =  {
    0
}
; // Dummy keycode
xcb_keysym_t keysyms[] =  {
    0
}
; // Dummy keysym
xcb_change_keyboard_mapping(conn, 1, keycodes[0], 1, keysyms);
printf("ChangeKeyboardMapping: Modified one key (dummy)\n");

// Free allocated color cells
if (color_cells)
{
    free(color_cells);
}

// Event loop
xcb_generic_event_t *event;
while ((event = xcb_wait_for_event(conn))) {
    if ((event->response_type & ~0x80) == XCB_KEY_PRESS) {
        break;
    }
    free(event);
}

// Cleanup
xcb_close_font(conn, font);
xcb_disconnect(conn);

*/