using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Response.Replies.Internals;
using Xcsb.Response.Contract;
using Xcsb.Handlers;
using Xcsb.Response.Replies;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Requests;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Masks;
using Xcsb.Response.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Infrastructure.Response;
using System.Diagnostics;
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

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }
    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(this);

    internal Socket Socket;

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult) : base(socket)
    {
        Socket = socket;
        HandshakeSuccessResponseBody = connectionResult;
        _globalId = 0;
    }


    public AllocColorReply? AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<AllocColorReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);

        return result;
    }

    public AllocColorCellsReply? AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var request = new AllocColorCellsType(contiguous, colorMap, colors, planes);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<AllocColorCellsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new AllocColorCellsReply(result);
    }

    public AllocColorPlanesReply? AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues)
    {
        var request = new AllocColorPlanesType(contiguous, colorMap, colors, reds, greens, blues);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<AllocColorPlanesResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new AllocColorPlanesReply(result);
    }

    public AllocNamedColorReply? AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name)
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

        var (result, error) = ProtoIn.ReceivedResponse<AllocNamedColorReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }


    public GetAtomNameReply? GetAtomName(ATOM atom)
    {
        var request = new GetAtomNameType(atom);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetAtomNameResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetAtomNameReply(result);
    }

    public InternAtomReply? InternAtom(bool onlyIfExist, string atomName)
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

        var (result, error) = ProtoIn.ReceivedResponse<InternAtomReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetFontPathReply? GetFontPath()
    {
        var request = new GetFontPathType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetFontPathResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetFontPathReply(result);
    }

    public GetGeometryReply? GetGeometry(uint drawable)
    {
        var request = new GetGeometryType(drawable);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<GetGeometryReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetImageReply? GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask)
    {
        var request = new GetImageType(format, drawable, x, y, width, height, planeMask);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetImageResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetImageReply(result);
    }

    public GetInputFocusReply? GetInputFocus()
    {
        var request = new GetInputFocusType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<GetInputFocusReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetKeyboardControlReply? GetKeyboardControl()
    {
        var request = new GetKeyboardControlType();
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<GetKeyboardControlResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetKeyboardControlReply(result!.Value);
    }

    public GetKeyboardMappingReply? GetKeyboardMapping(byte firstKeycode, byte count)
    {
        var request = new GetKeyboardMappingType(firstKeycode, count);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetKeyboardMappingResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetKeyboardMappingReply(result, count);
    }

    public GetModifierMappingReply? GetModifierMapping()
    {
        var request = new GetModifierMappingType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetModifierMappingResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetModifierMappingReply(result);
    }

    public GetMotionEventsReply? GetMotionEvents(uint window, uint startTime, uint endTime)
    {
        var request = new GetMotionEventsType(window, startTime, endTime);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetMotionEventsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetMotionEventsReply(result);
    }

    public GetPointerControlReply? GetPointerControl()
    {
        var request = new GetPointerControlType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<GetPointerControlReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetPointerMappingReply? GetPointerMapping()
    {
        var request = new GetPointerMappingType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetPointerMappingResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetPointerMappingReply(result);
    }

    public GetPropertyReply? GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<GetPropertyResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new GetPropertyReply(result);
    }

    public GetScreenSaverReply? GetScreenSaver()
    {
        var request = new GetScreenSaverType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<GetScreenSaverReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetSelectionOwnerReply? GetSelectionOwner(ATOM atom)
    {
        var request = new GetSelectionOwnerType(atom);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<GetSelectionOwnerReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GetWindowAttributesReply? GetWindowAttributes(uint window)
    {
        var request = new GetWindowAttributesType(window);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<GetWindowAttributesReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public ListExtensionsReply? ListExtensions()
    {
        var request = new ListExtensionsType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<ListExtensionsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new ListExtensionsReply(result);
    }

    public ListFontsReply? ListFonts(ReadOnlySpan<byte> pattern, int maxNames)
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

        var (result, error) = ProtoIn.ReceivedResponseSpan<ListFontsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new ListFontsReply(result);
    }

    public ListFontsWithInfoReply[]? ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames)
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

        var (result, error) = ProtoIn.ReceivedResponseSpan<ListFontsWithInfoResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        throw new NotImplementedException();
        return result?.AsSpan().GetListFontsWithInfoReplies(maxNames, ProtoIn);
    }

    public ListHostsReply? ListHosts()
    {
        var request = new ListHostsType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<ListHostsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new ListHostsReply(result);
    }

    public ListInstalledColormapsReply? ListInstalledColormaps(uint window)
    {
        var request = new ListInstalledColormapsType(window);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<ListInstalledColormapsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new ListInstalledColormapsReply(result);
    }

    public ListPropertiesReply? ListProperties(uint window)
    {
        var request = new ListPropertiesType(window);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<ListPropertiesResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new ListPropertiesReply(result);
    }

    public LookupColorReply? LookupColor(uint colorMap, ReadOnlySpan<byte> name)
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

        var (result, error) = ProtoIn.ReceivedResponse<LookupColorReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public QueryBestSizeReply? QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var request = new QueryBestSizeType(shape, drawable, width, height);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<QueryBestSizeReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public QueryColorsReply? QueryColors(uint colorMap, Span<uint> pixels)
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

        var (result, error) = ProtoIn.ReceivedResponseSpan<QueryColorsResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new QueryColorsReply(result);
    }

    public QueryExtensionReply? QueryExtension(ReadOnlySpan<byte> name)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
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

        var (result, error) = ProtoIn.ReceivedResponse<QueryExtensionReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public QueryFontReply? QueryFont(uint fontId)
    {
        var request = new QueryFontType(fontId);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponseSpan<QueryFontResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new QueryFontReply(result);
    }

    public QueryKeymapReply? QueryKeymap()
    {
        var request = new QueryKeymapType();
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<QueryKeymapResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new QueryKeymapReply(result!.Value);
    }

    public QueryPointerReply? QueryPointer(uint window)
    {
        var request = new QueryPointerType(window);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<QueryPointerReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public QueryTextExtentsReply? QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery)
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

        var (result, error) = ProtoIn.ReceivedResponse<QueryTextExtentsReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public QueryTreeReply? QueryTree(uint window)
    {
        var request = new QueryTreeType(window);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponseSpan<QueryTreeResponse>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error!.Value);

        return result == null ? null : new QueryTreeReply(result);
    }


    public GrabKeyboardReply? GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyboardType(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<GrabKeyboardReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public GrabPointerReply? GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        ProtoOut.Send(ref request);

        var (result, error) = ProtoIn.ReceivedResponse<GrabPointerReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public SetModifierMappingReply? SetModifierMapping(Span<ulong> keycodes)
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

        var (result, error) = ProtoIn.ReceivedResponse<SetModifierMappingReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public SetPointerMappingReply? SetPointerMapping(Span<byte> maps)
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

        var (result, error) = ProtoIn.ReceivedResponse<SetPointerMappingReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }

    public TranslateCoordinatesReply? TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY)
    {
        var request = new TranslateCoordinatesType(srcWindow, destinationWindow, srcX, srcY);
        ProtoOut.Send(ref request);
        var (result, error) = ProtoIn.ReceivedResponse<TranslateCoordinatesReply>(ProtoOut.Sequence);
        if (error.HasValue)
            throw new XEventException(error.Value);
        return result;
    }


    public void WaitForEvent()
    {
        if (!IsEventAvailable())
            Socket.Poll(-1, SelectMode.SelectRead);
    }

    public uint NewId()
    {
        return (uint)((HandshakeSuccessResponseBody.ResourceIDMask & _globalId++) |
                      HandshakeSuccessResponseBody.ResourceIDBase);
    }

    public XEvent GetEvent() =>
        ProtoIn.ReceivedResponse();

    public bool IsEventAvailable() =>
        ProtoIn.bufferEvents.Any() || Socket.Available >= Unsafe.SizeOf<GenericEvent>();

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing && Socket.Connected)
            Socket.Close();
        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ResponseProto CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask,
            args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args)
    {
        ChangeWindowAttributesBase(window, mask, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto DestroyWindow(uint window)
    {
        DestroyWindowBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto DestroySubwindows(uint window)
    {
        DestroySubwindowsBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        ChangeSaveSetBase(changeSaveSetMode, window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ReparentWindow(uint window, uint parent, short x, short y)
    {
        ReparentWindowBase(window, parent, x, y);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto MapWindow(uint window)
    {
        MapWindowBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto MapSubwindows(uint window)
    {
        MapSubwindowsBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UnmapWindow(uint window)
    {
        UnmapWindowBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UnmapSubwindows(uint window)
    {
        UnmapSubwindowsBase(window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        ConfigureWindowBase(window, mask, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CirculateWindow(Circulate circulate, uint window)
    {
        CirculateWindowBase(circulate, window);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        ChangePropertyBase<T>(mode, window, property, type, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto DeleteProperty(uint window, ATOM atom)
    {
        DeletePropertyBase(window, atom);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto RotateProperties(uint window, ushort delta, Span<ATOM> properties)
    {
        RotatePropertiesBase(window, delta, properties);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetSelectionOwner(uint owner, ATOM atom, uint timestamp)
    {
        SetSelectionOwnerBase(owner, atom, timestamp);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        ConvertSelectionBase(requestor, selection, target, property, timestamp);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        SendEventBase(propagate, destination, eventMask, evnt);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UngrabPointer(uint time)
    {
        UngrabPointerBase(time);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UngrabButton(Button button, uint grabWindow, ModifierMask mask)
    {
        UngrabButtonBase(button, grabWindow, mask);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        ChangeActivePointerGrabBase(cursor, time, mask);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UngrabKeyboard(uint time)
    {
        UngrabKeyboardBase(time);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UngrabKey(byte key, uint grabWindow, ModifierMask modifier)
    {
        UngrabKeyBase(key, grabWindow, modifier);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto AllowEvents(EventsMode mode, uint time)
    {
        AllowEventsBase(mode, time);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto GrabServer()
    {
        GrabServerBase();
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UngrabServer()
    {
        UngrabServerBase();
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        SetInputFocusBase(mode, focus, time);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto OpenFont(string fontName, uint fontId)
    {
        OpenFontBase(fontName, fontId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CloseFont(uint fontId)
    {
        CloseFontBase(fontId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetFontPath(string[] strPaths)
    {
        SetFontPathBase(strPaths);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        CreatePixmapBase(depth, pixmapId, drawable, width, height);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FreePixmap(uint pixmapId)
    {
        FreePixmapBase(pixmapId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        CreateGCBase(gc, drawable, mask, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeGC(uint gc, GCMask mask, Span<uint> args)
    {
        ChangeGCBase(gc, mask, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        CopyGCBase(srcGc, dstGc, mask);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetDashes(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        SetDashesBase(gc, dashOffset, dashes);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FreeGC(uint gc)
    {
        FreeGCBase(gc);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        ClearAreaBase(exposures, window, x, y, width, height);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        PolyPointBase(coordinate, drawable, gc, points);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        PolyLineBase(coordinate, drawable, gc, points);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolySegment(uint drawable, uint gc, Span<Segment> segments)
    {
        PolySegmentBase(drawable, gc, segments);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        PolyRectangleBase(drawable, gc, rectangles);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyArc(uint drawable, uint gc, Span<Arc> arcs)
    {
        PolyArcBase(drawable, gc, arcs);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        FillPolyBase(drawable, gc, shape, coordinate, points);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        PolyFillRectangleBase(drawable, gc, rectangles);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyFillArc(uint drawable, uint gc, Span<Arc> arcs)
    {
        PolyFillArcBase(drawable, gc, arcs);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x, short y, byte leftPad, byte depth, Span<byte> data)
    {
        PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        ImageText8Base(drawable, gc, x, y, text);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        ImageText16Base(drawable, gc, x, y, text);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        CreateColormapBase(alloc, colormapId, window, visual);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FreeColormap(uint colormapId)
    {
        FreeColormapBase(colormapId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        CopyColormapAndFreeBase(colormapId, srcColormapId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto InstallColormap(uint colormapId)
    {
        InstallColormapBase(colormapId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto UninstallColormap(uint colormapId)
    {
        UninstallColormapBase(colormapId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FreeColors(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        FreeColorsBase(colormapId, planeMask, pixels);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto StoreColors(uint colormapId, Span<ColorItem> item)
    {
        StoreColorsBase(colormapId, item);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        StoreNamedColorBase(mode, colormapId, pixels, name);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto FreeCursor(uint cursorId)
    {
        FreeCursorBase(cursorId);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> Keysym)
    {
        ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, Keysym);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto Bell(sbyte percent)
    {
        BellBase(percent);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args)
    {
        ChangeKeyboardControlBase(mask, args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangePointerControl(Acceleration acceleration, ushort? threshold)
    {
        ChangePointerControlBase(acceleration, threshold);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ForceScreenSaver(ForceScreenSaverMode mode)
    {
        ForceScreenSaverBase(mode);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto ChangeHosts(HostMode mode, Family family, Span<byte> address)
    {
        ChangeHostsBase(mode, family, address);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetAccessControl(AccessControlMode mode)
    {
        SetAccessControlBase(mode);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto SetCloseDownMode(CloseDownMode mode)
    {
        SetCloseDownModeBase(mode);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto KillClient(uint resource)
    {
        KillClientBase(resource);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto NoOperation(Span<uint> args)
    {
        NoOperationBase(args);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        PolyText8Base(drawable, gc, x, y, data);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public ResponseProto PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        PolyText16Base(drawable, gc, x, y, data);
        return new ResponseProto(ProtoOut.Sequence);
    }

    public void CreateWindowUnchecked(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask,
            args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeWindowAttributesUnchecked(uint window, ValueMask mask, Span<uint> args)
    {
        this.ChangeWindowAttributesBase(window, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void DestroyWindowUnchecked(uint window)
    {
        this.DestroyWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void DestroySubwindowsUnchecked(uint window)
    {
        this.DestroySubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeSaveSetUnchecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        this.ChangeSaveSetBase(changeSaveSetMode, window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ReparentWindowUnchecked(uint window, uint parent, short x, short y)
    {
        this.ReparentWindowBase(window, parent, x, y);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void MapWindowUnchecked(uint window)
    {
        this.MapWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void MapSubwindowsUnchecked(uint window)
    {
        this.MapSubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UnmapWindowUnchecked(uint window)
    {
        this.UnmapWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UnmapSubwindowsUnchecked(uint window)
    {
        this.UnmapSubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ConfigureWindowUnchecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        this.ConfigureWindowBase(window, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CirculateWindowUnchecked(Circulate circulate, uint window)
    {
        this.CirculateWindowBase(circulate, window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangePropertyUnchecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        this.ChangePropertyBase(mode, window, property, type, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void DeletePropertyUnchecked(uint window, ATOM atom)
    {
        this.DeletePropertyBase(window, atom);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void RotatePropertiesUnchecked(uint window, ushort delta, Span<ATOM> properties)
    {
        this.RotatePropertiesBase(window, delta, properties);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetSelectionOwnerUnchecked(uint owner, ATOM atom, uint timestamp)
    {
        this.SetSelectionOwnerBase(owner, atom, timestamp);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ConvertSelectionUnchecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SendEventUnchecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        this.SendEventBase(propagate, destination, eventMask, evnt);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UngrabPointerUnchecked(uint time)
    {
        this.UngrabPointerBase(time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void GrabButtonUnchecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button,
            modifiers);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UngrabButtonUnchecked(Button button, uint grabWindow, ModifierMask mask)
    {
        this.UngrabButtonBase(button, grabWindow, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeActivePointerGrabUnchecked(uint cursor, uint time, ushort mask)
    {
        this.ChangeActivePointerGrabBase(cursor, time, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UngrabKeyboardUnchecked(uint time)
    {
        this.UngrabKeyboardBase(time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void GrabKeyUnchecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UngrabKeyUnchecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        this.UngrabKeyBase(key, grabWindow, modifier);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void AllowEventsUnchecked(EventsMode mode, uint time)
    {
        this.AllowEventsBase(mode, time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void GrabServerUnchecked()
    {
        this.GrabServerBase();
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UngrabServerUnchecked()
    {
        this.UngrabServerBase();
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void WarpPointerUnchecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetInputFocusUnchecked(InputFocusMode mode, uint focus, uint time)
    {
        this.SetInputFocusBase(mode, focus, time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void OpenFontUnchecked(string fontName, uint fontId)
    {
        this.OpenFontBase(fontName, fontId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CloseFontUnchecked(uint fontId)
    {
        this.CloseFontBase(fontId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetFontPathUnchecked(string[] strPaths)
    {
        this.SetFontPathBase(strPaths);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreatePixmapUnchecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FreePixmapUnchecked(uint pixmapId)
    {
        this.FreePixmapBase(pixmapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreateGCUnchecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        this.CreateGCBase(gc, drawable, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeGCUnchecked(uint gc, GCMask mask, Span<uint> args)
    {
        this.ChangeGCBase(gc, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CopyGCUnchecked(uint srcGc, uint dstGc, GCMask mask)
    {
        this.CopyGCBase(srcGc, dstGc, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetDashesUnchecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        this.SetDashesBase(gc, dashOffset, dashes);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetClipRectanglesUnchecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FreeGCUnchecked(uint gc)
    {
        this.FreeGCBase(gc);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ClearAreaUnchecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        this.ClearAreaBase(exposures, window, x, y, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CopyAreaUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CopyPlaneUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyPointUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyPointBase(coordinate, drawable, gc, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyLineUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyLineBase(coordinate, drawable, gc, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolySegmentUnchecked(uint drawable, uint gc, Span<Segment> segments)
    {
        this.PolySegmentBase(drawable, gc, segments);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyRectangleBase(drawable, gc, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyArcBase(drawable, gc, arcs);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FillPolyUnchecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        this.FillPolyBase(drawable, gc, shape, coordinate, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyFillRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyFillRectangleBase(drawable, gc, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyFillArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyFillArcBase(drawable, gc, arcs);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PutImageUnchecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ImageText8Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        this.ImageText8Base(drawable, gc, x, y, text);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ImageText16Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        this.ImageText16Base(drawable, gc, x, y, text);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreateColormapUnchecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        this.CreateColormapBase(alloc, colormapId, window, visual);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FreeColormapUnchecked(uint colormapId)
    {
        this.FreeColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CopyColormapAndFreeUnchecked(uint colormapId, uint srcColormapId)
    {
        this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void InstallColormapUnchecked(uint colormapId)
    {
        this.InstallColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void UninstallColormapUnchecked(uint colormapId)
    {
        this.UninstallColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FreeColorsUnchecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        this.FreeColorsBase(colormapId, planeMask, pixels);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void StoreColorsUnchecked(uint colormapId, Span<ColorItem> item)
    {
        this.StoreColorsBase(colormapId, item);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void StoreNamedColorUnchecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        this.StoreNamedColorBase(mode, colormapId, pixels, name);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreateCursorUnchecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreateGlyphCursorUnchecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void FreeCursorUnchecked(uint cursorId)
    {
        this.FreeCursorBase(cursorId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void RecolorCursorUnchecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeKeyboardMappingUnchecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void BellUnchecked(sbyte percent)
    {
        this.BellBase(percent);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeKeyboardControlUnchecked(KeyboardControlMask mask, Span<uint> args)
    {
        this.ChangeKeyboardControlBase(mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangePointerControlUnchecked(Acceleration acceleration, ushort? threshold)
    {
        this.ChangePointerControlBase(acceleration, threshold);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetScreenSaverUnchecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ForceScreenSaverUnchecked(ForceScreenSaverMode mode)
    {
        this.ForceScreenSaverBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void ChangeHostsUnchecked(HostMode mode, Family family, Span<byte> address)
    {
        this.ChangeHostsBase(mode, family, address);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetAccessControlUnchecked(AccessControlMode mode)
    {
        this.SetAccessControlBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void SetCloseDownModeUnchecked(CloseDownMode mode)
    {
        this.SetCloseDownModeBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void KillClientUnchecked(uint resource)
    {
        this.KillClientBase(resource);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void NoOperationUnchecked(Span<uint> args)
    {
        this.NoOperationBase(args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyText8Unchecked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText8Base(drawable, gc, x, y, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void PolyText16Unchecked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText16Base(drawable, gc, x, y, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, false);
    }

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask,
            args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args)
    {
        this.ChangeWindowAttributesBase(window, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void DestroyWindowChecked(uint window)
    {
        this.DestroyWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void DestroySubwindowsChecked(uint window)
    {
        this.DestroySubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        this.ChangeSaveSetBase(changeSaveSetMode, window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        this.ReparentWindowBase(window, parent, x, y);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void MapWindowChecked(uint window)
    {
        this.MapWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void MapSubwindowsChecked(uint window)
    {
        this.MapSubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UnmapWindowChecked(uint window)
    {
        this.UnmapWindowBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        this.UnmapSubwindowsBase(window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        this.ConfigureWindowBase(window, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CirculateWindowChecked(Circulate circulate, uint window)
    {
        this.CirculateWindowBase(circulate, window);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        this.ChangePropertyBase(mode, window, property, type, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void DeletePropertyChecked(uint window, ATOM atom)
    {
        this.DeletePropertyBase(window, atom);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties)
    {
        this.RotatePropertiesBase(window, delta, properties);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp)
    {
        this.SetSelectionOwnerBase(owner, atom, timestamp);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        this.SendEventBase(propagate, destination, eventMask, evnt);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UngrabPointerChecked(uint time)
    {
        this.UngrabPointerBase(time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button,
            modifiers);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        this.UngrabButtonBase(button, grabWindow, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        this.ChangeActivePointerGrabBase(cursor, time, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UngrabKeyboardChecked(uint time)
    {
        this.UngrabKeyboardBase(time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        this.UngrabKeyBase(key, grabWindow, modifier);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        this.AllowEventsBase(mode, time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void GrabServerChecked()
    {
        this.GrabServerBase();
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UngrabServerChecked()
    {
        this.UngrabServerBase();
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        this.SetInputFocusBase(mode, focus, time);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        this.OpenFontBase(fontName, fontId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CloseFontChecked(uint fontId)
    {
        this.CloseFontBase(fontId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        this.SetFontPathBase(strPaths);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        this.FreePixmapBase(pixmapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        this.CreateGCBase(gc, drawable, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args)
    {
        this.ChangeGCBase(gc, mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        this.CopyGCBase(srcGc, dstGc, mask);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        this.SetDashesBase(gc, dashOffset, dashes);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FreeGCChecked(uint gc)
    {
        this.FreeGCBase(gc);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        this.ClearAreaBase(exposures, window, x, y, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyPointBase(coordinate, drawable, gc, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        this.PolyLineBase(coordinate, drawable, gc, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments)
    {
        this.PolySegmentBase(drawable, gc, segments);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyRectangleBase(drawable, gc, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyArcBase(drawable, gc, arcs);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        this.FillPolyBase(drawable, gc, shape, coordinate, points);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        this.PolyFillRectangleBase(drawable, gc, rectangles);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        this.PolyFillArcBase(drawable, gc, arcs);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        this.ImageText8Base(drawable, gc, x, y, text);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        this.ImageText16Base(drawable, gc, x, y, text);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        this.CreateColormapBase(alloc, colormapId, window, visual);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FreeColormapChecked(uint colormapId)
    {
        this.FreeColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void InstallColormapChecked(uint colormapId)
    {
        this.InstallColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        this.UninstallColormapBase(colormapId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        this.FreeColorsBase(colormapId, planeMask, pixels);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void StoreColorsChecked(uint colormapId, Span<ColorItem> item)
    {
        this.StoreColorsBase(colormapId, item);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        this.StoreNamedColorBase(mode, colormapId, pixels, name);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void FreeCursorChecked(uint cursorId)
    {
        this.FreeCursorBase(cursorId);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void BellChecked(sbyte percent)
    {
        this.BellBase(percent);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args)
    {
        this.ChangeKeyboardControlBase(mask, args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangePointerControlChecked(Acceleration acceleration, ushort? threshold)
    {
        this.ChangePointerControlBase(acceleration, threshold);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        this.ForceScreenSaverBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address)
    {
        this.ChangeHostsBase(mode, family, address);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        this.SetAccessControlBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        this.SetCloseDownModeBase(mode);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void KillClientChecked(uint resource)
    {
        this.KillClientBase(resource);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void NoOperationChecked(Span<uint> args)
    {
        this.NoOperationBase(args);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText8Base(drawable, gc, x, y, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText16Base(drawable, gc, x, y, data);
        ProtoIn.SkipErrorForSequence(ProtoOut.Sequence, true);
    }
}