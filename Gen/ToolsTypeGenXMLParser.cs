#:sdk Microsoft.NET.Sdk
using System.Text;
using System.Xml.Linq;

var source = args.Length > 0 ? args[0] : "/mnt/hardDrive/testP/xcb-proto/src";
while (!Directory.Exists(source))
{
    Console.WriteLine("Put the Folder path.");
    source = Console.ReadLine();
    if (source == null) return;
}

const string I1 = "    ";
const string I2 = "        ";
const string I3 = "            ";

using (var stream = new StreamWriter("out.G.cs"))
{
    stream.WriteLine("using System;");
    stream.WriteLine("using System.Runtime.CompilerServices;");
    stream.WriteLine("using System.Runtime.InteropServices;");
    stream.WriteLine("using Xcsb.Connection.Helpers;");
    stream.WriteLine("using Xcsb.Connection.Models;");
    stream.WriteLine("using Xcsb.Connection.Response;");
    stream.WriteLine("using Xcsb.Connection.Response.Contract;");

    var text = new StringBuilder();
    foreach (var item in Directory.GetFiles(source, "*.xml").Order(StringComparer.Ordinal))
    {
        text.Clear();
        var reader = new ProtocolReader(item);
        text.AppendLine($"namespace {reader.Namespace}");
        text.AppendLine("{");
        foreach (var request in reader.Requests)
        {
            text.AppendLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1, Size = {request.StructSize})]");
            text.AppendLine($"{I1}[method: MethodImpl(MethodImplOptions.AggressiveInlining)]");
            text.AppendLine($"{I1}internal readonly struct {request.Name}Type({request.ParameterList})");
            text.AppendLine($"{I1}{{");
            foreach (var m in request.Declarations)
                text.AppendLine($"{I2}{m.Access} readonly {m.Type} {m.Name} = {m.Value};");
            text.AppendLine($"{I1}}}");
        }
        foreach (var reply in reader.Replies)
        {
            if (reply.Todo != null)
            {
                text.AppendLine($"{I1}{reply.Todo}");
                continue;
            }

            text.AppendLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1)]");
            text.AppendLine($"{I1}public readonly struct {reply.Name} : IXReply<{reply.Name}, {reply.Name}>, IVerify");
            text.AppendLine($"{I1}{{");
            text.AppendLine($"{I2}private readonly byte[] _data;");
            text.AppendLine();
            text.AppendLine($"{I2}private {reply.Name}(byte[] data)");
            text.AppendLine($"{I2}{{");
            text.AppendLine($"{I3}if (data is null) throw new ArgumentNullException(nameof(data));");
            text.AppendLine($"{I3}if (data.Length < {reply.MinSize})");
            text.AppendLine($"{I3}    throw new ArgumentException(\"Buffer too small for {reply.Name}.\", nameof(data));");
            text.AppendLine($"{I3}_data = data;");
            text.AppendLine($"{I2}}}");
            text.AppendLine();

            foreach (var line in reply.Header)
                text.Append(I2).AppendLine(line);
            foreach (var line in reply.Properties)
                text.Append(I2).AppendLine(line);

            if (reply.NeedsSumPop)
            {
                text.AppendLine();
                text.AppendLine($"{I2}private static int SumPop(ReadOnlySpan<uint> s)");
                text.AppendLine($"{I2}{{");
                text.AppendLine($"{I3}var n = 0;");
                text.AppendLine($"{I3}foreach (var v in s) n += v.CountFlags();");
                text.AppendLine($"{I3}return n;");
                text.AppendLine($"{I2}}}");
            }

            text.AppendLine();
            text.AppendLine($"{I2}public {reply.Name} FromBytes(byte[] response) => new {reply.Name}(response);");
            text.AppendLine($"{I1}}}");
            text.AppendLine();
        }
        foreach (var ev in reader.Events)
        {
            if (ev.Todo != null)
            {
                text.AppendLine($"{I1}{ev.Todo}");
                continue;
            }

            text.AppendLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1)]");
            text.AppendLine($"{I1}public readonly struct {ev.Name} : IXEvent<{ev.Name}>");
            text.AppendLine($"{I1}{{");
            text.AppendLine($"{I2}private readonly byte[] _data;");
            text.AppendLine($"{I2}public const byte Number = {ev.Number};");
            text.AppendLine();
            text.AppendLine($"{I2}private {ev.Name}(byte[] data)");
            text.AppendLine($"{I2}{{");
            text.AppendLine($"{I3}if (data is null) throw new ArgumentNullException(nameof(data));");
            text.AppendLine($"{I3}if (data.Length < {ev.MinSize})");
            text.AppendLine($"{I3}    throw new ArgumentException(\"Buffer too small for {ev.Name}.\", nameof(data));");
            text.AppendLine($"{I3}_data = data;");
            text.AppendLine($"{I2}}}");
            text.AppendLine();

            foreach (var line in ev.Header)
                text.Append(I2).AppendLine(line);
            foreach (var line in ev.Properties)
                text.Append(I2).AppendLine(line);

            if (ev.NeedsSumPop)
            {
                text.AppendLine();
                text.AppendLine($"{I2}private static int SumPop(ReadOnlySpan<uint> s)");
                text.AppendLine($"{I2}{{");
                text.AppendLine($"{I3}var n = 0;");
                text.AppendLine($"{I3}foreach (var v in s) n += v.CountFlags();");
                text.AppendLine($"{I3}return n;");
                text.AppendLine($"{I2}}}");
            }

            text.AppendLine();
            text.AppendLine($"{I2}public {ev.Name} FromBytes(byte[] response) => new {ev.Name}(response);");
            text.AppendLine($"{I1}}}");
            text.AppendLine();
        }

        var name = reader.Namespace.Split('.')[^1].ToPascalCase();
        text.AppendLine($"{I1}public interface I{name}VoidProto");
        text.AppendLine($"{I1}{{");
        foreach (var r in reader.Requests.Where(r => !r.HasReply))
        {
            List<string>? counts = null;
            foreach (var part in r.Signature.Trailing)
                if (part is ListOf { Length: FieldRef length })
                    (counts ??= []).Add(length.Name.ToCamelCase());

            List<string> methodArgs = [];
            foreach (var p in r.Signature.Parameters)
            {
                var paramName = p.Name.ToCamelCase();
                if (counts is null || !counts.Contains(paramName))
                    methodArgs.Add($"{p.Type} {paramName}");
            }

            string? todo = null;
            foreach (var part in r.Signature.Trailing)
            {
                switch (part)
                {
                    case Pad:
                        break;
                    case ListOf list:
                        var element = list.Type.CsName is "bool" or "char" or "void" ? "byte" : list.Type.CsName;
                        methodArgs.Add($"ReadOnlySpan<{element}> {list.Name.ToCamelCase()}");
                        break;
                    default:
                        var what = part switch
                        {
                            Field f => $"field '{f.Name}'",
                            Switch s => $"switch '{s.Name}'",
                            _ => "member",
                        };
                        todo ??= $"// TODO: {r.Name} has a {what} which can't be represented in c#";
                        break;
                }
            }

            var list2 = string.Join(", ", methodArgs);
            text.Append(I2).AppendLine(todo ?? $"ResponseProto {r.Name}({list2});");
        }
        text.AppendLine($"{I1}}}");

        text.AppendLine($"{I1}public interface I{name}ResponseProto");
        text.AppendLine($"{I1}{{");
        foreach (var r in reader.Requests.Where(r => r.HasReply))
        {
            List<string>? counts = null;
            foreach (var part in r.Signature.Trailing)
                if (part is ListOf { Length: FieldRef length })
                    (counts ??= []).Add(length.Name.ToCamelCase());

            List<string> methodArgs = [];
            foreach (var p in r.Signature.Parameters)
            {
                var paramName = p.Name.ToCamelCase();
                if (counts is null || !counts.Contains(paramName))
                    methodArgs.Add($"{p.Type} {paramName}");
            }

            string? todo = null;
            foreach (var part in r.Signature.Trailing)
            {
                switch (part)
                {
                    case Pad:
                        break;
                    case ListOf list:
                        var element = list.Type.CsName is "bool" or "char" or "void" ? "byte" : list.Type.CsName;
                        methodArgs.Add($"ReadOnlySpan<{element}> {list.Name.ToCamelCase()}");
                        break;
                    default:
                        var what = part switch
                        {
                            Field f => $"field '{f.Name}'",
                            Switch s => $"switch '{s.Name}'",
                            _ => "member",
                        };
                        todo ??= $"// TODO: {r.Name} has a {what} which can't be represented in c#";
                        break;
                }
            }

            var list2 = string.Join(", ", methodArgs);
            text.Append(I2).AppendLine(todo ?? $"ReplyLease<{r.Name}Reply> {r.Name}({list2});");
        }
        text.AppendLine($"{I1}}}");

        text.AppendLine($"{I1}public interface I{name}Request : I{name}ResponseProto, I{name}VoidProto");
        text.AppendLine($"{I1}{{{Environment.NewLine}{I1}}}");
        text.AppendLine("}");
        stream.Write(text);
    }
}

Console.WriteLine("Written");

sealed record Declaration(string Access, string Type, string Name, string Value);

readonly record struct WireType(string CsName, int Size, bool IsFixed, bool IsStruct = false);

sealed record ExtensionInfo(string XName, string Name, bool MultiWord, int MajorVersion, int MinorVersion);

abstract class NamedType
{
    public string Name { get; protected init; } = "";
    public WireType Type { get; protected init; }
}

sealed class AliasType : NamedType
{
    public string? OldName { get; }

    public AliasType(XElement e, ProtocolReader protocol)
    {
        OldName = e.Attribute("oldname")?.Value;
        Name = OldName is null ? e.Attr("name") : e.Attr("newname");
        Type = OldName is null ? new(Name, 4, true) : protocol.Resolve(OldName);
    }
}

sealed class XidUnionType : NamedType
{
    public string[] Types { get; }

    public XidUnionType(XElement e)
    {
        Name = e.Attr("name");
        Types = [.. e.Elements("type").Select(t => t.Value.Trim())];
        Type = new(Name, 4, true);
    }
}

sealed class StructType : NamedType
{
    public Member[] Members { get; }
    public AllowedEvents[]? Allowed { get; }

    public StructType(XElement e, ProtocolReader protocol)
    {
        Name = e.Attr("name");
        if (e.Name.LocalName == "eventstruct")
        {
            Members = [];
            Allowed = [.. e.Elements("allowed").Select(a => new AllowedEvents(a.Attr("extension"), a.BoolAttr("xge"),
                int.Parse(a.Attr("opcode-min")), int.Parse(a.Attr("opcode-max"))))];
            Type = new(Name, 32, true, true);
        }
        else
        {
            Members = protocol.ReadMembers(e.Elements());
            var (size, isFixed) = ProtocolReader.Measure(Members);
            Type = new(Name, size, isFixed, true);
        }
    }
}
sealed record AllowedEvents(string Extension, bool Xge, int OpcodeMin, int OpcodeMax);

sealed class UnionType : NamedType
{
    public Member[] Members { get; }

    public UnionType(XElement e, ProtocolReader protocol)
    {
        Name = e.Attr("name");
        Members = protocol.ReadMembers(e.Elements());
        
        var parts = Members.Select(m => ProtocolReader.Measure([m])).ToArray();
        Type = new(Name, parts.Select(p => p.Size).DefaultIfEmpty(0).Max(), parts.All(p => p.IsFixed), true);
    }
}

sealed class EnumType
{
    public string Name { get; }
    public EnumItem[] Items { get; }
    public Doc? Doc { get; }
    public int Width { get; set; } 
    public long this[string item] => Items.First(i => i.Name == item).Value;

    public EnumType(XElement e)
    {
        Name = e.Attr("name");
        Doc = ProtocolReader.DocOf(e);
        List<EnumItem> items = [];
        long next = 0;
        foreach (var item in e.Elements("item"))
        {
            int? bit = item.Element("bit") is { } b ? (int)ProtocolReader.ParseNumber(b.Value) : null;
            var value = bit is { } n ? 1L << n : item.Element("value") is { } v ? ProtocolReader.ParseNumber(v.Value) : next;
            next = value + 1;
            items.Add(new(item.Attr("name"), value, bit));
        }
        Items = [.. items];
    }
}
sealed record EnumItem(string Name, long Value, int? Bit); 

sealed class Error(XElement e, ProtocolReader protocol)
{
    public string Name { get; } = e.Attr("name");
    public int Number { get; } = int.Parse(e.Attr("number"));
    public string? Ref { get; } = e.Attribute("ref")?.Value;
    public Member[]? Members { get; } =
        (e.Name.LocalName == "error" ? e : e.Parent!.Elements("error").LastOrDefault(x => x.Attr("name") == e.Attr("ref").Local()))
            is { } source ? protocol.ReadMembers(source.Elements()) : null;
}

sealed record StartAlign(int Align, int Offset);

sealed record Doc(string? Brief, string? Description, string? Example, DocField[] Fields, DocError[] Errors, DocSee[] See);
sealed record DocField(string Name, string Text);
sealed record DocError(string Type, string Text);
sealed record DocSee(string Type, string Name);

abstract record Expr { }
sealed record Const(long Value) : Expr;
sealed record EnumRef(string Enum, string Item, long Value) : Expr;  
sealed record FieldRef(string Name) : Expr;
sealed record ParamRef(string Name, string Type) : Expr; 
sealed record Binary(string Op, Expr Left, Expr Right) : Expr;
sealed record Unary(string Op, Expr Operand) : Expr;
sealed record PopCount(Expr Operand) : Expr;
sealed record SumOf(string List, Expr? Body) : Expr;
sealed record ListElement : Expr { }
sealed record UnknownExpr(string Tag) : Expr;

sealed record EnumUse(string Kind, string Name);

abstract record Member { }
sealed record Pad(int? Bytes, int? Align, bool Serialize = false) : Member;
sealed record Field(string Name, string TypeName, WireType Type, EnumUse? EnumUse, Expr? Value) : Member 
{
    public string? Enum => EnumUse is { Kind: "enum" } use ? use.Name.Local() : null; 
}
sealed record ListOf(string Name, string TypeName, WireType Type, EnumUse? EnumUse, Expr? Length) : Member;
sealed record Fd(string Name) : Member;
sealed record FdList(string Name, Expr? Length) : Member;     
sealed record ValueParam(string MaskType, string MaskName, string ListName) : Member; 
sealed record Switch(string Name, Expr Selector, Case[] Cases, StartAlign? RequiredStartAlign) : Member;
sealed record Case(string? Name, bool IsBitCase, Expr[] Values, long Bits, Member[] Members,
    StartAlign? RequiredStartAlign, int Size, bool IsFixed);

sealed record Param(string Type, string Name);
sealed record Signature(Param[] Parameters, Member[] Trailing);


sealed class ProtocolReader
{
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, XElement> XmlCache = new();

    readonly string directory;
    readonly Dictionary<string, List<object>> loaded = []; 

    readonly Dictionary<string, WireType> types = new()
    {
        ["CARD8"] = new("byte", 1, true),
        ["CARD16"] = new("ushort", 2, true),
        ["CARD32"] = new("uint", 4, true),
        ["CARD64"] = new("ulong", 8, true),
        ["INT8"] = new("sbyte", 1, true),
        ["INT16"] = new("short", 2, true),
        ["INT32"] = new("int", 4, true),
        ["INT64"] = new("long", 8, true),
        ["BYTE"] = new("byte", 1, true),
        ["BOOL"] = new("bool", 1, true),
        ["char"] = new("byte", 1, true),
        ["void"] = new("byte", 1, true),
        ["float"] = new("float", 4, true),
        ["double"] = new("double", 8, true),
    };
    readonly Dictionary<string, EnumType> enums = [];

    public string Namespace { get; }
    public ExtensionInfo? Extension { get; }        
    public bool IsCore => Extension is null;
    public string[] Imports { get; }

    public EnumType[] Enums { get; }
    public AliasType[] Aliases { get; }             
    public XidUnionType[] XidUnions { get; }
    public StructType[] Structs { get; }            
    public UnionType[] Unions { get; }
    public Request[] Requests { get; }
    public Reply[] Replies { get; }
    public Event[] Events { get; }
    public Error[] Errors { get; }

    public ProtocolReader(string path)
    {
        directory = Path.GetDirectoryName(path)!;
        var file = Path.GetFileNameWithoutExtension(path);
        var root = Load(file);
        Namespace = root.Attr("header");
        Extension = root.Attribute("extension-name") is null ? null : new ExtensionInfo(root.Attr("extension-xname"),
            root.Attr("extension-name"), root.BoolAttr("extension-multiword"),
            root.IntAttr("major-version") ?? 0, root.IntAttr("minor-version") ?? 0);
        Imports = [.. root.Elements("import").Select(i => i.Value.Trim())];

        Collect("xproto"); 
        var defined = Collect(file);
        Enums = [.. defined.OfType<EnumType>()];
        Aliases = [.. defined.OfType<AliasType>()];
        XidUnions = [.. defined.OfType<XidUnionType>()];
        Structs = [.. defined.OfType<StructType>()];
        Unions = [.. defined.OfType<UnionType>()];

        Requests = [.. root.Elements("request").Select(e => new Request(e, this))];
        foreach (var request in Requests)
            request.Analyze();

        Replies = [.. Requests.Select(r => r.Reply).OfType<Reply>()];
        foreach (var reply in Replies)
            reply.Analyze();

        Events = [.. root.Elements().Where(e => e.Name.LocalName is "event" or "eventcopy").Select(e => new Event(e, this))];
        foreach (var ev in Events)
            ev.Analyze();

        Errors = [.. root.Elements().Where(e => e.Name.LocalName is "error" or "errorcopy").Select(e => new Error(e, this))];
    }

    public static Doc? DocOf(XElement e) => e.Element("doc") is not { } d ? null : new(
        d.Element("brief")?.Value.Trim(),
        d.Element("description")?.Value.Trim(),
        d.Element("example")?.Value.Trim(),
        [.. d.Elements("field").Select(f => new DocField(f.Attr("name"), f.Value.Trim()))],
        [.. d.Elements("error").Select(x => new DocError(x.Attr("type"), x.Value.Trim()))],
        [.. d.Elements("see").Select(s => new DocSee(s.Attr("type"), s.Attr("name")))]);

    public static StartAlign? StartAlignOf(XElement e) =>
        e.Element("required_start_align") is { } a ? new(int.Parse(a.Attr("align")), a.IntAttr("offset") ?? 0) : null;

    static EnumUse? EnumUseOf(XElement e) => e.Attributes()
        .Where(a => a.Name.LocalName is "enum" or "altenum" or "mask" or "altmask")
        .Select(a => new EnumUse(a.Name.LocalName, a.Value))
        .FirstOrDefault();

    XElement Load(string file) => XmlCache.GetOrAdd(Path.Join(directory, file + ".xml"), p => XDocument.Load(p).Root!);

    List<object> Collect(string file)
    {
        if (loaded.TryGetValue(file, out var defined))
            return defined;
        loaded[file] = defined = [];

        var xml = Load(file);
        foreach (var import in xml.Elements("import"))
            Collect(import.Value);

        foreach (var e in xml.Elements("enum")) 
        {
            var en = new EnumType(e);
            enums[en.Name] = en; 
            defined.Add(en);
        }

        foreach (var e in xml.Elements())
        {
            NamedType? type = e.Name.LocalName switch
            {
                "typedef" or "xidtype" => new AliasType(e, this),
                "xidunion" => new XidUnionType(e),
                "struct" or "eventstruct" => new StructType(e, this),
                "union" => new UnionType(e, this),
                "enum" or "import" or "request" or "event" or "eventcopy" or "error" or "errorcopy" => null,
                var tag => throw new NotSupportedException($"<{tag}> in {file} is not a known top-level element"),
            };
            if (type is null)
                continue;
            types[type.Name] = type.Type;
            defined.Add(type);
        }

        
        foreach (var field in xml.Descendants("field"))
            if (field.Attribute("enum") is { } use && enums.TryGetValue(use.Value.Local(), out var en) && en.Width == 0
                && types.TryGetValue(field.Attr("type").Local(), out var type))
                en.Width = type.Size;

        return defined;
    }

    public WireType Resolve(string name) =>
        types.TryGetValue(name.Local(), out var type) ? type : throw new KeyNotFoundException($"Unknown type '{name}'.");

    public int EnumWidth(string enumName) => enums.TryGetValue(enumName, out var en) ? en.Width : 0;

    public static (int Size, bool IsFixed) Measure(IEnumerable<Member> members)
    {
        var size = 0;
        var isFixed = true;
        foreach (var member in members.OnWire())
        {
            var bytes = WireMath.SizeOf(member, size);
            size += bytes ?? (member as Field)?.Type.Size ?? 0; 
            isFixed &= bytes is not null;
        }
        return (size, isFixed);
    }

    public Member[] ReadMembers(IEnumerable<XElement> elements) =>
    [
        .. elements
            .Where(e => e.Name.LocalName is not ("doc" or "reply" or "required_start_align" or "enumref" or "value" or "bit"))
            .Select<XElement, Member>(m => m.Name.LocalName switch
            {
                "pad" => new Pad(m.IntAttr("bytes"), m.IntAttr("align"), m.BoolAttr("serialize")),
                "fd" => new Fd(m.Attr("name")),
                "list" when m.Attr("type") == "fd" => new FdList(m.Attr("name"), LengthOf(m)),
                "field" => new Field(m.Attr("name"), m.Attr("type"), TypeOf(m), EnumUseOf(m), null),
                "exprfield" => new Field(m.Attr("name"), m.Attr("type"), TypeOf(m), EnumUseOf(m), ReadExpr(m.FirstChild()!)),
                "list" => new ListOf(m.Attr("name"), m.Attr("type"), TypeOf(m), EnumUseOf(m), LengthOf(m)),
                "valueparam" => new ValueParam(m.Attr("value-mask-type"), m.Attr("value-mask-name"), m.Attr("value-list-name")),
                "switch" => new Switch(m.Attr("name"), ReadExpr(m.FirstChild()!),
                [
                    .. m.Elements()
                        .Where(c => c.Name.LocalName is "case" or "bitcase")
                        .Select(c =>
                        {
                            var values = c.Elements().Where(x => x.Name.LocalName is "enumref" or "value" or "bit").ToArray();
                            var members = ReadMembers(c.Elements());
                            var (size, isFixed) = Measure(members);
                            return new Case(c.Attribute("name")?.Value, c.Name.LocalName == "bitcase",
                                [.. values.Select(ReadExpr)], values.Aggregate(0L, (all, x) => all | Number(x)),
                                members, StartAlignOf(c), size, isFixed);
                        })
                ], StartAlignOf(m)),
                var tag => throw new NotSupportedException($"<{tag}> is not a known member (reading {Namespace})"),
            })
    ];

    Expr? LengthOf(XElement list) => list.FirstChild() is { } length ? ReadExpr(length) : null;

    WireType TypeOf(XElement e) => Resolve(e.Attr("type"));

    Expr ReadExpr(XElement e)
    {
        switch (e.Name.LocalName)
        {
            case "fieldref": return new FieldRef(e.Value.Trim());
            case "paramref": return new ParamRef(e.Value.Trim(), e.Attr("type"));
            case "value" or "bit": return new Const(Number(e));
            case "enumref": return new EnumRef(e.Attr("ref"), e.Value.Trim(), Number(e));
        }

        var args = e.Elements().Where(x => x.Name.LocalName != "doc").Select(ReadExpr).ToArray();
        return e.Name.LocalName switch
        {
            "op" => new Binary(e.Attr("op"), args[0], args[1]),
            "unop" => new Unary(e.Attr("op"), args[0]),
            "popcount" => new PopCount(args[0]),
            "sumof" => new SumOf(e.Attr("ref"), args.FirstOrDefault()),
            "listelement-ref" => new ListElement(),
            var tag => new UnknownExpr(tag),
        };
    }

    long Number(XElement e) => e.Name.LocalName switch
    {
        "value" => ParseNumber(e.Value),
        "bit" => 1L << (int)ParseNumber(e.Value),
        _ => enums[e.Attr("ref").Local()][e.Value.Trim()], 
    };

    public static long ParseNumber(string text)
    {
        text = text.Trim();
        return text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text[2..], 16) : long.Parse(text);
    }
}


sealed class Request(XElement e, ProtocolReader protocol)
{
    static readonly (int Size, string Type)[] PadTypes = [(8, "ulong"), (4, "uint"), (2, "ushort"), (1, "byte")];

    public string Name { get; } = e.Attr("name");
    public int Opcode { get; } = int.Parse(e.Attr("opcode"));
    public bool CombineAdjacent { get; } = e.BoolAttr("combine-adjacent");
    public Member[] Members { get; } = protocol.ReadMembers(e.Elements()); 
    public StartAlign? RequiredStartAlign { get; } = ProtocolReader.StartAlignOf(e);
    public Doc? Doc { get; } = ProtocolReader.DocOf(e);
    public Reply? Reply { get; } = e.Element("reply") is { } reply ? new Reply(reply, protocol) : null;
    public bool HasReply => Reply is not null;

    public int StructSize { get; private set; }
    public string ParameterList { get; private set; } = "";
    public Signature Signature { get; private set; } = new([], []);
    public Declaration[] Declarations { get; private set; } = [];

    readonly List<Param> parameters = [];   
    readonly List<string> extras = [];      
    readonly List<Declaration> body = [];   
    int padCount;

    public void Analyze()
    {
        var rest = new Queue<Member>(Members.OnWire());

        if (!protocol.IsCore)
        {
            parameters.Add(new("byte", "majorOpCode"));
            body.Add(new("public", "byte", "MajorOpcode", "majorOpCode"));
        }
        body.Add(new("public", "Opcode", "Opcode", $"Opcode.{Name}"));

        if (protocol.IsCore)
        {
            if (rest.TryPeek(out var first) && first is Pad { Bytes: 1 } or Field { Type: { IsFixed: true, Size: 1 } })
                Emit(rest.Dequeue(), 1);
            else
                body.AddRange(Pads(1));
        }

        var lengthAt = body.Count;
        var size = 4;
        while (rest.TryPeek(out var next) && next is Pad or Field { Type.IsFixed: true })
            size += Emit(rest.Dequeue(), size);
        body.Insert(lengthAt, new("public", "ushort", "Length", Length(size, rest)));

        StructSize = rest.Count == 0 ? WireMath.Align4(size) : size;
        ParameterList = string.Join(", ", parameters.Select(p => $"{p.Type} {p.Name.Escape()}")
            .Concat(extras.Select(x => $"int {x.Escape()}")));
        Declarations = [.. body];
        Signature = new Signature([.. parameters.Skip(protocol.IsCore ? 0 : 1)], [.. rest]);
    }


    int Emit(Member m, int offset)
    {
        switch (m)
        {
            case Field f:
                {
                    var name = f.Name.Escape();
                    var storage = f.Type.CsName == "bool" ? "byte" : f.Type.CsName; 

                    string? parameterType;
                    string fieldType, value;
                    if (f.Value is { } expr)
                        (parameterType, fieldType, value) = (null, storage, $"({storage}){Cs.Paren(Render(expr))}");
                    else if (f.Enum is { } en && protocol.EnumWidth(en) == f.Type.Size)
                        (parameterType, fieldType, value) = (en, en, name);
                    else if (f.Enum is { } otherEnum)
                        (parameterType, fieldType, value) = (otherEnum, storage, $"({storage}){name}");
                    else if (f.Type.CsName == "bool")
                        (parameterType, fieldType, value) = ("bool", "byte", $"(byte)({name} ? 1 : 0)");
                    else
                        (parameterType, fieldType, value) = (f.Type.CsName, f.Type.CsName, name);

                    if (parameterType != null)
                        parameters.Add(new(parameterType, f.Name));

                    var access = f.Name.EndsWith("_len") ? "private" : "public";
                    body.Add(new(access, fieldType, f.Name.ToPascalCase(), value));
                    return f.Type.Size;
                }
            case Pad p:
                var bytes = WireMath.PadLength(p.Bytes, p.Align, offset);
                body.AddRange(Pads(bytes));
                return bytes;
            default:
                throw new InvalidOperationException($"{m} is not a fixed member");
        }
    }

    List<Declaration> Pads(int bytes)
    {
        List<Declaration> result = [];
        foreach (var (size, type) in PadTypes)
            for (; bytes >= size; bytes -= size)
                result.Add(new("private", type, $"_pad{padCount++}", "0"));
        return result;
    }

    string Length(int headerSize, IEnumerable<Member> trailing)
    {
        var length = new LengthBuilder(headerSize);
        foreach (var part in trailing)
        {
            switch (part)
            {
                case Pad { Align: not null }:
                    length.EndRun();
                    break;
                case Pad pad:
                    length.AddConstant(WireMath.PadLength(pad.Bytes, pad.Align, 0));
                    break;
                case Switch sw:
                    var cases = sw.Cases;
                    if (!cases.All(c => c.IsBitCase && c.IsFixed))
                        length.Add(Extra($"{sw.Name}_bytes"), 1);
                    else if (sw.Selector is FieldRef selector && cases.Select(c => c.Size).Distinct().Count() == 1)
                        length.Add($"{selector.Name.Escape()}.CountFlags()", cases[0].Size);
                    else
                    {
                        var mask = Render(sw.Selector);
                        foreach (var c in cases)
                        {
                            var unit = c.Size % 4 == 0 ? 4 : 1;
                            length.Add($"(({mask} & {c.Bits}) != 0 ? {c.Size / unit} : 0)", unit);
                        }
                    }
                    break;
                case ListOf { Type.IsFixed: true } list:
                    length.Add(list.Length is null ? Extra($"{list.Name}_len") : Render(list.Length), list.Type.Size);
                    break;
                case Field field: 
                    length.Add(Extra($"{field.Name}_bytes"), 1);
                    break;
                case ListOf list: 
                    length.Add(Extra($"{list.Name}_bytes"), 1);
                    break;
            }
        }
        return length.Build();
    }

    string Render(Expr e) => Exprs.Render(e, name =>
    {
        var field = Members.OfType<Field>().FirstOrDefault(f => f.Value is null && f.Name == name);
        if (field == null)
            return Extra(name); 

        var parameter = name.Escape();
        var type = field.Type.CsName;
        if (field.Enum == null && type == "bool")
            return $"({parameter} ? 1 : 0)";
        return field.Enum == null && type == "int" ? parameter : $"(int){parameter}";
    }, _ => null, out var why) ?? throw new NotSupportedException($"{why} in request {Name}");

    string Extra(string name)
    {
        if (!extras.Contains(name))
            extras.Add(name);
        return name.Escape();
    }
}

abstract class Message
{
    sealed record Prop(string Name, string Type, bool IsList);

    static readonly HashSet<string> Primitives =
        ["byte", "ushort", "uint", "ulong", "sbyte", "short", "int", "long", "bool", "float", "double"];

    static string? Primitive(WireType t) =>
        t.IsStruct ? null
        : Primitives.Contains(t.CsName) ? t.CsName
        : t.Size switch { 1 => "byte", 2 => "ushort", 4 => "uint", 8 => "ulong", _ => null };

    readonly HashSet<string> usedNames = ["ResponseType", "Sequence", "Number", "Length", "ByteSize", "EventType", "Extension", "ErrorCode", "Verify", "FromBytes"];
    readonly List<string> properties = [];
    readonly Dictionary<string, Prop> props = [];         
    readonly List<string> runtime = [];
    int offset = 1;
    int fixedEnd = -1;
    bool known = true;
    bool needsSumPop;

    public string Name { get; protected init; } = "";
    public Member[]? Members { get; protected init; }    
    public StartAlign? RequiredStartAlign { get; protected init; }
    public Doc? Doc { get; protected init; }

    protected abstract int HeaderAt { get; }
    protected abstract int BodyAt { get; }

    public string? Todo { get; private set; }
    public int MinSize { get; private set; }
    public string[] Header { get; private set; } = [];
    public string[] Properties { get; private set; } = [];
    public bool NeedsSumPop { get; private set; }

    protected abstract void AddHeader(List<string> lines);
    protected virtual string? Intrinsic(string field) => null;

    public void Analyze()
    {
        usedNames.Add(Name);
        if (Members == null)
        {
            Todo = $"// TODO: {Name} is a copy of an event defined in another file";
            return;
        }

        foreach (var member in Members.OnWire())
        {
            SkipHeader(member);
            switch (member)
            {
                case Pad { Bytes: { } bytes }:
                    offset += bytes;
                    break;
                case Pad pad when known && runtime.Count == 0:
                    offset += WireMath.PadLength(null, pad.Align, offset);
                    break;
                case Pad:
                    Unknown();
                    break;
                case Field field:
                    AddField(field);
                    break;
                case ListOf list:
                    AddList(list);
                    break;
                case Switch sw:
                    properties.Add($"// TODO: '{sw.Name}' (switch) is not supported");
                    Unknown();
                    break;
            }
        }
        MarkEnd();

        MinSize = Math.Max(32, fixedEnd);
        List<string> header = ["public byte ResponseType => _data[0];"];
        AddHeader(header);
        Header = [.. header];
        Properties = [.. properties];
        NeedsSumPop = needsSumPop;
    }

    void AddField(Field f)
    {
        var primitive = Primitive(f.Type);
        if (primitive == null)
        {
            properties.Add($"// TODO: field '{f.Name}' has type {f.Type.CsName} which can't be represented in c#");
            if (f.Type.IsFixed) offset += f.Type.Size;
            else Unknown();
            return;
        }
        if (!known)
        {
            properties.Add($"// TODO: field '{f.Name}' follows a member with an unknown size");
            return;
        }

        var prop = PropName(f.Name);
        props[f.Name] = new(prop, primitive, false);
        var at = At();
        var read = primitive switch
        {
            "bool" => $"_data[{at}] != 0",
            "byte" => $"_data[{at}]",
            "sbyte" => $"(sbyte)_data[{at}]",
            _ => $"Unsafe.As<byte, {primitive}>(ref _data[{at}])",
        };
        var access = f.Name.EndsWith("_len") ? "private" : "public";
        properties.Add($"{access} {primitive} {prop} => {read};");
        offset += f.Type.Size;
    }

    void AddList(ListOf list)
    {
        if (!known)
        {
            properties.Add($"// TODO: list '{list.Name}' follows a member with an unknown size");
            return;
        }

        string? why = "list without length";
        if (list.Length is null || Render(list.Length, out why) is not { } count)
        {
            properties.Add($"// TODO: list '{list.Name}' has an unsupported length ({why})");
            Unknown();
            return;
        }

        var bytes = list.Type.Size == 1 ? count : $"{Cs.Paren(count)} * {list.Type.Size}";
        var advance = int.TryParse(count, out var n) ? (n * list.Type.Size).ToString() : bytes;

        var primitive = Primitive(list.Type);
        if (primitive == null)
        {
            properties.Add($"// TODO: list '{list.Name}' of {list.Type.CsName} can't be represented in c#");
            if (list.Type.IsFixed) Advance(advance);
            else Unknown();
            return;
        }

        var prop = PropName(list.Name);
        props[list.Name] = new(prop, primitive, true);
        var span = $"new ReadOnlySpan<byte>(_data, {At()}, {bytes})";
        var element = primitive == "bool" ? "byte" : primitive;
        var value = element == "byte" ? span : $"MemoryMarshal.Cast<byte, {element}>({span})";
        properties.Add($"public ReadOnlySpan<{element}> {prop} => {value};");
        Advance(advance);
    }

    string? Render(Expr e, out string? why) => Exprs.Render(e, PropertyRef, list =>
    {
        if (!props.TryGetValue(list, out var p) || p is not { IsList: true, Type: "uint" })
            return null;
        needsSumPop = true;
        return $"SumPop({p.Name})";
    }, out why);

    string? PropertyRef(string field)
    {
        if (!props.ContainsKey(field) && Intrinsic(field) is { } intrinsic)
            return intrinsic;
        if (!props.TryGetValue(field, out var p) || p.IsList)
            return null;
        return p.Type == "bool" ? $"({p.Name} ? 1 : 0)" : $"(int){p.Name}";
    }

    void SkipHeader(Member member)
    {
        if (offset >= BodyAt || HeaderAt == BodyAt)
            return; 
        if (offset >= HeaderAt || WireMath.SizeOf(member, offset) is not { } s || offset + s > HeaderAt)
            offset = BodyAt;
    }

    string At() => runtime.Count == 0 ? offset.ToString() : $"{offset} + {string.Join(" + ", runtime)}";

    void Advance(string bytes)
    {
        if (int.TryParse(bytes, out var n))
            offset += n;
        else
        {
            MarkEnd();
            runtime.Add(bytes);
        }
    }

    void MarkEnd()
    {
        if (fixedEnd < 0)
            fixedEnd = offset;
    }

    void Unknown()
    {
        MarkEnd();
        known = false;
    }

    string PropName(string raw)
    {
        var candidate = raw.ToPascalCase();
        while (!usedNames.Add(candidate))
            candidate += "Field";
        return candidate;
    }
}

sealed class Reply : Message
{
    public Reply(XElement e, ProtocolReader protocol) 
    {
        Name = e.Parent!.Attr("name") + "Reply";
        Members = protocol.ReadMembers(e.Elements());
        RequiredStartAlign = ProtocolReader.StartAlignOf(e);
        Doc = ProtocolReader.DocOf(e);
    }

    protected override int HeaderAt => 2;
    protected override int BodyAt => 8;

    protected override void AddHeader(List<string> lines)
    {
        lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
        lines.Add("public uint Length => Unsafe.As<byte, uint>(ref _data[4]);");
        lines.Add("public int ByteSize => checked(32 + (int)Length * 4);");
        lines.Add("public bool Verify(in ushort sequence) => ResponseType == 1 && Sequence == sequence;");
    }

    protected override string? Intrinsic(string field) => field == "length" ? "(int)Length" : null;
}

sealed class Event : Message
{
    public int Number { get; }
    public bool Xge { get; }
    public bool NoSequenceNumber { get; }
    public string? Ref { get; }

    public Event(XElement e, ProtocolReader protocol)
    {
        var source = e.Name.LocalName == "event" ? e
            : e.Parent!.Elements("event").LastOrDefault(x => x.Attr("name") == e.Attr("ref").Local());
        var name = e.Attr("name");
        Name = name.EndsWith("Event") ? name : name + "Event";
        Number = int.Parse(e.Attr("number"));
        Ref = e.Attribute("ref")?.Value;
        Xge = source?.BoolAttr("xge") ?? false;
        NoSequenceNumber = source?.BoolAttr("no-sequence-number") ?? true;
        Members = source is null ? null : protocol.ReadMembers(source.Elements());
        RequiredStartAlign = source is null ? null : ProtocolReader.StartAlignOf(source);
        Doc = ProtocolReader.DocOf(e);
    }

    protected override int HeaderAt => Xge ? 1 : 2;
    protected override int BodyAt => Xge ? 10 : NoSequenceNumber ? 2 : 4;

    protected override void AddHeader(List<string> lines)
    {
        if (Xge)
        {
            lines.Add("public byte Extension => _data[1];");
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
            lines.Add("public uint Length => Unsafe.As<byte, uint>(ref _data[4]);");
            lines.Add("public int ByteSize => checked(32 + (int)Length * 4);");
            lines.Add("public ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);");
        }
        else if (!NoSequenceNumber)
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
    }
}


sealed class LengthBuilder(int headerBytes)
{
    readonly List<string> words = [];
    readonly List<string> run = [];
    int constantWords;
    int constantBytes = headerBytes;

    public void AddConstant(int bytes) => constantBytes += bytes;

    public void Add(string count, int unit)
    {
        if (int.TryParse(count, out var n))
            constantBytes += n * unit;
        else if (unit % 4 == 0)
            words.Add(Scale(count, unit / 4));
        else
            run.Add(Scale(count, unit));
    }
    public void EndRun()
    {
        if (run.Count == 0)
            constantWords += WireMath.Align4(constantBytes) / 4;
        else
        {
            constantWords += constantBytes / 4;
            var rest = constantBytes % 4;
            var terms = rest == 0 ? run : run.Prepend(rest.ToString());
            words.Add($"{Cs.Paren(string.Join(" + ", terms))}.AddPadding() / 4");
        }
        constantBytes = 0;
        run.Clear();
    }

    public string Build()
    {
        EndRun();
        if (words.Count == 0)
            return constantWords.ToString();
        if (constantWords != 0)
            words.Insert(0, constantWords.ToString());
        return $"(ushort)({string.Join(" + ", words)})";
    }

    static string Scale(string count, int factor) => factor == 1 ? count : $"{Cs.Paren(count)} * {factor}";
}

static class Exprs
{
    public static string? Render(Expr e, Func<string, string?> field, Func<string, string?> sumPop, out string? why)
    {
        why = null;
        switch (e)
        {
            case Const c:
                return c.Value.ToString();
            case EnumRef r:
                return r.Value.ToString();
            case FieldRef f:
                var value = field(f.Name);
                if (value is null)
                    why = $"fieldref {f.Name}";
                return value;
            case Binary b:
                return Render(b.Left, field, sumPop, out why) is { } left && Render(b.Right, field, sumPop, out why) is { } right
                    ? $"({left} {b.Op} {right})"
                    : null;
            case Unary u:
                return Render(u.Operand, field, sumPop, out why) is { } operand ? $"{u.Op}{Cs.Paren(operand)}" : null;
            case PopCount p:
                return Render(p.Operand, field, sumPop, out why) is { } bits ? $"{Cs.Paren(bits)}.CountFlags()" : null;
            case SumOf { Body: PopCount { Operand: ListElement } } s when sumPop(s.List) is { } call:
                return call;
            default:
                why = e switch { SumOf => "sumof", UnknownExpr x => $"<{x.Tag}>", _ => e.GetType().Name };
                return null;
        }
    }
}


static class WireMath
{
    public static int Align4(int bytes) => (bytes + 3) & ~3;

    public static Member[] OnWire(this IEnumerable<Member> members) => [.. members.Where(m => m is not (Fd or FdList))];

    public static int PadLength(int? bytes, int? align, int offset) =>
        bytes ?? (align!.Value - offset % align.Value) % align.Value;

    public static int? SizeOf(Member m, int? offset) => m switch
    {
        Pad { Bytes: { } bytes } => bytes,
        Pad { Align: { } align } => offset is { } o ? PadLength(null, align, o) : null,
        Field { Type.IsFixed: true } f => f.Type.Size,
        ListOf { Length: Const count, Type.IsFixed: true } list => list.Type.Size * (int)count.Value,
        _ => null,
    };
}


static class Cs
{
    static readonly HashSet<string> Keywords =
    [
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof",
        "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    ];

    public static string Escape(this string name) => Keywords.Contains(name) ? "@" + name : name;

    public static string ToPascalCase(this string name)
    {
        Span<char> buffer = name.Length <= 256 ? stackalloc char[name.Length] : new char[name.Length];
        var length = 0;
        var upper = true;
        foreach (var c in name)
        {
            if (c == '_')
                upper = true;
            else
            {
                buffer[length++] = upper ? char.ToUpperInvariant(c) : c;
                upper = false;
            }
        }
        var result = buffer[..length];
        return result.SequenceEqual(name) ? name : new string(result);
    }

    public static string ToCamelCase(this string name)
    {
        var pascal = name.ToPascalCase();
        var lower = char.ToLowerInvariant(pascal[0]);
        return (lower == pascal[0] ? pascal : string.Create(pascal.Length, (pascal, lower), static (span, s) =>
        {
            s.pascal.AsSpan().CopyTo(span);
            span[0] = s.lower;
        })).Escape();
    }

    public static string Paren(string expression) =>
        IsSimple(expression) || IsWrapped(expression) ? expression : $"({expression})";

    static bool IsSimple(string e)
    {
        var body = e.AsSpan();
        if (body.EndsWith("()"))
            body = body[..^2];
        if (body.IsEmpty)
            return false;
        foreach (var c in body)
            if (!(char.IsLetterOrDigit(c) || c is '_' or '@' or '.'))
                return false;
        return true;
    }
    static bool IsWrapped(string e)
    {
        if (e.Length < 2 || e[0] != '(')
            return false;
        var depth = 0;
        for (var i = 0; i < e.Length; i++)
        {
            depth += e[i] switch { '(' => 1, ')' => -1, _ => 0 };
            if (depth == 0)
                return i == e.Length - 1;
        }
        return false;
    }
}


static class XmlExt
{
    public static string Attr(this XElement e, string name) => e.Attribute(name)!.Value;

    public static int? IntAttr(this XElement e, string name) => e.Attribute(name) is { } a ? int.Parse(a.Value) : null;

    public static bool BoolAttr(this XElement e, string name) => e.Attribute(name)?.Value == "true";

    public static XElement? FirstChild(this XElement e) => e.Elements().FirstOrDefault(c => c.Name.LocalName != "doc");
    public static string Local(this string name) => name[(name.IndexOf(':') + 1)..];
}
