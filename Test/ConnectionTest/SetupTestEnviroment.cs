using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ConnectionTest;

public class SetupTestEnviroment : IDisposable
{
    private static readonly string _workingDirectory = Path.Join(Path.GetTempPath(), "out");
    public string MoniterFile { get; }
    public string CCompiler { get; }
    public string CSCompailer { get; }

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

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public SetupTestEnviroment()
    {
        Assert.False(Directory.Exists(_workingDirectory));
        Directory.CreateDirectory(_workingDirectory,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);

        CCompiler = GetCCompiler();
        MoniterFile = GenerateMoniterFile(CCompiler);
        CSCompailer = GetCsCompailer();
        CompileXcbWithCustomFlag(CSCompailer);
    }

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
    private static string GetCsCompailer()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "which",
                Arguments = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
        var response = process.StandardOutput.ReadToEnd();
        if (!string.IsNullOrWhiteSpace(response))
        {
            return response;
        }

        if (File.Exists("/usr/share/dotnet"))
            return "/usr/share/dotnet/dotnet";
        if (File.Exists("/usr/local/share/dotnet"))
            return "/usr/local/share/dotnet/dotnet";
        if (File.Exists("/usr/lib/dotnet"))
            return "/usr/lib/dotnet/dotnet";
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dotnetDirectory = Path.Combine(homeDirectory, ".dotnet", "dotnet");
        if (File.Exists(dotnetDirectory))
            return dotnetDirectory;

        Assert.Fail("dotnet not found");
        return "";
    }
    private static string GenerateMoniterFile(string compailer)
    {
        var result = Path.Join(_workingDirectory, "moniterLog.so");
        Assert.False(Path.Exists(result));

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compailer,
                Arguments = $"-shared -fPIC -o \"{result}\" -ldl -x c -",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardInput.Write(_moniterContent);
        process.StandardInput.Close();
        process.WaitForExit();

        Assert.True(string.IsNullOrWhiteSpace(process.StandardError.ReadToEnd()));
        Assert.True(string.IsNullOrWhiteSpace(process.StandardOutput.ReadToEnd()));
        Assert.True(File.Exists(result));
        return result;
    }
    private static void CompileXcbWithCustomFlag(string compiler)
    {
        var currentDirectory = Directory.GetCurrentDirectory().AsSpan();
        var binFolder = currentDirectory.LastIndexOf("/Test/", StringComparison.CurrentCulture);
        Assert.True(binFolder != -1);
        var workingDirectory = currentDirectory[..binFolder];
        var xcbProjectPath =  Path.Join(workingDirectory, "Src");
        var csproj = Directory.GetFiles(xcbProjectPath, "*.csproj", SearchOption.TopDirectoryOnly);
        Assert.NotEmpty(csproj);
        
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"publish \"{csproj[0]}\" -p:FLAG=\"DEBUGSEND;\" -o \"{_workingDirectory}\" -c Release -f net8.0 --sc",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
        Assert.True(File.Exists(Path.Join(_workingDirectory, "Xcsb.dll")));
    }

    public void Dispose()
    {
        Directory.Delete(_workingDirectory, true);
    }
}