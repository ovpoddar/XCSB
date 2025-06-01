using System.Runtime.InteropServices;

if (!File.Exists("/usr/include/xcb/xproto.h"))
        Console.WriteLine("Put the xproto.h path.");
var tempPath = Path.GetTempFileName();
{
    using var fs = File.OpenRead(
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? "/usr/include/xcb/xproto.h"
            : Console.ReadLine()!);
    using var wfs = File.OpenWrite(tempPath);
    wfs.Position = 0;
    Span<byte> scratchBuffer = stackalloc byte[1];
    bool isComment = false;
    while (fs.CanRead && fs.Position != fs.Length && fs.Read(scratchBuffer) == scratchBuffer.Length)
    {
        switch (scratchBuffer[0])
        {
            case (byte)'/':
                {
                    if (fs.Read(scratchBuffer) == scratchBuffer.Length
                        && scratchBuffer[0] == (byte)'*')
                        isComment = true;
                    break;
                }
            case (byte)'*':
                {
                    if (fs.Read(scratchBuffer) == scratchBuffer.Length
                        && scratchBuffer[0] == (byte)'/')
                        isComment = false;
                    else
                        fs.Seek(-1, SeekOrigin.Current);
                    break;
                }
            default:
                if (!isComment) wfs.Write(scratchBuffer);
                break;
        }
    }
}
{
    using var rfs = File.OpenRead(tempPath);
    using var wfs = File.OpenWrite(Path.Join(Environment.CurrentDirectory, "ToolsProto.cs"));
    rfs.Position = 0;
}