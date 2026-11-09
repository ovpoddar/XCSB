#:sdk Microsoft.NET.Sdk
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System.Collections.Frozen;
using System.Xml;
using System.ComponentModel;
using System.Formats.Asn1;

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
    using var fs = File.Open("out.G.cs", FileMode.OpenOrCreate);
    foreach (var i in Directory.EnumerateFiles(location, "*.xml", SearchOption.TopDirectoryOnly))
    {
        var gen = new Generator(i);
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
    private readonly List<(OutPutType, XElement, bool)> _elements;
    private readonly Dictionary<string, (string csName, int length, bool isFixed)> _buildTypeSystem;
    private readonly string _workingDir;
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
        Error,
        Import,
        Union,
        XIdUnion,
        Ignore
    }


    public Generator(string path)
    {
        _workingDir = Path.GetDirectoryName(path) ?? throw new Exception();
        _elements = new List<(OutPutType, XElement, bool)>();
        _buildTypeSystem = new Dictionary<string, (string csName, int length, bool isFixed)>()
        {
            { "CARD8", ("byte", 1, true) },
            { "CARD16", ("ushort", 2, true)},
            { "CARD32", ("uint", 4, true)},
            { "CARD64", ("ulong", 8, true)},
            { "INT8", ("sbyte", 1, true)},
            { "INT16", ("short", 2, true)},
            { "INT32", ("int", 4, true)},
            { "INT64", ("long", 8, true)},
            { "BYTE", ("byte", 1, true)},
            { "BOOL", ("bool", 1, true)},
            { "char", ("byte", 1, true)},
            { "float", ("float", 4, true)},
            { "double", ("double", 8, true)},

            /**/{ "fd", ("int", 4, true)},
        };

        using (var reader = XmlReader.Create(path))
        {
            reader.ReadToFollowing("xcb");
            _isCoreLib = reader.GetAttribute("extension-name") == null;
            _headerName = reader.GetAttribute("header")!;
        }
        BuildTypes(Path.GetFileName(path), _elements, _buildTypeSystem, true);
    }

    void BuildTypes(string fileName, List<(OutPutType, XElement, bool)> elements,
        Dictionary<string, (string csName, int length, bool isFixed)> typeInfo,
        bool track)
    {
        var path = Path.Join(_workingDir, fileName);
        var root = XDocument.Load(path).Root!;
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
                BuildTypes(element.Value + ".xml", elements, typeInfo, false);
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
                Debug.Assert(unionName != null);
                (string csName, int length, bool isFixed) len = default;
                foreach (var item in element.Elements())
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

            elements.Add((output, value, track));
        }
    }

    void WriteField(FileStream fs, XElement element, string typeAssing = "", string? assign = null)
    {
        Debug.Assert(element.Name.LocalName == "field");
        Debug.Assert(element.Attribute("name") != null);
        Debug.Assert(element.Attribute("type") != null);

        var name = element.Attribute("name");
        var type = element.Attribute("enum") ?? element.Attribute("type");

        fs.Write(name!.Value.EndsWith("_len") ? "        private readonly "u8 : "        public readonly "u8);
        var csType = typeAssing == "" ? _buildTypeSystem[type!.Value.Split(':').Last()].csName : typeAssing;
        fs.Write(Encoding.UTF8.GetBytes(csType));
        fs.Write(" "u8);
        fs.Write(Encoding.UTF8.GetBytes(name.Value).FixName());
        if (assign is not null)
        {
            var temp = assign.Length == 0
                ? name.Value.ToLower().Fix()
                : assign;

            fs.Write(Encoding.UTF8.GetBytes($" = {temp}"));
        }
        fs.Write(";"u8);
        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
    }
    static void CalculateStructureLength(XElement element, Dictionary<string, (string csName, int length, bool isFixed)> typeInfo)
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
                        var type = items.Attribute("type")!.Value.Split(':').Last();
                        if (!typeInfo.TryGetValue(type, out var val))
                        {
                            val = typeInfo[type];
                        }
                        len += val.length;
                    }
                    break;
                case "pad":
                    {
                        var size = items.Attribute("bytes")?.Value;
                        if (size != null)
                            len += int.Parse(size);

                    }
                    break;
                case "list":
                    isFixed = false;
                    break;
                case "switch":
                    if (items.Element("fieldref") != null)
                        isFixed = false;
                    break;
                case "exprfield":
                    {
                        Debug.Assert(items.Attribute("type") != null);
                        var val = typeInfo[items.Attribute("type")!.Value];
                        len += val.length;
                    }
                    break;
                case "fd":
                    {
                        len += 4;
                    }
                    break;
                case "doc":
                case "reply":
                case "required_start_align":
                    break;
                default:
                    // System.Console.WriteLine(items.Name.LocalName + " " + name); // [i. e.] fd find doc how it should work
                    break;
                    // throw new NotImplementedException();
            }
        }
        typeInfo.TryAdd(name, (name, len, isFixed));
    }

    static void CalculateUnionLength(XElement element, Dictionary<string, (string csName, int length, bool isFixed)> typeInfo)
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
                        var type = items.Attribute("type");
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
                    System.Console.WriteLine(items.Name.LocalName + " " + name);
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
                    foreach (var element in _elements.Where(a => a.Item1 == OutPutType.Request && a.Item3 == true))
                    {
                        var fields = element.Item2.Elements().ToList();
                        var hasStanderedPad = fields.Any(a => a.Name.LocalName == "pad" && a.Attribute("bytes")!.Value is "1" or "2" or "4" or "8");
                        var name = element.Item2.Attribute("name")!.Value;
                        var opCode = element.Item2.Attribute("opcode")!.Value;
                        var size = _buildTypeSystem[name].length + (_isCoreLib ? 3 : 4);
                        size = size < 4 ? 4 : size;
                        var padCount = 0;
                        fs.Write(Encoding.UTF8.GetBytes($"    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = {size})]" + Environment.NewLine));
                        fs.Write("    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]"u8);
                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        fs.Write(hasStanderedPad ? "    internal unsafe readonly struct "u8 : "    internal readonly struct "u8);
                        IEnumerable<string> list = [(_isCoreLib ? string.Empty : "byte majorOpCode"), .. fields.Where(a => a.Name.LocalName == "field").Select(b =>
                                $"{_buildTypeSystem[b.Attribute("enum")?.Value ?? b.Attribute("type")?.Value.Split(':').Last() ?? throw new Exception()].csName} {b.Attribute("name")!.Value.Fix()}")
                                                    ];
                        fs.Write(Encoding.UTF8.GetBytes(name
                            + "Type("
                            + string.Join(", ", list.Where(a => a != string.Empty))
                            + ')'
                            + Environment.NewLine
                            + "    {" + Environment.NewLine));

                        if (!_isCoreLib)
                        {
                            fs.Write("        public readonly byte MajorOpcode = majorOpCode;"u8);
                            fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        }
                        fs.Write("        public readonly Opcode Opcode = Opcode."u8);
                        fs.Write(Encoding.UTF8.GetBytes($"{name};"));
                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        if (_isCoreLib)
                        {
                            var fieldI = fields.FirstOrDefault();
                            if (fieldI == null || fieldI.Name.LocalName == "reply")
                                Pad.WritePad(fs, Pad.PadType.Pad, 1, padCount++, true);
                            else if (fieldI.Name.LocalName == "pad")
                                Pad.WritePad(fs, fieldI, padCount++, true);
                            else if (fieldI.Name.LocalName == "field")
                                WriteField(fs, fieldI, "", "");
                        }
                        var listItem = fields
                            .Where(a => a.Name.LocalName == "list"
                                && a.Attribute("type") != null
                                && a.Element("fieldref") != null
                                && _buildTypeSystem.ContainsKey(a.Attribute("type")!.Value))
                                .ToArray();
                        var val = new StringBuilder();
                        if (listItem.Length != 0)
                        {
                            var fieldNames = listItem.Select(a => a.Element("fieldref")!.Value).ToArray(); // field len targeting field
                            var fieldInfo = listItem.Select(a => _buildTypeSystem[a.Attribute("type")!.Value].length).ToArray();// type
                            Debug.Assert(fieldNames.Length == fieldInfo.Length);

                            for (var i = 0; i < fieldInfo.Length; i++)
                            {
                                switch (fieldInfo[i])
                                {
                                    case 1:
                                        val.Append(" + ").Append(fieldNames[i]).Append(".AddPadding() / 4");
                                        break;
                                    case 2:
                                        val.Append(" + ").Append(fieldNames[i]).Append(".AddPadding() / 2");
                                        break;
                                    case 4:
                                        val.Append(" + ").Append(fieldNames[i]);
                                        break;
                                    case 8:
                                        val.Append(" + ").Append(fieldNames[i]).Append(" / 2");
                                        break;
                                    case 9:
                                        val.Append(" + ").Append(fieldNames[i]).Append(".AddPadding() / 3");
                                        break;
                                    case 12:
                                        val.Append(" + ").Append(fieldNames[i]).Append(" / 3");
                                        break;
                                    case 20:
                                        val.Append(" + ").Append(fieldNames[i]).Append(" / 5");
                                        break;
                                    case 24:
                                        val.Append(" + ").Append(fieldNames[i]).Append(" / 6");
                                        break;
                                    default:
                                        throw new Exception(fieldNames[i] + " " + fieldInfo[i].ToString());
                                }
                            }
                        }
                        fs.Write("        public readonly ushort Length = "u8);
                        fs.Write(Encoding.UTF8.GetBytes($"(ushort)({Math.Ceiling((double)(size / 4))}{val});" + Environment.NewLine));

                        foreach (var field in fields.Skip(_isCoreLib ? 1 : 0))
                        {
                            switch (field.Name.LocalName)
                            {
                                case "field":
                                    {
                                        // for bool to processing
                                        var type = field.Attribute("type");
                                        var fieldName = field.Attribute("name");
                                        if (_buildTypeSystem[type!.Value.Split(':').Last()].csName == "bool")
                                            WriteField(fs, field, "byte", $"{fieldName!.Value} ? 1 : 0");
                                        else
                                            WriteField(fs, field, "", "");
                                    }
                                    break;
                                case "pad":
                                    Pad.WritePad(fs, field, padCount++, true);
                                    break;
                                    // case "list":
                                    //     if (field.Elements().Any() && field.Element("value") != null)
                                    //         System.Console.WriteLine(field.ToString());
                                    //     if (field.Elements().Any() && field.Element("fieldref") != null)
                                    //         fs.Write("// has ref and list"u8);
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

                        size %= 4;
                        if (size != 0)
                        {
                            Pad.WritePad(fs, Pad.PadType.Pad, size, padCount, true);
                        }
                        fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine + "    }" + Environment.NewLine));
                    }
                    fs.Write("}"u8);
                    fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                    fs.Flush();
                }
                break;
            case OutPutType.Error:
                throw new NotImplementedException();
        }

    }

    static class Pad
    {
        public static int WritePad(FileStream fs, XElement element, int index, bool assign = false)
        {
            Debug.Assert(element.Name.LocalName == "pad");
            if (element.Attribute("bytes") == null)
            {
                var align = element.Attribute("align");
                Debug.Assert(align != null);
                WritePad(fs, PadType.Align, int.Parse(align.Value), index, assign);
                return 4;
            }
            var size = element.Attribute("bytes");
            Debug.Assert(size != null);
            var len = int.Parse(size!.Value);
            WritePad(fs, PadType.Pad, len, index, assign);
            return len;
        }

        public enum PadType
        {
            Align,
            Pad
        }

        public static void WritePad(FileStream fs, PadType type, int length, int index, bool assign)
        {
            if (type is PadType.Align)
            {
                fs.Write("        // paded by x ammount"u8);
                fs.Write(Encoding.UTF8.GetBytes(length + Environment.NewLine));
                return;
            }
            else if (type is PadType.Pad)
            {
                var cType = length switch
                {
                    1 => "        private readonly byte _pad"u8,
                    2 => "        private readonly ushort _pad"u8,
                    4 => "        private readonly uint _pad"u8,
                    8 => "        private readonly ulong _pad"u8,
                    _ => "        private fixed byte _pad"u8
                };
                fs.Write(cType);
                fs.Write(Encoding.UTF8.GetBytes(index.ToString()));

                if (length is not 1 and not 2 and not 4 and not 8)
                    fs.Write(Encoding.UTF8.GetBytes($"[{length}];{Environment.NewLine}"));
                else
                {
                    if (assign) fs.Write(" = 0"u8);
                    fs.Write(";"u8);
                    fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                }
            }
            else
            {
                throw new NotImplementedException(type.ToString());
            }
        }
    }
}
static class TempI
{

    public static string Fix(this string value)
    {

        if (value == "class")
            return "@class";
        else if (value == "lock")
            return "@lock";
        else if (value == "event")
            return "@event";
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