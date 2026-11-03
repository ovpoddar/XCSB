#:sdk Microsoft.NET.Sdk
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System.Collections.Frozen;

var location = "/mnt/hardDrive/testP/xcb-proto/src";
if (!Directory.Exists(location))
{
    Console.WriteLine("Put the Folder path.");
    location = Console.ReadLine();
    if (location == null) return;
}

var pathAttributes = File.GetAttributes(location);
if ((pathAttributes & FileAttributes.Directory) == FileAttributes.Directory)
{
    foreach (var i in Directory.EnumerateFiles(location, "*.xml", SearchOption.TopDirectoryOnly))
    {
        var gen = new Generator(i);
        using var fs = File.Open("out.G.cs", FileMode.OpenOrCreate);
        gen.Write(Generator.OutPutType.Request, fs);
    }
}
else
{
    Console.WriteLine("Fail to process the location.");
}




System.Console.WriteLine("Written");

class Generator
{
    private readonly List<(OutPutType, XElement)> _elements;
    private readonly Dictionary<string, (string csName, int length, bool isFixed)> _buildTypeSystem;
    private readonly string _workingDir;
    private bool _isCoreLib;
    private string? _headerName;
    public enum OutPutType
    {
        Struct,
        Event,
        XidType,
        Enum,
        Typedef,
        Request,
        Error,
        Import,
        Union,
        XIdUnion,
        Ignore
    }


    public Generator(string path)
    {
        _workingDir = Path.GetDirectoryName(path) ?? throw new Exception();
        _elements = new List<(OutPutType, XElement)>();
        _buildTypeSystem = new Dictionary<string, (string csName, int length, bool isFixed)>()
        {
            { "CARD8", ("sbyte", 1, true) },
            { "CARD16", ("ushort", 2, true)},
            { "CARD32", ("uint", 4, true)},
            { "CARD64", ("ulong", 8, true)},
            { "INT8", ("byte", 1, true)},
            { "INT16", ("short", 2, true)},
            { "INT32", ("int", 4, true)},
            { "INT64", ("long", 8, true)},
            { "BYTE", ("byte", 1, true)},
            { "BOOL", ("bool", 1, true)},
            { "char", ("byte", 1, true)},
            { "float", ("float", 4, true)},
            { "double", ("double", 8, true)},
            { "DRAWABLE", ("uint", 4, true)},

        };
        BuildTypes(Path.GetFileName(path), _elements, _buildTypeSystem);
    }

    void BuildTypes(string fileName, List<(OutPutType, XElement)> elements,
        Dictionary<string, (string csName, int length, bool isFixed)> typeInfo)
    {
        var path = Path.Join(_workingDir, fileName);
        var root = XDocument.Load(path).Root!;
        _isCoreLib = root.Attribute("extension-name") == null;
        _headerName = root.Attribute("header")!.Value;
        foreach (var element in root.Elements())
        {
            var name = element.Name.LocalName;
            var output = name switch
            {
                "struct" => OutPutType.Struct,
                "event" or "eventcopy" => OutPutType.Event,
                "xidtype" => OutPutType.XidType,
                "enum" => OutPutType.Enum,
                "typedef" => OutPutType.Typedef,
                "request" => OutPutType.Request,
                "error" or "errorcopy" => OutPutType.Error,
                "import" => OutPutType.Import,
                "union" => OutPutType.Union,
                "xidunion" => OutPutType.XIdUnion,
                "eventstruct" => OutPutType.Ignore,
                _ => throw new ArgumentOutOfRangeException(name)
            };
            if (output == OutPutType.Import)
            {
                BuildTypes(element.Value + ".xml", elements, typeInfo);
                continue;
            }

            var isCopy = name is "eventcopy" or "errorcopy";
            var value = element;
            if (isCopy)
            {
                var refAttribute = element.Attribute("ref");
                value = new XElement(elements.First(e => e.Item2.Attribute("name")?.Value == refAttribute?.Value).Item2);
                foreach (var attribute in element.Attributes().Where(a => a != refAttribute))
                    value.SetAttributeValue(attribute.Name.LocalName, attribute.Value);
            }

            if (output is OutPutType.XidType)
            {
                var typeName = element.Attribute("name")?.Value;
                Debug.Assert(typeName != null);
                typeInfo.TryAdd(typeName, (typeName, 4, true));
            }

            if (output is OutPutType.Typedef)
            {
                Debug.Assert(element.Attribute("oldname")?.Value != null);
                Debug.Assert(element.Attribute("newname")?.Value != null);

                var info = typeInfo[element.Attribute("oldname")!.Value];
                typeInfo.TryAdd(element.Attribute("newname")!.Value, info);
            }

            if (output is OutPutType.Enum)
            {
                var enumName = element.Attribute("name")?.Value;
                var items = element.Element("item");
                Debug.Assert(enumName != null);
                Debug.Assert(items != null);
                var firstItem = items.Elements().FirstOrDefault();
                Debug.Assert(firstItem != null);
                Debug.Assert(firstItem!.Name.LocalName != null);
                var len = firstItem!.Name.LocalName switch
                {
                    "value" => 4,
                    "bit" => 1,
                    _ => throw new Exception(firstItem!.Name.LocalName)
                };
                typeInfo.TryAdd(enumName, (enumName, len, true));
            }

            if (output is OutPutType.XIdUnion)
            {
                var unionName = element.Attribute("name")?.Value;
                Debug.Assert(unionName  != null);
                (string csName, int length, bool isFixed) len = default;
                foreach(var item in element.Elements())
                {
                    Debug.Assert(item.Name.LocalName == "type");
                    var details = typeInfo[item.Value.Split(':').Last()];
                    if (len.length < details.length)
                            len = details;
                }
                typeInfo.TryAdd(unionName, len);
            }

            if (output is OutPutType.Union)
                CalculateUnionLength(value, typeInfo);

            if (output is OutPutType.Event or OutPutType.Error or OutPutType.Struct or OutPutType.Request)
                CalculateStructureLength(value, typeInfo);

            elements.Add((output, value));
        }
    }

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

    void CalculateStructureLength(XElement element, Dictionary<string, (string csName, int length, bool isFixed)> typeInfo)
    {
        var name = element.Attribute("name")?.Value;
        Debug.Assert(name != null);
        var isFixed = true;
        var len = 0;
        foreach (var items in element.Elements())
        {
            switch (items.Name.LocalName)
            {
                case "field":
                    {
                        Debug.Assert(items.Attribute("name") != null);
                        Debug.Assert(items.Attribute("type") != null);
                        var type = items.Attribute("enum") ?? items.Attribute("type");
                        if (!typeInfo.TryGetValue(type!.Value.Split(':').Last(), out var val))
                        {
                            val = typeInfo[items.Attribute("type")!.Value];
                        }
                        len += val.length;
                    }
                    break;
                case "pad":
                    {
                        var size = element.Attribute("bytes")?.Value;
                        if (size != null)
                            len += int.Parse(size);

                    }
                    break;
                case "list":
                    isFixed = false;
                    break;
                case "doc":
                case "reply":
                case "switch":
                case "required_start_align":
                    break;
                default:
                    System.Console.WriteLine(items.Name.LocalName);
                    break;
                    // throw new NotImplementedException();
            }
        }
        typeInfo.TryAdd(name, (name, len, isFixed));
    }

    void CalculateUnionLength(XElement element, Dictionary<string, (string csName, int length, bool isFixed)> typeInfo)
    {
        var name = element.Attribute("name")?.Value;
        Debug.Assert(name != null);
        var len = 0;
        foreach (var items in element.Elements())
        {
            switch (items.Name.LocalName)
            {
                case "field":
                    {
                        Debug.Assert(items.Attribute("name") != null);
                        Debug.Assert(items.Attribute("type") != null);
                        var type = items.Attribute("enum") ?? items.Attribute("type");
                        if (!typeInfo.TryGetValue(type!.Value.Split(':').Last(), out var val))
                            val = typeInfo[items.Attribute("type")!.Value];
                        if (len < val.length)
                            len = val.length;
                    }
                    break;
                case "list":
                    {
                        Debug.Assert(items.Attribute("type") != null);
                        Debug.Assert(items.Element("value") != null);
                        var typeLen = typeInfo[items.Attribute("type")!.Value].length;
                        var value = items.Element("value")!.Value;
                        var val = typeLen * int.Parse(value);
                        if (len < val)
                            len = val;
                    }
                    break;
                default:
                    System.Console.WriteLine(items.Name.LocalName);
                    break;
            }
        }

        typeInfo.TryAdd(name, (name, len, true));
    }

    public void Write(OutPutType output, FileStream fs)
    {
        // for each file open the associate 
        switch (output)
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
                    foreach (var element in _elements.Where(a => a.Item1 == OutPutType.Request))
                    {
                        var fields = element.Item2.Elements().ToList();
                        var hasNonStanderedPad = fields.Any(a => a.Name.LocalName == "pad" && a.Attribute("bytes")!.Value is "1" or "2" or "4" or "8");
                        var name = element.Item2.Attribute("name")!.Value;
                        var opCode = element.Item2.Attribute("opcode")!.Value;
                        var size = _buildTypeSystem[name].length;
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
                                    // case "list":
                                    //     if (field.Elements().Any() && field.Element("value") != null)
                                    //         System.Console.WriteLine(field.ToString());
                                    //     break;
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