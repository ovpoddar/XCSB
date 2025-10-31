#:sdk Microsoft.NET.Sdk

using System;
using System.IO;
using System.Runtime.InteropServices;


string xprotoFilePath;
if (File.Exists("/usr/include/xcb/xproto.h"))
    xprotoFilePath = "/usr/include/xcb/xproto.h";
else
{
    Console.WriteLine("Put the xproto.h path.");
    xprotoFilePath = Console.ReadLine()!;
}

var finalPath = Path.Join(Environment.CurrentDirectory, "ToolsProto.Generated.cs");
Span<byte> scratchBuffer = stackalloc byte[1];
(long, long) range = new();

using var wfs = new MemoryStream();
using (var fs = File.OpenRead(xprotoFilePath))
{
    wfs.Position = 0;
    Span<byte> typeDef = stackalloc byte[7];
    var isComment = false;
    var isEndName = false;
    var function = false;
    var lastIndex = 0L;
    var lastHighest = 0L;

    while (fs.CanRead && fs.Position != fs.Length && fs.Read(scratchBuffer) == scratchBuffer.Length)
    {
        switch (scratchBuffer[0])
        {
            case (byte)'/':
                if (fs.Read(scratchBuffer) == scratchBuffer.Length && scratchBuffer[0] == (byte)'*')
                    isComment = true;
                break;

            case (byte)'*':
                if (fs.Read(scratchBuffer) == scratchBuffer.Length && scratchBuffer[0] == (byte)'/')
                    isComment = false;
                else
                    fs.Seek(-1, SeekOrigin.Current);
                break;

            case (byte)';':
                if (!isComment)
                {
                    if (isEndName)
                    {
                        isEndName = false;
                        break;
                    }
                    if (function)
                    {
                        function = false;
                        break;
                    }

                    wfs.Write(scratchBuffer);
                }
                break;

            case (byte)'(':
                if (!isComment) function = true;
                break;

            case (byte)'}':
                if (!isComment)
                {
                    wfs.Write(scratchBuffer);
                    isEndName = true;
                    var length = wfs.Position - lastIndex;
                    if (lastHighest < length)
                    {
                        lastHighest = length;
                        range = new(lastIndex, wfs.Position);
                    }

                    lastIndex = wfs.Position;
                }
                break;

            case (byte)'t':
                if (!isComment && !isEndName && !function)
                {
                    var seek = fs.Position;
                    fs.ReadExactly(typeDef);
                    if (typeDef.SequenceEqual("ypedef "u8))
                    {
//todo: add more read to determine is it a enum or struct then for enum add more decoration
// for struct add the layout def
                        wfs.Write("public "u8);
                    }
                    else
                    {
                        wfs.Write(scratchBuffer);
                        fs.Seek(seek, SeekOrigin.Begin);
                    }
                }
                break;

            default:
                if (!isComment && !isEndName && !function)
                {
                    wfs.Write(scratchBuffer);
                }
                break;
        }
    }
}

using var ffs = File.OpenWrite(finalPath);


wfs.Position = 0;
ffs.Position = 0;
var isFirstCurlyBracePassed = false;
while (wfs.CanRead && wfs.Position != wfs.Length && wfs.Read(scratchBuffer) == scratchBuffer.Length)
{
    if (!isFirstCurlyBracePassed)
    {
        if (scratchBuffer[0] == (byte)'{')
            isFirstCurlyBracePassed = true;

        continue;
    }

    if (ffs.Position >= range.Item1 + 1 && ffs.Position <= range.Item2)
        continue;

    ffs.Write(scratchBuffer);
}

Console.WriteLine("this is not something ready to copy and paste, you still need to do some modification to make it work. this generation is for a head start");