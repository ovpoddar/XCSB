using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Handlers;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Models.Infrastructure.Response;
using Xcsb.Requests;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Response.Contract;

internal class BaseProtoClient
{
    public readonly ProtoIn ProtoIn;
    public readonly ProtoOut ProtoOut;

    public BaseProtoClient(Socket socket)
    {
        this.ProtoIn = new ProtoIn(socket);
        this.ProtoOut = new ProtoOut(socket);
    }

    protected ResponseProto ChangeWindowAttributesBase(uint window, ValueMask mask, Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto DestroyWindowBase(uint window)
    {
        var request = new DestroyWindowType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto AllowEventsBase(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto BellBase(sbyte percent)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ChangeActivePointerGrabBase(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ChangeGCBase(uint gc, GCMask mask, Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ChangeHostsBase(HostMode mode, Family family, Span<byte> address)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ChangeKeyboardControlBase(KeyboardControlMask mask, Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ChangeKeyboardMappingBase(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> keysym)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ChangePointerControlBase(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ChangePropertyBase<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ChangeSaveSetBase(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CirculateWindowBase(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ClearAreaBase(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CloseFontBase(uint fontId)
    {
        var request = new CloseFontType(fontId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ConfigureWindowBase(uint window, ConfigureValueMask mask, Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ConvertSelectionBase(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CopyAreaBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CopyColormapAndFreeBase(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CopyGCBase(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CopyPlaneBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CreateColormapBase(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CreateCursorBase(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CreateGCBase(uint gc, uint drawable, GCMask mask, Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto CreateGlyphCursorBase(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CreatePixmapBase(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto CreateWindowBase(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto DeletePropertyBase(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto DestroySubwindowsBase(uint window)
    {
        var request = new DestroySubWindowsType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FillPolyBase(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ForceScreenSaverBase(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FreeColormapBase(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FreeColorsBase(uint colormapId, uint planeMask, Span<uint> pixels)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FreeCursorBase(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FreeGCBase(uint gc)
    {
        var request = new FreeGCType(gc);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto FreePixmapBase(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto GrabButtonBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto GrabKeyBase(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto GrabServerBase()
    {
        var request = new GrabServerType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto ImageText16Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
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
            ProtoOut.SendExact(scratchBuffer);
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
            ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ImageText8Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto InstallColormapBase(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto KillClientBase(uint resource)
    {
        var request = new KillClientType(resource);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto MapSubwindowsBase(uint window)
    {
        var request = new MapSubWindowsType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto MapWindowBase(uint window)
    {
        var request = new MapWindowType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto NoOperationBase(Span<uint> args)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto OpenFontBase(string fontName, uint fontId)
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
            ProtoOut.SendExact(scratchBuffer);
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
            ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyArcBase(uint drawable, uint gc, Span<Arc> arcs)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyFillArcBase(uint drawable, uint gc, Span<Arc> arcs)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyFillRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyLineBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyPointBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolySegmentBase(uint drawable, uint gc, Span<Segment> segments)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyText16Base(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
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
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PolyText8Base(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
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
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto PutImageBase(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto RecolorCursorBase(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto ReparentWindowBase(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto RotatePropertiesBase(uint window, ushort delta, Span<ATOM> properties)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto SendEventBase(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto SetAccessControlBase(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto SetClipRectanglesBase(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto SetCloseDownModeBase(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto SetDashesBase(uint gc, ushort dashOffset, Span<byte> dashes)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);


    }

    protected ResponseProto SetFontPathBase(string[] strPaths)
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
            ProtoOut.SendExact(scratchBuffer);
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
            ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto SetInputFocusBase(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto SetScreenSaverBase(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto SetSelectionOwnerBase(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto StoreColorsBase(uint colormapId, Span<ColorItem> item)
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
            ProtoOut.SendExact(scratchBuffer);
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
        }
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto StoreNamedColorBase(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
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
            ProtoOut.SendExact(scratchBuffer);
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
        return new ResponseProto(ProtoOut.Sequence);

    }

    protected ResponseProto UngrabButtonBase(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UngrabKeyBase(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UngrabKeyboardBase(uint time)
    {
        var request = new UngrabKeyboardType(time);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UngrabPointerBase(uint time)
    {
        var request = new UngrabPointerType(time);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UngrabServerBase()
    {
        var request = new UnGrabServerType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UninstallColormapBase(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UnmapSubwindowsBase(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto UnmapWindowBase(uint window)
    {
        var request = new UnmapWindowType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto WarpPointerBase(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence);
    }

    protected ResponseProto AllocColorBase(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryPointerBase(uint window)
    {
        var request = new QueryPointerType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GrabPointerBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto InternAtomBase(bool onlyIfExist, string atomName)
    {
        var request = new InternAtomType(onlyIfExist, atomName.Length);
        var requiredBuffer = 8 + atomName.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.ASCII.GetBytes(atomName, scratchBuffer[8..(atomName.Length + 8)]);
            scratchBuffer[(atomName.Length + 8)..requiredBuffer].Clear();
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.ASCII.GetBytes(atomName, scratchBuffer[8..(atomName.Length + 8)]);
            scratchBuffer[(atomName.Length + 8)..requiredBuffer].Clear();
            ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetPropertyBase(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetWindowAttributesBase(uint window)
    {
        var request = new GetWindowAttributesType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetGeometryBase(uint drawable)
    {
        var request = new GetGeometryType(drawable);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryTreeBase(uint window)
    {
        var request = new QueryTreeType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetAtomNameBase(ATOM atom)
    {
        var request = new GetAtomNameType(atom);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListPropertiesBase(uint window)
    {
        var request = new ListPropertiesType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetSelectionOwnerBase(ATOM atom)
    {
        var request = new GetSelectionOwnerType(atom);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GrabKeyboardBase(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyboardType(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetMotionEventsBase(uint window, uint startTime, uint endTime)
    {
        var request = new GetMotionEventsType(window, startTime, endTime);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto TranslateCoordinatesBase(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY)
    {
        var request = new TranslateCoordinatesType(srcWindow, destinationWindow, srcX, srcY);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetInputFocusBase()
    {
        var request = new GetInputFocusType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryKeymapBase()
    {
        var request = new QueryKeymapType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryFontBase(uint fontId)
    {
        var request = new QueryFontType(fontId);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryTextExtentsBase(uint font, ReadOnlySpan<char> stringForQuery)
    {

        var request = new QueryTextExtentsType(font, stringForQuery.Length);
        var requiredBuffer = 8 + (stringForQuery.Length * 2).AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.Unicode.GetBytes(stringForQuery, scratchBuffer[8..(stringForQuery.Length * 2 + 8)]);
            scratchBuffer[(stringForQuery.Length * 2 + 8)..requiredBuffer].Clear();
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.Unicode.GetBytes(stringForQuery, scratchBuffer[8..(stringForQuery.Length * 2 + 8)]);
            scratchBuffer[(stringForQuery.Length * 2 + 8)..requiredBuffer].Clear();
            ProtoOut.SendExact(scratchBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListFontsBase(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var request = new ListFontsType(pattern.Length, maxNames);
        var requiredBuffer = 8 + (pattern.Length * 2).AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                pattern
            );
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                pattern
            );
            ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListFontsWithInfoBase(ReadOnlySpan<byte> pattan, int maxNames)
    {
        var request = new ListFontsWithInfoType(pattan.Length, maxNames);
        var requiredBuffer = 8 + pattan.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                pattan
            );
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                pattan
            );
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetFontPathBase()
    {
        var request = new GetFontPathType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetImageBase(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask)
    {
        var request = new GetImageType(format, drawable, x, y, width, height, planeMask);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListInstalledColormapsBase(uint window)
    {
        var request = new ListInstalledColormapsType(window);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto AllocNamedColorBase(uint colorMap, ReadOnlySpan<byte> name)
    {
        var request = new AllocNamedColorType(colorMap, name.Length);
        var requiredBuffer = 12 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProtoOut.SendExact(workingBuffer);
        }

        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto AllocColorCellsBase(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var request = new AllocColorCellsType(contiguous, colorMap, colors, planes);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }

    protected ResponseProto AllocColorPlanesBase(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens,
        ushort blues)
    {
        var request = new AllocColorPlanesType(contiguous, colorMap, colors, reds, greens, blues);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryColorsBase(uint colorMap, Span<uint> pixels)
    {
        var request = new QueryColorsType(colorMap, pixels.Length);
        var requiredBuffer = 8 + pixels.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(pixels));
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(pixels));
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto LookupColorBase(uint colorMap, ReadOnlySpan<byte> name)
    {
        var request = new LookupColorType(colorMap, name.Length);
        var requiredBuffer = 12 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryBestSizeBase(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var request = new QueryBestSizeType(shape, drawable, width, height);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto QueryExtensionBase(ReadOnlySpan<byte> name)
    {
        var request = new QueryExtensionType((ushort)name.Length);
        var requiredBuffer = 8 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(ref request, 8, name);
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request, 8, name);
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListExtensionsBase()
    {
        var request = new ListExtensionsType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto SetModifierMappingBase(Span<ulong> keycodes)
    {
        var request = new SetModifierMappingType(keycodes.Length);
        var requiredBuffer = 4 + keycodes.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<ulong, byte>(keycodes));
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchbuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchbuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<ulong, byte>(keycodes));
            ProtoOut.SendExact(workingBuffer);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetModifierMappingBase()
    {
        var request = new GetModifierMappingType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetKeyboardMappingBase(byte firstKeycode, byte count)
    {
        var request = new GetKeyboardMappingType(firstKeycode, count);
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetKeyboardControlBase()
    {
        var request = new GetKeyboardControlType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto SetPointerMappingBase(Span<byte> maps)
    {
        var request = new SetPointerMappingType(maps);
        var requiredBuffer = maps.Length.AddPadding() + 5;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                maps);
            ProtoOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                maps);
            ProtoOut.SendExact(workingBuffer[..requiredBuffer]);
        }
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetPointerMappingBase()
    {
        var request = new GetPointerMappingType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetPointerControlBase()
    {
        var request = new GetPointerControlType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto GetScreenSaverBase()
    {
        var request = new GetScreenSaverType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
    protected ResponseProto ListHostsBase()
    {
        var request = new ListHostsType();
        ProtoOut.Send(ref request);
        return new ResponseProto(ProtoOut.Sequence, true);
    }
}