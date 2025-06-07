using System;
using System.Collections.Generic;
using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;

int screen_num;
var x = XcsbClient.Initialized();

var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

var win = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 300, 200,
    1, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    0, []);
x.MapWindow(win);

var fontId = x.NewId();
x.OpenFont("fixed", fontId);

var _gc = x.NewId();
x.CreateGC(_gc, win, GCMask.Foreground | GCMask.Background | GCMask.Font, [screen.BlackPixel, screen.WhitePixel, fontId]);

x.ImageText8(win, _gc, 10, 40, "the background will change"u8);
Thread.Sleep(5000);
string[] colors = ["Red", "Green", "Blue"];
string[] propNames = ["COLOR_A", "COLOR_B", "COLOR_C"];
var atoms = new uint[3];

for (var i = 0; i < colors.Length; i++)
{
    var reply = x.InternAtom(false, propNames[i]);
    atoms[i] = reply.Atom;
    x.ChangeProperty<byte>(PropertyMode.Replace, win, atoms[i], 31, Encoding.UTF8.GetBytes(colors[i]));
}

for (var i = 0; i < 6; i++)
{
    var reply = x.GetProperty(false, win, atoms[0], 31, 0, 32);
    if (reply.Data.Length > 0)
    {
        x.ChangeWindowAttributes(win, ValueMask.BackgroundPixel, [(GetNameColor(reply.Data, screen))]);
        x.ClearArea(false, win, 0, 0, 0, 0);
    }

    x.RotateProperties(win, 1, atoms);
    Thread.Sleep(1000);
}

foreach (var atom in atoms)
    x.DeleteProperty(win, atom);

x.ImageText8(win, _gc, 10, 40, "Change the GC's foreground red to white"u8);
Thread.Sleep(5000);
var gc = x.NewId();
x.CreateGC(gc, win, GCMask.Foreground, [0xFF0000]);
var rect = new Rectangle()
{
    x = 10,
    y = 10,
    width = 100,
    height = 50,
};
x.PolyFillRectangle(win, gc, [rect]);

Thread.Sleep(1000);
x.ChangeGC(gc, GCMask.Foreground, [screen.WhitePixel]);
rect.x += 20;
rect.y += 60;

x.PolyFillRectangle(win, gc, [rect]);

x.FreeGC(gc);
Thread.Sleep(3000);

var gc1 = x.NewId();
var gc2 = x.NewId();
x.CreateGC(gc1, win, GCMask.Foreground, [0x0000FF]);
x.CreateGC(gc2, win, 0, []);
rect.x -= 15;
rect.y -= 65;
x.PolyFillRectangle(win, gc1, [rect]);
Thread.Sleep(1500);

x.CopyGC(gc1, gc2, GCMask.Foreground);
rect.x += 20;
rect.y += 60;

x.PolyFillRectangle(win, gc2, [rect]);

x.FreeGC(gc1);
Thread.Sleep(3000);
x.Dispose();
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