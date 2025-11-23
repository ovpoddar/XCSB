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
    new MethodDetails1("IndependentMethod", "SetFontPath", [$"new string[] {{ \"built-ins\" , \"{Environment.CurrentDirectory}\" }}", $"new string[] {{\"{Environment.CurrentDirectory}\", \"/usr/bin\"}}", "new string[] {\"build-ins\"}"], ["string[]"], true, true, true),
    new MethodDetails1("IndependentMethod", "SetCloseDownMode", ["0", "1", "2"], ["Xcsb.Models.CloseDownMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeKeyboardControl", ["7, new uint[] {80, 90, 1200}"], ["Xcsb.Masks.KeyboardControlMask", "uint[]"], false),
    new MethodDetails1("IndependentMethod", "SetScreenSaver", ["5, 10, 1, 1", "0, 0, 0, 0", "2, 2, 0, 0"], ["short", "short", "Xcsb.Models.TriState", "Xcsb.Models.TriState"], false),
    new MethodDetails1("IndependentMethod", "ForceScreenSaver", ["1", "0"], ["Xcsb.Models.ForceScreenSaverMode"], false),
    new MethodDetails1("IndependentMethod", "SetAccessControl", ["1", "0"], ["Xcsb.Models.AccessControlMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeHosts", ["0, 4, new byte[] {127, 0, 0, 1}" , "1, 4, new byte[] {127, 0, 0, 1}"], ["Xcsb.Models.HostMode", "Xcsb.Models.Family", "byte[]"], true),
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
    new MethodDetails4("DependentOnPixmapRootDepth", "CreatePixmap", ["$0, $1, $2, 65535, 65535", "$0, $1, $2, 0, 65535"] , ["byte", "uint", "uint", "ushort", "ushort"])
];
//CreateGC                          (uint gc, uint drawable, GCMask mask, Span<uint> args)
//CreateColormap                    (ColormapAlloc alloc, uint colormapId, uint window, uint visual)
//CreateCursor                      (uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
//CreateGlyphCursor                 (uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)

//CreateWindow                      (byte depth, uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
//ReparentWindow                    (uint window, uint parent, short x, short y)
//ChangeProperty                    (PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
//DeleteProperty                    (uint window, ATOM atom)
//RotateProperties                  (uint window, ushort delta, Span<ATOM> properties)
//SetSelectionOwner                 (uint owner, ATOM atom, uint timestamp)
//ConvertSelection                  (uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
//SendEvent                         (bool propagate, uint destination, uint eventMask, XEvent evnt)
//ChangeActivePointerGrab           (uint cursor, uint time, ushort mask)
//WarpPointer                       (uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destinationX, short destinationY)
//OpenFont                          (string fontName, uint fontId)
//CloseFont                         (uint fontId)
//FreePixmap                        (uint pixmapId)
//ChangeGC                          (uint gc, GCMask mask, Span<uint> args)
//CopyGC                            (uint srcGc, uint dstGc, GCMask mask)
//SetDashes                         (uint gc, ushort dashOffset, Span<byte> dashes)
//SetClipRectangles                 (ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles)
//FreeGC                            (uint gc)
//ClearArea                         (bool exposures, uint window, short x, short y, ushort width, ushort height)
//CopyArea                          (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height)
//CopyPlane                         (uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
//PolyPoint                         (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
//PolyLine                          (CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
//PolySegment                       (uint drawable, uint gc, Span<Segment> segments)
//PolyRectangle                     (uint drawable, uint gc, Span<Rectangle> rectangles)
//PolyArc                           (uint drawable, uint gc, Span<Arc> arcs)
//FillPoly                          (uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
//PolyFillRectangle                 (uint drawable, uint gc, Span<Rectangle> rectangles)
//PolyFillArc                       (uint drawable, uint gc, Span<Arc> arcs)
//PutImage                          (ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y, byte leftPad, byte depth, Span<byte> data)
//ImageText8                        (uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
//ImageText16                       (uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
//FreeColormap                      (uint colormapId)
//CopyColormapAndFree               (uint colormapId, uint srcColormapId)
//InstallColormap                   (uint colormapId)
//UninstallColormap                 (uint colormapId)
//FreeColors                        (uint colormapId, uint planeMask, Span<uint> pixels)
//StoreColors                       (uint colormapId, Span<ColorItem> item)
//StoreNamedColor                   (ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
//FreeCursor                        (uint cursorId)
//RecolorCursor                     (uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
//ChangeKeyboardMapping             (byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym)
//PolyText8                         (uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
//PolyText16                        (uint drawable, uint gc, ushort x, ushort y, Span<byte> data)


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
    private static int CountUpperCaseLetter(ReadOnlySpan<char> value)
    {
        var result = 0;
        foreach (var c in value)
            if (char.IsUpper(c)) result++;
        return result;
    }

    private static int CalculateLen(Span<string> value)
    {
        var result = 0;
        foreach (var items in value)
        {
            if (items.Trim() == "}")
                break;
            result++;
        }
        return result + 1;
    }

    public static string? ToCParams(this string? value, bool needCast, bool addLenInCCall, bool isXcbStr)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (needCast)
        {
            value = value.Replace('{', '(')
                .Replace('}', ')');
        }
        var items = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        var index = 0;
        foreach (var field in items)
        {
            index++;
            if (field.StartsWith("new "))
            {
                var f = field
                    .ReplaceOnece('"', "XS(\"")
                    .ReplaceAtLast('"', "\")");
                var fieldStart = f.IndexOf('{');
                if (fieldStart == -1)
                    fieldStart = f.IndexOf('(');
                if (addLenInCCall)
                {
                    sb.Append(", ")
                        .Append(CalculateLen(items.AsSpan()[index..]));
                }
                sb.Append(", (")
                    .Append(GetCType(f, isXcbStr))
                    .Append(')')
                    .Append(f[fieldStart..]);
            }
            else if (field.Contains('$'))
                sb.Append($", ").Append(field.Replace("$", "params").Trim());
            else if (field.Contains("false"))
                sb.Append(", 0");
            else if (field.Contains("true"))
                sb.Append(", 1");
            else if (field.StartsWith('"'))
                sb.Append(", XS(").Append(field).Append(')');
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

    private static string GetCType(string type, bool isXcbStr)
    {
        var typeStartPosition = type.IndexOf("new ") + 4;
        var arrayStartIndex = type.IndexOf('[');

        return type.AsSpan()[typeStartPosition..arrayStartIndex].Trim() switch
        {
            "uint" => "uint32_t[]",
            "int" => "int32_t[]",
            "byte" => "uint8_t[]",
            "string" => isXcbStr ? "xcb_str_t *" : "const char *",
            _ => "int16_t[]",
        };
    }

    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var upperCount = CountUpperCaseLetter(value);
        if (upperCount == 0)
            return value;

        var length = value.Length + upperCount;
        Span<char> buffer = length <= 256
            ? stackalloc char[length]
            : new char[length];

        var pos = 0;
        for (var i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            var isUpper = char.IsUpper(chr);
            if (isUpper && i != 0)
                buffer[pos++] = '_';

            buffer[pos++] = isUpper
                ? (char)(chr | 32)
                : chr;
        }

        return new string(buffer[..pos]);
    }
}

file class MethodDetails1 : BaseBuilder
{
    public MethodDetails1(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, bool isXcbStr = false, bool needCast = false) : base(categories, methodName, parameters,
        paramSignature, addLenInCCall, isXcbStr, needCast)
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

#define XS(s)                       \
    ((const void *)&(const struct { \
        uint8_t len;                \
        char data[sizeof(s) - 1];   \
    }){(uint8_t)(sizeof(s) - 1), s})

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
        bool addLenInCCall) : base(categories, methodName, parameters, paramSignature, addLenInCCall, false, false)
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
        ["uint[]"], false, false, false)
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
    public MethodDetails4(string categories, string methodName, string[] parameters, string[] parameterSignature) : base
        (categories, methodName, parameters, parameterSignature, false, false, false)
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

file abstract class BaseBuilder : IBuilder
{
    public BaseBuilder(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, bool isXcbStr, bool needCast)
    {
        Categories = categories;
        MethodName = methodName;
        Parameters = parameters;
        ParamSignature = paramSignature;
        AddLenInCCall = addLenInCCall;
        IsXcbStr = isXcbStr;
        NeedCast = needCast;
    }

    public string Categories { get; }
    public string MethodName { get; }
    public string[] Parameters { get; }
    public string[] ParamSignature { get; }
    public bool AddLenInCCall { get; }
    public bool IsXcbStr { get; }
    public bool NeedCast { get; }

    protected static string FillPassingParameter(int parameterCount)
    {
        if (parameterCount == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (var i = 0; i < parameterCount; i++)
            sb.Append("params")
                .Append(i)
                .Append(", ");
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
        var cMainBody = GetCMethodBody(method, parameter.ToCParams(NeedCast, AddLenInCCall, IsXcbStr), MARKER);
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
#if false // todo add some kind of flag pass down from env
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

}

file interface IBuilder
{
    void WriteCsMethodContent(FileStream stream, string compiler, string monitorFile);
}
