using Xcsb.Connection.Models;

namespace Xcsb.Models.TypeInfo;

public sealed record ErrorCode : XEventType
{
    private ErrorCode(byte value, string original) : base(value, original) { }

    public static readonly ErrorCode Request = new ErrorCode(1, "Request");
    public static readonly ErrorCode Value = new ErrorCode(2, "Value");
    public static readonly ErrorCode Window = new ErrorCode(3, "Window");
    public static readonly ErrorCode Pixmap = new ErrorCode(4, "Pixmap");
    public static readonly ErrorCode Atom = new ErrorCode(5, "Atom");
    public static readonly ErrorCode Cursor = new ErrorCode(6, "Cursor");
    public static readonly ErrorCode Font = new ErrorCode(7, "Font");
    public static readonly ErrorCode Match = new ErrorCode(8, "Match");
    public static readonly ErrorCode Drawable = new ErrorCode(9, "Drawable");
    public static readonly ErrorCode Access = new ErrorCode(10, "Access");
    public static readonly ErrorCode Alloc = new ErrorCode(11, "Alloc");
    public static readonly ErrorCode Colormap = new ErrorCode(12, "Colormap");
    public static readonly ErrorCode GContext = new ErrorCode(13, "GContext");
    public static readonly ErrorCode IDChoice = new ErrorCode(14, "IDChoice");
    public static readonly ErrorCode Name = new ErrorCode(15, "Name");
    public static readonly ErrorCode Length = new ErrorCode(16, "Length");
    public static readonly ErrorCode Implementation = new ErrorCode(17, "Implementation");
}
