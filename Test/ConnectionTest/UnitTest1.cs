
using System.Diagnostics;

namespace ConnectionTest;

public class UnitTest1
{
    [Theory]
    //[InlineData("CreateWindow")]
    //[InlineData("ChangeWindowAttributes")]
    //[InlineData("DestroyWindow")]
    //[InlineData("DestroySubwindows")]
    //[InlineData("ChangeSaveSet")]
    //[InlineData("ReparentWindow")]
    //[InlineData("MapWindow")]
    //[InlineData("MapSubwindows")]
    //[InlineData("UnmapWindow")]
    //[InlineData("UnmapSubwindows")]
    //[InlineData("ConfigureWindow")]
    //[InlineData("CirculateWindow")]
    //[InlineData("ChangeProperty")]
    //[InlineData("DeleteProperty")]
    //[InlineData("RotateProperties")]
    //[InlineData("SetSelectionOwner")]
    //[InlineData("ConvertSelection")]
    //[InlineData("SendEvent")]
    //[InlineData("UngrabPointer")]
    //[InlineData("GrabButton")]
    //[InlineData("UngrabButton")]
    //[InlineData("ChangeActivePointerGrab")]
    //[InlineData("UngrabKeyboard")]
    //[InlineData("GrabKey")]
    //[InlineData("UngrabKey")]
    //[InlineData("AllowEvents")]
    [InlineData("GrabServer")]
    //[InlineData("UngrabServer")]
    //[InlineData("WarpPointer")]
    //[InlineData("SetInputFocus")]
    //[InlineData("OpenFont")]
    //[InlineData("CloseFont")]
    //[InlineData("SetFontPath")]
    //[InlineData("CreatePixmap")]
    //[InlineData("FreePixmap")]
    //[InlineData("CreateGC")]
    //[InlineData("ChangeGC")]
    //[InlineData("CopyGC")]
    //[InlineData("SetDashes")]
    //[InlineData("SetClipRectangles")]
    //[InlineData("FreeGC")]
    //[InlineData("ClearArea")]
    //[InlineData("CopyArea")]
    //[InlineData("CopyPlane")]
    //[InlineData("PolyPoint")]
    //[InlineData("PolyLine")]
    //[InlineData("PolySegment")]
    //[InlineData("PolyRectangle")]
    //[InlineData("PolyArc")]
    //[InlineData("FillPoly")]
    //[InlineData("PolyFillRectangle")]
    //[InlineData("PolyFillArc")]
    //[InlineData("PutImage")]
    //[InlineData("ImageText8")]
    //[InlineData("ImageText16")]
    //[InlineData("CreateColormap")]
    //[InlineData("FreeColormap")]
    //[InlineData("CopyColormapAndFree")]
    //[InlineData("InstallColormap")]
    //[InlineData("UninstallColormap")]
    //[InlineData("FreeColors")]
    //[InlineData("StoreColors")]
    //[InlineData("StoreNamedColor")]
    //[InlineData("CreateCursor")]
    //[InlineData("CreateGlyphCursor")]
    //[InlineData("FreeCursor")]
    //[InlineData("RecolorCursor")]
    //[InlineData("ChangeKeyboardMapping")]
    //[InlineData("Bell")]
    //[InlineData("ChangeKeyboardControl")]
    //[InlineData("ChangePointerControl")]
    //[InlineData("SetScreenSaver")]
    //[InlineData("ForceScreenSaver")]
    //[InlineData("ChangeHosts")]
    //[InlineData("SetAccessControl")]
    //[InlineData("SetCloseDownMode")]
    //[InlineData("KillClient")]
    //[InlineData("NoOperation")]
    //[InlineData("PolyText8")]
    //[InlineData("PolyText16")]
    public void Test1(string methodName)
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.NotNull(methodName);
            return;
        }
        MakeProgram(methodName);
        Assert.NotNull(methodName);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private Process MakeProgram(string methodName)
    {
        var compiler = GetCCompiler();
        var tempLocation = Path.GetTempPath() + "out";
        if (!Directory.Exists(tempLocation))
            Directory.CreateDirectory(tempLocation, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);

        var monitorPath = Path.Join(tempLocation, "moniterLog.so");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "gcc",
                Arguments = $"-shared -fPIC -o \"{monitorPath}\" -ldl -x c -",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardInput.Write($$"""
            #define _GNU_SOURCE
            #include <dlfcn.h>
            #include <stdio.h>
            #include <sys/uio.h>
            #include <sys/socket.h>
            #include <unistd.h>

            const char* RECEIVED = "RECV";
            const char* SEND = "SEND";

            ssize_t (*real_write)(int, const void *, size_t) = NULL;
            ssize_t (*real_writev)(int, const struct iovec *, int) = NULL;
            ssize_t (*real_sendmsg)(int, const struct msghdr *, int) = NULL;

            ssize_t (*real_read)(int, void *, size_t) = NULL;
            ssize_t (*real_readv)(int, const struct iovec *, int) = NULL;
            ssize_t (*real_recvmsg)(int, struct msghdr *, int) = NULL;

            __attribute__((constructor))
            void init() {
                real_write = dlsym(RTLD_NEXT, "write");
                real_writev = dlsym(RTLD_NEXT, "writev");
                real_sendmsg = dlsym(RTLD_NEXT, "sendmsg");

                real_read = dlsym(RTLD_NEXT, "read");
                real_readv = dlsym(RTLD_NEXT, "readv");
                real_recvmsg = dlsym(RTLD_NEXT, "recvmsg");
            }

            static void hex_dump(const char* callerType, const void *buf, size_t len) {
                const unsigned char *p = buf;
                fprintf(stderr, "[%s] : ", callerType);
                for (size_t i = 0; i < len; ++i) {
                    // fprintf(stderr, "%02x ", p[i]);
                    fprintf(stderr, "%d ", p[i]);
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
            /***************************************************************/

            ssize_t read(int fd, void *buf, size_t count) {
                ssize_t result = real_read(fd, buf, count);
                if (result > 0) {
                    hex_dump(RECEIVED, buf, result);
                }
                return result;
            }

            ssize_t readv(int fd, const struct iovec *iov, int iovcnt) {
                ssize_t result = real_readv(fd, iov, iovcnt);
                if (result > 0) {
                    ssize_t remaining = result;
                    for (int i = 0; i < iovcnt && remaining > 0; ++i) {
                        size_t to_dump = (remaining < iov[i].iov_len) ? remaining : iov[i].iov_len;
                        hex_dump(RECEIVED, iov[i].iov_base, to_dump);
                        remaining -= to_dump;
                    }
                }
                return result;
            }

            ssize_t recvmsg(int fd, struct msghdr *msg, int flags) {
                ssize_t result = real_recvmsg(fd, msg, flags);
                if (result > 0) {
                    ssize_t remaining = result;
                    for (size_t i = 0; i < msg->msg_iovlen && remaining > 0; ++i) {
                        size_t to_dump = (remaining < msg->msg_iov[i].iov_len) ? remaining : msg->msg_iov[i].iov_len;
                        hex_dump(RECEIVED, msg->msg_iov[i].iov_base, to_dump);
                        remaining -= to_dump;
                    }
                }
                return result;
            }
        
        """);
        process.StandardInput.Close();
        Debug.Assert(Path.Exists(monitorPath));

        var targetApp = Path.Join(tempLocation, "main");
        // todo targetapp path
        /*
         * var name = "xcb_";
        foreach (var chars in methodName)
        {
            if (!char.IsUpper(chars))
                continue;
            name += "_" + char.ToLower(chars);
        }
        name += "_checked";
        return $$"""
            #include <xcb/xcb.h>

            int main()
            {
                xcb_connection_t *connection = xcb_connect(NULL, NULL);
                if (xcb_connection_has_error(connection)) 
                {
                    return -1;
                }
                const xcb_setup_t *setup = xcb_get_setup(connection);
                //todo iter check
                xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
                // todo screen check
                xcb_void_cookie_t cookie = {{name}}(connection);
                xcb_generic_error_t *error = xcb_request_check(connection, cookie);
                if (!error)
                    return window;

                free(error);
                return -1;
            }
            """;
        */
        Debug.Assert(Path.Exists(monitorPath));


        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = targetApp,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.EnvironmentVariables["LD_PRELOAD"] = monitorPath;
        return process;


    }

    private static string GetCCompiler()
    {
        string[] compilerCommands = ["gcc", "clang"];
        foreach (var command in compilerCommands)
        {
            try
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
            catch (Exception)
            {
                continue;
            }
        }
        throw new Exception("No Compailer Found");
    }
}
