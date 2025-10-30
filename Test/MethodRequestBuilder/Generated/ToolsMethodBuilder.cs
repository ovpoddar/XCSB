#:sdk Microsoft.NET.Sdk

using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
// Global Set Up
var compiler = GetCCompiler();
var monitorFile = GenerateMonitorFile(compiler);

// No Parameter Methods Set Up
MethodDetails[] noParamMethod = [new("NoParameter", "GrabServer", [""]), new("NoParameter", "UngrabServer", [""])];
// independentMethod ["UngrabPointer", "UngrabKeyboard", "AllowEvents", "SetFontPath", "Bell",
// "ChangeKeyboardControl", "ChangePointerControl", "SetScreenSaver", "ForceScreenSaver", "ChangeHosts", "SetAccessControl",
// "SetCloseDownMode", "NoOperation"];
using (var fileStream = File.Open("./NoParameter.cs", FileMode.OpenOrCreate))
{
    fileStream.Write(
    """
// DO NOT MODIFY THIS
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
    foreach (var method in noParamMethod)
        WriteCsMethodContent(method, fileStream);
    fileStream.Write(
    """
    public void Dispose() => 
        _xProto.Dispose();
}
"""u8);
}

// Global Clean Up
File.Delete(monitorFile);
return 0;

void WriteCsMethodContent(MethodDetails method, FileStream fileStream)
{
    fileStream.Write(
"""

    [Theory]
"""u8);
    foreach (var testCase in method.Parameters)
    {
        var cResponse = GetCResult(compiler, method.MethodName, testCase.ToCParams(), monitorFile);
        fileStream.Write(GetDataAttribute(testCase, cResponse));
    }
    fileStream.Write(Encoding.UTF8.GetBytes(
$$"""
    public void {{method.Categories.ToSnakeCase()}}_{{method.MethodName.ToSnakeCase()}}_test(
    //todo
    
    byte[] expectedResult)
    {
        // arrange
        var workingField = typeof(Xcsb.Handlers.BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.{{method.MethodName}}(
        // todo:
        
        );
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

static Span<byte> GetDataAttribute(string parameter, string cResponse) =>
    Encoding.UTF8.GetBytes(
$$"""
    [InlineData({{(string.IsNullOrWhiteSpace(parameter) ? "" : parameter + ",")}} new byte[] { {{cResponse}} })]
""");

static string GetCResult(string compiler, string method, string? parameter, string monitorFile)
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

    public static string? ToCParams(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var items = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        foreach (var field in items)
        {
            if (field.StartsWith('"') && field.EndsWith('"'))
            {
                sb.Append(" ," + (field.Length - 2));
            }

            sb.Append(", " + field);
        }
        return sb.ToString();
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

file record MethodDetails(string Categories, string MethodName, string[] Parameters);