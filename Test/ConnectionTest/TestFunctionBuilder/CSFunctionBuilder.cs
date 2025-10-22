using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTest.TestFunctionBuilder;

internal class CSFunctionBuilder : BaseTestBuilder
{
    private const string rawCsProjContent =
        $"""
         <Project Sdk="Microsoft.NET.Sdk">
             <PropertyGroup>
                  <OutputType>Exe</OutputType>
                  <TargetFramework>net9.0</TargetFramework>
                  <ImplicitUsings>enable</ImplicitUsings>
             </PropertyGroup>
             
             <ItemGroup>
               <Reference Include="Xcsb">
                <HintPath>Xcsb.dll</HintPath>
               </Reference>
             </ItemGroup>
         </Project>
         """;

    private string GenerateTestXCB()
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

    string GenerateVoidProgramFile(string functionName, params int[] arguments) =>
        $"""
         using Xcsb;

         var xcsb = XcsbClient.Initialized();
         xcsb.{functionName}({(arguments.Length == 0 ? "" : string.Join(", ", arguments))});
         return 0;
         """;

    public override Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments)
    {
        var compiler = GenerateTestXCB();
        var projectDir = CompileXCBWithCustomeFlag(compiler);
        var executable = CreateApplication(compiler, projectDir, functionName, arguments);
        Assert.True(File.Exists(executable));

        return null;
    }

    private string CompileXCBWithCustomeFlag(string compiler)
    {
        var xcbProjectPath = XCBProjectPath();
        Assert.NotEmpty(Directory.GetFiles(xcbProjectPath, "*.csproj"));
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"build -p:FLAG=\"DEBUGSEND;\" -o \"{GetWorkingFolder}\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = xcbProjectPath
            }
        };
        process.Start();
        process.WaitForExit();
        return GetWorkingFolder;
    }

    private string CreateApplication(string compiler, string projectDir, string functionName, params int[] arguments)
    {
        File.WriteAllText(
            Path.Join(projectDir, "Program.cs"),
            GenerateVoidProgramFile(functionName, arguments)
        );
        File.WriteAllText(
            Path.Join(projectDir, "Xcsb.csproj"),
            rawCsProjContent
        );

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"build -o \"{projectDir}\" -v quiet",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = projectDir
            }
        };
        process.Start();
        process.WaitForExit();
        return projectDir;
    }

    public CSFunctionBuilder() : base("csdir")
    {
    }

    private static string XCBProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory().AsSpan();
        var binFolder = currentDirectory.LastIndexOf("/Test/", StringComparison.CurrentCulture);
        Assert.True(binFolder != -1);
        var workingDirectory = currentDirectory[..binFolder];

        return Path.Join(workingDirectory, "Src");
    }
}