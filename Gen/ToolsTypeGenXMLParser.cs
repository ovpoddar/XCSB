#:sdk Microsoft.NET.Sdk
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System.Collections.Frozen;

var location = "/mnt/hardDrive/testP/xcb-proto/src";
// var location = "/usr/share/xcb/";
var generators = new List<Generator>();
if (!Directory.Exists(location))
{
    Console.WriteLine("Put the xproto.xml path.");
    location = Console.ReadLine();
    if (location == null) return;
}

var pathAttributes = File.GetAttributes(location);
if ((pathAttributes & FileAttributes.Directory) == FileAttributes.Directory)
{
    foreach (var file in Directory.EnumerateFiles(location, "*.xml", SearchOption.TopDirectoryOnly))
    {
        generators.Add(new Generator(Generator.OutPutType.Request, file));
    }
}
else if (File.Exists(location))
{
    generators.Add(new Generator(Generator.OutPutType.Request, location));
}
else
{
    Console.WriteLine("Fail to process the location.");
}


using var fs = File.Open("out.G.cs", FileMode.OpenOrCreate);
foreach (var gen in generators)
    gen.Write(fs);

System.Console.WriteLine("Written");

class Generator
{
    private readonly OutPutType _output;
    private Dictionary<string, XElement> _elements = new Dictionary<string, XElement>();
    private bool _isCoreLib;
    private string _headerName;
    public enum OutPutType
    {
        Struct,
        Event,
        XidType,
        Enum,
        Typedef,
        Request,
        Error
    }


    public Generator(OutPutType output, string path)
    {
        _output = output;
        var root = XDocument.Load(path).Root!;
        _isCoreLib = root.Attribute("extension-name") == null;
        _headerName = root.Attribute("header")!.Value;
        foreach (var element in root.Elements())
        {
            var name = element.Name.LocalName;

            var (expectedName, keyAttribute) = output switch
            {
                OutPutType.Struct => ("struct", "name"),
                OutPutType.Event => ("event", "name"),
                OutPutType.XidType => ("xidtype", "name"),
                OutPutType.Enum => ("enum", "name"),
                OutPutType.Typedef => ("typedef", "newname"),
                OutPutType.Request => ("request", "name"),
                OutPutType.Error => ("error", "name"),
                _ => throw new ArgumentOutOfRangeException(nameof(output), output, null)
            };

            var isCopy = (output == OutPutType.Event && name == "eventcopy") || (output == OutPutType.Error && name == "errorcopy");

            if (name != expectedName && !isCopy)
                continue;

            var value = isCopy
                ? _elements[element.Attribute("ref")!.Value]
                : element;

            _elements.Add(element.Attribute(keyAttribute)!.Value,
                value);
        }

    }
    // int WriteField(FileStream fs, XElement element, bool assign = false)
    // {
    //     if (element.Name.LocalName = "field")
    //     {
    //         var name = element.Attribute("name");
    //         var type = element.Attribute("enum") ?? element.Attribute("type");

    //         name.Value.EndsWith("_len") ? fs.Write("private "u8) : fs.Write("public "u8);
    //         fs.Write(Encoding.UTF8.GetBytes($"{type.Value} {name.Value}"));
    //         if (assign) fs.Write(Encoding.UTF8.GetBytes($" = {name.Value.ToLower()}"));
    //         fs.Write(";");
    //     }
    //     else if (element.Name.LocalName = "pad")
    //     {
    //         var size = element.Attribute("bytes")!.Value;
    //         var cType = (size) switch
    //         {
    //             "1" => "private byte "u8,
    //             "2" => "private ushort "u8,
    //             "4" => "private uint "u8,
    //             "8" => "private ulong "u8,
    //             _ => "private fixed byte"u8
    //         };
    //         fs.Write(cType);
    //         var name = (size) switch
    //         {
    //             "1" or "2" or "4" or "8" => "_pad;"u8,
    //             _ => Encoding.UTF8.GetBytes($"_pad[{size}];")
    //         };
    //         fs.Write(name);
    //     }
    //     else
    //     {
    //         throw NotImplementedException();
    //     }
    // }

    void WriteField(FileStream fs, XElement element, string? assign = null)
    {
        Debug.Assert(element.Name.LocalName == "field");
        Debug.Assert(element.Attribute("name") != null);
        Debug.Assert(element.Attribute("type") != null);

        var name = element.Attribute("name");
        var type = element.Attribute("enum") ?? element.Attribute("type");

        fs.Write(name!.Value.EndsWith("_len") ? "        private readonly"u8 : "        public readonly "u8);
        fs.Write(Encoding.UTF8.GetBytes(type!.Value).MapCsType());
        fs.Write(" "u8);
        fs.Write(Encoding.UTF8.GetBytes(name.Value).FixName());
        if (assign == "") fs.Write(Encoding.UTF8.GetBytes($" = {name.Value.ToLower()}"));
        if (assign?.Length != 0) fs.Write(Encoding.UTF8.GetBytes($" = {assign}"));
        fs.Write(";"u8);
        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
    }

    int WritePad(FileStream fs, XElement element, int index, bool assign = false)
    {
        Debug.Assert(element.Name.LocalName == "pad");
        if (element.Attribute("bytes") == null)
        {
            Debug.Assert(element.Attribute("align") != null);
            fs.Write("        // paded by x ammount"u8);
            fs.Write(Encoding.UTF8.GetBytes(element.Attribute("align") + Environment.NewLine));
            return 4;
        }

        var size = element.Attribute("bytes")!.Value;
        var cType = (size) switch
        {
            "1" => "        private readonly byte _pad"u8,
            "2" => "        private readonly ushort _pad"u8,
            "4" => "        private readonly uint _pad"u8,
            "8" => "        private readonly ulong _pad"u8,
            _ => "        private fixed byte _pad"u8
        };
        fs.Write(cType);
        fs.Write(Encoding.UTF8.GetBytes(index.ToString()));

        if (size != "1" && size != "2" && size != "4" && size != "8")
        {
            fs.Write(Encoding.UTF8.GetBytes($"[{size}];{Environment.NewLine}"));
        }
        else
        {
            if (assign) fs.Write(" = 0"u8);
            fs.Write(";"u8);
        }
        return int.Parse(size);
    }


    public void Write(FileStream fs)
    {
        switch (_output)
        {
            case OutPutType.Struct:
            case OutPutType.Event:
            case OutPutType.XidType:
            case OutPutType.Enum:
            case OutPutType.Typedef:
            case OutPutType.Request:
                {
                    fs.Write("namespace "u8);
                    fs.Write(Encoding.UTF8.GetBytes(_headerName + Environment.NewLine));
                    fs.Write("{"u8);
                    fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                    foreach (var element in _elements)
                    {
                        var fields = element.Value.Elements().ToList();
                        var hasNonStanderedPad = fields.Any(a => a.Name.LocalName == "pad" && a.Attribute("bytes")!.Value is "1" or "2" or "4" or "8");
                        var size = 32;// todo: calculate size;
                        var opCode = element.Value.Attribute("opcode")!.Value;
                        var name = element.Value.Attribute("name")!.Value;
                        var padLen = 0;
                        var writePos = 0;

                        fs.Write(Encoding.UTF8.GetBytes($"    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = {size})]" + Environment.NewLine));
                        fs.Write("    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]"u8);
                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        fs.Write(hasNonStanderedPad ? "    internal unsafe readonly struct "u8 : "    internal readonly struct "u8);
                        fs.Write(Encoding.UTF8.GetBytes(name + Environment.NewLine + "    {" + Environment.NewLine));

                        if (!_isCoreLib)
                        {
                            fs.Write("        public readonly byte MajorOpcode = majorOpCode;"u8);
                            fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                            writePos++;
                        }
                        fs.Write("        public readonly Opcode Opcode = Opcode."u8);
                        fs.Write(Encoding.UTF8.GetBytes($"{name};"));
                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        writePos++;
                        if (_isCoreLib)
                        {
                            var fieldI = fields.FirstOrDefault();
                            if (fieldI == null)
                            {
                                fs.Write("        private readonly byte _pad"u8);
                                fs.Write(Encoding.UTF8.GetBytes($"{padLen++} = 0;" + Environment.NewLine));
                            }
                            else if (fieldI.Name.LocalName == "pad")
                                WritePad(fs, fieldI, padLen++, true);
                            else if (fieldI.Name.LocalName == "field")
                                WriteField(fs, fieldI, "");
                            writePos++;
                        }
                        var val = "";// todo: get all the lengths fields and make farthe calculation
                        fs.Write("        public readonly ushort Length = "u8);
                        fs.Write(Encoding.UTF8.GetBytes($"(ushort)({size / 4}{val});" + Environment.NewLine));
                        writePos += 2;

                        foreach (var field in fields.Skip(_isCoreLib ? 1 : 0))
                        {
                            switch (field.Name.LocalName)
                            {
                                case "field":
                                    WriteField(fs, field, "");
                                    break;
                                case "pad":
                                    WritePad(fs, field, padLen, true);
                                    break;
                                    //     case "list":
                                    //         break;
                                    //     case "reply":
                                    //         continue;
                                    //         break;
                                    //     case "fd":
                                    //     case "required_start_align":
                                    //         //todo: handle
                                    //         break;
                                    //     default:
                                    //         throw new NotImplementedException(field.Name.LocalName);
                            }
                        }

                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine + "    }" + Environment.NewLine));
                    }
                    fs.Write("}"u8);
                    fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                }
                break;
            case OutPutType.Error:
                throw new NotImplementedException();
        }

    }
}
static class TempI
{

    private static readonly FrozenDictionary<string, byte[]> _map = new Dictionary<string, byte[]>
    {
        { "uint8_t", "byte"u8.ToArray() },
        { "BOOL", "bool"u8.ToArray() },
        { "INT8", "sbyte"u8.ToArray() },
        { "uint16_t", "ushort"u8.ToArray() },
        { "int16_t", "short"u8.ToArray() },
        { "xcb_window_t", "uint"u8.ToArray() },
        { "xcb_atom_t", "ATOM"u8.ToArray() },
    }.ToFrozenDictionary();


    public static int GetCsSize(this string value) => value switch
    {
        "CARD8" or "BOOL" or "BYTE" or "INT8" or "KEYCODE" => 1,
        _ => 4
    };


    public static ReadOnlySpan<byte> MapCsType(this ReadOnlySpan<byte> value)
    {
        // todo: pick data from processed source;
        string key = Encoding.UTF8.GetString(value);
        if (_map.TryGetValue(key, out var result))
            return result;
        return value;
    }

    public static byte[] FixName(this ReadOnlySpan<byte> value)
    {
        var result = new byte[value.Length - value.Count((byte)'_')];
        var wIndex = 0;
        var isUpperCase = true;
        foreach (var item in value)
        {
            if (item == '_')
            {
                isUpperCase = true;
                continue;
            }

            if (isUpperCase)
            {
                var tempI = item;
                result[wIndex] = tempI &= 0xDF;
                wIndex++;
                isUpperCase = false;
            }
            else
            {
                result[wIndex] = item;
                wIndex++;
            }
        }

        return result;
    }
}