
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;

const ushort width = 400, height = 300;

var xcsb = Xcsb.XcsbClient.Initialized();
var screen = xcsb.HandshakeSuccessResponseBody.Screens[0];

var window = xcsb.NewId();
xcsb.CreateWindow(screen.RootDepth.DepthValue,
    window,
    screen.Root,
    0, 0, width, height,
    10, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

var pixmap = xcsb.NewId();
xcsb.CreatePixmap(screen.RootDepth!.DepthValue,
                  pixmap,
                  screen.Root,
                  width, height);


var gc = xcsb.NewId();
xcsb.CreateGC(gc, pixmap, GCMask.Foreground | GCMask.Background, [screen.BlackPixel, screen.WhitePixel]);

xcsb.ChangeGC(gc, GCMask.Foreground, screen.WhitePixel);
xcsb.PolyFillRectangle(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);


xcsb.SetClipRectangles(ClipOrdering.Unsorted,
                       gc,
                       0, 0,
                       [
                           new Rectangle{X =50, Y = 50, Width = 100, Height = 80},
                           new Rectangle{ X = 200, Y = 100, Width = 120, Height = 60},
                           new Rectangle{ X = 100, Y = 180, Width = 80, Height = 90}]);

xcsb.ChangeGC(gc, GCMask.Foreground, screen.BlackPixel);

xcsb.PolyFillRectangle(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);

xcsb.ChangeGC(gc, GCMask.ClipMask, 0);

xcsb.PolyRectangle(pixmap, gc, [new Rectangle{X =50, Y = 50, Width = 100, Height = 80},
                           new Rectangle{ X = 200, Y = 100, Width = 120, Height = 60},
                           new Rectangle{ X = 100, Y = 180, Width = 80, Height = 90}]);

xcsb.MapWindow(window);


Span<byte> buffer = stackalloc byte[XcsbClient.GetEventSize()];
var isRunning = true;

while (isRunning)
{
    ref var evnt = ref xcsb.GetEvent(buffer);
    switch (evnt.EventType)
    {
        case Xcsb.Models.Event.EventType.Expose:
            xcsb.CopyArea(pixmap,           // source
                             window,           // destination
                             gc,               // graphics context
                             0, 0,             // src x, y
                             0, 0,             // dst x, y
                             width, height);   // width, height
            break;

        case Xcsb.Models.Event.EventType.KeyPress:
            isRunning = false; 
            break;
    }
}
xcsb.FreeGC(gc);
xcsb.FreePixmap(pixmap);