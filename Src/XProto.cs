using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Requests;
using Xcsb.Response;
using Xcsb.Response.Internals;
using Xcsb.Response.Event;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb;

#if !NETSTANDARD
[SkipLocalsInit]
#endif
internal class XProto : BaseProtoClient, IXProto
{
    private bool _disposedValue;
    private int _globalId;
    private XBufferProto? _xBufferProto;

    #region private implementation

    private AllocColorReply? AllocColor(uint colorMap, ushort red, ushort green, ushort blue, bool isThrow)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        ProcessEvents(isThrow);
        socket.Send(ref request);

        var (result, error) = ReceivedResponseAndVerify<AllocColorReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        Debug.Assert(sequenceNumber == result.Value.ResponseHeader.Sequence);
        return result.Value;
    }

    private AllocColorCellsReply? AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes,
        bool isThrow)
    {
        var request = new AllocColorCellsType(contiguous, colorMap, colors, planes);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<AllocColorCellsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new AllocColorCellsReply(result.Value, socket);
    }

    private AllocColorPlanesReply? AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues, bool isThrow)
    {
        var request = new AllocColorPlanesType(contiguous, colorMap, colors, reds, greens, blues);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<AllocColorPlanesResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new AllocColorPlanesReply(result.Value, socket);
    }

    private AllocNamedColorReply? AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<AllocNamedColorReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }


    private GetAtomNameReply? GetAtomName(ATOM atom, bool isThrow)
    {
        var request = new GetAtomNameType(atom);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetAtomNameResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetAtomNameReply(result.Value, socket);
    }

    private InternAtomReply? InternAtom(bool onlyIfExist, string atomName, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        var (result, error) = ReceivedResponseAndVerify<InternAtomReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        Debug.Assert(sequenceNumber == result.Value.ResponseHeader.Sequence);
        return result.Value;
    }

    private GetFontPathReply? GetFontPath(bool isThrow)
    {
        var request = new GetFontPathType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetFontPathResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetFontPathReply(result.Value, socket);
    }

    private GetGeometryReply? GetGeometry(uint drawable, bool isThrow)
    {
        var request = new GetGeometryType(drawable);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetGeometryReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GetImageReply? GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask, bool isThrow)
    {
        var request = new GetImageType(format, drawable, x, y, width, height, planeMask);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetImageResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetImageReply(result.Value, socket);
    }

    private GetInputFocusReply? GetInputFocus(bool isThrow)
    {
        var request = new GetInputFocusType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetInputFocusReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GetKeyboardControlReply? GetKeyboardControl(bool isThrow)
    {
        var request = new GetKeyboardControlType();
        ProcessEvents(isThrow);
        socket.Send(ref request);

        var (result, error) = ReceivedResponseAndVerify<GetKeyboardControlResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetKeyboardControlReply(result.Value);
    }

    private GetKeyboardMappingReply? GetKeyboardMapping(byte firstKeycode, byte count, bool isThrow)
    {
        var request = new GetKeyboardMappingType(firstKeycode, count);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetKeyboardMappingResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetKeyboardMappingReply(result.Value, count, socket);
    }

    private GetModifierMappingReply? GetModifierMapping(bool isThrow)
    {
        var request = new GetModifierMappingType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetModifierMappingResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetModifierMappingReply(result.Value, socket);
    }

    private GetMotionEventsReply? GetMotionEvents(uint window, uint startTime, uint endTime, bool isThrow)
    {
        var request = new GetMotionEventsType(window, startTime, endTime);
        ProcessEvents(throwOnError: isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetMotionEventsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetMotionEventsReply(result.Value, socket);
    }

    private GetPointerControlReply? GetPointerControl(bool isThrow)
    {
        var request = new GetPointerControlType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetPointerControlReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GetPointerMappingReply? GetPointerMapping(bool isThrow)
    {
        var request = new GetPointerMappingType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetPointerMappingResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetPointerMappingReply(result.Value, socket);
    }

    private GetPropertyReply? GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length,
        bool isThrow)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetPropertyResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new GetPropertyReply(result.Value, socket);
    }

    private GetScreenSaverReply? GetScreenSaver(bool isThrow)
    {
        var request = new GetScreenSaverType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetScreenSaverReply>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GetSelectionOwnerReply? GetSelectionOwner(ATOM atom, bool isThrow)
    {
        var request = new GetSelectionOwnerType(atom);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetSelectionOwnerReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GetWindowAttributesReply? GetWindowAttributes(uint window, bool isThrow)
    {
        var request = new GetWindowAttributesType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<GetWindowAttributesReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private ListExtensionsReply? ListExtensions(bool isThrow)
    {
        var request = new ListExtensionsType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<ListExtensionsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new ListExtensionsReply(result.Value, socket);
    }

    private ListFontsReply? ListFonts(ReadOnlySpan<byte> pattern, int maxNames, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<ListFontsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new ListFontsReply(result.Value, socket);
    }

    private ListFontsWithInfoReply[]? ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (response, error) = ReceivedResponseAndVerify<ListFontsWithInfoResponse>();
        if (!error.HasValue && !response.HasValue) return null;
        if (error.HasValue || !response.HasValue)
            throw new XEventException(error!.Value);
        var result = new List<ListFontsWithInfoReply>(maxNames);
        foreach (var item in socket.GetNextStrValue(response.Value))
            result.Add(item);
        return result.ToArray();
    }

    private ListHostsReply? ListHosts(bool isThrow)
    {
        var request = new ListHostsType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<ListHostsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new ListHostsReply(result.Value, socket);
    }

    private ListInstalledColormapsReply? ListInstalledColormaps(uint window, bool isThrow)
    {
        var request = new ListInstalledColormapsType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<ListInstalledColormapsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new ListInstalledColormapsReply(result.Value, socket);
        ;
    }

    private ListPropertiesReply? ListProperties(uint window, bool isThrow)
    {
        var request = new ListPropertiesType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<ListPropertiesResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new ListPropertiesReply(result.Value, socket);
    }

    private LookupColorReply? LookupColor(uint colorMap, ReadOnlySpan<byte> name, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<LookupColorReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private QueryBestSizeReply? QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height,
        bool isThrow)
    {
        var request = new QueryBestSizeType(shape, drawable, width, height);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<QueryBestSizeReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private QueryColorsReply? QueryColors(uint colorMap, Span<uint> pixels, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(pixels));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<QueryColorsResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new QueryColorsReply(result.Value, socket);
    }

    private QueryExtensionReply? QueryExtension(ReadOnlySpan<byte> name, bool isThrow)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
        var request = new QueryExtensionType((ushort)name.Length);
        var requiredBuffer = 8 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(ref request, 8, name);
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request, 8, name);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<QueryExtensionReply>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private QueryFontReply? QueryFont(uint fontId, bool isThrow)
    {
        var request = new QueryFontType(fontId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<QueryFontResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new QueryFontReply(result.Value, socket);
    }

    private QueryKeymapReply? QueryKeymap(bool isThrow)
    {
        var request = new QueryKeymapType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<QueryKeymapResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new QueryKeymapReply(result.Value, socket);
    }

    private QueryPointerReply? QueryPointer(uint window, bool isThrow)
    {
        var request = new QueryPointerType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);

        var (result, error) = ReceivedResponseAndVerify<QueryPointerReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private QueryTextExtentsReply? QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        var (result, error) = ReceivedResponseAndVerify<QueryTextExtentsReply>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private QueryTreeReply? QueryTree(uint window, bool isThrow)
    {
        var request = new QueryTreeType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<QueryTreeResponse>();
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return new QueryTreeReply(result.Value, socket);
    }


    private GrabKeyboardReply? GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode, bool isThrow)
    {
        var request = new GrabKeyboardType(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        ProcessEvents(isThrow);
        socket.Send(ref request);

        var (result, error) = ReceivedResponseAndVerify<GrabKeyboardReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private GrabPointerReply? GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp, bool isThrow)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        ProcessEvents(isThrow);
        socket.Send(ref request);

        var (result, error) = ReceivedResponseAndVerify<GrabPointerReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private SetModifierMappingReply? SetModifierMapping(Span<ulong> keycodes, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchbuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchbuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<ulong, byte>(keycodes));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        var (result, error) = ReceivedResponseAndVerify<SetModifierMappingReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private SetPointerMappingReply? SetPointerMapping(Span<byte> maps, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                maps);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer[..requiredBuffer]);
        }

        var (result, error) = ReceivedResponseAndVerify<SetPointerMappingReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }

    private TranslateCoordinatesReply? TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY, bool isThrow)
    {
        var request = new TranslateCoordinatesType(srcWindow, destinationWindow, srcX, srcY);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        var (result, error) = ReceivedResponseAndVerify<TranslateCoordinatesReply>(true);
        if (!error.HasValue && !result.HasValue) return null;
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        return result.Value;
    }


    private void ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void DestroyWindow(uint window, bool isThrow)
    {
        var request = new DestroyWindowType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void AllowEvents(EventsMode mode, uint time, bool isThrow)
    {
        var request = new AllowEventsType(mode, time);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void Bell(sbyte percent, bool isThrow)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ChangeActivePointerGrab(uint cursor, uint time, ushort mask, bool isThrow)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ChangeGC(uint gc, GCMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ChangeHosts(HostMode mode, Family family, Span<byte> address, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                address);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> keysym,
        bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ChangePointerControl(Acceleration? acceleration, ushort? threshold, bool isThrow)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window, bool isThrow)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CirculateWindow(Circulate circulate, uint window, bool isThrow)
    {
        var request = new CirculateWindowType(circulate, window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height, bool isThrow)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CloseFont(uint fontId, bool isThrow)
    {
        var request = new CloseFontType(fontId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp,
        bool isThrow)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, bool isThrow)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CopyColormapAndFree(uint colormapId, uint srcColormapId, bool isThrow)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CopyGC(uint srcGc, uint dstGc, GCMask mask, bool isThrow)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane, bool isThrow)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual, bool isThrow)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y, bool isThrow)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue,
        bool isThrow)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height, bool isThrow)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void DeleteProperty(uint window, ATOM atom, bool isThrow)
    {
        var request = new DeletePropertyType(window, atom);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void DestroySubwindows(uint window, bool isThrow)
    {
        var request = new DestroySubWindowsType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points,
        bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void ForceScreenSaver(ForceScreenSaverMode mode, bool isThrow)
    {
        var request = new ForceScreenSaverType(mode);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void FreeColormap(uint colormapId, bool isThrow)
    {
        var request = new FreeColormapType(colormapId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void FreeColors(uint colormapId, uint planeMask, Span<uint> pixels, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void FreeCursor(uint cursorId, bool isThrow)
    {
        var request = new FreeCursorType(cursorId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void FreeGC(uint gc, bool isThrow)
    {
        var request = new FreeGCType(gc);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void FreePixmap(uint pixmapId, bool isThrow)
    {
        var request = new FreePixmapType(pixmapId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers, bool isThrow)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode, bool isThrow)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void GrabServer(bool isThrow)
    {
        var request = new GrabServerType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    private void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void InstallColormap(uint colormapId, bool isThrow)
    {
        var request = new InstallColormapType(colormapId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void KillClient(uint resource, bool isThrow)
    {
        var request = new KillClientType(resource);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void MapSubwindows(uint window, bool isThrow)
    {
        var request = new MapSubWindowsType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void MapWindow(uint window, bool isThrow)
    {
        var request = new MapWindowType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void NoOperation(Span<uint> args, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void OpenFont(string fontName, uint fontId, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    private void PolyArc(uint drawable, uint gc, Span<Arc> arcs, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyFillArc(uint drawable, uint gc, Span<Arc> arcs, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolySegment(uint drawable, uint gc, Span<Segment> segments, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y,
        byte leftPad, byte depth, Span<byte> data, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                data);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue, bool isThrow)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void ReparentWindow(uint window, uint parent, short x, short y, bool isThrow)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void RotateProperties(uint window, ushort delta, Span<ATOM> properties, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt, bool isThrow)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void SetAccessControl(AccessControlMode mode, bool isThrow)
    {
        var request = new SetAccessControlType(mode);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void SetCloseDownMode(CloseDownMode mode, bool isThrow)
    {
        var request = new SetCloseDownModeType(mode);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void SetDashes(uint gc, ushort dashOffset, Span<byte> dashes, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void SetFontPath(string[] strPaths, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    private void SetInputFocus(InputFocusMode mode, uint focus, uint time, bool isThrow)
    {
        var request = new SetInputFocusType(mode, focus, time);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures,
        bool isThrow)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void SetSelectionOwner(uint owner, ATOM atom, uint timestamp, bool isThrow)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void StoreColors(uint colormapId, Span<ColorItem> item, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
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
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name, bool isThrow)
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
            ProcessEvents(isThrow);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
            ProcessEvents(isThrow);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    private void UngrabButton(Button button, uint grabWindow, ModifierMask mask, bool isThrow)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UngrabKey(byte key, uint grabWindow, ModifierMask modifier, bool isThrow)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UngrabKeyboard(uint time, bool isThrow)
    {
        var request = new UngrabKeyboardType(time);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UngrabPointer(uint time, bool isThrow)
    {
        var request = new UngrabPointerType(time);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UngrabServer(bool isThrow)
    {
        var request = new UnGrabServerType();
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UninstallColormap(uint colormapId, bool isThrow)
    {
        var request = new UninstallColormapType(colormapId);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UnmapSubwindows(uint window, bool isThrow)
    {
        var request = new UnMapSubwindowsType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void UnmapWindow(uint window, bool isThrow)
    {
        var request = new UnmapWindowType(window);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    private void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY, bool isThrow)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        ProcessEvents(isThrow);
        socket.Send(ref request);
        sequenceNumber++;
    }

    #endregion

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }

    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(this);

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult) : base(socket)
    {
        HandshakeSuccessResponseBody = connectionResult;
        _globalId = 0;
        sequenceNumber = 0;
    }

    public AllocColorReply? AllocColor(uint colorMap, ushort red, ushort green, ushort blue) =>
        this.AllocColor(colorMap, red, green, blue, false);

    public AllocColorCellsReply? AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes) =>
        this.AllocColorCells(contiguous, colorMap, colors, planes, false);

    public AllocColorPlanesReply? AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues) =>
        this.AllocColorPlanes(contiguous, colorMap, colors, reds, greens, blues, false);

    public AllocNamedColorReply? AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name) =>
        this.AllocNamedColor(colorMap, name, false);

    public GetAtomNameReply? GetAtomName(ATOM atom) =>
        this.GetAtomName(atom, false);

    public InternAtomReply? InternAtom(bool onlyIfExist, string atomName) =>
        this.InternAtom(onlyIfExist, atomName, false);

    public GetFontPathReply? GetFontPath() =>
        this.GetFontPath(false);

    public GetGeometryReply? GetGeometry(uint drawable) =>
        this.GetGeometry(drawable, false);

    public GetImageReply? GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask) =>
        this.GetImage(format, drawable, x, y, width, height, planeMask, false);

    public GetInputFocusReply? GetInputFocus() =>
        this.GetInputFocus(false);

    public GetKeyboardControlReply? GetKeyboardControl() =>
        this.GetKeyboardControl(false);

    public GetKeyboardMappingReply? GetKeyboardMapping(byte firstKeycode, byte count) =>
        this.GetKeyboardMapping(firstKeycode, count, false);

    public GetModifierMappingReply? GetModifierMapping() =>
        this.GetModifierMapping(false);

    public GetMotionEventsReply? GetMotionEvents(uint window, uint startTime, uint endTime) =>
        this.GetMotionEvents(window, startTime, endTime, false);

    public GetPointerControlReply? GetPointerControl() =>
        this.GetPointerControl(false);

    public GetPointerMappingReply? GetPointerMapping() =>
        this.GetPointerMapping(false);

    public GetPropertyReply? GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length) =>
        this.GetProperty(delete, window, property, type, offset, length, false);

    public GetScreenSaverReply? GetScreenSaver() =>
        this.GetScreenSaver(false);

    public GetSelectionOwnerReply? GetSelectionOwner(ATOM atom) =>
        this.GetSelectionOwner(atom, false);

    public GetWindowAttributesReply? GetWindowAttributes(uint window) =>
        this.GetWindowAttributes(window, false);

    public GrabKeyboardReply? GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode) =>
        this.GrabKeyboard(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode, false);

    public GrabPointerReply? GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp) =>
        this.GrabPointer(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, timeStamp,
            false);

    public ListExtensionsReply? ListExtensions() =>
        this.ListExtensions(false);

    public ListFontsReply? ListFonts(ReadOnlySpan<byte> pattern, int maxNames) =>
        this.ListFonts(pattern, maxNames, false);

    public ListFontsWithInfoReply[]? ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames) =>
        this.ListFontsWithInfo(pattan, maxNames, false);

    public ListHostsReply? ListHosts() =>
        this.ListHosts(false);

    public ListInstalledColormapsReply? ListInstalledColormaps(uint window) =>
        this.ListInstalledColormaps(window, false);

    public ListPropertiesReply? ListProperties(uint window) =>
        this.ListProperties(window, false);

    public LookupColorReply? LookupColor(uint colorMap, ReadOnlySpan<byte> name) =>
        this.LookupColor(colorMap, name, false);

    public QueryBestSizeReply? QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height) =>
        this.QueryBestSize(shape, drawable, width, height, false);

    public QueryColorsReply? QueryColors(uint colorMap, Span<uint> pixels) =>
        this.QueryColors(colorMap, pixels, false);

    public QueryExtensionReply? QueryExtension(ReadOnlySpan<byte> name) =>
        this.QueryExtension(name, false);

    public QueryFontReply? QueryFont(uint fontId) =>
        this.QueryFont(fontId, false);

    public QueryKeymapReply? QueryKeymap() =>
        this.QueryKeymap(false);

    public QueryPointerReply? QueryPointer(uint window) =>
        this.QueryPointer(window, false);

    public QueryTextExtentsReply? QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery) =>
        this.QueryTextExtents(font, stringForQuery, false);

    public QueryTreeReply? QueryTree(uint window) =>
        this.QueryTree(window, false);

    public SetModifierMappingReply? SetModifierMapping(Span<ulong> keycodes) =>
        this.SetModifierMapping(keycodes, false);

    public SetPointerMappingReply? SetPointerMapping(Span<byte> maps) =>
        this.SetPointerMapping(maps, false);

    public TranslateCoordinatesReply? TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY) =>
        this.TranslateCoordinates(srcWindow, destinationWindow, srcX, srcY, false);

    public void AllowEvents(EventsMode mode, uint time) =>
        this.AllowEvents(mode, time, false);

    public void Bell(sbyte percent) =>
        this.Bell(percent, false);

    public void ChangeActivePointerGrab(uint cursor, uint time, ushort mask) =>
        this.ChangeActivePointerGrab(cursor, time, mask, false);

    public void ChangeGC(uint gc, GCMask mask, Span<uint> args) =>
        this.ChangeGC(gc, mask, args, false);

    public void ChangeHosts(HostMode mode, Family family, Span<byte> address) =>
        this.ChangeHosts(mode, family, address, false);

    public void ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args) =>
        this.ChangeKeyboardControl(mask, args, false);

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym) =>
        this.ChangeKeyboardMapping(keycodeCount, firstKeycode, keysymsPerKeycode, keysym, false);

    public void ChangePointerControl(Acceleration? acceleration, ushort? threshold) =>
        this.ChangePointerControl(acceleration, threshold, false);

    public void ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
        => this.ChangeProperty<T>(mode, window, property, type, args, false);

    public void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window) =>
        this.ChangeSaveSet(changeSaveSetMode, window, false);

    public void ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args) =>
        this.ChangeWindowAttributes(window, mask, args, false);

    public void CirculateWindow(Circulate circulate, uint window) =>
        this.CirculateWindow(circulate, window, false);

    public void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height) =>
        this.ClearArea(exposures, window, x, y, width, height, false);

    public void CloseFont(uint fontId) =>
        this.CloseFont(fontId, false);

    public void ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args) =>
        this.ConfigureWindow(window, mask, args, false);

    public void ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp) =>
        this.ConvertSelection(requestor, selection, target, property, timestamp, false);

    public void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height) =>
        this.CopyArea(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width,
            height, false);

    public void CopyColormapAndFree(uint colormapId, uint srcColormapId) =>
        this.CopyColormapAndFree(colormapId, srcColormapId, false);

    public void CopyGC(uint srcGc, uint dstGc, GCMask mask) =>
        this.CopyGC(srcGc, dstGc, mask, false);

    public void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane) =>
        this.CopyPlane(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width,
            height, bitPlane, false);

    public void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual) =>
        this.CreateColormap(alloc, colormapId, window, visual, false);

    public void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y) =>
        this.CreateCursor(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y,
            false);

    public void CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args) =>
        this.CreateGC(gc, drawable, mask, args, false);

    public void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue) =>
        this.CreateGlyphCursor(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed,
            backGreen, backBlue, false);

    public void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height) =>
        this.CreatePixmap(depth, pixmapId, drawable, width, height, false);

    public void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args) =>
        this.CreateWindow(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask,
            args, false);

    public void DeleteProperty(uint window, ATOM atom) =>
        this.DeleteProperty(window, atom, false);

    public void DestroySubwindows(uint window) =>
        this.DestroySubwindows(window, false);

    public void DestroyWindow(uint window) =>
        this.DestroyWindow(window, false);

    public void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points) =>
        this.FillPoly(drawable, gc, shape, coordinate, points, false);

    public void ForceScreenSaver(ForceScreenSaverMode mode) =>
        this.ForceScreenSaver(mode, false);

    public void FreeColormap(uint colormapId) =>
        this.FreeColormap(colormapId, false);

    public void FreeColors(uint colormapId, uint planeMask, Span<uint> pixels) =>
        this.FreeColors(colormapId, planeMask, pixels, false);

    public void FreeCursor(uint cursorId) =>
        this.FreeCursor(cursorId, false);

    public void FreeGC(uint gc) =>
        this.FreeGC(gc, false);

    public void FreePixmap(uint pixmapId) =>
        this.FreePixmap(pixmapId, false);

    public void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers) =>
        this.GrabButton(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers,
            false);

    public void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode) =>
        this.GrabKey(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode, false);

    public void GrabServer() =>
        this.GrabServer(false);

    public void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text) =>
        this.ImageText16(drawable, gc, x, y, text, false);

    public void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text) =>
        this.ImageText8(drawable, gc, x, y, text, false);

    public void InstallColormap(uint colormapId) =>
        this.InstallColormap(colormapId, false);

    public void KillClient(uint resource) =>
        this.KillClient(resource, false);

    public void MapSubwindows(uint window) =>
        this.MapSubwindows(window, false);

    public void MapWindow(uint window) =>
        this.MapWindow(window, false);

    public void NoOperation(Span<uint> args) =>
        this.NoOperation(args, false);

    public void OpenFont(string fontName, uint fontId) =>
        this.OpenFont(fontName, fontId, false);

    public void PolyArc(uint drawable, uint gc, Span<Arc> arcs) =>
        this.PolyArc(drawable, gc, arcs, false);

    public void PolyFillArc(uint drawable, uint gc, Span<Arc> arcs) =>
        this.PolyFillArc(drawable, gc, arcs, false);

    public void PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles) =>
        this.PolyFillRectangle(drawable, gc, rectangles, false);

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points) =>
        this.PolyLine(coordinate, drawable, gc, points, false);

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points) =>
        this.PolyPoint(coordinate, drawable, gc, points, false);

    public void PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles) =>
        this.PolyRectangle(drawable, gc, rectangles, false);

    public void PolySegment(uint drawable, uint gc, Span<Segment> segments) =>
        this.PolySegment(drawable, gc, segments, false);

    public void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data) =>
        this.PolyText16(drawable, gc, x, y, data, false);

    public void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data) =>
        this.PolyText8(drawable, gc, x, y, data, false);

    public void PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data) =>
        this.PutImage(format, drawable, gc, width, height, x, y, leftPad, depth, data, false);

    public void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue) =>
        this.RecolorCursor(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, false);

    public void ReparentWindow(uint window, uint parent, short x, short y) =>
        this.ReparentWindow(window, parent, x, y, false);

    public void RotateProperties(uint window, ushort delta, Span<ATOM> properties) =>
        this.RotateProperties(window, delta, properties, false);

    public void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt) =>
        this.SendEvent(propagate, destination, eventMask, evnt, false);

    public void SetAccessControl(AccessControlMode mode) =>
        this.SetAccessControl(mode, false);

    public void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles) =>
        this.SetClipRectangles(ordering, gc, clipX, clipY, rectangles, false);

    public void SetCloseDownMode(CloseDownMode mode) =>
        this.SetCloseDownMode(mode, false);

    public void SetDashes(uint gc, ushort dashOffset, Span<byte> dashes) =>
        this.SetDashes(gc, dashOffset, dashes, false);

    public void SetFontPath(string[] strPaths) =>
        this.SetFontPath(strPaths, false);

    public void SetInputFocus(InputFocusMode mode, uint focus, uint time) =>
        this.SetInputFocus(mode, focus, time, false);

    public void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures) =>
        this.SetScreenSaver(timeout, interval, preferBlanking, allowExposures, false);

    public void SetSelectionOwner(uint owner, ATOM atom, uint timestamp) =>
        this.SetSelectionOwner(owner, atom, timestamp, false);

    public void StoreColors(uint colormapId, Span<ColorItem> item) =>
        this.StoreColors(colormapId, item, false);

    public void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name) =>
        this.StoreNamedColor(mode, colormapId, pixels, name, false);

    public void UngrabButton(Button button, uint grabWindow, ModifierMask mask) =>
        this.UngrabButton(button, grabWindow, mask, false);

    public void UngrabKey(byte key, uint grabWindow, ModifierMask modifier) =>
        this.UngrabKey(key, grabWindow, modifier, false);

    public void UngrabKeyboard(uint time) =>
        this.UngrabKeyboard(time, false);

    public void UngrabPointer(uint time) =>
        this.UngrabPointer(time, false);

    public void UngrabServer() =>
        this.UngrabServer(false);

    public void UninstallColormap(uint colormapId) =>
        this.UninstallColormap(colormapId, false);

    public void UnmapSubwindows(uint window) =>
        this.UnmapSubwindows(window, false);

    public void UnmapWindow(uint window) =>
        this.UnmapWindow(window, false);

    public void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY) =>
        this.WarpPointer(srcWindow, destinationWindow, srcX, srcY, srcWidth,
            srcHeight, destinationX, destinationY, false);

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void WaitForEvent()
    {
        if (!IsEventAvailable())
            socket.Poll(-1, SelectMode.SelectRead);
    }

    public uint NewId()
    {
        return (uint)((HandshakeSuccessResponseBody.ResourceIDMask & _globalId++) |
                      HandshakeSuccessResponseBody.ResourceIDBase);
    }

    public XEvent GetEvent()
    {
        if (bufferEvents.TryDequeue(out var result))
            return result.As<XEvent>();
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];

        if (socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = socket.Receive(scratchBuffer);
            if (totalRead == 0)
                return scratchBuffer.Make<XEvent, LastEvent>(new(base.sequenceNumber));
        }

        return scratchBuffer.ToStruct<XEvent>();
    }

    public bool IsEventAvailable() =>
        bufferEvents.Any() || socket.Available >= Unsafe.SizeOf<GenericEvent>();

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        this.CreateWindow(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask, args,
            true);
        CheckError();
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args)
    {
        this.ChangeWindowAttributes(window, mask, args, true);
        CheckError();
    }

    public void DestroyWindowChecked(uint window)
    {
        this.DestroyWindow(window, true);
        CheckError();
    }

    public void DestroySubwindowsChecked(uint window)
    {
        this.DestroySubwindows(window, true);
        CheckError();
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        this.ChangeSaveSet(changeSaveSetMode, window, true);
        CheckError();
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        this.ReparentWindow(window, parent, x, y, true);
        CheckError();
    }

    public void MapWindowChecked(uint window)
    {
        this.MapWindow(window, true);
        CheckError();
    }

    public void MapSubwindowsChecked(uint window)
    {
        this.MapSubwindows(window, true);
        CheckError();
    }

    public void UnmapWindowChecked(uint window)
    {
        this.UnmapWindow(window, true);
        CheckError();
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        this.UnmapSubwindows(window, true);
        CheckError();
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        this.ConfigureWindow(window, mask, args, true);
        CheckError();
    }

    public void CirculateWindowChecked(Circulate circulate, uint window)
    {
        this.CirculateWindow(circulate, window, true);
        CheckError();
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        this.ChangeProperty(mode, window, property, type, args, true);
        CheckError();
    }

    public void DeletePropertyChecked(uint window, ATOM atom)
    {
        this.DeleteProperty(window, atom, true);
        CheckError();
    }

    public void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties)
    {
        this.RotateProperties(window, delta, properties, true);
        CheckError();
    }

    public void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp)
    {
        this.SetSelectionOwner(owner, atom, timestamp, true);
        CheckError();
    }

    public void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        this.ConvertSelection(requestor, selection, target, property, timestamp, true);
        CheckError();
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        this.SendEvent(propagate, destination, eventMask, evnt, true);
        CheckError();
    }

    public void UngrabPointerChecked(uint time)
    {
        this.UngrabPointer(time, true);
        CheckError();
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        this.GrabButton(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers,
            true);
        CheckError();
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        this.UngrabButton(button, grabWindow, mask, true);
        CheckError();
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        this.ChangeActivePointerGrab(cursor, time, mask, true);
        CheckError();
    }

    public void UngrabKeyboardChecked(uint time)
    {
        this.UngrabKeyboard(time, true);
        CheckError();
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        this.GrabKey(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode, true);
        CheckError();
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        this.UngrabKey(key, grabWindow, modifier, true);
        CheckError();
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        this.AllowEvents(mode, time, true);
        CheckError();
    }

    public void GrabServerChecked()
    {
        this.GrabServer(true);
        CheckError();
    }

    public void UngrabServerChecked()
    {
        this.UngrabServer(true);
        CheckError();
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        this.WarpPointer(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY,
            true);
        CheckError();
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        this.SetInputFocus(mode, focus, time, true);
        CheckError();
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        this.OpenFont(fontName, fontId, true);
        CheckError();
    }

    public void CloseFontChecked(uint fontId)
    {
        this.CloseFont(fontId, true);
        CheckError();
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        this.SetFontPath(strPaths, true);
        CheckError();
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        this.CreatePixmap(depth, pixmapId, drawable, width, height, true);
        CheckError();
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        this.FreePixmap(pixmapId, true);
        CheckError();
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        this.CreateGC(gc, drawable, mask, args, true);
        CheckError();
    }

    public void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args)
    {
        this.ChangeGC(gc, mask, args, true);
        CheckError();
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        this.CopyGC(srcGc, dstGc, mask, true);
        CheckError();
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        this.SetDashes(gc, dashOffset, dashes, true);
        CheckError();
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        this.SetClipRectangles(ordering, gc, clipX, clipY, rectangles, true);
        CheckError();
    }

    public void FreeGCChecked(uint gc)
    {
        this.FreeGC(gc, true);
        CheckError();
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        this.ClearArea(exposures, window, x, y, width, height, true);
        CheckError();
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        this.CopyArea(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            true);
        CheckError();
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        this.CopyPlane(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane, true);
        CheckError();
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyPoint(coordinate, drawable, gc, points, true);
        CheckError();
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyLine(coordinate, drawable, gc, points, true);
        CheckError();
    }

    public void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments)
    {
        this.PolySegment(drawable, gc, segments, true);
        CheckError();
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyRectangle(drawable, gc, rectangles, true);
        CheckError();
    }

    public void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyArc(drawable, gc, arcs, true);
        CheckError();
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        this.FillPoly(drawable, gc, shape, coordinate, points, true);
        CheckError();
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyFillRectangle(drawable, gc, rectangles, true);
        CheckError();
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyFillArc(drawable, gc, arcs, true);
        CheckError();
    }

    public void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        this.PutImage(format, drawable, gc, width, height, x, y, leftPad, depth, data, true);
        CheckError();
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        this.ImageText8(drawable, gc, x, y, text, true);
        CheckError();
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        this.ImageText16(drawable, gc, x, y, text, true);
        CheckError();
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        this.CreateColormap(alloc, colormapId, window, visual, true);
        CheckError();
    }

    public void FreeColormapChecked(uint colormapId)
    {
        this.FreeColormap(colormapId, true);
        CheckError();
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        this.CopyColormapAndFree(colormapId, srcColormapId, true);
        CheckError();
    }

    public void InstallColormapChecked(uint colormapId)
    {
        this.InstallColormap(colormapId, true);
        CheckError();
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        this.UninstallColormap(colormapId, true);
        CheckError();
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        this.FreeColors(colormapId, planeMask, pixels, true);
        CheckError();
    }

    public void StoreColorsChecked(uint colormapId, Span<ColorItem> item)
    {
        this.StoreColors(colormapId, item, true);
        CheckError();
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        this.StoreNamedColor(mode, colormapId, pixels, name, true);
        CheckError();
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        this.CreateCursor(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y,
            true);
        CheckError();
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        this.CreateGlyphCursor(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue, true);
        CheckError();
    }

    public void FreeCursorChecked(uint cursorId)
    {
        this.FreeCursor(cursorId, true);
        CheckError();
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        this.RecolorCursor(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, true);
        CheckError();
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        this.ChangeKeyboardMapping(keycodeCount, firstKeycode, keysymsPerKeycode, keysym, true);
        CheckError();
    }

    public void BellChecked(sbyte percent)
    {
        this.Bell(percent, true);
        CheckError();
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args)
    {
        this.ChangeKeyboardControl(mask, args, true);
        CheckError();
    }

    public void ChangePointerControlChecked(Acceleration acceleration, ushort? threshold)
    {
        this.ChangePointerControl(acceleration, threshold, true);
        CheckError();
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        this.SetScreenSaver(timeout, interval, preferBlanking, allowExposures, true);
        CheckError();
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        this.ForceScreenSaver(mode, true);
        CheckError();
    }

    public void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address)
    {
        this.ChangeHosts(mode, family, address, true);
        CheckError();
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        this.SetAccessControl(mode, true);
        CheckError();
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        this.SetCloseDownMode(mode, true);
        CheckError();
    }

    public void KillClientChecked(uint resource)
    {
        this.KillClient(resource, true);
        CheckError();
    }

    public void NoOperationChecked(Span<uint> args)
    {
        this.NoOperation(args, true);
        CheckError();
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText8(drawable, gc, x, y, data, true);
        CheckError();
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText16(drawable, gc, x, y, data, true);
        CheckError();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing && socket.Connected)
            socket.Close();
        _disposedValue = true;
    }

    private void CheckError([CallerMemberName] string name = "")
    {
        var error = Received();
        if (error.HasValue) throw new XEventException(error.Value, name);
    }
}