using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Handlers;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Requests;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Response.Contract;

internal class BaseProtoClient
{
    protected ProtoIn _protoIn;
    protected ProtoOut _protoOut;

    public BaseProtoClient(Socket socket)
    {
        this._protoIn = new ProtoIn(socket);
        this._protoOut = new ProtoOut(socket);
    }

    protected void ChangeWindowAttributesBase(uint window, ValueMask mask, Span<uint> args)
    {
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void DestroyWindowBase(uint window)
    {
        var request = new DestroyWindowType(window);
        _protoOut.Send(ref request);
    }

    protected void AllowEventsBase(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _protoOut.Send(ref request);
    }

    protected void BellBase(sbyte percent)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _protoOut.Send(ref request);
    }

    protected void ChangeActivePointerGrabBase(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _protoOut.Send(ref request);
    }

    protected void ChangeGCBase(uint gc, GCMask mask, Span<uint> args)
    {
        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ChangeHostsBase(HostMode mode, Family family, Span<byte> address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = 8 + address.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                address);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                address);
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ChangeKeyboardControlBase(KeyboardControlMask mask, Span<uint> args)
    {
        var requiredBuffer = 8 + args.Length * 4;
        var request = new ChangeKeyboardControlType(mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ChangeKeyboardMappingBase(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> keysym)
    {
        var requiredBuffer = 8 + keycodeCount * keysymsPerKeycode * 4;
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ChangePointerControlBase(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        _protoOut.Send(ref request);
    }

    protected void ChangePropertyBase<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
            , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 or 2 or 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, (byte)(size * 8));
        var requiredBuffer = 24 + size * args.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ChangeSaveSetBase(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _protoOut.Send(ref request);
    }

    protected void CirculateWindowBase(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _protoOut.Send(ref request);
    }

    protected void ClearAreaBase(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _protoOut.Send(ref request);
    }

    protected void CloseFontBase(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _protoOut.Send(ref request);
    }

    protected void ConfigureWindowBase(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var requiredBuffer = 12 + args.Length * 4;
        var request = new ConfigureWindowType(window, mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ConvertSelectionBase(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _protoOut.Send(ref request);
    }

    protected void CopyAreaBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _protoOut.Send(ref request);
    }

    protected void CopyColormapAndFreeBase(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _protoOut.Send(ref request);
    }

    protected void CopyGCBase(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _protoOut.Send(ref request);
    }

    protected void CopyPlaneBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        _protoOut.Send(ref request);
    }

    protected void CreateColormapBase(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _protoOut.Send(ref request);
    }

    protected void CreateCursorBase(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _protoOut.Send(ref request);
    }

    protected void CreateGCBase(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = 16 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void CreateGlyphCursorBase(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _protoOut.Send(ref request);
    }

    protected void CreatePixmapBase(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _protoOut.Send(ref request);
    }

    protected void CreateWindowBase(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var requiredBuffer = 32 + args.Length * 4;
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }
    }

    protected void DeletePropertyBase(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        _protoOut.Send(ref request);
    }

    protected void DestroySubwindowsBase(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _protoOut.Send(ref request);
    }

    protected void FillPolyBase(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = 16 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void ForceScreenSaverBase(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _protoOut.Send(ref request);
    }

    protected void FreeColormapBase(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _protoOut.Send(ref request);
    }

    protected void FreeColorsBase(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var requiredBuffer = 12 + pixels.Length * 4;
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void FreeCursorBase(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _protoOut.Send(ref request);
    }

    protected void FreeGCBase(uint gc)
    {
        var request = new FreeGCType(gc);
        _protoOut.Send(ref request);
    }

    protected void FreePixmapBase(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _protoOut.Send(ref request);
    }

    protected void GrabButtonBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _protoOut.Send(ref request);
    }

    protected void GrabKeyBase(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _protoOut.Send(ref request);
    }

    protected void GrabServerBase()
    {
        var request = new GrabServerType();
        _protoOut.Send(ref request);
    }

    protected void ImageText16Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + (text.Length * 2).AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            _protoOut.SendExact(scratchBuffer);
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
            _protoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

    }

    protected void ImageText8Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + text.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                text
            );
            _protoOut.SendExact(scratchBuffer);
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
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void InstallColormapBase(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _protoOut.Send(ref request);
    }

    protected void KillClientBase(uint resource)
    {
        var request = new KillClientType(resource);
        _protoOut.Send(ref request);
    }

    protected void MapSubwindowsBase(uint window)
    {
        var request = new MapSubWindowsType(window);
        _protoOut.Send(ref request);
    }

    protected void MapWindowBase(uint window)
    {
        var request = new MapWindowType(window);
        _protoOut.Send(ref request);

    }

    protected void NoOperationBase(Span<uint> args)
    {
        var request = new NoOperationType(args.Length);
        var requiredBuffer = 4 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void OpenFontBase(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requiredBuffer = 12 + fontName.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            _protoOut.SendExact(scratchBuffer);
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
            _protoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

    }

    protected void PolyArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = 12 + arcs.Length * 12;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyFillArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillArcType>() + arcs.Length * 12;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyFillRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + rectangles.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyLineBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyPointBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolySegmentBase(uint drawable, uint gc, Span<Segment> segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = 12 + segments.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyText16Base(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Length);
        var requiredBuffer = 16 + data.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                data);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PolyText8Base(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Length);
        var requiredBuffer = 16 + data.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                data);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void PutImageBase(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var requiredBuffer = data.Length.AddPadding() + 24;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                data);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                data);
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void RecolorCursorBase(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _protoOut.Send(ref request);

    }

    protected void ReparentWindowBase(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _protoOut.Send(ref request);
    }

    protected void RotatePropertiesBase(uint window, ushort delta, Span<ATOM> properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = 12 + properties.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void SendEventBase(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _protoOut.Send(ref request);
    }

    protected void SetAccessControlBase(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _protoOut.Send(ref request);
    }

    protected void SetClipRectanglesBase(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * Marshal.SizeOf<Rectangle>();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void SetCloseDownModeBase(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _protoOut.Send(ref request);
    }

    protected void SetDashesBase(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = 12 + dashes.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            _protoOut.SendExact(workingBuffer);
        }


    }

    protected void SetFontPathBase(string[] strPaths)
    {
        var strPathsLength = strPaths.Sum(a => a.Length + 1).AddPadding();
        var request = new SetFontPathType((ushort)strPaths.Length, strPathsLength);
        var requiredBuffer = 8 + strPathsLength;
        var writIndex = 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths)
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths)
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _protoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

    }

    protected void SetInputFocusBase(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _protoOut.Send(ref request);
    }

    protected void SetScreenSaverBase(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _protoOut.Send(ref request);
    }

    protected void SetSelectionOwnerBase(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _protoOut.Send(ref request);
    }

    protected void StoreColorsBase(uint colormapId, Span<ColorItem> item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = 8 + 12 * item.Length;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            scratchBuffer[requiredBuffer - 1] = 0;
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            scratchBuffer[requiredBuffer - 1] = 0;
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void StoreNamedColorBase(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = 16 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                name);
            _protoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
            _protoOut.SendExact(workingBuffer);
        }

    }

    protected void UngrabButtonBase(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _protoOut.Send(ref request);
    }

    protected void UngrabKeyBase(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _protoOut.Send(ref request);
    }

    protected void UngrabKeyboardBase(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _protoOut.Send(ref request);
    }

    protected void UngrabPointerBase(uint time)
    {
        var request = new UngrabPointerType(time);
        _protoOut.Send(ref request);
    }

    protected void UngrabServerBase()
    {
        var request = new UnGrabServerType();
        _protoOut.Send(ref request);
    }

    protected void UninstallColormapBase(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _protoOut.Send(ref request);
    }

    protected void UnmapSubwindowsBase(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _protoOut.Send(ref request);
    }

    protected void UnmapWindowBase(uint window)
    {
        var request = new UnmapWindowType(window);
        _protoOut.Send(ref request);
    }

    protected void WarpPointerBase(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _protoOut.Send(ref request);
    }

}