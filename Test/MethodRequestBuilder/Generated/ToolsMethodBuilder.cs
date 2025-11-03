#:sdk Microsoft.NET.Sdk

using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using System.Linq;

// Global Set Up
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
    new MethodDetails1("IndependentMethod", "SetFontPath", [$"new string[] {{ \"built-ins\" , \"{Environment.CurrentDirectory}\" }}", "new string[] {\"build-ins\"}"], ["string[]"], true, true, true),
    new MethodDetails1("IndependentMethod", "SetCloseDownMode", ["0", "1", "2"], ["Xcsb.Models.CloseDownMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeKeyboardControl", ["7, new uint[] {80, 90, 1200}"], ["Xcsb.Masks.KeyboardControlMask", "uint[]"], false),
    new MethodDetails1("IndependentMethod", "SetScreenSaver", ["5, 10, 1, 1", "0, 0, 0, 0", "2, 2, 0, 0"], ["short", "short", "Xcsb.Models.TriState", "Xcsb.Models.TriState"], false),
    new MethodDetails1("IndependentMethod", "ForceScreenSaver", ["1", "0"], ["Xcsb.Models.ForceScreenSaverMode"], false),
    new MethodDetails1("IndependentMethod", "SetAccessControl", ["1", "0"], ["Xcsb.Models.AccessControlMode"], false),
    new MethodDetails1("IndependentMethod", "ChangeHosts", ["0, 4, new byte[] {127, 0, 0, 1}" , "1, 4, new byte[] {127, 0, 0, 1}"], ["Xcsb.Models.HostMode", "Xcsb.Models.Family", "byte[]"], true),

    // new MethodDetails2("DependentOnWindow", "DestroyWindow", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "DestroySubwindows", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "ChangeSaveSet", ["1, $", "0, $"], ["Xcsb.Models.ChangeSaveSetMode", "uint"], false),
    // new MethodDetails2("DependentOnWindow", "MapWindow", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "MapSubwindows", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "UnmapWindow", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "UnmapSubwindows", ["$"], ["uint"], false),
    // new MethodDetails2("DependentOnWindow", "ConfigureWindow", ["$, 1, new uint[] {100}", "$, 2, new uint[] {100}", "$, 4, new uint[] {100}", "$, 1, new uint[] {100}", "$, 32, new uint[] {0}", "$, 111, new uint[] {100, 100, 500, 500, 0}"], ["uint", "Xcsb.Masks.ConfigureValueMask", "uint[]"], false),
    // new MethodDetails2("DependentOnWindow", "CirculateWindow", ["0, $", "1, $" ], ["Xcsb.Masks.Circulate", "uint"], false)
];

// new("IndependentMethod", "NoOperation", [""], []), // this is a special case official method, doesnt take any parameters but their is a 4n in x11 protocol
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
            else if (field.StartsWith('"'))
            {
                sb.Append(", XS(").Append(field).Append(')');
            }
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

file class MethodDetails1 : BaseBuilder, IBuilder
{
    public MethodDetails1(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall, bool isXcbStr = false, bool needCast = false) : base(categories, methodName, parameters,
        paramSignature)
    {
        AddLenInCCall = addLenInCCall;
        IsXcbStr = isXcbStr;
        NeedCast = needCast;
    }

    public bool AddLenInCCall { get; }
    public bool IsXcbStr { get; }
    public bool NeedCast { get; }

    public override string FillPassingParameter(int parameterCount)
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

    public override string GetTestMethodSignature(string[] paramsSignature)
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

    public override Span<byte> GetDataAttribute(string parameter, string cResponse, string[] paramSignature)
    {
        var veriables = "";
        if (!string.IsNullOrWhiteSpace(parameter))
        {
            foreach (var signature in paramSignature)
            {
                parameter = GetField(parameter, out var field);
                if (field.Trim().StartsWith("new"))
                    veriables += $"{field}, ";
                else
                    veriables += $"({signature}){field}, ";
            }
        }

        return Encoding.UTF8.GetBytes(
$$"""
    [InlineData({{veriables}} new byte[] { {{cResponse}} })]

""");
    }

    static string GetField(string parameter, out string field)
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

    public override string GetCResult(string compiler, string method, string? parameter, string monitorFile)
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

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = execFile,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.EnvironmentVariables["LD_PRELOAD"] = monitorFile;
        process.Start();
        var response = process.StandardError.ReadToEnd();
        var startIndex = response.IndexOf(MARKER) + MARKER.Length + 2;
        var lastIndex = response.LastIndexOf(MARKER) - 2;
        File.Delete(execFile);
        return response[startIndex..lastIndex];
    }

    static string GetCMethodBody(string method, string? parameter, ReadOnlySpan<char> marker)
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
    const xcb_setup_t *setup = xcb_get_setup(connection);
    xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
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

    public override string? GetCParams(string parameter) =>
        parameter.ToCParams(NeedCast, AddLenInCCall, IsXcbStr);
}
file class MethodDetails2 : BaseBuilder, IBuilder
{
    public MethodDetails2(string categories, string methodName, string[] parameters, string[] paramSignature,
        bool addLenInCCall) : base(categories, methodName, parameters, paramSignature)
    {
        AddLenInCCall = addLenInCCall;
    }

    public bool AddLenInCCall { get; }

    public override string FillPassingParameter(int parameterCount)
    {
        throw new NotImplementedException();
    }

    public override string? GetCParams(string parameter)
    {
        throw new NotImplementedException();
    }

    public override string GetCResult(string compiler, string method, string? parameter, string monitorFile)
    {
        throw new NotImplementedException();
    }

    public override Span<byte> GetDataAttribute(string parameter, string cResponse, string[] paramSignature)
    {
        throw new NotImplementedException();
    }

    public override string GetTestMethodSignature(string[] paramsSignature)
    {
        throw new NotImplementedException();
    }
}

file abstract class BaseBuilder
{
    public BaseBuilder(string categories, string methodName, string[] parameters, string[] paramSignature)
    {
        Categories = categories;
        MethodName = methodName;
        Parameters = parameters;
        ParamSignature = paramSignature;
    }

    public string Categories { get; }
    public string MethodName { get; }
    public string[] Parameters { get; }
    public string[] ParamSignature { get; }

    public abstract string FillPassingParameter(int parameterCount);
    public abstract string GetTestMethodSignature(string[] paramsSignature);
    public abstract Span<byte> GetDataAttribute(string parameter, string cResponse, string[] paramSignature);
    public abstract string GetCResult(string compiler, string method, string? parameter, string monitorFile);

    public abstract string? GetCParams(string parameter);

    public void WriteCsMethodContent(FileStream fileStream, string compiler, string monitorFile)
    {
        fileStream.Write(
"""

    [Theory]

"""u8);
        foreach (var testCase in Parameters)
        {
            var cResponse = GetCResult(compiler, MethodName, GetCParams(testCase), monitorFile);
            fileStream.Write(GetDataAttribute(testCase, cResponse, ParamSignature));
        }

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
}

file interface IBuilder
{
    void WriteCsMethodContent(FileStream stream, string compiler, string monitorFile);
}