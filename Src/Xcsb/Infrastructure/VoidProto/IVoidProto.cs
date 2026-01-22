using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Response;
using Xcsb.Models.String;
using Xcsb.Response.Errors;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Infrastructure;

public interface IVoidProto
{
    ResponseProto CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args);

    ResponseProto ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args);

    ResponseProto DestroyWindow(uint window);
    ResponseProto DestroySubwindows(uint window);

    ResponseProto ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window);

    ResponseProto ReparentWindow(uint window, uint parent, short x, short y);

    ResponseProto MapWindow(uint window);
    ResponseProto MapSubwindows(uint window);
    ResponseProto UnmapWindow(uint window);
    ResponseProto UnmapSubwindows(uint window);

    ResponseProto ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args);

    ResponseProto CirculateWindow(Circulate circulate, uint window);

    ResponseProto ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    ResponseProto DeleteProperty(uint window, ATOM atom);

    ResponseProto RotateProperties(uint window, ushort delta, Span<ATOM> properties);

    ResponseProto SetSelectionOwner(uint owner, ATOM atom, uint timestamp);

    ResponseProto ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp);

    ResponseProto SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt);

    ResponseProto UngrabPointer(uint time);

    ResponseProto GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers);

    ResponseProto UngrabButton(Button button, uint grabWindow, ModifierMask mask);

    ResponseProto ChangeActivePointerGrab(uint cursor, uint time, ushort mask);

    ResponseProto UngrabKeyboard(uint time);

    ResponseProto GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode);

    ResponseProto UngrabKey(byte key, uint grabWindow, ModifierMask modifier);

    ResponseProto AllowEvents(EventsMode mode, uint time);

    ResponseProto GrabServer();
    ResponseProto UngrabServer();

    ResponseProto WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight,
        short destinationX, short destinationY);

    ResponseProto SetInputFocus(InputFocusMode mode, uint focus, uint time);

    ResponseProto OpenFont(string fontName, uint fontId);

    ResponseProto CloseFont(uint fontId);
    ResponseProto SetFontPath(string[] strPaths);

    ResponseProto CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height);

    ResponseProto FreePixmap(uint pixmapId);

    ResponseProto CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args);

    ResponseProto ChangeGC(uint gc, GCMask mask, Span<uint> args);

    ResponseProto CopyGC(uint srcGc, uint dstGc, GCMask mask);

    ResponseProto SetDashes(uint gc, ushort dashOffset, Span<byte> dashes);

    ResponseProto SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles);

    ResponseProto FreeGC(uint gc);

    ResponseProto ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height);

    ResponseProto CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY, ushort width, ushort height);

    ResponseProto CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY, ushort width, ushort height, uint bitPlane);

    ResponseProto PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    ResponseProto PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    ResponseProto PolySegment(uint drawable, uint gc, Span<Segment> segments);

    ResponseProto PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles);

    ResponseProto PolyArc(uint drawable, uint gc, Span<Arc> arcs);

    ResponseProto FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points);

    ResponseProto PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles);

    ResponseProto PolyFillArc(uint drawable, uint gc, Span<Arc> arcs);

    ResponseProto PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data);

    ResponseProto ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);

    ResponseProto ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);

    ResponseProto CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual);

    ResponseProto FreeColormap(uint colormapId);

    ResponseProto CopyColormapAndFree(uint colormapId, uint srcColormapId);

    ResponseProto InstallColormap(uint colormapId);
    ResponseProto UninstallColormap(uint colormapId);

    ResponseProto FreeColors(uint colormapId, uint planeMask, Span<uint> pixels);

    ResponseProto StoreColors(uint colormapId, Span<ColorItem> item);

    ResponseProto StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);

    ResponseProto CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y);

    ResponseProto CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);

    ResponseProto FreeCursor(uint cursorId);

    ResponseProto RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue);

    // suppose need changes
    ResponseProto ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym);

    ResponseProto Bell(sbyte percent);

    ResponseProto ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args);

    ResponseProto ChangePointerControl(Acceleration? acceleration, ushort? threshold);

    ResponseProto SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures);

    ResponseProto ForceScreenSaver(ForceScreenSaverMode mode);

    ResponseProto ChangeHosts(HostMode mode, Family family, Span<byte> address);

    ResponseProto SetAccessControl(AccessControlMode mode);
    ResponseProto SetCloseDownMode(CloseDownMode mode);
    ResponseProto KillClient(uint resource);

    ResponseProto NoOperation(Span<uint> args);

    ResponseProto PolyText8(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data);
    ResponseProto PolyText16(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data);
    GenericError? CheckRequest(ResponseProto proto);
    
}