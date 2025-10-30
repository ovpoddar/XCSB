using System.IO;

string[] noParamMethod = ["GrabServer", "UngrabServer", "GetInputFocus", "QueryKeymap", "GetFontPath", "ListExtensions",
"GetModifierMapping", "GetKeyboardControl", "GetPointerMapping", "GetPointerControl", "GetScreenSaver", "ListHosts"];
File.WriteAllText(
    "NoParameter.cs",
    """
    
    
    
    """
    )





var cCompiler = GetCCompiler();


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