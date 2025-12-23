#:sdk Microsoft.NET.Sdk
#:property TreatWarningsAsErrors=true

using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;

// Global Set Up
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    return 0;
var compiler = GetCCompiler();
var monitorFile = GenerateMonitorFile(compiler);
using var fileStream = File.Open("./BufferVoidMethodsTest.Generated.cs", FileMode.OpenOrCreate);

// No Parameter Methods Set Up
IBuilder[] noParamMethod = [
    new MethodDetails1("NoParameter", "GrabServer", [""], [], false),
    new MethodDetails1("NoParameter", "UngrabServer", [""], [], false),
    new MethodDetails1("IndependentMethod", "Bell", ["0", "50", "90", "99", "100"], ["sbyte"], false),
    new MethodDetails1("IndependentMethod", "UngrabPointer", ["0", "10", "100", "1000", "10000", "100000", "1000000", "10000000","100000000", "1000000000", "4294967295"], ["uint"], false),
    new MethodDetails1("IndependentMethod", "UngrabKeyboard", ["0", "10", "100", "1000", "10000", "100000", "1000000", "10000000","100000000", "1000000000", "4294967295"], ["uint"], false),
    new MethodDetails1("IndependentMethod", "AllowEvents", ["0, 0", "1, 10", "2, 100", "3, 1000", "4, 10000", "5, 100000", "6, 1000000", "7, 10000000", "7, 100000000", "7, 1000000000", "7, 4294967295"], ["Xcsb.Models.EventsMode" ,"uint"], false),
    new MethodDetails1("IndependentMethod", "SetFontPath", [$"new string[] {{ \"fixed\" , \"{Environment.CurrentDirectory}\" }}", $"new string[] {{\"{Environment.CurrentDirectory}\", \"/usr/bin\"}}", "new string[] {\"build-ins\"}"], ["string[]"], true, STRType.XcbStr),
    new MethodDetails1("IndependentMethod", "SetCloseDownMode", ["0", "1", "2"], ["Xcsb.Models.CloseDownMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeKeyboardControl", ["7, new uint[] {80, 90, 1200}"], ["Xcsb.Masks.KeyboardControlMask", "uint[]"], false, STRType.XcbUint),
    new MethodDetails1("IndependentMethod", "SetScreenSaver", ["5, 10, 1, 1", "0, 0, 0, 0", "2, 2, 0, 0"], ["short", "short", "Xcsb.Models.TriState", "Xcsb.Models.TriState"], false),
    new MethodDetails1("IndependentMethod", "ForceScreenSaver", ["1", "0"], ["Xcsb.Models.ForceScreenSaverMode"], false),
    new MethodDetails1("IndependentMethod", "SetAccessControl", ["1", "0"], ["Xcsb.Models.AccessControlMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeHosts", ["0, 0, new byte[] {127, 0, 0, 1}" , "1, 4, new byte[] {127, 0, 0, 1}"], ["Xcsb.Models.HostMode", "Xcsb.Models.Family", "byte[]"], true, STRType.XcbByte),
    new MethodDetails2("DependentOnWindow", "DestroyWindow", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "DestroySubwindows", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "ChangeSaveSet", ["1, $0", "0, $0"], ["Xcsb.Models.ChangeSaveSetMode", "uint"], false),
    new MethodDetails2("DependentOnWindow", "MapWindow", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "MapSubwindows", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "UnmapWindow", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "UnmapSubwindows", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow", "CirculateWindow", ["0, $0", "1, $0" ], ["Xcsb.Models.Circulate", "uint"], false),
    new MethodDetails2("DependentOnWindow", "ConfigureWindow", ["$0, 1, new uint[] {100}", "$0, 2, new uint[] {100}", "$0, 4, new uint[] {100}", "$0, 8, new uint[] {100}", "$0, 16, new uint[] {0}", "$0, 64, new uint[] {0}", "$0, 111, new uint[] {100, 100, 500, 500, 0, 0}"], ["uint", "Xcsb.Masks.ConfigureValueMask", "uint[]"], false),
    new MethodDetails2("DependentOnWindow", "ChangeWindowAttributes", ["$0, 1, new uint[] {167772}", "$0, 2, new uint[] {16777215}", "$0, 4, new uint[] {167772}", "$0, 8, new uint[] {16777215}", "$0, 16, new uint[] {1}", "$0, 32, new uint[] {1}", "$0, 64, new uint[] {167772}", "$0, 128, new uint[] {167772}", "$0, 256, new uint[] {167772}", "$0, 512, new uint[] {0}", "$0, 1024, new uint[] {1}", "$0, 2048, new uint[] {32769}", "$0, 4096, new uint[] {1}", "$0, 8192, new uint[] {167772}", "$0, 16384, new uint[] {167772}"], ["uint", "Xcsb.Masks.ValueMask", "uint[]"], false),
    new MethodDetails2("DependentOnWindow", "GrabButton", ["false, $0, 12, 1, 0, 0, 0, 5, 32768", "true, $0, 12, 1, 0, 0, 0, 0, 1"], ["bool", "uint", "ushort", "Xcsb.Models.GrabMode", "Xcsb.Models.GrabMode", "uint", "uint", "Xcsb.Models.Button", "Xcsb.Masks.ModifierMask"], false),
    new MethodDetails2("DependentOnWindow", "UngrabButton", ["0, $0, 0", "5, $0, 32768"], ["Xcsb.Models.Button", "uint", "Xcsb.Masks.ModifierMask"], false),
    new MethodDetails2("DependentOnWindow", "GrabKey", ["false, $0, 1, 0, 0, 1", "true, $0, 32768, 255, 0, 1"], ["bool", "uint", "Xcsb.Masks.ModifierMask", "byte", "Xcsb.Models.GrabMode", "Xcsb.Models.GrabMode"], false),
    new MethodDetails2("DependentOnWindow", "UngrabKey", ["0, $0, 0", "255, $0, 32768"], ["byte", "uint", "Xcsb.Masks.ModifierMask"], false),
    new MethodDetails2("DependentOnWindow", "SetInputFocus", ["0, $0, 0", "2, $0, 0"], ["Xcsb.Models.InputFocusMode", "uint", "uint"], false),
    new MethodDetails2("DependentOnWindow", "KillClient", ["$0"], ["uint"], false),
    new MethodDetails2("DependentOnWindow",    "RotateProperties", ["$0, 1, new uint[] {30, 1, 37}"], ["uint", "ushort", "Span<ATOM>"], false, STRType.XcbByte),
    new MethodDetails2Dynamic("DependentOnfontId", "CloseFont", ["$0"], ["uint"], false, MethodDetails2Dynamic.DynamicType.FontId),
    new MethodDetails2Dynamic("DependentOnpixmapId", "FreePixmap", ["$0"], ["uint"], false, MethodDetails2Dynamic.DynamicType.PixmapId),
    new MethodDetails2Dynamic("DependentOngc", "FreeGc", ["$0"], ["uint"], false, MethodDetails2Dynamic.DynamicType.Gc),
    new MethodDetails2Dynamic("DependentOncursorId", "FreeCursor", ["$0"], ["uint"], false, MethodDetails2Dynamic.DynamicType.CursorId),
    new MethodDetails3("SpecialMethod", "NoOperation"),
    new MethodDetails4("DependentOnPixmapRootDepthRoot", "CreatePixmap", ["$0, $1, $2, 65535, 65535", "$0, $1, $2, 0, 65535"] , ["byte", "uint", "uint", "ushort", "ushort"]),
    new MethodDetails5("DependentOnWindowId", "CreateGc", ["$0, $1, 1, new uint[] {6}", "$0, $1, 4194304, new uint[] {1}"], ["uint", "uint", "Xcsb.Masks.GCMask", "uint[]"]),
    new MethodDetails6("DependentOnWindowId", "CreateColormap", ["0, $0, $1, $2", "1, $0, $1, $2"], ["Xcsb.Models.ColormapAlloc", "uint", "uint", "uint"]),
    new MethodDetails7("DependentOnColorMap", "FreeColormap", ["$0"], ["uint"]),
    new MethodDetails7("DependentOnColorMap", "InstallColormap", ["$0"], ["uint"]),
    new MethodDetails7("DependentOnColorMap", "UninstallColormap", ["$0"], ["uint"]),
    new MethodDetails8("DependentOnDrawableGc", "PolyText8", ["$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"xcb\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"cb\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"x\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\"}"], ["uint", "uint", "ushort", "ushort", "string[]"], true, STRType.Xcb8, "Xcsb.Models.String.TextItem8"),
    new MethodDetails8("DependentOnDrawableGc", "PolyText16", ["$0, $1, 0, 0, new string[] { \"Hellow\", \"World\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"cb\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"x\" }", "$0, $1, 0, 0, new string[] { \"Hellow\", \"world\"}"], ["uint", "uint", "ushort", "ushort", "string[]" ], true, STRType.Xcb16, "Xcsb.Models.String.TextItem16"),
    new MethodDetails8("DependentOnDrawableGc", "ImageText8", [$"$0, $1, 0, 0, \"XCB System Control Demo\"", "$0, $1, 0, 0, \"XCB System Control Dem\"", "$0, $1, 0, 0, \"XCB System Control De\"", "$0, $1, 0, 0, \"XCB System Control D\"", "$0, $1, 0, 0, \"XCB System Control \""], ["uint", "uint", "short", "short", "string"], false, STRType.XcbStr8, ""),
    new MethodDetails8("DependentOnDrawableGc", "ImageText16", ["$0, $1, 0, 0, \"XCB System Control Demo\"", "$0, $1, 0, 0, \"XCB System Control Dem\"", "$0, $1, 0, 0, \"XCB System Control De\"", "$0, $1, 0, 0, \"XCB System Control D\"", "$0, $1, 0, 0, \"XCB System Control \""], ["uint", "uint", "short", "short", "string"], false, STRType.XcbStr16),
    new MethodDetails8("DependentOnDrawableGc", "PolySegment", ["$0, $1, [{ \"X1\" = 8, \"Y1\" = 0, \"X2\" = 8, \"Y2\" = 15 }, { \"X1\" = 0, \"Y1\" = 8, \"X2\" = 15, \"Y2\" = 8 } ]"], ["uint", "uint", "Xcsb.Models.Segment[]"], true, STRType.XcbSegment, ""),
    new MethodDetails8("DependentOnDrawableGc", "PolyRectangle", ["$0, $1, [{ \"X\" = 50, \"Y\" = 50, \"Width\" = 100, \"Height\" = 80 }, { \"X\" = 200, \"Y\" = 100, \"Width\" = 120, \"Height\" = 60 }, { \"X\" = 100, \"Y\" = 180, \"Width\" = 80, \"Height\" = 90 }]"], ["uint", "uint", "Xcsb.Models.Rectangle[]"], true, STRType.XcbRectangle, ""),
    new MethodDetails8("DependentOnDrawableGc", "PolyArc", ["$0, $1, [{ \"X\" = 20, \"Y\" = 200, \"Width\" = 40, \"Height\" = 40, \"Angle1\" = 0, \"Angle2\" = 360 }, { \"X\" = 100, \"Y\" = 200, \"Width\" = 30, \"Height\" = 30, \"Angle1\" = 0, \"Angle2\" = 180 }, { \"X\" = 180, \"Y\" = 200, \"Width\" = 35, \"Height\" = 25, \"Angle1\" = 45, \"Angle2\" = 90  }]"], ["uint", "uint", "Xcsb.Models.Arc[]"], true, STRType.XcbArc, ""),
    new MethodDetails8("DependentOnDrawableGc", "FillPoly", ["$0, $1, 0, 0, [{ \"X\" = 120, \"Y\" = 130 }, { \"X\" = 80, \"Y\" = 180 }, { \"X\" = 160, \"Y\" = 180 }]", "$0, $1, 2, 1, [{ \"X\" = 120, \"Y\" = 130 }, { \"X\" = 80, \"Y\" = 180 }, { \"X\" = 160, \"Y\" = 180 }]"], ["uint", "uint", "Xcsb.Models.PolyShape", "Xcsb.Models.CoordinateMode", "Xcsb.Models.Point[]"], true, STRType.XcbPoient, ""),
    new MethodDetails8("DependentOnDrawableGc", "PolyFillRectangle", ["$0, $1,  [{ \"X\" = 0, \"Y\" = 0, \"Width\" = 500, \"Height\" = 500 }]"], ["uint", "uint", "Xcsb.Models.Rectangle[]"], true, STRType.XcbRectangle, ""),
    new MethodDetails8("DependentOnDrawableGc", "PolyFillArc", ["$0, $1, [{ \"X\" = 20, \"Y\" = 200, \"Width\" = 40, \"Height\" = 40, \"Angle1\" = 0, \"Angle2\" = 360 }, { \"X\" = 100, \"Y\" = 200, \"Width\" = 30, \"Height\" = 30, \"Angle1\" = 0, \"Angle2\" = 180 }, { \"X\" = 180, \"Y\" = 200, \"Width\" = 35, \"Height\" = 25, \"Angle1\" = 45, \"Angle2\" = 90  }]"], ["uint", "uint", "Xcsb.Models.Arc[]"], true, STRType.XcbArc, "")
];
// ChangeProperty                    (PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
// ChangeGC                          (uint gc, GCMask mask, Span<uint> args)
// SetDashes                         (uint gc, ushort dashOffset, Span<byte> dashes)
// SetClipRectangles                 (ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles)
// PolyPoint                         (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
// PolyLine                          (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
// PutImage                          (ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y, byte leftPad, byte depth, Span<byte> data)
// FreeColors                        (uint colormapId, uint planeMask, Span<uint> pixels)
// StoreColors                       (uint colormapId, Span<ColorItem> item)
// StoreNamedColor                   (ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
// ChangeKeyboardMapping             (byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym)
// CreateWindow                      (byte depth, uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)


// CreateCursor                      (uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
// CreateGlyphCursor                 (uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)

// ReparentWindow                    (uint window, uint parent, short x, short y)
// DeleteProperty                    (uint window, ATOM atom)
// SetSelectionOwner                 (uint owner, ATOM atom, uint timestamp)
// ConvertSelection                  (uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
// SendEvent                         (bool propagate, uint destination, uint eventMask, XEvent evnt)
// ChangeActivePointerGrab           (uint cursor, uint time, ushort mask)
// WarpPointer                       (uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destinationX, short destinationY)
// OpenFont                          (string fontName, uint fontId)
// CopyGC                            (uint srcGc, uint dstGc, GCMask mask)
// ClearArea                         (bool exposures, uint window, short x, short y, ushort width, ushort height)
// CopyArea                          (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height)
// CopyPlane                         (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
// CopyColormapAndFree               (uint colormapId, uint srcColormapId)
// RecolorCursor                     (uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)


// new("IndependentMethod", "ChangePointerControl", ["new Xcsb.Models.Acceleration(1, 1), 4"], ["Xcsb.Models.Acceleration", "ushort"], false), // special case when the params being different


fileStream.Write(
"""
// DO NOT MODIFY THIS FILE
// IT WILL BE OVERWRITTEN WHEN GENERATING
#nullable enable
using Xcsb;

namespace MethodRequestBuilder.Test.Generated;

public class VoidMethodsTest : IDisposable
{
    private readonly IXProto _xProto;
    public VoidMethodsTest()
    {
        _xProto = XcsbClient.Initialized();
    }

"""u8);
{
    foreach (var method in noParamMethod)
        method.WriteCsMethodContent(fileStream, compiler, monitorFile);
}
fileStream.Write(
"""

    public void Dispose() => 
        _xProto.Dispose();
}
#nullable restore

"""u8);
File.Delete(monitorFile);
return 0;

static string GenerateMonitorFile(string compiler)
{
    var result = Path.Join(Environment.CurrentDirectory, "monitorFile.so");
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = compiler,
            Arguments = $"-shared -fPIC -o \"{result}\" -ldl -x c -",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    process.StandardInput.Write(
$$"""
#define _GNU_SOURCE
#include <dlfcn.h>
#include <stdio.h>
#include <sys/uio.h>
#include <sys/socket.h>
#include <unistd.h>

ssize_t (*real_write)(int, const void *, size_t) = NULL;
ssize_t (*real_writev)(int, const struct iovec *, int) = NULL;
ssize_t (*real_sendmsg)(int, const struct msghdr *, int) = NULL;

__attribute__((constructor))
void init() {
    real_write = dlsym(RTLD_NEXT, "write");
    real_writev = dlsym(RTLD_NEXT, "writev");
    real_sendmsg = dlsym(RTLD_NEXT, "sendmsg");
}

static void hex_dump(const void *buf, size_t len) {
    const unsigned char *p = buf;
    for (size_t i = 0; i < len; ++i) {
        fprintf(stderr, " %d,", p[i]);
    }
    fprintf(stderr, "\n");
}

/****************************************************************/

ssize_t write(int fd, const void *buf, size_t count) {
    hex_dump(buf, count);
    return real_write(fd, buf, count);
}

ssize_t writev(int fd, const struct iovec *iov, int iovcnt) {
    for (int i = 0; i < iovcnt; ++i) {
        hex_dump(iov[i].iov_base, iov[i].iov_len);
    }
    return real_writev(fd, iov, iovcnt);
}

ssize_t sendmsg(int fd, const struct msghdr *msg, int flags) {
    for (int i = 0; i < msg->msg_iovlen; ++i) {
        hex_dump(msg->msg_iov[i].iov_base, msg->msg_iov[i].iov_len);
    }
    return real_sendmsg(fd, msg, flags);
}
                    
""");
    process.StandardInput.Close();
    process.WaitForExit();

    Debug.Assert(string.IsNullOrWhiteSpace(process.StandardError.ReadToEnd()));
    Debug.Assert(string.IsNullOrWhiteSpace(process.StandardOutput.ReadToEnd()));
    Debug.Assert(File.Exists(result));
    return result;
}

static string GetCCompiler()
{
    string[] compilerCommands = ["gcc", "clang"];
    foreach (var command in compilerCommands)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = command == "cl" ? "" : "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0
            && !output.Contains("version", StringComparison.OrdinalIgnoreCase))
            continue;
        return command;
    }

    throw new Exception("Could not find any compiler to build c project");
}

file static class StringHelper
{

    public static int CalculateSize(this ReadOnlySpan<char> parameter)
    {
        var toCount = false;
        var result = 0;
        foreach (var item in parameter)
        {
            if (item == '"')
            {
                toCount = !toCount;
                continue;
            }
            if (toCount)
                result++;

        }
        return result;
    }

    public static int CalculateLen(ReadOnlySpan<char> content, STRType isXcbStr)
    {
        var result = 0;
        foreach (var item in content)
        {
            if (isXcbStr == STRType.XcbStr || isXcbStr == STRType.Xcb8 || isXcbStr == STRType.Xcb16)
            {
                if (item == '"') result++;
                continue;
            }
            if (isXcbStr is STRType.XcbSegment or STRType.XcbRectangle or STRType.XcbArc or STRType.XcbPoient)
            {
                if (item == '}')
                    result++;

                continue;
            }
            if (isXcbStr == STRType.XcbByte)
            {
                if (item == ',')
                    result++;

                continue;
            }
            throw new NotImplementedException(isXcbStr.ToString());
        }
        switch (isXcbStr)
        {
            case STRType.Xcb16:
                return result + content.CalculateSize() * 2;
            case STRType.Xcb8:
                return result + content.CalculateSize();
            case STRType.XcbStr:
                return result / 2;
            case STRType.XcbByte:
                return ++result;
            default:
                return result;
        }
    }

    private static void WriteCArray(StringBuilder sb, ReadOnlySpan<char> data, STRType type)
    {
        if (type == STRType.XcbStr)
        {
            var isStart = true;
            foreach (var item in data)
            {
                switch (item)
                {
                    case '}':
                        sb.Append(')');
                        break;
                    case '{':
                        sb.Append('(');
                        break;
                    case '"':
                        {
                            if (isStart) sb.Append("XS(\"");
                            else sb.Append("\")");
                            isStart = !isStart;
                            break;
                        }
                    default:
                        sb.Append(item);
                        break;
                }
            }
        }
        else if (type == STRType.Xcb16 || type == STRType.Xcb8)
        {
            sb.Append("XS");
            foreach (var item in data)
            {
                switch (item)
                {
                    case '{':
                        sb.Append('(');
                        break;
                    case '}':
                        sb.Append(')');
                        break;
                    default:
                        sb.Append(item);
                        break;
                }
            }
        }
        else if (type == STRType.XcbStr16)
        {
            var isStart = true;
            foreach (var item in data)
            {
                switch (item)
                {
                    case '"':
                        {
                            if (isStart) sb.Append("XS(\"");
                            else sb.Append("\")");
                            isStart = !isStart;
                            break;
                        }
                    default:
                        sb.Append(item);
                        break;
                }
            }
        }
        else if (type == STRType.XcbUint || type == STRType.XcbByte || type == STRType.XcbStr8)
        {
            sb.Append(data);
        }
        else
        {
            throw new NotImplementedException(data.ToString() + type.ToString());
        }
    }

    public static int GetCsField(ReadOnlySpan<char> content, out ReadOnlySpan<char> field)
    {
        for (var i = 0; i < content.Length; i++)
        {
            if (content[i] == 'n')
            {
                // new path
                if (i + 3 > content.Length) continue; // has enough items
                if (content[i + 1] != 'e' || content[i + 2] != 'w' || content[i + 3] != ' ') continue;
                // its new 
                var endIndex = content.IndexOf('}');
                if (endIndex == -1) throw new InvalidDataException();
                field = content[..++endIndex];
                return endIndex;
            }
            else if (content[i] == '[')
            {
                // object path []
                var endIndex = content.IndexOf(']');
                if (endIndex == -1) throw new InvalidDataException();
                field = content[i..++endIndex];
                return endIndex;
            }
            else if (content[i] == ',')
            {
                // , path
                field = content[..i];
                return ++i;
            }
            else
            {
                continue;
            }
        }

        // last section
        field = content[..];
        return content.Length;
    }

    public static ReadOnlySpan<char> GetLastItem(this ReadOnlySpan<char> content)
    {
        ReadOnlySpan<char> result;
        while (true)
        {
            var context = GetCsField(content, out result);
            content = content[context..];
            if (content.Length == 0) break;
        }

        return result;
    }

    public static string ToCParams(this string? parameter, STRType lastElementType, bool addLen, params string[] items)
    {
        var sb = new StringBuilder();
        sb.Append("connection");
        if (string.IsNullOrWhiteSpace(parameter))
            return sb.ToString();

        var content = parameter.AsSpan();
        while (true)
        {
            var context = GetCsField(content, out var item);
            content = content[context..];
            sb.Append(", ");
            var field = item.Trim();
            var targetIndex = field.IndexOf('$');
            if (targetIndex != -1)
            {
                var index = int.Parse(field[++targetIndex..]);
                sb.Append(items[index]);
                if (content.Length == 0) break;
                else continue;
            }
            targetIndex = field.IndexOf("new ");
            if (targetIndex != -1 || field.StartsWith('"'))
            {
                targetIndex = field.IndexOf('{');
                if (targetIndex == -1)
                {
                    switch (lastElementType)
                    {
                        case STRType.XcbStr16:
                        case STRType.XcbStr8:
                            break;
                        default:
                            throw new InvalidFilterCriteriaException();
                    }
                }
                else
                {
                    field = field[targetIndex..];
                }

                if (addLen) sb.Append(CalculateLen(field, lastElementType)).Append(", ");
                sb.Append('(')
                    .Append(GetCType(lastElementType))
                    .Append(')');
                WriteCArray(sb, field, lastElementType);
                break;
            }

            targetIndex = field.IndexOf('[');
            if (targetIndex != -1)
            {
                var endObjectIndex = field.IndexOf(']');
                if (endObjectIndex == -1) throw new InvalidFilterCriteriaException();
                if (addLen) sb.Append(CalculateLen(field[++targetIndex..endObjectIndex], lastElementType)).Append(", ");
                sb.Append(GetCType(lastElementType)).Append('{');
                WriteObject(sb, field[targetIndex..endObjectIndex]);
                sb.Append('}');
                break;
            }
            if (bool.TryParse(field, out var boolValue))
            {
                sb.Append(boolValue ? "1 " : "0 ");
                continue;
            }

            sb.Append(field);
            if (content.Length == 0) break;
        }
        return sb.ToString();
    }

    private static void WriteObject(StringBuilder sb, ReadOnlySpan<char> data)
    {
        var isStartCort = false;
        foreach (var item in data)
        {
            if (item == '"')
            {
                isStartCort = !isStartCort;
                if (isStartCort)
                    sb.Append('.');
                continue;
            }
            sb.Append(char.ToLower(item));
        }
    }

    public static string ReplaceOnece(this string value, char target, string replaced, int targetIndex = 0)
    {
        var sb = new StringBuilder();
        var foundIndex = -1;
        for (int i = 0; i < value.Length; i++)
        {
            char item = value[i];
            if (item == target)
            {
                foundIndex++;
                if (targetIndex == foundIndex)
                {
                    sb.Append(replaced);
                    continue;
                }
            }

            sb.Append(item);
        }

        return sb.ToString();
    }

    public static string GetCType(STRType isXcbStr) => isXcbStr switch
    {
        STRType.XcbStr8 or STRType.RawBuffer => "const char *",
        STRType.XcbByte => "uint8_t[]",
        STRType.XcbInt => "int32_t[]",
        STRType.XcbUint => "uint32_t[]",
        STRType.XcbStr => "xcb_str_t *",
        STRType.Xcb8 or STRType.Xcb16 => "uint8_t *",
        STRType.XcbStr16 => "xcb_char2b_t *",
        STRType.XcbSegment => "(xcb_segment_t[])",
        STRType.XcbRectangle => "(xcb_rectangle_t[])",
        STRType.XcbArc => "(xcb_arc_t[])",
        STRType.XcbPoient => "(xcb_point_t[])",
        _ => throw new Exception(isXcbStr.ToString()),
    };

    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        var sb = new StringBuilder();
        var foundNumber = false;
        for (var i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            if (char.IsUpper(chr) && i != 0 || char.IsNumber(chr) && !foundNumber)
                sb.Append('_');
            if (char.IsNumber(chr))
                foundNumber = true;

            sb.Append((char)(chr | 32));
        }

        return sb.ToString();
    }

    public static string Fix(this string name)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i == name.Length - 1)
                sb.Append(char.ToUpper(name[i]));
            else
                sb.Append(name[i]);
        }
        return sb.ToString();
    }
}

file class MethodDetails1 : StaticBuilder
{
    public MethodDetails1(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr = STRType.RawBuffer) : base(categories, methodName, parameters,
        paramSignature, addLenInCCall, isXcbStr)
    { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall);
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <stdint.h>
{{GetCStringMacro()}}

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }
}

file class MethodDetails2 : StaticBuilder
{
    public MethodDetails2(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType strType = STRType.XcbUint) : base(categories, methodName, parameters, paramSignature,
        addLenInCCall, strType)
    { }

    public virtual string WriteUpValueOfCSetup(out string name)
    {
        name = "window";
        return
$$"""
    xcb_window_t {{name}} = xcb_generate_id(connection);
    xcb_create_window(connection, 0, {{name}}, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
""";
    }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        var workingTypes = WriteUpValueOfCSetup(out var type);
        var lastParameter = parameter.GetLastItem();
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, type);
        if (base.IsXcbStr == STRType.XcbByte)
            parameter = parameter.ReplaceOnece(',', $", {StringHelper.CalculateLen(lastParameter, IsXcbStr)}, ", 1);
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h> 

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    const xcb_setup_t *setup = xcb_get_setup(connection);
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    
{{workingTypes}}

    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", {{type}});
    fprintf(stderr, "{{marker}}\n");
    
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = xcb_{{method.ToSnakeCase()}}_checked({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }


    private static string GetPlaceHolderOfWindow(string parameter)
    {
        if (!parameter.Contains('$'))
            throw new UnreachableException();
        var items = parameter.Split(',');
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Trim().Contains('$'))
                return "params" + i;
        }
        throw new UnreachableException();
    }


    public virtual string WriteUpValueOfCsSetup(out string name)
    {
        name = "window";
        return
$$"""
        var {{name}} = _xProto.NewId();
        var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
        _xProto.CreateWindowChecked(0, {{name}}, screen.Root, 0, 0, 100, 100, 0, Xcsb.Models.ClassType.InputOutput,
                    screen.RootVisualId, Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask, [0, (uint)(Xcsb.Masks.EventMask.ExposureMask)]);
""";
    }

    public virtual string GetMethodNameUpdated(string name) => name;

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        var hasWindowPlaceHolder = GetPlaceHolderOfWindow(Parameters[0]);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;
{{WriteUpValueOfCsSetup(out var typeName)}}

        // act
        bufferClient.{{GetMethodNameUpdated(MethodName)}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.Equal({{typeName}}, {{hasWindowPlaceHolder}});
        Assert.NotNull(buffer);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }
}

file class MethodDetails2Dynamic : MethodDetails2
{
    private readonly DynamicType _type;
    public MethodDetails2Dynamic(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, DynamicType type) : base(categories, methodName, parameters, paramSignature, addLenInCCall)
    {
        _type = type;
    }

    public override string GetMethodNameUpdated(string name)
    {
        if (base.MethodName == "FreeGc")
            return base.GetMethodNameUpdated(name).Fix();
        return base.GetMethodNameUpdated(name);
    }

    public override string WriteUpValueOfCsSetup(out string name)
    {
        name = _type.ToString().ToLower();
        return _type switch
        {
            DynamicType.Gc =>
$"""
    var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
    var window = _xProto.NewId();
    _xProto.CreateWindowChecked(0, window, screen.Root, 0, 0, 100, 100, 0, Xcsb.Models.ClassType.InputOutput,
                    screen.RootVisualId, Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask, [0, (uint)(Xcsb.Masks.EventMask.ExposureMask)]);
    var {name} = _xProto.NewId();
    _xProto.CreateGCChecked({name}, window, (Xcsb.Masks.GCMask)(4|8), [screen.BlackPixel, screen.WhitePixel]);
""",
            DynamicType.FontId =>
$"""
    var {name} = _xProto.NewId();
    _xProto.OpenFontChecked("fixed", {name});

""",
            DynamicType.PixmapId =>
$"""
    var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
    var {name} = _xProto.NewId();
    _xProto.CreatePixmapChecked(screen.RootDepth!.DepthValue, {name}, screen.Root, 1, 1);
""",
            DynamicType.CursorId =>
$"""
    var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
    
    var window = _xProto.NewId();
    _xProto.CreateWindowChecked(0, window, screen.Root, 0, 0, 100, 100, 0, Xcsb.Models.ClassType.InputOutput,
                           screen.RootVisualId, Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask, [0, (uint)(Xcsb.Masks.EventMask.ExposureMask)]);
    var src = _xProto.NewId();
    _xProto.CreatePixmapChecked(1, src, window, 8, 8);
    var mask = _xProto.NewId();
    _xProto.CreatePixmapChecked(1, mask, window, 8, 8);
     var {name} = _xProto.NewId();
    _xProto.CreateCursorChecked(
        {name}, src, mask, 0, 0, 0, 65535, 65535, 65535, 0, 0);
""",
            _ => throw new NullReferenceException()
        };
    }

    public override string WriteUpValueOfCSetup(out string name)
    {
        name = _type.ToString().ToLower();
        return _type switch
        {
            DynamicType.Gc =>
$$"""

    xcb_window_t window = xcb_generate_id(connection);
    xcb_void_cookie_t c = xcb_create_window_checked(connection, 0, window, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                                                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    xcb_generic_error_t *e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }

    xcb_gcontext_t {{name}} = xcb_generate_id(connection);
    c = xcb_create_gc_checked(connection, {{name}}, window, 4|8, (uint32_t[]){screen->black_pixel, screen->white_pixel});
    e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }
""",
            DynamicType.CursorId =>
$$"""
    xcb_window_t window = xcb_generate_id(connection);
    xcb_void_cookie_t c = xcb_create_window_checked(connection, 0, window, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                                                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    xcb_generic_error_t *e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }

    xcb_pixmap_t src = xcb_generate_id(connection);
    c = xcb_create_pixmap_checked(connection, 1, src, window, 8, 8);
    e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }

    xcb_pixmap_t mask = xcb_generate_id(connection);
    c = xcb_create_pixmap_checked(connection, 1, mask, window, 8, 8);
    e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }

    xcb_cursor_t {{name}} = xcb_generate_id(connection);
    c = xcb_create_cursor_checked(
        connection,
        {{name}},
        src,
        mask,
        0, 0, 0,                 
        65535, 65535, 65535,     
        0, 0                     
    );
    e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }
    
""",
            DynamicType.FontId =>
$$"""
    xcb_font_t {{name}} = xcb_generate_id(connection);
    xcb_void_cookie_t c = xcb_open_font_checked(connection, {{name}}, 5, "fixed");
    xcb_generic_error_t *e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }
""",
            DynamicType.PixmapId =>
$$"""
    xcb_pixmap_t {{name}} = xcb_generate_id(connection);
    uint8_t rootDepth = screen->root_depth;
    xcb_window_t root = screen->root;
    xcb_void_cookie_t c = xcb_create_pixmap_checked(connection, rootDepth, {{name}}, root, 1, 1);
    xcb_generic_error_t *e = xcb_request_check(connection, c);
    if (e) 
    {
        free(e);
        return 1;
    }
""",
            _ => throw new NotImplementedException()
        };
    }

    public enum DynamicType
    {
        FontId,
        PixmapId,
        Gc,
        CursorId
    }
}

file class MethodDetails3 : StaticBuilder
{
    public MethodDetails3(string categories, string methodName) : base(categories, methodName, ["new uint[] {}"],
        ["uint[]"], false, STRType.RawBuffer)
    { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h> 

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection);
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }
}

file class MethodDetails4 : StaticBuilder
{
    public MethodDetails4(string categories, string methodName, string[] parameters, string[] parameterSignature)
        : base(categories, methodName, parameters, parameterSignature, false, STRType.RawBuffer) { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, "rootDepth", "newId", "root");
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h> 

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    const xcb_setup_t *setup = xcb_get_setup(connection);
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;

    uint8_t rootDepth = screen->root_depth;
    xcb_window_t newId = xcb_generate_id(connection);
    xcb_window_t root = screen->root;
    xcb_flush(connection);


    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", rootDepth);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", newId);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", root);
    fprintf(stderr, "{{marker}}\n");
    
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.Equal(params0, _xProto.HandshakeSuccessResponseBody.Screens[0].RootDepth!.DepthValue);
        Assert.Equal(params1, _xProto.NewId());
        Assert.Equal(params2, _xProto.HandshakeSuccessResponseBody.Screens[0].Root);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }
}

file class MethodDetails5 : StaticBuilder
{
    public MethodDetails5(string categories, string methodName, string[] parameters, string[] paramSignature)
        : base(categories, methodName, parameters, paramSignature, false, STRType.XcbUint) { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, "newId", "window");
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h> 

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t newId = xcb_generate_id(connection);

    xcb_window_t window = xcb_generate_id(connection);
    xcb_create_window(connection, 0, window, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", newId);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", window);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.{{MethodName.Fix()}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.Equal(params0, _xProto.NewId());
        Assert.Equal(params1, _xProto.NewId());
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));

    }
}

file class MethodDetails6 : StaticBuilder
{
    public MethodDetails6(string categories, string methodName, string[] parameters, string[] paramSignature)
            : base(categories, methodName, parameters, paramSignature, false, STRType.RawBuffer) { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, "newId", "window", "visual");
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h> 

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t newId = xcb_generate_id(connection);

    xcb_window_t window = xcb_generate_id(connection);
    xcb_create_window(connection, 0, window, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    xcb_window_t visual = screen->root_visual;
    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", newId);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", window);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", visual);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.Equal(params1, _xProto.NewId());
        Assert.Equal(params2, _xProto.NewId());
        Assert.Equal(params3, _xProto.HandshakeSuccessResponseBody.Screens[0].RootVisualId);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }

}

file class MethodDetails7 : StaticBuilder
{
    public MethodDetails7(string categories, string methodName, string[] parameters, string[] paramSignature)
        : base(categories, methodName, parameters, paramSignature, false, STRType.RawBuffer) { }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, "pixmap");
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t pixmap = xcb_generate_id(connection);
    xcb_create_pixmap_checked(connection, 1, pixmap, screen->root, 16, 16);
    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", pixmap);
    fprintf(stderr, "{{marker}}\n");


    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");

    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
} 
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;
        var cursor_pixmap = _xProto.NewId();
        _xProto.CreatePixmapUnchecked(1, cursor_pixmap, _xProto.HandshakeSuccessResponseBody.Screens[0].Root, 16, 16);

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.Equal(params0, cursor_pixmap);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }

}

file class MethodDetails8 : StaticBuilder
{
    private string? _castType;
    public MethodDetails8(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr, string? castType = null) : base(categories, methodName, parameters,
        paramSignature, addLenInCCall, isXcbStr)
    {
        _castType = castType;
    }

    private string GetItems() => IsXcbStr switch
    {
        STRType.XcbStr8 => $"var items = System.Text.Encoding.UTF8.GetBytes(params{ParamSignature.Length - 1});",
        STRType.XcbSegment or STRType.XcbRectangle or STRType.XcbArc or STRType.XcbPoient => $"var items = Newtonsoft.Json.JsonConvert.DeserializeObject<{ParamSignature[^1]}>(params{ParamSignature.Length - 1}.Replace('=', ':'));",
        STRType.Xcb8 or STRType.Xcb16 => $"var items = Array.ConvertAll(params{ParamSignature.Length - 1}, a => ({_castType})a);",
        STRType.XcbStr16 => "",
        _ => throw new NotImplementedException(IsXcbStr.ToString())
    };

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;
        var root = _xProto.HandshakeSuccessResponseBody.Screens[0].Root;
        var gc = _xProto.NewId();
        _xProto.CreateGCChecked(gc, root, Xcsb.Masks.GCMask.Foreground, [_xProto.HandshakeSuccessResponseBody.Screens[0].BlackPixel]);
        {{GetItems()}}

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length, (_castType == null ? null : "items"))}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.Equal(root, params0);
        Assert.Equal(gc, params1);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        parameter = parameter.ToCParams(IsXcbStr, AddLenInCCall, "root", "gc");
        if (IsXcbStr is STRType.XcbStr8 or STRType.XcbStr16)
            parameter = parameter.ReplaceOnece(',', $", {parameter.CalculateSize()}, ");

        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <stdarg.h>

{{GetCStringMacro()}}

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t root = screen->root;
    xcb_gcontext_t gc = xcb_generate_id(connection);
    xcb_void_cookie_t cookie = xcb_create_gc_checked(connection, gc, screen->root, 4, (u_int32_t[]){screen->black_pixel});
    xcb_flush(connection);

    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (error)
        return -1;

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", root);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", gc);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    cookie = {{functionName}}({{parameter}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");

    error = xcb_request_check(connection, cookie);
    if (!error)
        return 0;

    free(error);
    return -1;
} 
""";
    }
}

file abstract class StaticBuilder : BaseBuilder
{
    public StaticBuilder(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr) : base(parameters, methodName, paramSignature)
    {
        Categories = categories;
        AddLenInCCall = addLenInCCall;
        IsXcbStr = isXcbStr;
    }

    public string Categories { get; }
    public bool AddLenInCCall { get; }
    public STRType IsXcbStr { get; }

    public static string Format(string value, string[] values, string[] paramSignature)
    {
        var sb = new StringBuilder();
        var content = value.AsSpan();
        for (var i = 0; i < paramSignature.Length; i++)
        {
            var context = StringHelper.GetCsField(content, out var field);
            content = content[context..];
            field = field.Trim();

            var index = field.IndexOf('$');
            if (index != -1)
            {
                field = field[++index..];
                sb.Append('(')
                    .Append(paramSignature[i])
                    .Append(')')
                    .Append(values[int.Parse(field)])
                    .Append(", ");
            }
            else if (field.StartsWith('[') && field.EndsWith(']'))
            {
                sb.Append("\"")
                    .Append(field.ToString().Replace("\"", "\\\""))
                    .Append("\", ");
            }
            else if (field.Contains("[]", StringComparison.InvariantCultureIgnoreCase))
            {
                sb.Append(field)
                    .Append(", ");
            }
            else
            {
                sb.Append('(')
                    .Append(paramSignature[i])
                    .Append(')')
                    .Append(field)
                    .Append(", ");
            }
        }

        sb.Append("new byte[] {")
            .Append(values[^1])
            .Append('}');

        return sb.ToString();
    }

    public override void WriteCsTestCases(FileStream fileStream, string compiler, string monitorFile,
        string[] parameters, string methodName, string[] paramSignature)
    {
        foreach (var parameter in parameters)
        {
            var cResponse = GetCResult(compiler, methodName, parameter, monitorFile);
            fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    [InlineData({{Format(parameter, cResponse, paramSignature)}})]

"""));
        }
    }

    public override void WriteCsMethodBody(FileStream fileStream)
    {
        var methodSignature = GetTestMethodSignature(ParamSignature);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }

    public string GetCStringMacro() => IsXcbStr switch
    {
        STRType.XcbByte or STRType.XcbInt or STRType.RawBuffer or STRType.XcbUint or STRType.XcbStr8 or STRType.XcbSegment or STRType.XcbRectangle or STRType.XcbArc or STRType.XcbPoient => "",
        STRType.XcbStr =>
"""
#define XS(s)                       \
    ((const void *)&(const struct { \
        uint8_t len;                \
        char data[sizeof(s) - 1];   \
    }){(uint8_t)(sizeof(s) - 1), s})
""",
        STRType.Xcb8 =>
"""
uint8_t *__XS(int count, ...)
{
    va_list args;
    int size = 0;
    int workingIndex = 0;

    va_start(args, count);
    for (int i = 0; i < count; i++)
    {
        size += strlen(va_arg(args, const char *));
        size++;
        size++;
    }
    va_end(args);

    uint8_t *result = malloc(size);
    if (!result)
        return NULL;

    va_start(args, count);
    while (workingIndex < size)
    {
        const char *text = va_arg(args, const char *);
        result[workingIndex++] = (uint8_t)strlen(text);
        result[workingIndex++] = (uint8_t)0;
        for (size_t i = 0; i < strlen(text); i++)
        {
            result[workingIndex++] = (uint8_t)text[i];
        }
    }
    va_end(args);
    return result;
}
#define PP_NARG(...) PP_NARG_(__VA_ARGS__, PP_RSEQ_N())
#define PP_NARG_(...) PP_ARG_N(__VA_ARGS__)
#define PP_ARG_N(_1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, N, ...) N
#define PP_RSEQ_N() 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0

#define XS(...) __XS(PP_NARG(__VA_ARGS__), __VA_ARGS__)
""",
        STRType.Xcb16 =>
"""

uint8_t *__XS(int count, ...)
{
    va_list args;
    int size = 0;
    int workingIndex = 0;

    va_start(args, count);
    for (int i = 0; i < count; i++)
    {
        size += strlen(va_arg(args, const char *)) * 2;
        size++;
        size++;
    }
    va_end(args);

    uint8_t *result = malloc(size);
    if (!result)
        return NULL;

    va_start(args, count);
    while (workingIndex < size)
    {
        const char *text = va_arg(args, const char *);
        result[workingIndex++] = (uint8_t)strlen(text);
        result[workingIndex++] = (uint8_t)0;
        for (size_t i = 0; i < strlen(text); i++)
        {
            result[workingIndex++] = (uint8_t)0;
            result[workingIndex++] = (uint8_t)text[i];
        }
    }
    va_end(args);
    return result;
}
#define PP_NARG(...) PP_NARG_(__VA_ARGS__, PP_RSEQ_N())
#define PP_NARG_(...) PP_ARG_N(__VA_ARGS__)
#define PP_ARG_N(_1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, N, ...) N
#define PP_RSEQ_N() 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0

#define XS(...) __XS(PP_NARG(__VA_ARGS__), __VA_ARGS__)

""",
        STRType.XcbStr16 =>
"""
xcb_char2b_t *XS(const char *data)
{
    int len = strlen(data);
    // i know its not right
    // but it will claim by os after program end
    xcb_char2b_t *buf = calloc(len, sizeof(xcb_char2b_t));
    for (int i = 0; i < len; ++i)
    {
        buf[i].byte1 = 0;
        buf[i].byte2 = data[i];
    }
    return buf;
}
""",
        _ => throw new Exception(),
    };
}

file abstract class BaseBuilder : IBuilder
{
    protected readonly string[] Parameters;
    protected readonly string MethodName;
    protected readonly string[] ParamSignature;

    public BaseBuilder(string[] parameters, string methodName, string[] paramSignature)
    {
        this.ParamSignature = paramSignature;
        this.MethodName = methodName;
        this.Parameters = parameters;

    }

    public abstract void WriteCsTestCases(FileStream fileStream, string compiler, string monitorFile, string[] parameters,
        string MethodName, string[] paramSignature);

    public abstract string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker);

    public abstract void WriteCsMethodBody(FileStream fileStream);

    public void WriteCsMethodContent(FileStream fileStream, string compiler, string monitorFile)
    {
        fileStream.Write(
"""

    [Theory]

"""u8);
        WriteCsTestCases(fileStream, compiler, monitorFile, Parameters, MethodName, ParamSignature);
        WriteCsMethodBody(fileStream);
    }

    public static string FillPassingParameter(int parameterCount, string? lastItemName = null)
    {
        if (parameterCount == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (var i = 0; i < parameterCount; i++)
        {
            if (i == (parameterCount - 1) && lastItemName != null)
                sb.Append(lastItemName);
            else
                sb.Append("params")
                    .Append(i);
            sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        return sb.ToString();
    }

    protected static string GetTestMethodSignature(string[] paramsSignature)
    {
        if (paramsSignature.Length == 0) return string.Empty;

        var sb = new StringBuilder();

        for (var i = 0; i < paramsSignature.Length; i++)
            sb.Append(UpdateParameter(paramsSignature[i]))
                .Append(" params")
                .Append(i)
                .Append(", ");

        return sb.ToString();
    }

    private static ReadOnlySpan<char> UpdateParameter(ReadOnlySpan<char> value) =>
        value switch
        {
            "Xcsb.Models.Segment[]" or "Xcsb.Models.Rectangle[]" or "Xcsb.Models.Arc[]" or "Xcsb.Models.Point[]" => "string",
            _ => value
        };

    public string[] GetCResult(string compiler, string method, string? parameter, string monitorFile)
    {
        const string MARKER = "****************************************************************";
        var execFile = Path.Join(Environment.CurrentDirectory, "main");
        var cMainBody = GetCMethodBody(method, parameter, MARKER);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"-x c -o \"{execFile}\" - -lxcb",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardInput.Write(cMainBody);
        process.StandardInput.Close();
        process.WaitForExit();

        Debug.Assert(string.IsNullOrWhiteSpace(process.StandardError.ReadToEnd()));
        Debug.Assert(string.IsNullOrWhiteSpace(process.StandardOutput.ReadToEnd()));
        Debug.Assert(File.Exists(execFile));

#if DOCKERENV
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xvfb-run",
                    Arguments = $"-a env LD_PRELOAD={monitorFile} {execFile}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var response = process.StandardOutput.ReadToEnd();
#else
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = execFile,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };
        process.StartInfo.Environment["LD_PRELOAD"] = monitorFile;

        process.Start();
        var response = process.StandardError.ReadToEnd();
#endif

        process.WaitForExit();
        File.Delete(execFile);
        var result = new List<string>();
        var currentPos = 0;
        while (true)
        {
            var startPos = response.IndexOf(MARKER, currentPos, StringComparison.Ordinal);
            if (startPos == -1) break;
            startPos += MARKER.Length + 1;
            var endPos = response.IndexOf(MARKER, startPos, StringComparison.Ordinal);

            result.Add(response[startPos..(endPos - 1)]);
            currentPos = endPos + MARKER.Length;

        }
        return [.. result];
    }
}

file interface IBuilder
{
    void WriteCsMethodContent(FileStream stream, string compiler, string monitorFile);
}

file enum STRType
{
    RawBuffer,
    XcbStr,
    Xcb8,
    Xcb16,
    XcbStr8,
    XcbStr16,
    XcbUint,
    XcbInt,
    XcbByte,
    XcbSegment,
    XcbRectangle,
    XcbArc,
    XcbPoient
}
