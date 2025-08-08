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

Console.WriteLine("******************************************************************************************");
Console.WriteLine("******************************************************************************************");

Console.WriteLine("Calling xcb_get_font_path");
var font_path_reply = client.GetFontPath();
Console.WriteLine($"received path length {font_path_reply.Paths.Length}");
Console.WriteLine("calling set font path");
client.SetFontPathChecked(font_path_reply.Paths);
// xcb sending: 33 00 09 00 01 00 00 00 19 2f 75 73 72 2f 73 68 61 72 65 2f 66 6f 6e 74 73 2f 58 31 31 2f 6d 69 73 63 00 00 2b 00 01 00
// i'm sending: 33 00 09 00 01 00 00 00 19 2f 75 73 72 2f 73 68 61 72 65 2f 66 6f 6e 74 73 2f 58 31 31 2f 6d 69 73 63 00 00
/* todo check on other places.
 
#include <xcb/xcb.h>
#include <stdio.h>
   
   int main(int argc, char *argv[]) 
   {
     int screen_len;
     xcb_connection_t* conn = xcb_connect(NULL, &screen_len);
     xcb_screen_t* screen = xcb_setup_roots_iterator(xcb_get_setup(conn)).data;
     xcb_window_t window = xcb_generate_id(conn);
     xcb_create_window(conn,
       screen->root_depth,
       window,
       screen->root,
       0, 0, 500, 500,
       0, XCB_WINDOW_CLASS_INPUT_OUTPUT, screen->root_visual,
       XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK,
         (uint32_t[]){screen->white_pixel, XCB_EVENT_MASK_EXPOSURE | XCB_EVENT_MASK_KEY_PRESS});
       xcb_map_window(conn, window);
     xcb_flush(conn);
     
     printf("----------------------------------------------------------------------------------------------------\n");
     printf("Calling xcb_get_font_path\n");
     xcb_generic_error_t* e;
     
     xcb_get_font_path_cookie_t cookie = xcb_get_font_path (conn);
     printf("*********************************************************************************************************\n");
     xcb_get_font_path_reply_t* fontPath = xcb_get_font_path_reply(conn, cookie, &e);
     printf("*********************************************************************************************************\n");
     xcb_str_t* fontPath_iter =  xcb_get_font_path_path_iterator(fontPath).data;
     printf("received path length %d\n", xcb_get_font_path_path_length(fontPath));
     
     printf("calling set font path\n");
     xcb_void_cookie_t replyCookie =  xcb_set_font_path_checked(conn, xcb_get_font_path_path_length(fontPath), fontPath_iter);
     printf("*********************************************************************************************************\n");
     e = xcb_request_check(conn, replyCookie);
     printf("*********************************************************************************************************\n");
     if (e)
     {
       printf("error");
     }
   }
*/