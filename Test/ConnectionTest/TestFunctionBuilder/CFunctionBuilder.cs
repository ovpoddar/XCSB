using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConnectionTest.TestFunctionBuilder;

internal class CFunctionBuilder : BaseTestBuilder
{
    private const string _moniterContent =
        $$"""
          #define _GNU_SOURCE
          #include <dlfcn.h>
          #include <stdio.h>
          #include <sys/uio.h>
          #include <sys/socket.h>
          #include <unistd.h>

          const char* SEND = "SEND";

          ssize_t (*real_write)(int, const void *, size_t) = NULL;
          ssize_t (*real_writev)(int, const struct iovec *, int) = NULL;
          ssize_t (*real_sendmsg)(int, const struct msghdr *, int) = NULL;

          __attribute__((constructor))
          void init() {
              real_write = dlsym(RTLD_NEXT, "write");
              real_writev = dlsym(RTLD_NEXT, "writev");
              real_sendmsg = dlsym(RTLD_NEXT, "sendmsg");
          }

          static void hex_dump(const char* callerType, const void *buf, size_t len) {
              const unsigned char *p = buf;
              fprintf(stderr, "[%s] :", callerType);
              for (size_t i = 0; i < len; ++i) {
                  // fprintf(stderr, "%02x ", p[i]);
                  fprintf(stderr, " %d", p[i]);
              }
              fprintf(stderr, "\n");
          }

          /****************************************************************/

          ssize_t write(int fd, const void *buf, size_t count) {
              hex_dump(SEND, buf, count);
              return real_write(fd, buf, count);
          }

          ssize_t writev(int fd, const struct iovec *iov, int iovcnt) {
              for (int i = 0; i < iovcnt; ++i) {
                  hex_dump(SEND, iov[i].iov_base, iov[i].iov_len);
              }
              return real_writev(fd, iov, iovcnt);
          }

          ssize_t sendmsg(int fd, const struct msghdr *msg, int flags) {
              for (int i = 0; i < msg->msg_iovlen; ++i) {
                  hex_dump(SEND, msg->msg_iov[i].iov_base, msg->msg_iov[i].iov_len);
              }
              return real_sendmsg(fd, msg, flags);
          }
                                
          """;

    private static string VoidMethodTemplate(string functionName, int[] arguments) =>
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
              fprintf(stderr, "------------\n");
              xcb_void_cookie_t cookie = {{functionName}}(connection{{(arguments.Length == 0 ? "" : ", " + string.Join(", ", arguments))}});
              xcb_flush(connection);
              fprintf(stderr, "------------\n");
              xcb_generic_error_t *error = xcb_request_check(connection, cookie);
              if (!error)
                  return 1;

              free(error);
              return -1;
          }
          """;

    private static string GetCCompiler()
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

        Assert.Fail("Could not find any compiler to build c project");
        return null;
    }

    private static void CreateProcess(string compiler, string command, string input)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = command,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardInput.Write(input);
        process.StandardInput.Close();
        process.WaitForExit();

        Assert.True(string.IsNullOrWhiteSpace(process.StandardError.ReadToEnd()));
        Assert.True(string.IsNullOrWhiteSpace(process.StandardOutput.ReadToEnd()));
    }

    private string GenerateMonitorFile(string compiler)
    {
        var monitorFile = Path.Join(this.GetWorkingFolder, "moniterLog.so");
        if (Path.Exists(monitorFile))
            File.Delete(monitorFile);
        CreateProcess(compiler,
            $"-shared -fPIC -o \"{monitorFile}\" -ldl -x c -",
            _moniterContent);
        Assert.True(File.Exists(monitorFile));
        return monitorFile;
    }

    private string GenerateExecutableFile(string compiler, string functionName, bool isVoidReturn,
        params int[] arguments)
    {
        var outPutFile = Path.Join(this.GetWorkingFolder, "main");
        if (Path.Exists(outPutFile))
            File.Delete(outPutFile);

        var name = functionName.Aggregate("xcb",
            (current, chars) => current + (char.IsUpper(chars) ? "_" + char.ToLower(chars) : chars));
        if (isVoidReturn)
            name += "_checked";
        var functionContent = isVoidReturn
            ? VoidMethodTemplate(name, arguments)
            : "assert(false)";

        CreateProcess(compiler,
            $"-x c -o \"{outPutFile}\" - -lxcb",
            functionContent);
        Assert.True(File.Exists(outPutFile));
        return outPutFile;
    }
   
    protected override Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments)
    {
        var compiler = GetCCompiler();
        var monitorFile = GenerateMonitorFile(compiler);
        var mainFile = GenerateExecutableFile(compiler, functionName, isVoidReturn, arguments);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = mainFile,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.EnvironmentVariables["LD_PRELOAD"] = monitorFile;
        return process;
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public CFunctionBuilder() : base("cdir")
    {
    }
}