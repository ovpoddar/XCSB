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
    private readonly SetupTestEnviroment _enviroment;

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

    private static string NonVoidMethodTemplate(string functionName, int[] arguments) =>
        $$"""
        #include <xcb/xcb.h>
        #include <stdlib.h>
        #include <stdio.h>
        #include <unistd.h>
        #include <assert.h>

        int main()
        {
            xcb_connection_t * connection = xcb_connect(NULL, NULL);
            if (xcb_connection_has_error(connection))
            {
                return -1;
            }
            const xcb_setup_t *setup = xcb_get_setup(connection);
            xcb_screen_t *screen = xcb_setup_roots_iterator(xcb_get_setup(connection)).data;
            fprintf(stderr, "------------\n");
            {{functionName}}_cookie_t cookie = {{functionName}}(connection{{(arguments.Length == 0 ? "" : ", " + string.Join(", ", arguments))}});
            xcb_flush(connection);
            fprintf(stderr, "------------\n");
            xcb_generic_error_t *error;
            {{functionName}}_reply_t *reply = {{functionName}}_reply(connection, cookie, &error);
            if (error)
            {
                free(error);
                return -1;
            }
            assert(reply->sequence =! 0);
            free(reply);
        }
        """;

    

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
            : NonVoidMethodTemplate(name, arguments);

        CreateProcess(compiler,
            $"-x c -o \"{outPutFile}\" - -lxcb",
            functionContent);
        Assert.True(File.Exists(outPutFile));
        return outPutFile;
    }

    protected override Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments)
    {
        var compiler = _enviroment.CCompiler;
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
        process.StartInfo.EnvironmentVariables["LD_PRELOAD"] = _enviroment.MoniterFile;
        return process;
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public CFunctionBuilder(SetupTestEnviroment enviroment) : base("cdir")
    {
        _enviroment = enviroment;
    }
}