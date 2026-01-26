using System.Text;
using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Models.Handshake;
using Xcsb.Masks;
using Xcsb.Models;

int screen_num = 0;
var connection = XcsbClient.Connect();
var x = connection.Initialized();

var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

var win = connection.NewId();
x.CreateWindowUnchecked(screen.RootDepth!.DepthValue,
    win,
    root,
    0, 0, 300, 200,
    1, ClassType.InputOutput, screen.RootVisualId,
    0, []);
x.MapWindowUnchecked(win);

var alloc_cookie = x.AllocColor(screen.DefaultColormap, 65535, 0, 0); // Red
Console.WriteLine("Allocated red color, Pixel value: {0}", alloc_cookie.Pixel);

// Free the color
x.FreeColorsUnchecked(screen.DefaultColormap, 0, [alloc_cookie.Pixel]);
Console.WriteLine("Color freed successfully");

var grabResult = x.GrabPointer(false,
    screen.Root,
    64,
    GrabMode.Asynchronous, GrabMode.Asynchronous,
    0, 0, 0);

Console.WriteLine($"Grab status {grabResult.Status}");

x.UngrabPointerUnchecked(0);
Console.WriteLine("Ungrab pointer completed.");

var fontId = connection.NewId();
x.OpenFontUnchecked("fixed", fontId);

var _gc = connection.NewId();
x.CreateGCUnchecked(_gc, win, GCMask.Foreground | GCMask.Background | GCMask.Font, [screen.BlackPixel, screen.WhitePixel, fontId]);

x.ImageText8Unchecked(win, _gc, 10, 40, "the background will change"u8);
Thread.Sleep(5000);
string[] colors = ["Red", "Green", "Blue"];
string[] propNames = ["COLOR_A", "COLOR_B", "COLOR_C"];
var atoms = new ATOM[3];

for (var i = 0; i < colors.Length; i++)
{
    var reply = x.InternAtom(false, propNames[i]);
    atoms[i] = reply.Atom;
    x.ChangePropertyUnchecked<byte>(PropertyMode.Replace, win, atoms[i], ATOM.String, Encoding.UTF8.GetBytes(colors[i]));
}

foreach (var atom in atoms)
{
    var details = x.GetAtomName(atom);
    Console.WriteLine($"{atom}: {details.Name}");
}


for (var i = 0; i < 6; i++)
{
    var reply = x.GetProperty(false, win, atoms[0], ATOM.String, 0, 32);
    if (reply.Data.Length > 0)
    {
        x.ChangeWindowAttributesUnchecked(win, ValueMask.BackgroundPixel, [(GetNameColor(reply.Data, screen))]);
        x.ClearAreaUnchecked(false, win, 0, 0, 0, 0);
    }

    x.RotatePropertiesUnchecked(win, 1, atoms);
    Thread.Sleep(1000);
}

foreach (var atom in atoms)
    x.DeletePropertyUnchecked(win, atom);

x.ImageText8Unchecked(win, _gc, 10, 40, "Change the GC's foreground red to white"u8);
Thread.Sleep(5000);
var gc = connection.NewId();
x.CreateGCUnchecked(gc, win, GCMask.Foreground, [0xFF0000]);
var rect = new Rectangle()
{
    X = 10,
    Y = 10,
    Width = 100,
    Height = 50,
};
x.PolyFillRectangleUnchecked(win, gc, [rect]);

Thread.Sleep(1000);
x.ChangeGCUnchecked(gc, GCMask.Foreground, [screen.WhitePixel]);
rect.X += 20;
rect.Y += 60;

x.PolyFillRectangleUnchecked(win, gc, [rect]);
x.FreeGCUnchecked(gc);
Thread.Sleep(3000);

var gc1 = connection.NewId();
var gc2 = connection.NewId();
x.CreateGCUnchecked(gc1, win, GCMask.Foreground, [0x0000FF]);
x.CreateGCUnchecked(gc2, win, 0, []);
rect.X -= 15;
rect.Y -= 65;
x.PolyFillRectangleUnchecked(win, gc1, [rect]);
Thread.Sleep(1500);

x.CopyGCUnchecked(gc1, gc2, GCMask.Foreground);
rect.X += 20;
rect.Y += 60;

x.PolyFillRectangleUnchecked(win, gc2, [rect]);

x.FreeGCUnchecked(gc1);
Thread.Sleep(3000);

var accl = new Acceleration
{
    Denominator = 1,
    Numerator = 2
};
x.ChangePointerControlUnchecked(accl, 4);
Console.WriteLine("Pointer control changed: acceleration 2:1, threshold 4 pixels");
Thread.Sleep(3000);


x.ConvertSelectionUnchecked(win, ATOM.Primary, ATOM.String, ATOM.CutBuffer0, 0);
Console.WriteLine("Selection conversion requested");
Thread.Sleep(3000);


x.SetScreenSaverUnchecked(5, 10, TriState.Yes, TriState.Yes);
Console.WriteLine("Screen saver set: 5s timeout, blanking preferred");
Thread.Sleep(20000);


x.ForceScreenSaverUnchecked(ForceScreenSaverMode.Activate);
Console.WriteLine("Screen saver activated");
Thread.Sleep(3000);

x.ForceScreenSaverUnchecked(ForceScreenSaverMode.Reset);
Console.WriteLine("Screen saver reset");
Thread.Sleep(3000);

x.NoOperationUnchecked([1, 3, 323, 32, 2323]);
Console.WriteLine("NoOperation");
Thread.Sleep(3000);

x.SetAccessControlUnchecked(AccessControlMode.Enable);
Console.WriteLine("Access control enabled");
Thread.Sleep(1500);

x.SetAccessControlUnchecked(AccessControlMode.Disable);
Console.WriteLine("Access control disabled");
Thread.Sleep(1500);


var font = connection.NewId();
x.OpenFontUnchecked("cursor", font);
var cursor = connection.NewId();
x.CreateGlyphCursorUnchecked(cursor, font, font, 'D', 69, 0, 0, 0, 65535, 65535, 65535);

Console.WriteLine("Created cursor with ID: {0}", cursor);
x.ChangeWindowAttributesUnchecked(win, ValueMask.Cursor, [cursor]);

Thread.Sleep(3000);

x.FreeCursorUnchecked(cursor);
Console.WriteLine("freed cursor");

x.CloseFontUnchecked(font);

x.SetSelectionOwnerUnchecked(win, ATOM.Primary, 0);

Console.WriteLine("Selection owner set for PRIMARY selection");
Thread.Sleep(1500);

x.SetSelectionOwnerUnchecked(0, ATOM.Primary, 0);
Console.WriteLine("Selection owner cleared");

x.SetCloseDownModeUnchecked(CloseDownMode.Destroy);

connection.Dispose();
return 0;


uint GetNameColor(Span<byte> name, Screen screen)
{
    if (name.Slice(0, 3).SequenceEqual("Red"u8))
        return 0xFF0000;
    else if (name.Slice(0, 4).SequenceEqual("Blue"u8))
        return 0x0000FF;
    else if (name.Slice(0, 5).SequenceEqual("Green"u8))
        return 0x00FF00;

    return screen.WhitePixel;
}