using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct GenericError : IXError
{
    [FieldOffset(0)] public readonly ResponseHeader<ErrorCode> ResponseHeader;
    [FieldOffset(4)] public fixed byte Data[28];

    [FieldOffset(0)] private fixed byte _data[32];

    public readonly bool Verify(in int sequence)
    {
        if (this.ResponseHeader.Reply != ResponseType.Error)
            return false;

        var errorType = this.ResponseHeader.GetValue();

#if NETSTANDARD
        if (!Enum.IsDefined(typeof(ErrorCode), errorType))
#else
        if (!Enum.IsDefined(errorType))
#endif
            return false;

        fixed (byte* ptr = this._data)
        {
            return errorType switch
            {
                ErrorCode.Request => new Span<byte>(ptr, 32).AsStruct<RequestError>().Verify(in sequence),
                ErrorCode.Value => new Span<byte>(ptr, 32).AsStruct<ValueError>().Verify(in sequence),
                ErrorCode.Window => new Span<byte>(ptr, 32).AsStruct<WindowError>().Verify(in sequence),
                ErrorCode.Pixmap => new Span<byte>(ptr, 32).AsStruct<PixmapError>().Verify(in sequence),
                ErrorCode.Atom => new Span<byte>(ptr, 32).AsStruct<AtomError>().Verify(in sequence),
                ErrorCode.Cursor => new Span<byte>(ptr, 32).AsStruct<CursorError>().Verify(in sequence),
                ErrorCode.Font => new Span<byte>(ptr, 32).AsStruct<FontError>().Verify(in sequence),
                ErrorCode.Match => new Span<byte>(ptr, 32).AsStruct<MatchError>().Verify(in sequence),
                ErrorCode.Drawable => new Span<byte>(ptr, 32).AsStruct<DrawableError>().Verify(in sequence),
                ErrorCode.Access => new Span<byte>(ptr, 32).AsStruct<AccessError>().Verify(in sequence),
                ErrorCode.Alloc => new Span<byte>(ptr, 32).AsStruct<AllocError>().Verify(in sequence),
                ErrorCode.Colormap => new Span<byte>(ptr, 32).AsStruct<ColormapError>().Verify(in sequence),
                ErrorCode.GContext => new Span<byte>(ptr, 32).AsStruct<GContextError>().Verify(in sequence),
                ErrorCode.IDChoice => new Span<byte>(ptr, 32).AsStruct<IDChoiceError>().Verify(in sequence),
                ErrorCode.Name => new Span<byte>(ptr, 32).AsStruct<NameError>().Verify(in sequence),
                ErrorCode.Length => new Span<byte>(ptr, 32).AsStruct<LengthError>().Verify(in sequence),
                ErrorCode.Implementation => new Span<byte>(ptr, 32).AsStruct<ImplementationError>().Verify(in sequence),
                _ => false
            };
        }
    }

    public readonly ref T As<T>() where T : struct
    {
        var isNotValid = this.ResponseHeader.GetValue() switch
        {
            ErrorCode.Request when typeof(T) == typeof(RequestError) => false,
            ErrorCode.Value when typeof(T) == typeof(ValueError) => false,
            ErrorCode.Window when typeof(T) == typeof(WindowError) => false,
            ErrorCode.Pixmap when typeof(T) == typeof(PixmapError) => false,
            ErrorCode.Atom when typeof(T) == typeof(AtomError) => false,
            ErrorCode.Cursor when typeof(T) == typeof(CursorError) => false,
            ErrorCode.Font when typeof(T) == typeof(FontError) => false,
            ErrorCode.Match when typeof(T) == typeof(MatchError) => false,
            ErrorCode.Drawable when typeof(T) == typeof(DrawableError) => false,
            ErrorCode.Access when typeof(T) == typeof(AccessError) => false,
            ErrorCode.Alloc when typeof(T) == typeof(AllocError) => false,
            ErrorCode.Colormap when typeof(T) == typeof(ColormapError) => false,
            ErrorCode.GContext when typeof(T) == typeof(GContextError) => false,
            ErrorCode.IDChoice when typeof(T) == typeof(IDChoiceError) => false,
            ErrorCode.Name when typeof(T) == typeof(NameError) => false,
            ErrorCode.Length when typeof(T) == typeof(LengthError) => false,
            ErrorCode.Implementation when typeof(T) == typeof(ImplementationError) => false,
            _ when typeof(T) == typeof(GenericError) => false,
            _ => true
        };

        if (isNotValid)
            throw new InvalidCastException();

        fixed (byte* ptr = this._data)
            return ref new Span<byte>(ptr, 32).AsStruct<T>();
    }
}