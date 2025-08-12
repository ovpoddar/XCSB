using System.Runtime.InteropServices;
using System.Xml.Linq;
using Xcsb.Errors;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 31)]
public unsafe struct GenericError
{
    [FieldOffset(0)] public ErrorCode ErrorCode;
    [FieldOffset(1)] public RequestError RequestError;
    [FieldOffset(1)] public ValueError ValueError;
    [FieldOffset(1)] public WindowError WindowError;
    [FieldOffset(1)] public PixmapError PixmapError;
    [FieldOffset(1)] public AtomError AtomError;
    [FieldOffset(1)] public CursorError CursorError;
    [FieldOffset(1)] public FontError FontError;
    [FieldOffset(1)] public MatchError MatchError;
    [FieldOffset(1)] public DrawableError DrawableError;
    [FieldOffset(1)] public AccessError AccessError;
    [FieldOffset(1)] public AllocError AllocError;
    [FieldOffset(1)] public ColormapError ColormapError;
    [FieldOffset(1)] public GContextError GContextError;
    [FieldOffset(1)] public IDChoiceError IDChoiceError;
    [FieldOffset(1)] public NameError NameError;
    [FieldOffset(1)] public LengthError LengthError;
    [FieldOffset(1)] public ImplementationError ImplementationError;
}