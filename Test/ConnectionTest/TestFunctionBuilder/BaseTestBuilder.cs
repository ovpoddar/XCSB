using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace ConnectionTest.TestFunctionBuilder;
public abstract class BaseTestBuilder : IDisposable
{
    private static readonly string _workingDirectory = Path.GetTempPath() + "out";
    private bool _disposedValue;

    public virtual string GetWorkingFolder => _workingDirectory;

    public abstract Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments);

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    protected BaseTestBuilder()
    {
        Assert.False(Directory.Exists(_workingDirectory));
        Directory.CreateDirectory(_workingDirectory,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            Directory.Delete(_workingDirectory, true);
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
