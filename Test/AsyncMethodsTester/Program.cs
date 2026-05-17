using System.Runtime.CompilerServices;
using System.Text.Json;
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.TypeInfo;

var asyncResult = new Dictionary<string, string>();
var nonAsyncResult = new Dictionary<string, string>();


using (var con = XcsbClient.Connect())
{
    var x = con.Initialized();
    var jsonSerializerOptions = new JsonSerializerOptions
    {
        IncludeFields = true,
        WriteIndented = true
    };
    if (con.HandshakeSuccessResponseBody is null)
    {
        Console.WriteLine("Connection failed: " + con.FailReason);
        return;
    }

    var screen = con.HandshakeSuccessResponseBody.Screens[0];
    var window = con.NewId();
    x.CreateWindowUnchecked(0,
        window,
        screen.Root,
        100, 100, 500, 500,
        2, ClassType.InputOutput,
        screen.RootVisualId,
        ValueMask.BackgroundPixel | ValueMask.EventMask,
        [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
    );
    x.MapWindow(window);

    while (true)
    {
        var ev = x.GetEvent();
        if (ev.ReplyType == EventType.Expose)
        {
            var c = x.GetImage(ImageFormat.XYPixmap, window, 0, 0, 400, 200, uint.MaxValue);
            nonAsyncResult.Add("GetImage", JsonSerializer.Serialize(c, jsonSerializerOptions));
            break;
        }
    }
    {
        var c = x.AllocColor(screen.DefaultColormap, 65535, 0, 0);
        nonAsyncResult.Add("AllocColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryPointer(screen.Root);
        nonAsyncResult.Add("QueryPointer", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GrabPointer(false, screen.Root, 64, GrabMode.Asynchronous,
            GrabMode.Asynchronous, 0, 0, 0);
        nonAsyncResult.Add("GrabPointer", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.InternAtom(false, "COLOR_A");
        nonAsyncResult.Add("InternAtom", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = x.GetProperty(false, window, c.Atom, ATOM.String, 0, 32);
        nonAsyncResult.Add("GetProperty", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = x.GetWindowAttributes(window);
        nonAsyncResult.Add("GetWindowAttributes", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetGeometry(window);
        nonAsyncResult.Add("GetGeometry", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryTree(window);
        nonAsyncResult.Add("QueryTree", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.ListProperties(screen.Root);
        nonAsyncResult.Add("ListProperties", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = x.GetAtomName(c.Atoms[0]);
        nonAsyncResult.Add("GetAtomName", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = x.GetSelectionOwner(ATOM.Primary);
        nonAsyncResult.Add("GetSelectionOwner", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GrabKeyboard(
            false,
            window,
            0,
            GrabMode.Asynchronous,
            GrabMode.Asynchronous
        );
        nonAsyncResult.Add("GrabKeyboard", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetMotionEvents(window, 0, 0);
        nonAsyncResult.Add("GetMotionEvents", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetInputFocus();
        nonAsyncResult.Add("GetInputFocus", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryKeymap();
        nonAsyncResult.Add("QueryKeymap", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    var font = con.NewId();
    {
        x.OpenFontUnchecked("fixed", font);
        var c = x.QueryFont(font);
        nonAsyncResult.Add("QueryFont", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryTextExtents(font, "Hello, XCB!");
        nonAsyncResult.Add("QueryTextExtents", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.ListFonts("*"u8, 5);
        nonAsyncResult.Add("ListFonts", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.ListFontsWithInfo("*"u8, 5);
        nonAsyncResult.Add("ListFontsWithInfo", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetFontPath();
        nonAsyncResult.Add("GetFontPath", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.ListInstalledColormaps(window);
        nonAsyncResult.Add("ListInstalledColormaps", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.AllocNamedColor(screen.DefaultColormap, "Red"u8);
        nonAsyncResult.Add("AllocNamedColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var window1 = con.NewId();
        x.CreateWindowUnchecked(0,
            window1,
            screen.Root,
            100, 100, 500, 500,
            2, ClassType.InputOutput,
            screen.RootVisualId,
            ValueMask.BackgroundPixel | ValueMask.EventMask,
            [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
        );
        var c = x.TranslateCoordinates(window, window1, 0, 0);
        nonAsyncResult.Add("TranslateCoordinates", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryColors(screen.DefaultColormap, [0x0000, 0x00FF, 0xFF00, 0xFFFF]);
        nonAsyncResult.Add("QueryColors", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.LookupColor(screen.DefaultColormap, "Light Yellow"u8);
        nonAsyncResult.Add("LookupColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.QueryBestSize(QueryShapeOf.LargestCursor, window, 32, 32);
        nonAsyncResult.Add("QueryBestSize", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetModifierMapping();
        nonAsyncResult.Add("GetModifierMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetKeyboardMapping(
            con.HandshakeSuccessResponseBody.MinKeyCode,
            (byte)(con.HandshakeSuccessResponseBody.MaxKeyCode - con.HandshakeSuccessResponseBody.MinKeyCode + 1));
        nonAsyncResult.Add("GetKeyboardMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetKeyboardControl();
        nonAsyncResult.Add("GetKeyboardControl", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetPointerMapping();
        nonAsyncResult.Add("GetPointerMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = x.SetPointerMapping(c.Map);
        nonAsyncResult.Add("SetPointerMapping", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = x.GetPointerControl();
        nonAsyncResult.Add("GetPointerControl", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetScreenSaver();
        nonAsyncResult.Add("GetScreenSaver", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.ListHosts();
        nonAsyncResult.Add("ListHosts", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    con.Dispose();
}

await Task.Delay(5000);

using (var con = XcsbClient.Connect())
{
    var x = con.Initialized();
    var jsonSerializerOptions = new JsonSerializerOptions
    {
        IncludeFields = true,
        WriteIndented = true
    };
    if (con.HandshakeSuccessResponseBody is null)
    {
        Console.WriteLine("Connection failed: " + con.FailReason);
        return;
    }

    var screen = con.HandshakeSuccessResponseBody.Screens[0];
    var window = con.NewId();
    x.CreateWindowUnchecked(0,
        window,
        screen.Root,
        100, 100, 500, 500,
        2, ClassType.InputOutput,
        screen.RootVisualId,
        ValueMask.BackgroundPixel | ValueMask.EventMask,
        [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
    );
    x.MapWindow(window);

    while (true)
    {
        var ev = x.GetEvent();
        if (ev.ReplyType == EventType.Expose)
        {
            var c = await x.GetImageAsync(ImageFormat.XYPixmap, window, 0, 0, 400, 200, uint.MaxValue);
            asyncResult.Add("GetImage", JsonSerializer.Serialize(c, jsonSerializerOptions));
            break;
        }
    }
    {
        var c = await x.AllocColorAsync(screen.DefaultColormap, 65535, 0, 0);
        asyncResult.Add("AllocColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.QueryPointerAsync(screen.Root);
        asyncResult.Add("QueryPointer", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GrabPointerAsync(false, screen.Root, 64, GrabMode.Asynchronous,
            GrabMode.Asynchronous, 0, 0, 0);
        asyncResult.Add("GrabPointer", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.InternAtomAsync(false, "COLOR_A");
        asyncResult.Add("InternAtom", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = x.GetProperty(false, window, c.Atom, ATOM.String, 0, 32);
        asyncResult.Add("GetProperty", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = await x.GetWindowAttributesAsync(window);
        asyncResult.Add("GetWindowAttributes", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetGeometryAsync(window);
        asyncResult.Add("GetGeometry", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.QueryTreeAsync(window);
        asyncResult.Add("QueryTree", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.ListPropertiesAsync(screen.Root);
        asyncResult.Add("ListProperties", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = x.GetAtomName(c.Atoms[0]);
        asyncResult.Add("GetAtomName", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = await x.GetSelectionOwnerAsync(ATOM.Primary);
        asyncResult.Add("GetSelectionOwner", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GrabKeyboardAsync(
            false,
            window,
            0,
            GrabMode.Asynchronous,
            GrabMode.Asynchronous
        );
        asyncResult.Add("GrabKeyboard", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetMotionEventsAsync(window, 0, 0);
        asyncResult.Add("GetMotionEvents", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetInputFocusAsync();
        asyncResult.Add("GetInputFocus", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.QueryKeymapAsync();
        asyncResult.Add("QueryKeymap", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    var font = con.NewId();
    {
        x.OpenFontUnchecked("fixed", font);
        var c = await x.QueryFontAsync(font);
        asyncResult.Add("QueryFont", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.QueryTextExtentsAsync(font, "Hello, XCB!");
        asyncResult.Add("QueryTextExtents", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.ListFontsAsync("*"u8.ToArray(), 5);
        asyncResult.Add("ListFonts", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.ListFontsWithInfoAsync("*"u8.ToArray(), 5);
        asyncResult.Add("ListFontsWithInfo", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetFontPathAsync();
        asyncResult.Add("GetFontPath", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.ListInstalledColormapsAsync(window);
        asyncResult.Add("ListInstalledColormaps", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.AllocNamedColorAsync(screen.DefaultColormap, "Red"u8.ToArray());
        asyncResult.Add("AllocNamedColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var window1 = con.NewId();
        x.CreateWindowUnchecked(0,
            window1,
            screen.Root,
            100, 100, 500, 500,
            2, ClassType.InputOutput,
            screen.RootVisualId,
            ValueMask.BackgroundPixel | ValueMask.EventMask,
            [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
        );
        var c = await x.TranslateCoordinatesAsync(window, window1, 0, 0);
        asyncResult.Add("TranslateCoordinates", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var data = new uint[] { 0x0000, 0x00FF, 0xFF00, 0xFFFF };
        var c = await x.QueryColorsAsync(screen.DefaultColormap, data);
        asyncResult.Add("QueryColors", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.LookupColorAsync(screen.DefaultColormap, "Light Yellow"u8.ToArray());
        asyncResult.Add("LookupColor", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.QueryBestSizeAsync(QueryShapeOf.LargestCursor, window, 32, 32);
        asyncResult.Add("QueryBestSize", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetModifierMappingAsync();
        asyncResult.Add("GetModifierMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetKeyboardMappingAsync(
            con.HandshakeSuccessResponseBody.MinKeyCode,
            (byte)(con.HandshakeSuccessResponseBody.MaxKeyCode - con.HandshakeSuccessResponseBody.MinKeyCode + 1));
        asyncResult.Add("GetKeyboardMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetKeyboardControlAsync();
        asyncResult.Add("GetKeyboardControl", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = x.GetPointerMapping();
        asyncResult.Add("GetPointerMapping", JsonSerializer.Serialize(c, jsonSerializerOptions));
        var d = await x.SetPointerMappingAsync(c.Map);
        asyncResult.Add("SetPointerMapping", JsonSerializer.Serialize(d, jsonSerializerOptions));
    }
    {
        var c = await x.GetPointerControlAsync();
        asyncResult.Add("GetPointerControl", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.GetScreenSaverAsync();
        asyncResult.Add("GetScreenSaver", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
    {
        var c = await x.ListHostsAsync();
        asyncResult.Add("ListHosts", JsonSerializer.Serialize(c, jsonSerializerOptions));
    }
}

foreach (var (key, asyncValue) in asyncResult)
{
    var nonAsyncValue = nonAsyncResult[key];
    Console.Write(key);
    Console.Write(" ");
    var check = nonAsyncResult.ContainsKey(key);
    if (check)
    {
        var result = nonAsyncValue == asyncValue;
        Console.ForegroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(result);
        if (!result)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine(nonAsyncValue);
            System.Console.WriteLine(asyncValue);
        }
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Missing");
        Console.ResetColor();
    }
}
return;