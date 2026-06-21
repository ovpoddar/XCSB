#:sdk Microsoft.NET.Sdk
#:property TreatWarningsAsErrors=true

using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

// Global Set Up
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    return 0;
var compiler = GetCCompiler();
var monitorFile = GenerateMonitorFile(compiler);
using var fileStream = File.Open("./BufferVoidXInputMethodsTest.Generated.cs", FileMode.OpenOrCreate);

IBuilder[] noParamMethod = [
    //InputCloseDevice(byte DeviceId);
	//InputSelectExtensionEvent(uint Window, ushort NumClasses);
	//InputUngrabDevice(xcb_timestamp_t Time, byte DeviceId);
	//InputChangeDeviceDontPropagateList(uint Window, ushort NumClasses, byte Mode);
	//InputAllowDeviceEvents(xcb_timestamp_t Time, byte Mode, byte DeviceId);
	//InputGrabDeviceKey(uint GrabWindow, ushort NumClasses, ushort Modifiers, byte ModifierDevice, byte GrabbedDevice, byte Key, byte ThisDeviceMode, byte OtherDeviceMode, byte OwnerEvents);
	//InputUngrabDeviceKey(uint GrabWindow, ushort Modifiers, byte ModifierDevice, byte Key, byte GrabbedDevice);
	//InputGrabDeviceButton(uint GrabWindow, byte GrabbedDevice, byte ModifierDevice, ushort NumClasses, ushort Modifiers, byte ThisDeviceMode, byte OtherDeviceMode, byte Button, byte OwnerEvents);
	//InputUngrabDeviceButton(uint GrabWindow, ushort Modifiers, byte ModifierDevice, byte Button, byte GrabbedDevice);
	//InputSetDeviceFocus(uint Focus, xcb_timestamp_t Time, byte RevertTo, byte DeviceId);
	//InputChangeFeedbackControl(uint32_t Mask, byte DeviceId, byte FeedbackId);
	//InputChangeDeviceKeyMapping(byte DeviceId, xcb_input_key_code_t FirstKeycode, byte KeysymsPerKeycode, byte KeycodeCount);
	//InputDeviceBell(byte DeviceId, byte FeedbackId, byte FeedbackClass, int8_t Percent);
	//InputChangeDeviceProperty(ATOM Property, ATOM Type, byte DeviceId, byte Format, byte Mode, uint32_t NumItems);
	//InputDeleteDeviceProperty(ATOM Property, byte DeviceId);
	//InputXiWarpPointer(uint SrcWin, uint DstWin, xcb_input_fp1616_t SrcX, xcb_input_fp1616_t SrcY, ushort SrcWidth, ushort SrcHeight, xcb_input_fp1616_t DstX, xcb_input_fp1616_t DstY, xcb_input_device_id_t Deviceid);
	//InputXiChangeCursor(uint Window, xcb_cursor_t Cursor, xcb_input_device_id_t Deviceid);
	//InputXiChangeHierarchy(byte NumChanges);
	//InputXiSetClientPointer(uint Window, xcb_input_device_id_t Deviceid);
	//InputXiSelectEvents(uint Window, ushort NumMask);
	//InputXiSetFocus(uint Window, xcb_timestamp_t Time, xcb_input_device_id_t Deviceid);
	//InputXiUngrabDevice(xcb_timestamp_t Time, xcb_input_device_id_t Deviceid);
	//InputXiAllowEvents(xcb_timestamp_t Time, xcb_input_device_id_t Deviceid, byte EventMode, uint32_t Touchid, uint GrabWindow);
	//InputXiPassiveUngrabDevice(uint GrabWindow, uint32_t Detail, xcb_input_device_id_t Deviceid, ushort NumModifiers, byte GrabType);
	//InputXiChangeProperty(xcb_input_device_id_t Deviceid, byte Mode, byte Format, ATOM Property, ATOM Type, uint32_t NumItems);
	//InputXiDeleteProperty(xcb_input_device_id_t Deviceid, ATOM Property);  
	//InputXiBarrierReleasePointer(uint32_t NumBarriers);
	//InputSendExtensionEvent(uint Destination, byte DeviceId, byte Propagate, ushort NumClasses, byte NumEvents);
new MethodDetails9("DependOnDeviceId", "InputCloseDevice", ["$0"], ["byte"], false, STRType.RawBuffer, [MethodDetails9.ImplType.DeviceId])
#if DOCKERENV
    // AVOID RUNNING IN YOUR PC. IT COULD BE CHANGE YOUR KEYBOARD KEYS
    new  ChangeKeyboardMapping(),
#endif
];

fileStream.Write(
"""
// DO NOT MODIFY THIS FILE
// IT WILL BE OVERWRITTEN WHEN GENERATING
#nullable enable
using Xcsb;
namespace MethodRequestBuilder.Test.Generated;
[Collection("Sequential Execution of Generated Methods")]
public class VoidXInputMethodsTest
{
    private static readonly System.Reflection.FieldInfo WorkingField =
        typeof(Xcsb.Handlers.Buffered.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        ?? throw new InvalidOperationException("Field '_buffer' not found.");
    private static readonly System.Reflection.FieldInfo BufferOutField =
        typeof(Xcsb.Implementation.XBufferProto)
            .GetField("_bufferProtoOut", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        ?? throw new InvalidOperationException("Field '_bufferProtoOut' not found.");
"""u8);
foreach (var method in noParamMethod)
    method.WriteCsMethodContent(fileStream, compiler, monitorFile);
fileStream.Write(
"""

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
            if (isXcbStr == STRType.XcbStr || isXcbStr == STRType.Xcb8 || isXcbStr == STRType.Xcb16 || isXcbStr == STRType.XcbStr8)
            {
                if (item == '"') result++;
                continue;
            }
            if (isXcbStr is STRType.XcbSegment or STRType.XcbRectangle or STRType.XcbArc or STRType.XcbPoient or STRType.XcbColorItem)
            {
                if (item == '}')
                    result++;

                continue;
            }
            if (isXcbStr is STRType.XcbByte or STRType.XcbShort or STRType.XcbAtom or STRType.XcbUint)
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
            case STRType.XcbShort:
            case STRType.XcbAtom:
            case STRType.XcbUint:
            case STRType.XcbStr8:
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
        else if (type is STRType.XcbUint or STRType.XcbByte or STRType.XcbStr8 or STRType.XcbAtom or STRType.XcbShort)
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
        STRType.XcbShort => "uint16_t[]",
        STRType.XcbInt => "int32_t[]",
        STRType.XcbUint => "uint32_t[]",
        STRType.XcbStr => "xcb_str_t *",
        STRType.Xcb8 or STRType.Xcb16 => "uint8_t *",
        STRType.XcbStr16 => "xcb_char2b_t *",
        STRType.XcbSegment => "(xcb_segment_t[])",
        STRType.XcbRectangle => "(xcb_rectangle_t[])",
        STRType.XcbArc => "(xcb_arc_t[])",
        STRType.XcbPoient => "(xcb_point_t[])",
        STRType.XcbAtom => "xcb_atom_t[]",
        STRType.XcbColorItem => "(xcb_coloritem_t[])",
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



file class MethodDetails9 : StaticBuilder
{
    public ImplType[] Types { get; }

    public MethodDetails9(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, STRType strType, params ImplType[] types) : base(categories, methodName, parameters, paramSignature,
        addLenInCCall, strType)
    {
        Types = types;
    }

    public string GetCImpl(ImplType type, int name)
    {
        return type switch
        {
            ImplType.Id => $"   xcb_colormap_t paramDynamic{name} = xcb_generate_id(connection);",
            ImplType.Window =>
$$"""
    xcb_window_t paramDynamic{{name}} = xcb_generate_id(connection);
    {
        xcb_void_cookie_t cookie = xcb_create_window_checked(connection, XCB_COPY_FROM_PARENT, paramDynamic{{name}}, screen->root, 0, 0, 640, 480, 0, 
            XCB_WINDOW_CLASS_INPUT_OUTPUT, screen->root_visual, XCB_CW_BACK_PIXEL | XCB_CW_EVENT_MASK, 
            (uint32_t[]){screen->white_pixel,XCB_EVENT_MASK_EXPOSURE | XCB_EVENT_MASK_KEY_PRESS});
        xcb_generic_error_t *error = xcb_request_check(connection, cookie);
        if (error) 
        {
            free(error);
            return 1;
        }
    }
""",
            ImplType.CursorFont =>
$$"""
    xcb_font_t paramDynamic{{name}} = xcb_generate_id(connection);
    {
        xcb_void_cookie_t cookie = xcb_open_font_checked(connection, paramDynamic{{name}}, {{"cursor".Length}}, "cursor");
        xcb_generic_error_t *error = xcb_request_check(connection, cookie);
        if (error) 
        {
            free(error);
            return 1;
        }
    }
""",
            ImplType.Root =>
$"""
    xcb_window_t paramDynamic{name} = screen->root;
""",
            ImplType.Rootdepth =>
$"""
    uint8_t paramDynamic{name} = screen->root_depth;
""",
            ImplType.VisualId =>
$"""
    xcb_visualid_t paramDynamic{name} = screen->root_visual;
""",
            ImplType.CursorId =>
$$"""
    xcb_cursor_t paramDynamic{{name}} = xcb_generate_id(connection);
    {
        xcb_pixmap_t src = xcb_generate_id(connection);
        xcb_void_cookie_t cookie = xcb_create_pixmap_checked(connection, 1, src, window, 8, 8);
        xcb_generic_error_t *error = xcb_request_check(connection, cookie);
        if (error) 
        {
            free(error);
            return 1;
        }

        xcb_pixmap_t mask = xcb_generate_id(connection);
        cookie = xcb_create_pixmap_checked(connection, 1, mask, window, 8, 8);
        error = xcb_request_check(connection, cookie);
        if (error) 
        {
            free(error);
            return 1;
        }

        cookie = xcb_create_cursor_checked(
            connection,
            paramDynamic{{name}},
            src,
            mask,
            0, 0, 0,                 
            65535, 65535, 65535,     
            0, 0                     
        );
        error = xcb_request_check(connection, cookie);
        if (error) 
        {
            free(error);
            return 1;
        }
    }
""",
            ImplType.DeviceId =>
$$"""
    xcb_input_device_id_t paramDynamic{{name}};
    {
        xcb_input_list_input_devices_cookie_t cookie = xcb_input_list_input_devices(connection);
        xcb_generic_error_t *error = NULL;
        xcb_input_list_input_devices_reply_t *reply = xcb_input_list_input_devices_reply(connection, cookie, &error);

        if (!reply)
        {
            free(error);
            xcb_disconnect(connection);
            return 1;
        }

        int count = xcb_input_list_input_devices_devices_length(reply);

        xcb_input_device_info_iterator_t it = xcb_input_list_input_devices_devices_iterator(reply);
        xcb_input_device_info_next(&it);
        xcb_input_device_info_t *dev = it.data;
        paramDynamic{{name}} = dev->device_id;
        free(reply);
    }
""",
            _ => throw new NotImplementedException()
        };
    }

    public string GetCsImpl(ImplType type, int name)
    {
        return type switch
        {
            ImplType.Id =>
$"""

        var item{name} = connect.NewId();
""",
            ImplType.Window =>
$"""

        var item{name} = connect.NewId();
        _xProto.CreateWindowChecked(0, item{name}, screen.Root, 0, 0, 640, 480, 0,  Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, (Xcsb.Masks.ValueMask)(2 | 2048),  [screen.WhitePixel, 32768 | 1 ]);
""",
            ImplType.CursorFont =>
$"""

        var item{name} = connect.NewId();
        _xProto.OpenFontChecked("cursor", item{name});
""",
            ImplType.Root =>
$"""

        var item{name} = screen.Root;
""",
            ImplType.Rootdepth =>
$"""

        var item{name} = screen.RootDepth!.DepthValue;
""",
            ImplType.VisualId =>
$"""

        var item{name} = screen.RootVisualId;
""",
            ImplType.CursorId =>
$"""

        var item{name} = connect.NewId();

        var srcPixmap = connect.NewId();
        _xProto.CreatePixmapChecked(1, srcPixmap, window, 8, 8);
        var maskPixmap = connect.NewId();
        _xProto.CreatePixmapChecked(1, maskPixmap, window, 8, 8);
        
        _xProto.CreateCursorChecked(item{name}, srcPixmap, maskPixmap, 0, 0, 0, 65535, 65535, 65535, 0, 0);
""",
            _ => throw new NotImplementedException(type.ToString())
        };
    }

    public string WriteMembers(Func<ImplType, int, string> selector)
    {
        var sb = new StringBuilder();
        if (this.Types.Contains(ImplType.CursorId))
        {
            sb.Append(selector(ImplType.Window, -99).Replace("paramDynamic-99", "window").Replace("item-99", "window"));
        }

        for (int i = 0; i < Types.Length; i++)
            sb.Append(selector(Types[i], i));

        return sb.ToString();
    }

    public override string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
    {
        var dynamicNames = Enumerable.Range(0, Types.Length).Select((a, i) => "paramDynamic" + i).ToArray();
        return
$$"""
#include <xcb/xcb.h>
#include <xcb/xinput.h>
#include <stdio.h>
#include <stdlib.h>

int main()
{
    xcb_connection_t *connection = xcb_connect(NULL, NULL);
    if (xcb_connection_has_error(connection)) return -1;
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
    {{WriteMembers(GetCImpl)}}
    xcb_flush(connection);

    {{WriteOutPutofCDynamic(marker)}}

    fprintf(stderr, "{{marker}}\n");
    xcb_void_cookie_t cookie = xcb_{{method.ToSnakeCase()}}_checked({{parameter.ToCParams(IsXcbStr, AddLenInCCall, dynamicNames)}});
    xcb_flush(connection);
    fprintf(stderr, "{{marker}}\n");

    xcb_generic_error_t *error = xcb_request_check(connection, cookie);
    return error ? -1 : 0;
}
""";
    }

    public string WriteOutPutofCDynamic(ReadOnlySpan<char> marker)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Types.Length; i++)
        {
            sb.Append("fprintf(stderr, \"");
            sb.Append(marker);
            sb.AppendLine("\\n\");");

            sb.Append("    fprintf(stderr, \"%d\\n\", paramDynamic");
            sb.Append(i);
            sb.AppendLine(");");

            sb.Append("    fprintf(stderr, \"");
            sb.Append(marker);
            sb.AppendLine("\\n\");");
        }
        return sb.ToString();

    }

    public override void WriteCsMethodBody(FileStream fs)
    {
        fs.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{Categories.ToSnakeCase()}}_{{MethodName.ToSnakeCase()}}_test({{GetTestMethodSignature(ParamSignature)}}byte[] expectedResult)
    {
        // arrange
        using var connect = Xcsb.Connection.XcsbClient.Connect();
        var _xProto = connect.Initialized();
        var bufferClient = _xProto.BufferClient;
        var bufferProtoOut = BufferOutField.GetValue(bufferClient)
            ?? throw new InvalidOperationException("_bufferProtoOut was null.");
        var screen = connect.HandshakeSuccessResponseBody!.Screens[0];
        var damage = connect.Extension.XInput();
        {{WriteMembers(GetCsImpl)}}
        
        // act
        bufferClient.{{(MethodName.Contains("gc", StringComparison.OrdinalIgnoreCase) ? MethodName.Fix() : MethodName)}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)WorkingField.GetValue(bufferProtoOut);

        // assert
{{WrittingAsserts(this.Parameters[0])}}
        Assert.NotNull(buffer);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
        buffer.Clear();
    }
"""));
    }

    public string WrittingAsserts(ReadOnlySpan<char> format)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (true)
        {
            var context = StringHelper.GetCsField(format, out var item);
            format = format[context..];
            var index = item.IndexOf('$');
            if (index != -1)
            {
                sb.Append("\t\tAssert.Equal(params");
                sb.Append(i);
                sb.Append(", item");
                sb.Append(int.Parse(item[++index..]));
                sb.AppendLine(");");
            }

            i++;
            if (format.Length == 0)
                break;
        }
        return sb.ToString();
    }

    public enum ImplType
    {
        Id,
        Window,
        CursorFont,
        VisualId,
        Root,
        Rootdepth,
        CursorId,
        DeviceId
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
        using var connect = Xcsb.Connection.XcsbClient.Connect();
        var _xProto = connect.Initialized();
        var bufferClient = _xProto.BufferClient;
        var bufferProtoOut = BufferOutField.GetValue(bufferClient)
            ?? throw new InvalidOperationException("_bufferProtoOut was null.");

        // act
        bufferClient.{{MethodName}}({{FillPassingParameter(ParamSignature.Length)}});
        var buffer = (List<byte>?)WorkingField.GetValue(bufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.NotNull(expectedResult);
        Assert.NotEmpty(buffer);
        Assert.NotEmpty(expectedResult);
        Assert.True(expectedResult.SequenceEqual(buffer));
        buffer.Clear();
    }

"""));
    }

    public string GetCStringMacro() => IsXcbStr switch
    {
        STRType.XcbByte or STRType.XcbInt or STRType.RawBuffer or STRType.XcbUint or STRType.XcbStr8 or STRType.XcbSegment
        or STRType.XcbRectangle or STRType.XcbArc or STRType.XcbPoient or STRType.XcbColorItem or STRType.XcbShort => "",
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

    public static string FillPassingParameter(int parameterCount, (string?, int)? replaceItems = null)
    {
        if (parameterCount == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (var i = 0; i < parameterCount; i++)
        {
            if (replaceItems.HasValue
                    && !string.IsNullOrWhiteSpace(replaceItems.Value.Item1)
                    && replaceItems.Value.Item2 == i)
                sb.Append(replaceItems.Value.Item1);
            else
                sb.Append("params").Append(i);
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
            "Xcsb.Models.Segment[]" or "Xcsb.Models.Rectangle[]" or "Xcsb.Models.Arc[]" or "Xcsb.Models.Point[]" or "Xcsb.Models.ColorItem[]" or "Xcsb.Models.Acceleration?" => "string",
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
                Arguments = $"-x c -o \"{execFile}\" - -lxcb -lxinput",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        System.Console.WriteLine(cMainBody);
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
    XcbShort,
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
    XcbPoient,
    XcbAtom,
    XcbColorItem
}