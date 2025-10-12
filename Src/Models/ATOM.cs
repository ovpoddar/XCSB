using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Xcsb.Models;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct ATOM : IEquatable<ATOM>, IComparable<ATOM>
{
    private readonly uint _value;
    public uint Value => _value;
    public ATOM(uint value) => _value = value;

    private ATOM(PredefinedAtom predefinedAtom) => _value = (uint)predefinedAtom;

    private string DebuggerDisplay => $"Value = {ToString()}";

    private enum PredefinedAtom : uint
    {
        Any = 0,
        Primary = 1,
        Secondary = 2,
        Arc = 3,
        Atom = 4,
        Bitmap = 5,
        Cardinal = 6,
        ColorMap = 7,
        Cursor = 8,
        CutBuffer0 = 9,
        CutBuffer1 = 10,
        CutBuffer2 = 11,
        CutBuffer3 = 12,
        CutBuffer4 = 13,
        CutBuffer5 = 14,
        CutBuffer6 = 15,
        CutBuffer7 = 16,
        Drawable = 17,
        Font = 18,
        Integer = 19,
        Pixmap = 20,
        Point = 21,
        Rectangle = 22,
        ResourceManager = 23,
        RGBColorMap = 24,
        RGBBestMap = 25,
        RGBBlueMap = 26,
        RGBDefaultMap = 27,
        RGBGrayMap = 28,
        RGBGreenMap = 29,
        RGBRedMap = 30,
        String = 31,
        VisualId = 32,
        Window = 33,
        WmCommand = 34,
        WmHints = 35,
        WmClientMachine = 36,
        WmIconName = 37,
        WmIconSize = 38,
        WmName = 39,
        WmNormalHints = 40,
        WmSizeHints = 41,
        WmZoomHints = 42,
        MinSpace = 43,
        NormSpace = 44,
        MaxSpace = 45,
        EndSpace = 46,
        SuperscriptX = 47,
        SuperscriptY = 48,
        SubscriptX = 49,
        SubscriptY = 50,
        UnderlinePosition = 51,
        UnderlineThickness = 52,
        StrikeoutAscent = 53,
        StrikeoutDescent = 54,
        ItalicAngle = 55,
        XHeight = 56,
        QuadWidth = 57,
        Weight = 58,
        PointSize = 59,
        Resolution = 60,
        Copyright = 61,
        Notice = 62,
        FontName = 63,
        FamilyName = 64,
        FullName = 65,
        CapHeight = 66,
        WmClass = 67,
        WmTransientFor = 68,
    }

    public static readonly ATOM Any = new(PredefinedAtom.Any);
    public static readonly ATOM Primary = new(PredefinedAtom.Primary);
    public static readonly ATOM Secondary = new(PredefinedAtom.Secondary);
    public static readonly ATOM Arc = new(PredefinedAtom.Arc);
    public static readonly ATOM Atom = new(PredefinedAtom.Atom);
    public static readonly ATOM Bitmap = new(PredefinedAtom.Bitmap);
    public static readonly ATOM Cardinal = new(PredefinedAtom.Cardinal);
    public static readonly ATOM ColorMap = new(PredefinedAtom.ColorMap);
    public static readonly ATOM Cursor = new(PredefinedAtom.Cursor);
    public static readonly ATOM CutBuffer0 = new(PredefinedAtom.CutBuffer0);
    public static readonly ATOM CutBuffer1 = new(PredefinedAtom.CutBuffer1);
    public static readonly ATOM CutBuffer2 = new(PredefinedAtom.CutBuffer2);
    public static readonly ATOM CutBuffer3 = new(PredefinedAtom.CutBuffer3);
    public static readonly ATOM CutBuffer4 = new(PredefinedAtom.CutBuffer4);
    public static readonly ATOM CutBuffer5 = new(PredefinedAtom.CutBuffer5);
    public static readonly ATOM CutBuffer6 = new(PredefinedAtom.CutBuffer6);
    public static readonly ATOM CutBuffer7 = new(PredefinedAtom.CutBuffer7);
    public static readonly ATOM Drawable = new(PredefinedAtom.Drawable);
    public static readonly ATOM Font = new(PredefinedAtom.Font);
    public static readonly ATOM Integer = new(PredefinedAtom.Integer);
    public static readonly ATOM Pixmap = new(PredefinedAtom.Pixmap);
    public static readonly ATOM Point = new(PredefinedAtom.Point);
    public static readonly ATOM Rectangle = new(PredefinedAtom.Rectangle);
    public static readonly ATOM ResourceManager = new(PredefinedAtom.ResourceManager);
    public static readonly ATOM RGBColorMap = new(PredefinedAtom.RGBColorMap);
    public static readonly ATOM RGBBestMap = new(PredefinedAtom.RGBBestMap);
    public static readonly ATOM RGBBlueMap = new(PredefinedAtom.RGBBlueMap);
    public static readonly ATOM RGBDefaultMap = new(PredefinedAtom.RGBDefaultMap);
    public static readonly ATOM RGBGrayMap = new(PredefinedAtom.RGBGrayMap);
    public static readonly ATOM RGBGreenMap = new(PredefinedAtom.RGBGreenMap);
    public static readonly ATOM RGBRedMap = new(PredefinedAtom.RGBRedMap);
    public static readonly ATOM String = new(PredefinedAtom.String);
    public static readonly ATOM VisualId = new(PredefinedAtom.VisualId);
    public static readonly ATOM Window = new(PredefinedAtom.Window);
    public static readonly ATOM WmCommand = new(PredefinedAtom.WmCommand);
    public static readonly ATOM WmHints = new(PredefinedAtom.WmHints);
    public static readonly ATOM WmClientMachine = new(PredefinedAtom.WmClientMachine);
    public static readonly ATOM WmIconName = new(PredefinedAtom.WmIconName);
    public static readonly ATOM WmIconSize = new(PredefinedAtom.WmIconSize);
    public static readonly ATOM WmName = new(PredefinedAtom.WmName);
    public static readonly ATOM WmNormalHints = new(PredefinedAtom.WmNormalHints);
    public static readonly ATOM WmSizeHints = new(PredefinedAtom.WmSizeHints);
    public static readonly ATOM WmZoomHints = new(PredefinedAtom.WmZoomHints);
    public static readonly ATOM MinSpace = new(PredefinedAtom.MinSpace);
    public static readonly ATOM NormSpace = new(PredefinedAtom.NormSpace);
    public static readonly ATOM MaxSpace = new(PredefinedAtom.MaxSpace);
    public static readonly ATOM EndSpace = new(PredefinedAtom.EndSpace);
    public static readonly ATOM SuperscriptX = new(PredefinedAtom.SuperscriptX);
    public static readonly ATOM SuperscriptY = new(PredefinedAtom.SuperscriptY);
    public static readonly ATOM SubscriptX = new(PredefinedAtom.SubscriptX);
    public static readonly ATOM SubscriptY = new(PredefinedAtom.SubscriptY);
    public static readonly ATOM UnderlinePosition = new(PredefinedAtom.UnderlinePosition);
    public static readonly ATOM UnderlineThickness = new(PredefinedAtom.UnderlineThickness);
    public static readonly ATOM StrikeoutAscent = new(PredefinedAtom.StrikeoutAscent);
    public static readonly ATOM StrikeoutDescent = new(PredefinedAtom.StrikeoutDescent);
    public static readonly ATOM ItalicAngle = new(PredefinedAtom.ItalicAngle);
    public static readonly ATOM XHeight = new(PredefinedAtom.XHeight);
    public static readonly ATOM QuadWidth = new(PredefinedAtom.QuadWidth);
    public static readonly ATOM Weight = new(PredefinedAtom.Weight);
    public static readonly ATOM PointSize = new(PredefinedAtom.PointSize);
    public static readonly ATOM Resolution = new(PredefinedAtom.Resolution);
    public static readonly ATOM Copyright = new(PredefinedAtom.Copyright);
    public static readonly ATOM Notice = new(PredefinedAtom.Notice);
    public static readonly ATOM FontName = new(PredefinedAtom.FontName);
    public static readonly ATOM FamilyName = new(PredefinedAtom.FamilyName);
    public static readonly ATOM FullName = new(PredefinedAtom.FullName);
    public static readonly ATOM CapHeight = new(PredefinedAtom.CapHeight);
    public static readonly ATOM WmClass = new(PredefinedAtom.WmClass);
    public static readonly ATOM WmTransientFor = new(PredefinedAtom.WmTransientFor);


    public static explicit operator uint(ATOM value) => value._value;
    public static explicit operator ATOM(uint value) => new(value);

    public bool Equals(ATOM other) =>
        _value == other._value;

    public override bool Equals(object? obj) =>
        obj is ATOM other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();

    public int CompareTo(ATOM other) =>
        _value.CompareTo(other._value);

    public bool IsValid =>
        _value != 0;
    public bool IsPredefined =>
        _value < Enum.GetValues(typeof(PredefinedAtom)).Length;

    public override string ToString() =>
        _value < Enum.GetValues(typeof(PredefinedAtom)).Length
            ? ((PredefinedAtom)_value).ToString()
            : _value.ToString();

}