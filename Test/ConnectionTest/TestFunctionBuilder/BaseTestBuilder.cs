using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace ConnectionTest.TestFunctionBuilder;

public abstract class BaseTestBuilder : IDisposable
{
    private static readonly string _workingDirectory = Path.Join(Path.GetTempPath(), "out");
    private const string _marker = "------------";
    private bool _disposedValue;

    protected string GetWorkingFolder;

    protected abstract Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments);

    public ReadOnlySpan<char> GetFunctionContent(string functionName, bool isVoidReturn, params int[] arguments)
    {
        var process = GetApplicationProcess(functionName, isVoidReturn, arguments);
        process.Start();
        var response = process.StandardError.ReadToEnd().AsSpan();
        var startIndex = response.IndexOf(_marker) + _marker.Length;
        var lastIndex = response.LastIndexOf(_marker);
        return response[startIndex..lastIndex];
    }


    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    protected BaseTestBuilder(string outPath)
    {
        GetWorkingFolder = Path.Join(_workingDirectory, outPath);

        if (Directory.Exists(GetWorkingFolder))
            Directory.Delete(GetWorkingFolder, true);
        Directory.CreateDirectory(GetWorkingFolder,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (Directory.Exists(_workingDirectory))
            Directory.Delete(_workingDirectory, true);
        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}