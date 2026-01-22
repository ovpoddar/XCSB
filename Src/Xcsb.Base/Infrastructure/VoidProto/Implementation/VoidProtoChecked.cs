using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Base.Infrastructure.VoidProto.Contracts;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Models.Infrastructure;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Models.Infrastructure.Response;
using Xcsb.Models.String;
using Xcsb.Requests;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Base.Infrastructure.VoidProto.Implementation;

public abstract class VoidProtoChecked : IVoidProtoChecked
{
    private readonly XConnection _clientConnection;
    public VoidProtoChecked(IXConnection connection, ReadOnlySpan<char> failReason)
    {
        if (connection is not XConnection clientConnection)
            throw new ArgumentNullException(nameof(connection));
        _clientConnection = clientConnection;
        if (_clientConnection.HandshakeStatus is not HandshakeStatus.Success 
            || _clientConnection.SuccessResponse is null)
            throw new UnauthorizedAccessException(failReason.ToString());
    }

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var cookie = this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args)
    {
        var cookie = this.ChangeWindowAttributesBase(window, mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroyWindowChecked(uint window)
    {
        var cookie = this.DestroyWindowBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroySubwindowsChecked(uint window)
    {
        var cookie = this.DestroySubwindowsBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var cookie = this.ChangeSaveSetBase(changeSaveSetMode, window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        var cookie = this.ReparentWindowBase(window, parent, x, y);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapWindowChecked(uint window)
    {
        var cookie = this.MapWindowBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapSubwindowsChecked(uint window)
    {
        var cookie = this.MapSubwindowsBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapWindowChecked(uint window)
    {
        var cookie = this.UnmapWindowBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        var cookie = this.UnmapSubwindowsBase(window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var cookie = this.ConfigureWindowBase(window, mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CirculateWindowChecked(Circulate circulate, uint window)
    {
        var cookie = this.CirculateWindowBase(circulate, window);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = this.ChangePropertyBase(mode, window, property, type, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DeletePropertyChecked(uint window, ATOM atom)
    {
        var cookie = this.DeletePropertyBase(window, atom);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties)
    {
        var cookie = this.RotatePropertiesBase(window, delta, properties);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp)
    {
        var cookie = this.SetSelectionOwnerBase(owner, atom, timestamp);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var cookie = this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var cookie = this.SendEventBase(propagate, destination, eventMask, evnt);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabPointerChecked(uint time)
    {
        var cookie = this.UngrabPointerBase(time);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var cookie = this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        var cookie = this.UngrabButtonBase(button, grabWindow, mask);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        var cookie = this.ChangeActivePointerGrabBase(cursor, time, mask);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyboardChecked(uint time)
    {
        var cookie = this.UngrabKeyboardBase(time);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        var cookie = this.UngrabKeyBase(key, grabWindow, modifier);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        var cookie = this.AllowEventsBase(mode, time);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabServerChecked()
    {
        var cookie = this.GrabServerBase();
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabServerChecked()
    {
        var cookie = this.UngrabServerBase();
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var cookie = this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        var cookie = this.SetInputFocusBase(mode, focus, time);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        var cookie = this.OpenFontBase(fontName, fontId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CloseFontChecked(uint fontId)
    {
        var cookie = this.CloseFontBase(fontId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        var cookie = this.SetFontPathBase(strPaths);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var cookie = this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        var cookie = this.FreePixmapBase(pixmapId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var cookie = this.CreateGCBase(gc, drawable, mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args)
    {
        var cookie = this.ChangeGCBase(gc, mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        var cookie = this.CopyGCBase(srcGc, dstGc, mask);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var cookie = this.SetDashesBase(gc, dashOffset, dashes);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var cookie = this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeGCChecked(uint gc)
    {
        var cookie = this.FreeGCBase(gc);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var cookie = this.ClearAreaBase(exposures, window, x, y, width, height);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var cookie = this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var cookie = this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyPointBase(coordinate, drawable, gc, points);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyLineBase(coordinate, drawable, gc, points);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments)
    {
        var cookie = this.PolySegmentBase(drawable, gc, segments);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyRectangleBase(drawable, gc, rectangles);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyArcBase(drawable, gc, arcs);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        var cookie = this.FillPolyBase(drawable, gc, shape, coordinate, points);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyFillRectangleBase(drawable, gc, rectangles);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyFillArcBase(drawable, gc, arcs);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        var cookie = this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var cookie = this.ImageText8Base(drawable, gc, x, y, text);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var cookie = this.ImageText16Base(drawable, gc, x, y, text);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var cookie = this.CreateColormapBase(alloc, colormapId, window, visual);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColormapChecked(uint colormapId)
    {
        var cookie = this.FreeColormapBase(colormapId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        var cookie = this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void InstallColormapChecked(uint colormapId)
    {
        var cookie = this.InstallColormapBase(colormapId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        var cookie = this.UninstallColormapBase(colormapId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var cookie = this.FreeColorsBase(colormapId, planeMask, pixels);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreColorsChecked(uint colormapId, Span<ColorItem> item)
    {
        var cookie = this.StoreColorsBase(colormapId, item);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var cookie = this.StoreNamedColorBase(mode, colormapId, pixels, name);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var cookie = this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var cookie = this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeCursorChecked(uint cursorId)
    {
        var cookie = this.FreeCursorBase(cursorId);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var cookie = this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var cookie = this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void BellChecked(sbyte percent)
    {
        var cookie = this.BellBase(percent);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args)
    {
        var cookie = this.ChangeKeyboardControlBase(mask, args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePointerControlChecked(Acceleration? acceleration, ushort? threshold)
    {
        var cookie = this.ChangePointerControlBase(acceleration, threshold);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var cookie = this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        var cookie = this.ForceScreenSaverBase(mode);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address)
    {
        var cookie = this.ChangeHostsBase(mode, family, address);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        var cookie = this.SetAccessControlBase(mode);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        var cookie = this.SetCloseDownModeBase(mode);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void KillClientChecked(uint resource)
    {
        var cookie = this.KillClientBase(resource);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void NoOperationChecked(Span<uint> args)
    {
        var cookie = this.NoOperationBase(args);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var cookie = this.PolyText8Base(drawable, gc, x, y, data);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var cookie = this.PolyText16Base(drawable, gc, x, y, data);
        _clientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }


    private ResponseProto ChangeWindowAttributesBase(uint window, ValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto DestroyWindowBase(uint window)
    {
        var request = new DestroyWindowType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto AllowEventsBase(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto BellBase(sbyte percent)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeActivePointerGrabBase(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeGCBase(uint gc, GCMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeHostsBase(HostMode mode, Family family, Span<byte> address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                address);
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                address);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeKeyboardControlBase(KeyboardControlMask mask, Span<uint> args)
    {
        var request = new ChangeKeyboardControlType(mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeKeyboardMappingBase(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangePointerControlBase(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangePropertyBase<T>(PropertyMode mode, uint window, ATOM property, ATOM type,
        Span<T> args)
        where T : struct
#if !NETSTANDARD
            , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, size);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ChangeSaveSetBase(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CirculateWindowBase(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ClearAreaBase(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CloseFontBase(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ConfigureWindowBase(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ConfigureWindowType(window, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ConvertSelectionBase(uint requestor, ATOM selection, ATOM target, ATOM property,
        uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CopyAreaBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CopyColormapAndFreeBase(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CopyGCBase(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CopyPlaneBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreateColormapBase(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreateCursorBase(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreateGCBase(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {

        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreateGlyphCursorBase(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreatePixmapBase(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto CreateWindowBase(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto DeletePropertyBase(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto DestroySubwindowsBase(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FillPolyBase(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ForceScreenSaverBase(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FreeColormapBase(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FreeColorsBase(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FreeCursorBase(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FreeGCBase(uint gc)
    {
        var request = new FreeGCType(gc);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto FreePixmapBase(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto GrabButtonBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto GrabKeyBase(bool exposures, uint grabWindow, ModifierMask mask, byte keycode,
        GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto GrabServerBase()
    {
        var request = new GrabServerType();
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ImageText16Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ImageText8Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                text
            );
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                text
            );
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto InstallColormapBase(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto KillClientBase(uint resource)
    {
        var request = new KillClientType(resource);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto MapSubwindowsBase(uint window)
    {
        var request = new MapSubWindowsType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto MapWindowBase(uint window)
    {
        var request = new MapWindowType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto NoOperationBase(Span<uint> args)
    {
        var request = new NoOperationType(args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto OpenFontBase(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyFillArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyFillRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyLineBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyPointBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolySegmentBase(uint drawable, uint gc, Span<Segment> segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyText16Base(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Sum(a => a.Count));
        var requiredBuffer = request.Length * 4;
        var writIndex = 16;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(scratchBuffer.Slice(writIndex, item.Count));

            scratchBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(workingBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(workingBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(workingBuffer.Slice(writIndex, item.Count));

            workingBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PolyText8Base(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Sum(a => a.Count));
        var requiredBuffer = request.Length * 4;
        var writIndex = 16;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(scratchBuffer.Slice(writIndex, item.Count));
            scratchBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(workingBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(workingBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(workingBuffer.Slice(writIndex, item.Count));
            workingBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto PutImageBase(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x,
        short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                data);
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                data);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto RecolorCursorBase(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto ReparentWindowBase(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto RotatePropertiesBase(uint window, ushort delta, Span<ATOM> properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SendEventBase(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetAccessControlBase(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetClipRectanglesBase(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetCloseDownModeBase(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetDashesBase(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                dashes);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetFontPathBase(string[] strPaths)
    {
        var length = strPaths.Length;
        strPaths = strPaths.Where(a => a != "fixed").ToArray();
        var request = new SetFontPathType((ushort)length, strPaths.Sum(a => a.Length + 1).AddPadding());
        var requiredBuffer = request.Length * 4;
        var writIndex = 8;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths.OrderBy(a => a.Length))
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths.OrderBy(a => a.Length))
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _clientConnection.ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetInputFocusBase(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetScreenSaverBase(short timeout, short interval, TriState preferBlanking,
        TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto SetSelectionOwnerBase(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto StoreColorsBase(uint colormapId, Span<ColorItem> item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto StoreNamedColorBase(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                name);
            _clientConnection.ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
        }

        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UngrabButtonBase(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UngrabKeyBase(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UngrabKeyboardBase(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UngrabPointerBase(uint time)
    {
        var request = new UngrabPointerType(time);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UngrabServerBase()
    {
        var request = new UnGrabServerType();
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UninstallColormapBase(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UnmapSubwindowsBase(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto UnmapWindowBase(uint window)
    {
        var request = new UnmapWindowType(window);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

    private ResponseProto WarpPointerBase(uint srcWindow, uint destinationWindow, short srcX, short srcY,
        ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _clientConnection.ProtoOut.Send(ref request);
        return new ResponseProto(_clientConnection.ProtoOut.Sequence);
    }

}
