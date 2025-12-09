#:sdk Microsoft.NET.Sdk

using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

// Global Set Up
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    return 0;
var compiler = GetCCompiler();
var monitorFile = GenerateMonitorFile(compiler);
using var fileStream = File.Open("./VoidMethodsTest.Generated.cs", FileMode.OpenOrCreate);

// No Parameter Methods Set Up
IBuilder[] noParamMethod = [
    new MethodDetails1("NoParameter", "GrabServer", [""], [], false),
    new MethodDetails1("NoParameter", "UngrabServer", [""], [], false),
    new MethodDetails1("IndependentMethod", "Bell", ["0", "50", "90", "99", "100"], ["sbyte"], false),
    new MethodDetails1("IndependentMethod", "UngrabPointer", ["0", "10", "100", "1000", "10000", "100000", "1000000", "10000000","100000000", "1000000000", "4294967295"], ["uint"], false),
    new MethodDetails1("IndependentMethod", "UngrabKeyboard", ["0", "10", "100", "1000", "10000", "100000", "1000000", "10000000","100000000", "1000000000", "4294967295"], ["uint"], false),
    new MethodDetails1("IndependentMethod", "AllowEvents", ["0, 0", "1, 10", "2, 100", "3, 1000", "4, 10000", "5, 100000", "6, 1000000", "7, 10000000", "7, 100000000", "7, 1000000000", "7, 4294967295"], ["Xcsb.Models.EventsMode" ,"uint"], false),
    new MethodDetails1("IndependentMethod", "SetFontPath", [$"new string[] {{ \"built-ins\" , \"{Environment.CurrentDirectory}\" }}", $"new string[] {{\"{Environment.CurrentDirectory}\", \"/usr/bin\"}}", "new string[] {\"build-ins\"}"], ["string[]"], true, STRType.XcbStr),
    new MethodDetails1("IndependentMethod", "SetCloseDownMode", ["0", "1", "2"], ["Xcsb.Models.CloseDownMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeKeyboardControl", ["7, new uint[] {80, 90, 1200}"], ["Xcsb.Masks.KeyboardControlMask", "uint[]"], false, STRType.XcbUint),
    new MethodDetails1("IndependentMethod", "SetScreenSaver", ["5, 10, 1, 1", "0, 0, 0, 0", "2, 2, 0, 0"], ["short", "short", "Xcsb.Models.TriState", "Xcsb.Models.TriState"], false),
    new MethodDetails1("IndependentMethod", "ForceScreenSaver", ["1", "0"], ["Xcsb.Models.ForceScreenSaverMode"], false),
    new MethodDetails1("IndependentMethod", "SetAccessControl", ["1", "0"], ["Xcsb.Models.AccessControlMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeHosts", ["0, 4, new byte[] {127, 0, 0, 1}" , "1, 4, new byte[] {127, 0, 0, 1}"], ["Xcsb.Models.HostMode", "Xcsb.Models.Family", "byte[]"], true, STRType.XcbByte),
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
    new MethodDetails3("SpecialMethod", "NoOperation"),
    new MethodDetails4("DependentOnPixmapRootDepthRoot", "CreatePixmap", ["$0, $1, $2, 65535, 65535", "$0, $1, $2, 0, 65535"] , ["byte", "uint", "uint", "ushort", "ushort"]),
    new MethodDetails5("DependentOnWindowId", "CreateGc", ["$0, $1, 1, new uint[] {6}", "$0, $1, 4194304, new uint[] {1}"], ["uint", "uint", "Xcsb.Masks.GCMask", "uint[]"]),
    new MethodDetails6("DependentOnWindowId", "CreateColormap", ["0, $0, $1, $2", "1, $0, $1, $2"], ["Xcsb.Models.ColormapAlloc", "uint", "uint", "uint"]),
    new MethodDetails7("DependentOnColorMap", "FreeColormap", ["$0"], ["uint"]),
    new MethodDetails7("DependentOnColorMap", "InstallColormap", ["$0"], ["uint"]),
    new MethodDetails7("DependentOnColorMap", "UninstallColormap", ["$0"], ["uint"]),
    new MethodDetails8("DependentOnDrawableGc", "PolyText8", ["$0, $1, 0, 0, new string[] { \"Hellow\", \"world\", \"xcb\" }"], ["uint", "uint", "ushort", "ushort", "string[]"], true, STRType.Xcb8, "Xcsb.Models.String.TextItem8"),
    new MethodDetails8("DependentOnDrawableGc", "PolyText16", ["$0, $1, 0, 0, new string[] { \"Hellow\", \"World\" }"], ["uint", "uint", "ushort", "ushort", "string[]" ], true, STRType.Xcb16, "Xcsb.Models.String.TextItem16"),
    new MethodDetails8("DependentOnDrawableGc", "ImageText8", [$"$0, $1,0, 0, \"XCB System Control Demo\" "], ["uint", "uint", "short", "short", "string"], false, STRType.XcbStr8, "items"),
    new MethodDetails8("DependentOnDrawableGc", "ImageText16", ["$0, $1, 0, 0, \"XCB System Control Demo\""], ["uint", "uint", "short", "short", "string"], false, STRType.XcbStr16),
];
// CreateCursor                      (uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
// CreateGlyphCursor                 (uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)

// CreateWindow                      (byte depth, uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
// ReparentWindow                    (uint window, uint parent, short x, short y)
// ChangeProperty                    (PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
// DeleteProperty                    (uint window, ATOM atom)
// RotateProperties                  (uint window, ushort delta, Span<ATOM> properties)
// SetSelectionOwner                 (uint owner, ATOM atom, uint timestamp)
// ConvertSelection                  (uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
// SendEvent                         (bool propagate, uint destination, uint eventMask, XEvent evnt)
// ChangeActivePointerGrab           (uint cursor, uint time, ushort mask)
// WarpPointer                       (uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destinationX, short destinationY)
// OpenFont                          (string fontName, uint fontId)
// CloseFont                         (uint fontId)
// FreePixmap                        (uint pixmapId)
// ChangeGC                          (uint gc, GCMask mask, Span<uint> args)
// CopyGC                            (uint srcGc, uint dstGc, GCMask mask)
// SetDashes                         (uint gc, ushort dashOffset, Span<byte> dashes)
// SetClipRectangles                 (ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles)
// FreeGC                            (uint gc)
// ClearArea                         (bool exposures, uint window, short x, short y, ushort width, ushort height)
// CopyArea                          (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height)
// CopyPlane                         (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
// PolyPoint                         (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
// PolyLine                          (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
// PutImage                          (ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y, byte leftPad, byte depth, Span<byte> data)
// CopyColormapAndFree               (uint colormapId, uint srcColormapId)
// FreeColors                        (uint colormapId, uint planeMask, Span<uint> pixels)
// StoreColors                       (uint colormapId, Span<ColorItem> item)
// StoreNamedColor                   (ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
// FreeCursor                        (uint cursorId)
// RecolorCursor                     (uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
// ChangeKeyboardMapping             (byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym)


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
    private static int CalculateLen(Span<string> value, STRType isXcbStr)
    {
        var result = 0;
        foreach (var items in value)
        {
            if (items.Trim() == "}")
                break;
            if (isXcbStr is STRType.Xcb8 or STRType.Xcb16)
                result += items.Length + 2;
            else
                result++;

        }
        return result + 1;
    }

    private static string GetCItem(string field, STRType isXcbStr, bool addComma)
    {
        var fieldStart = field.IndexOf('{');
        if (fieldStart == -1)
            fieldStart = field.IndexOf('(');
        if (fieldStart != -1)
        {
            field = field[fieldStart..];
        }
        else
        {
            if (field.Contains("="))
                return ", ." + field
                .ToLower();
        }
        return (addComma ? ", " : "") + isXcbStr switch
        {
            STRType.XcbStr or STRType.Xcb8 or STRType.Xcb16 or STRType.XcbStr16 => field
                            .ReplaceOnece('"', "XS(\"")
                            .ReplaceAtLast('"', "\")"),
            STRType.XcbStr8 or STRType.XcbUint or STRType.XcbByte or STRType.RawBuffer => field,
            _ => throw new Exception($"{field} {isXcbStr}"),
        };
    }

    public static string? ToCParams(this string? value, bool addLenInCCall, STRType isXcbStr)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (isXcbStr is STRType.XcbStr)
        {
            value = value.Replace('{', '(')
                .Replace('}', ')');
        }
        var items = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        if (isXcbStr is STRType.XcbStr8 or STRType.XcbStr16)
            sb.Append(',').Append(items[^1].Length);

        var index = 0;
        bool canCome = false;
        foreach (var field in items)
        {
            index++;
            if (field.StartsWith("new ") || canCome || field.StartsWith("\""))
            {
                canCome = true;
                if (field.Contains("[]") || isXcbStr is STRType.XcbStr16)
                {
                    if (addLenInCCall)
                    {
                        sb.Append(", ")
                            .Append(CalculateLen(items.AsSpan()[index..], isXcbStr));
                    }
                    sb.Append(", (")
                        .Append(GetCType(isXcbStr))
                        .Append(')')
                        .Append(GetCItem(field, isXcbStr, false));
                }
                else
                {
                    sb.Append(GetCItem(field, isXcbStr, true));
                }
            }
            else if (field.Contains('$'))
                sb.Append($", ").Append(field.Replace("$", "params").Trim());
            else if (field.Contains("false"))
                sb.Append(", 0");
            else if (field.Contains("true"))
                sb.Append(", 1");
            else
                sb.Append(", ").Append(field);
        }
        return sb.ToString();
    }

    private static string ReplaceOnece(this string value, char target, string replaced)
    {
        var sb = new StringBuilder();
        var isDone = false;
        foreach (var item in value)
        {
            if (item == target && !isDone)
            {
                sb.Append(replaced);
                isDone = true;
                continue;
            }

            sb.Append(item);
        }

        return sb.ToString();
    }

    private static string ReplaceAtLast(this string value, char target, string replaced)
    {
        var sb = new StringBuilder();
        var i = value.LastIndexOf(target);
        for (var index = 0; index < value.Length; index++)
        {
            if (index == i)
                sb.Append(replaced);
            else
                sb.Append(value[index]);
        }

        return sb.ToString();
    }

    private static string GetCType(STRType isXcbStr) => isXcbStr switch
    {
        STRType.XcbStr8 or STRType.RawBuffer => "const char *",
        STRType.XcbByte => "uint8_t[]",
        STRType.XcbInt => "int32_t[]",
        STRType.XcbUint => "uint32_t[]",
        STRType.XcbStr => "xcb_str_t *",
        STRType.Xcb8 or STRType.Xcb16 => "const uint8_t[]",
        STRType.XcbStr16 => "const xcb_char2b_t[]",
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

file class MethodDetails1 : BaseBuilder
{
    public MethodDetails1(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr = STRType.RawBuffer) : base(categories, methodName, parameters,
        paramSignature, addLenInCCall, isXcbStr)
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
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
}
""";
    }
}

file class MethodDetails2 : BaseBuilder
{
    public MethodDetails2(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall) : base(categories, methodName, parameters, paramSignature, addLenInCCall, STRType.XcbUint)
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
    const xcb_setup_t *setup = xcb_get_setup(connection);
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;

    xcb_window_t params0 = xcb_generate_id(connection);
    xcb_create_window(connection, 0, params0, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    
    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");
    
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

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


    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
        var hasWindowPlaceHolder = GetPlaceHolderOfWindow(Parameters[0]);
        fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{methodSignature}}byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;
        var window = _xProto.NewId();

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.Equal(window, {{hasWindowPlaceHolder}});
        Assert.NotNull(buffer);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
    }

"""));
    }
}

file class MethodDetails3 : BaseBuilder
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
        return 1;

    free(error);
    return -1;
}
""";
    }
}

file class MethodDetails4 : BaseBuilder
{
    public MethodDetails4(string categories, string methodName, string[] parameters, string[] parameterSignature)
        : base(categories, methodName, parameters, parameterSignature, false, STRType.RawBuffer) { }

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
    const xcb_setup_t *setup = xcb_get_setup(connection);
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;

    uint8_t params0 = screen->root_depth;
    xcb_window_t params1 = xcb_generate_id(connection);
    xcb_window_t params2 = screen->root;
    xcb_flush(connection);


    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params1);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params2);
    fprintf(stderr, "{{marker}}\n");
    
    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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

file class MethodDetails5 : BaseBuilder
{
    public MethodDetails5(string categories, string methodName, string[] parameters, string[] paramSignature)
        : base(categories, methodName, parameters, paramSignature, false, STRType.XcbUint) { }

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
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t params0 = xcb_generate_id(connection);

    xcb_window_t params1 = xcb_generate_id(connection);
    xcb_create_window(connection, 0, params1, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    
    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params1);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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

file class MethodDetails6 : BaseBuilder
{
    public MethodDetails6(string categories, string methodName, string[] parameters, string[] paramSignature)
            : base(categories, methodName, parameters, paramSignature, false, STRType.RawBuffer) { }

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
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t params0 = xcb_generate_id(connection);

    xcb_window_t params1 = xcb_generate_id(connection);
    xcb_create_window(connection, 0, params1, screen->root, 0, 0, 100, 100, 0, XCB_WINDOW_CLASS_INPUT_OUTPUT,
                    screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, (uint32_t[]){0, XCB_EVENT_MASK_EXPOSURE});
    xcb_window_t params2 = screen->root_visual;

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params1);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params2);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");
    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
}
""";
    }

    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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

file class MethodDetails7 : BaseBuilder
{
    public MethodDetails7(string categories, string methodName, string[] parameters, string[] paramSignature)
        : base(categories, methodName, parameters, paramSignature, false, STRType.RawBuffer) { }

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
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t params0 = xcb_generate_id(connection);
    xcb_create_pixmap_checked(connection, 1, params0, screen->root, 16, 16);
    xcb_flush(connection);

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");


    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");

    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
} 
""";
    }


    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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

file class MethodDetails8 : BaseBuilder
{
    private string? _castType;
    public MethodDetails8(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr, string? castType = null) : base(categories, methodName, parameters,
        paramSignature, addLenInCCall, isXcbStr)
    {
        _castType = castType;
    }

    private string GetItems()
    {
        if (base.IsXcbStr == STRType.XcbStr8)
        {
            return $"var items = System.Text.Encoding.UTF8.GetBytes(params{ParamSignature.Length - 1});";
        }
        return $"var items = Array.ConvertAll(params{ParamSignature.Length - 1}, a => ({_castType})a);";
    }

    public override void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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
        {{(string.IsNullOrWhiteSpace(_castType)
            ? ""
            : GetItems())}}

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length, (string.IsNullOrWhiteSpace(_castType) ? null : "items"))}});
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
        var functionName = "xcb_" + method.ToSnakeCase() + "_checked";
        return
$$"""
#include <xcb/xcb.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

{{GetCStringMacro()}}

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection))
    {
        return -1;
    }
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    xcb_window_t params0 = screen->root;
    xcb_gcontext_t params1 = xcb_generate_id(connection);
    xcb_void_cookie_t cookie = xcb_create_gc_checked(connection, params1, screen->root, 4, (u_int32_t[]){screen->black_pixel});
    xcb_flush(connection);

    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    if (error)
        return -1;

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params0);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    fprintf(stderr, "%d\n", params1);
    fprintf(stderr, "{{marker}}\n");

    fprintf(stderr, "{{marker}}\n");
    cookie = {{functionName}}(connection{{(parameter == null ? "" : parameter)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");

    error = xcb_request_check(connection, cookie);
    if (!error)
        return 1;

    free(error);
    return -1;
} 
""";
    }
}

file abstract class BaseBuilder : IBuilder
{
    public BaseBuilder(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType isXcbStr)
    {
        Categories = categories;
        MethodName = methodName;
        Parameters = parameters;
        ParamSignature = paramSignature;
        AddLenInCCall = addLenInCCall;
        IsXcbStr = isXcbStr;
    }

    public string Categories { get; }
    public string MethodName { get; }
    public string[] Parameters { get; }
    public string[] ParamSignature { get; }
    public bool AddLenInCCall { get; }
    public STRType IsXcbStr { get; }

    protected static string FillPassingParameter(int parameterCount, string? lastItemName = null)
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

    //todo: remove the string shit.
    private static string GetField(string parameter, out string field)
    {
        var result = "";
        var canReturn = true;
        for (int i = 0; i < parameter.Length; i++)
        {
            if (parameter[i] == ',' && canReturn)
            {
                field = result;
                return parameter[++i..];
            }

            if (parameter[i] == '"')
            {
                if (canReturn)
                    canReturn = false;
                else
                    canReturn = true;
            }

            if (parameter[i] == '{')
                canReturn = false;

            if (parameter[i] == '}')
                canReturn = true;

            result += parameter[i];
        }

        field = result;
        return "";

    }

    private static string GetTestMethodSignature(string[] paramsSignature)
    {
        if (paramsSignature.Length == 0) return string.Empty;

        var sb = new StringBuilder();

        for (var i = 0; i < paramsSignature.Length; i++)
            sb.Append(paramsSignature[i])
                .Append(" params")
                .Append(i)
                .Append(", ");

        return sb.ToString();
    }
    public abstract string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker);

    public virtual void WriteCsMethodBody(FileStream fileStream, ReadOnlySpan<char> methodSignature)
    {
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

    public virtual void WriteCsTestCases(FileStream fileStream, string compiler, string monitorFile, string[] parameters,
        string MethodName, string[] paramSignature)
    {
        foreach (var testCase in parameters)
        {
            var cResponse = GetCResult(compiler, MethodName, testCase, monitorFile);

            var veriables = "";
            if (!string.IsNullOrWhiteSpace(testCase))
            {
                var tempStr = testCase;
                foreach (var signature in paramSignature)
                {
                    tempStr = GetField(tempStr, out var field);
                    if (field.Trim().Contains('$'))
                        veriables += $"{cResponse[int.Parse(field.Trim().Replace("$", ""))]}, ";
                    else if (field.Trim().StartsWith("new"))
                        veriables += $"{field}, ";
                    else
                        veriables += $"({signature}){field}, ";
                }
            }

            fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    [InlineData({{veriables}}new byte[] {{{cResponse[^1]}} })]

"""));

        }
    }

    public void WriteCsMethodContent(FileStream fileStream, string compiler, string monitorFile)
    {
        fileStream.Write(
"""

    [Theory]

"""u8);

        WriteCsTestCases(fileStream, compiler, monitorFile, Parameters, MethodName, ParamSignature);
        var methodSignature = GetTestMethodSignature(ParamSignature);
        WriteCsMethodBody(fileStream, methodSignature);
    }

    public string[] GetCResult(string compiler, string method, string? parameter, string monitorFile)
    {
        const string MARKER = "****************************************************************";
        var execFile = Path.Join(Environment.CurrentDirectory, "main");
        var cMainBody = GetCMethodBody(method, parameter.ToCParams(AddLenInCCall, IsXcbStr), MARKER);
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

    public string GetCStringMacro() => IsXcbStr switch
    {
        STRType.XcbByte or STRType.XcbInt or STRType.RawBuffer or STRType.XcbUint or STRType.XcbStr8 => "",
        STRType.XcbStr => """
#define XS(s)                       \
    ((const void *)&(const struct { \
        uint8_t len;                \
        char data[sizeof(s) - 1];   \
    }){(uint8_t)(sizeof(s) - 1), s})
""",
        STRType.Xcb8 =>
"""
#define _XS_I(i, s) ((i < sizeof(s)-1) ? s[i] : 0)

#define XS(s) \
    sizeof(s) - 1, 0, \
    _XS_I(0, s), _XS_I(1, s), _XS_I(2, s), _XS_I(3, s), \
    _XS_I(4, s), _XS_I(5, s), _XS_I(6, s), _XS_I(7, s), \
    _XS_I(8, s), _XS_I(9, s), _XS_I(10, s), _XS_I(11, s), \
    _XS_I(12, s), _XS_I(13, s), _XS_I(14, s), _XS_I(15, s)

""",
        STRType.Xcb16 =>
"""
#define _XSI(i, s) ((i < sizeof(s) - 1) ? 0 : 0), ((i < sizeof(s) - 1) ? s[i] : 0)

#define XS(s)                                             \
    sizeof(s) - 1, 0,                                     \
        _XSI(0, s), _XSI(1, s), _XSI(2, s), _XSI(3, s),   \
        _XSI(4, s), _XSI(5, s), _XSI(6, s), _XSI(7, s),   \
        _XSI(8, s), _XSI(9, s), _XSI(10, s), _XSI(11, s), \
        _XSI(12, s), _XSI(13, s), _XSI(14, s), _XSI(15, s)
""",
        STRType.XcbStr16 =>
"""

#define _XS_I(str, i) \
    { 0, (i < sizeof(str)-1 ? str[i] : 0) }

#define XS(str)        \
    {                  \
        _XS_I(str, 0), \
        _XS_I(str, 1), \
        _XS_I(str, 2), \
        _XS_I(str, 3), \
        _XS_I(str, 4), \
        _XS_I(str, 5), \
        _XS_I(str, 6), \
        _XS_I(str, 7), \
        _XS_I(str, 8), \
        _XS_I(str, 9) \
    }
""",
        _ => throw new Exception(),
    };
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
    XcbByte
}
