using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTest.TestFunctionBuilder;

internal class CSFunctionBuilder : BaseTestBuilder
{
    private const string _rawCsProjContent =
        $"""
         <Project Sdk="Microsoft.NET.Sdk">
             <PropertyGroup>
                  <OutputType>Exe</OutputType>
                 <TargetFramework>net9.0</TargetFramework>
                 <ImplicitUsings>enable</ImplicitUsings>
                 <Nullable>enable</Nullable>
             </PropertyGroup>
             
             <ItemGroup>
               <Reference Include="Xcsb">
                <HintPath>Xcsb.dll</HintPath>
               </Reference>
             </ItemGroup>
         </Project>
         """;

    private static string GenerateTestXcb()
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

    private static string GenerateVoidProgramFile(string functionName, params int[] arguments) =>
        $"""
         using Xcsb;

         using var xcsb = XcsbClient.Initialized();
         Console.Error.WriteLine("------------");
         xcsb.{functionName}({(arguments.Length == 0 ? "" : string.Join(", ", arguments))});
         Console.Error.WriteLine("------------");
         """;

    protected override Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments)
    {
        var compiler = GenerateTestXcb();
        var projectDir = CompileXcbWithCustomeFlag(compiler);
        var executable = CreateApplication(compiler, projectDir, functionName, arguments);
        Assert.False(string.IsNullOrWhiteSpace(executable));

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = executable,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        return process;
    }

    private string CompileXcbWithCustomeFlag(string compiler)
    {
        var xcbProjectPath = XcbProjectPath();
        var csproj = Directory.GetFiles(xcbProjectPath, "*.csproj", SearchOption.TopDirectoryOnly);
        Assert.NotEmpty(csproj);
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"build \"{csproj[0]}\" -p:FLAG=\"DEBUGSEND;\" -o \"{GetWorkingFolder}\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
        return GetWorkingFolder;
    }

    private static string CreateApplication(string compiler, string projectDir, string functionName, params int[] arguments)
    {
        File.WriteAllText(
            Path.Join(projectDir, "Program.cs"),
            GenerateVoidProgramFile(functionName, arguments)
        );
        var csProjPath = Path.Join(projectDir, "Main.csproj");
        File.WriteAllText(
            csProjPath,
            _rawCsProjContent
        );
        projectDir = Path.Join(projectDir, "out");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"build \"{csProjPath}\" -o \"{projectDir}\" -v quiet",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        process.WaitForExit();
        return Directory.GetFiles(projectDir, "Main.dll", SearchOption.TopDirectoryOnly)[0];
    }

    public CSFunctionBuilder() : base("csdir")
    {
    }

    private static string XcbProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory().AsSpan();
        var binFolder = currentDirectory.LastIndexOf("/Test/", StringComparison.CurrentCulture);
        Assert.True(binFolder != -1);
        var workingDirectory = currentDirectory[..binFolder];

        return Path.Join(workingDirectory, "Src");
    }
}