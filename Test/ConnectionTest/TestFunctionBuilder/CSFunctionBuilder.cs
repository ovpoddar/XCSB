using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTest.TestFunctionBuilder;

internal class CSFunctionBuilder : BaseTestBuilder
{
    private readonly SetupTestEnviroment _setupTestEnviroment;

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
                <HintPath>../Xcsb.dll</HintPath>
               </Reference>
             </ItemGroup>
         </Project>
         """;


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
        var compiler = _setupTestEnviroment.CSCompailer;
        var executable = CreateApplication(compiler, GetWorkingFolder, functionName, arguments);
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

    public CSFunctionBuilder(SetupTestEnviroment setupTestEnviroment) : base("csdir")
    {
        _setupTestEnviroment = setupTestEnviroment;
    }

}