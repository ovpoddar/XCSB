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
            text.Append(I2).AppendLine(reader.InterfaceMethod(r));
        text.AppendLine($"{I1}}}");

        text.AppendLine($"{I1}public interface I{name}ResponseProto");
        text.AppendLine($"{I1}{{");
        foreach (var r in reader.Requests.Where(r => r.HasReply))
            text.Append(I2).AppendLine(reader.InterfaceMethod(r));
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

abstract record Expr { }
sealed record Const(long Value) : Expr;
sealed record FieldRef(string Name) : Expr;
sealed record Binary(string Op, Expr Left, Expr Right) : Expr;
sealed record Unary(string Op, Expr Operand) : Expr;
sealed record PopCount(Expr Operand) : Expr;
sealed record SumOf(string List, Expr? Body) : Expr;
sealed record ListElement : Expr { }
sealed record UnknownExpr(string Tag) : Expr;
sealed record ParamRef(string Name, string Type) : Expr; // a field of the enclosing request/reply

abstract record Member { }
sealed record Pad(int? Bytes, int? Align) : Member;
sealed record Field(string Name, WireType Type, string? Enum, Expr? Value) : Member;
sealed record ListOf(string Name, WireType Type, Expr? Length) : Member;
sealed record Switch(string Name, Expr Selector, Case[] Cases) : Member;
sealed record Case(bool IsBitCase, long Bits, int Size, bool IsFixed);

sealed record Param(string Type, string Name);
sealed record Signature(Param[] Parameters, Member[] Trailing);

// collected (not yet emitted) protocol types
sealed record ExtensionInfo(string XName, string Name, bool MultiWord, int MajorVersion, int MinorVersion);
sealed record OpcodeItem(string Name, int Value);
sealed record EnumItem(string Name, long Value, bool IsBit);
sealed record EnumType(string Name, EnumItem[] Items)
{
    public bool HasBits => Items.Any(i => i.IsBit);
}
// Owner: header of the module that defines the enum ("?" unresolved, "a|b" ambiguous)
// Kind: enum, altenum, mask or altmask; Width: byte size of the field/list element
sealed record EnumUse(string Owner, string Enum, string Kind, int Width, string Container, string Field);
sealed record XidType(string Name, string[] UnionOf);
sealed record TypedefInfo(string Name, string OldName, WireType Resolved);
// Offset is null once it depends on runtime data; Size is the element size for lists
sealed record StructMember(string Name, string Kind, string Type, int? Offset, int Size, string? Count);
sealed record StructType(string Name, string Kind, int Size, bool IsFixed, StructMember[] Members, string[] Notes);


sealed class ProtocolReader
{

    static readonly Dictionary<string, WireType> Builtins = new()
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

    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, XElement> XmlCache = new();

    readonly string directory;
    readonly HashSet<string> loaded = [];

    readonly Dictionary<string, XElement> rawTypes = [];
    readonly Dictionary<string, List<XElement>> rawEnums = [];
    readonly List<(string Enum, string Type)> enumFields = [];

    readonly Dictionary<string, WireType> types = [];
    readonly Dictionary<string, Dictionary<string, long>> enumItems = [];
    readonly Dictionary<string, int> enumWidths = [];

    readonly Dictionary<string, string> headers = [];             // loaded file -> its header (module name)
    readonly Dictionary<string, List<string>> enumOwners = [];    // enum name -> modules that define it
    readonly Dictionary<string, XElement> rawErrors = [];          // error name -> <error> (closest wins)

    public string Namespace { get; }
    public bool IsCore { get; }
    public Request[] Requests { get; }
    public Reply[] Replies { get; }
    public Event[] Events { get; }

    // everything below is collected and analysed but not emitted yet
    public string[] Imports { get; }
    public string[] Closure => [.. headers.Values];                // this module and everything it imports
    public ExtensionInfo? Extension { get; }
    public OpcodeItem[] Opcodes { get; }
    public EnumType[] Enums { get; }
    public EnumUse[] EnumUses { get; }                             // enum/altenum/mask/altmask refs in this file
    public XidType[] XidTypes { get; }
    public TypedefInfo[] Typedefs { get; }
    public StructType[] Structs { get; }                           // struct, union, eventstruct
    public Error[] Errors { get; }

    public ProtocolReader(string path)
    {
        directory = Path.GetDirectoryName(path)!;
        var root = Collect(Path.GetFileNameWithoutExtension(path));
        Namespace = root.Attr("header");
        IsCore = root.Attribute("extension-name") is null;

        Requests = [.. root.Elements("request").Select(r =>
        {
            var reply = r.Element("reply");
            var request = new Request(r.Attr("name"), ReadMembers(r.Elements()),
                reply is null ? null : ReadMembers(reply.Elements()));
            request.Analyze(this);
            return request;
        })];

        Replies = [.. Requests.Where(r => r.HasReply).Select(r =>
        {
            var reply = new Reply(r.Name + "Reply", r.ReplyMembers!);
            reply.Analyze();
            return reply;
        })];

        var eventElements = root.Elements().Where(e => e.Name.LocalName is "event" or "eventcopy").ToArray();
        Dictionary<string, (bool Xge, bool HasSequence, Member[] Members)> defined = [];
        foreach (var e in eventElements.Where(e => e.Name.LocalName == "event"))
            defined[e.Attr("name")] = (e.Attribute("xge")?.Value == "true",
                e.Attribute("no-sequence-number")?.Value != "true", ReadMembers(e.Elements()));

        Events = [.. eventElements.Select(e =>
        {
            var name = e.Attr("name");
            defined.TryGetValue(e.Name.LocalName == "event" ? name : e.Attr("ref").Local(), out var def);
            var ev = new Event(name, int.Parse(e.Attr("number")), def.Xge, def.HasSequence, def.Members);
            ev.Analyze();
            return ev;
        })];

        Imports = [.. root.Elements("import").Select(i => i.Value.Trim())];
        Extension = IsCore ? null : new ExtensionInfo(root.Attr("extension-xname"), root.Attr("extension-name"),
            root.Attribute("extension-multiword")?.Value == "true",
            root.IntAttr("major-version") ?? 0, root.IntAttr("minor-version") ?? 0);
        Opcodes = [.. root.Elements("request").Select(r => new OpcodeItem(r.Attr("name"), int.Parse(r.Attr("opcode"))))];
        Enums = [.. root.Elements("enum").Select(e => new EnumType(e.Attr("name"), ReadItems(e)))];
        EnumUses = [.. ReadEnumUses(root)];
        XidTypes = [.. root.Elements().Where(e => e.Name.LocalName is "xidtype" or "xidunion")
            .Select(e => new XidType(e.Attr("name"), [.. e.Elements("type").Select(t => t.Value.Trim().Local())]))];
        Typedefs = [.. root.Elements("typedef")
            .Select(e => new TypedefInfo(e.Attr("newname"), e.Attr("oldname"), Resolve(e.Attr("newname"))))];
        Structs = [.. root.Elements().Where(e => e.Name.LocalName is "struct" or "union" or "eventstruct")
            .Select(AnalyzeStruct)];
        Errors = [.. root.Elements().Where(e => e.Name.LocalName is "error" or "errorcopy").Select(e =>
        {
            var name = e.Attr("name");
            var copyOf = e.Name.LocalName == "errorcopy" ? e.Attr("ref").Local() : null;
            var error = new Error(name, int.Parse(e.Attr("number")), copyOf,
                rawErrors.TryGetValue(copyOf ?? name, out var def) ? ReadMembers(def.Elements()) : null);
            error.Analyze();
            return error;
        })];
    }

    public string EnumOwnerOf(string raw)
    {
        var colon = raw.IndexOf(':');
        if (colon >= 0)
            return raw[..colon];
        if (!enumOwners.TryGetValue(raw, out var owners))
            return "?";
        return owners.Contains(Namespace) ? Namespace : string.Join("|", owners);
    }

    static EnumItem[] ReadItems(XElement e)
    {
        List<EnumItem> items = [];
        long next = 0; // xcbgen numbers items without <value>/<bit> sequentially
        foreach (var item in e.Elements("item"))
        {
            var (value, isBit) = item.Element("value") is { } v ? (ParseNumber(v.Value), false)
                : item.Element("bit") is { } b ? (1L << (int)ParseNumber(b.Value), true)
                : (next, false);
            next = value + 1;
            items.Add(new(item.Attr("name"), value, isBit));
        }
        return [.. items];
    }

    IEnumerable<EnumUse> ReadEnumUses(XElement root)
    {
        string[] kinds = ["enum", "altenum", "mask", "altmask"];
        foreach (var e in root.Descendants())
        {
            if (e.Name.LocalName is not ("field" or "exprfield" or "list"))
                continue;
            foreach (var kind in kinds)
            {
                if (e.Attribute(kind)?.Value is not { } raw)
                    continue;
                var width = TryResolve(e.Attr("type").Local(), out var type) ? type.Size : 0;
                var container = e.AncestorsAndSelf().First(a => a.Parent == root);
                yield return new EnumUse(EnumOwnerOf(raw), raw.Local(), kind, width,
                    $"{container.Name.LocalName} {container.Attribute("name")?.Value}", e.Attribute("name")?.Value ?? "");
            }
        }
    }

    StructType AnalyzeStruct(XElement e)
    {
        var name = e.Attr("name");
        var kind = e.Name.LocalName;
        var type = Resolve(name);
        List<string> notes = [];
        if (kind == "eventstruct")
        {
            foreach (var a in e.Elements("allowed"))
                notes.Add($"carries {a.Attr("extension")} events {a.Attr("opcode-min")}..{a.Attr("opcode-max")}" +
                          (a.Attribute("xge")?.Value == "true" ? " (xge)" : ""));
            return new StructType(name, kind, type.Size, type.IsFixed, [], [.. notes]);
        }

        List<StructMember> members = [];
        int? offset = 0;
        foreach (var m in ReadMembers(e.Elements()))
        {
            var at = kind == "union" ? 0 : offset;
            switch (m)
            {
                case Pad p:
                    if (offset is { } o)
                        offset = o + WireMath.PadLength(p.Bytes, p.Align, o);
                    break;
                case Field f:
                    members.Add(new(f.Name, "field", f.Type.CsName, at, f.Type.Size, null));
                    offset = f.Type.IsFixed ? offset + f.Type.Size : null;
                    break;
                case ListOf l:
                    members.Add(new(l.Name, "list", l.Type.CsName, at, l.Type.Size,
                        l.Length is null ? "?" : Exprs.Describe(l.Length)));
                    offset = l is { Length: Const c, Type.IsFixed: true } ? offset + l.Type.Size * (int)c.Value : null;
                    if (l.Length is null)
                        notes.Add($"list '{l.Name}' has no length");
                    else if (l.Length is ParamRef pr)
                        notes.Add($"list '{l.Name}' takes its length from the enclosing message's '{pr.Name}'");
                    else if (!l.Type.IsFixed)
                        notes.Add($"list '{l.Name}' has variable-size elements ({l.Type.CsName})");
                    break;
                case Switch s:
                    members.Add(new(s.Name, "switch", "", at, 0, Exprs.Describe(s.Selector)));
                    notes.Add($"switch '{s.Name}' on {Exprs.Describe(s.Selector)}");
                    offset = null;
                    break;
            }
        }
        if (kind == "union" && !type.IsFixed)
            notes.Add("union with a variable-size member");
        return new StructType(name, kind, type.Size, type.IsFixed, [.. members], [.. notes]);
    }

    public string InterfaceMethod(Request request)
    {
        List<string>? counts = null;
        foreach (var part in request.Signature.Trailing)
            if (part is ListOf { Length: FieldRef length })
                (counts ??= []).Add(length.Name.ToCamelCase());

        List<string> args = [];
        foreach (var p in request.Signature.Parameters)
        {
            var name = p.Name.ToCamelCase();
            if (counts is null || !counts.Contains(name))
                args.Add($"{p.Type} {name}");
        }

        foreach (var part in request.Signature.Trailing)
        {
            switch (part)
            {
                case Pad:
                    break;
                case ListOf list:
                    var element = list.Type.CsName is "bool" or "char" or "void" ? "byte" : list.Type.CsName;
                    args.Add($"ReadOnlySpan<{element}> {list.Name.ToCamelCase()}");
                    break;
                default:
                    var what = part switch
                    {
                        Field f => $"field '{f.Name}'",
                        Switch s => $"switch '{s.Name}'",
                        _ => "member",
                    };
                    return $"// TODO: {request.Name} has a {what} which can't be represented in c#";
            }
        }

        var list2 = string.Join(", ", args);
        return request.HasReply
            ? $"ReplyLease<{request.Name}Reply> {request.Name}({list2});"
            : $"ResponseProto {request.Name}({list2});";
    }
    XElement Collect(string file)
    {
        var xml = XmlCache.GetOrAdd(Path.Join(directory, file + ".xml"), p => XDocument.Load(p).Root!);
        if (!loaded.Add(file))
            return xml;
        var header = headers[file] = xml.Attr("header");

        foreach (var import in xml.Elements("import"))
            Collect(import.Value);

        foreach (var e in xml.Elements())
        {
            switch (e.Name.LocalName)
            {
                case "enum":
                    var name = e.Attr("name");
                    if (!rawEnums.TryGetValue(name, out var list))
                        rawEnums[name] = list = [];
                    list.Add(e);
                    if (!enumOwners.TryGetValue(name, out var owners))
                        enumOwners[name] = owners = [];
                    if (!owners.Contains(header))
                        owners.Add(header);
                    break;
                case "error":
                    rawErrors[e.Attr("name")] = e;
                    break;
                case "typedef":
                    rawTypes[e.Attr("newname")] = e;
                    break;
                case "xidtype" or "xidunion" or "struct" or "union" or "eventstruct":
                    rawTypes[e.Attribute("name")?.Value ?? ""] = e;
                    break;
            }
        }

        foreach (var field in xml.Descendants("field"))
            if (field.EnumAttr() is { } enumName)
                enumFields.Add((enumName, field.Attr("type").Local()));

        return xml;
    }

    bool TryResolve(string name, out WireType type)
    {
        if (types.TryGetValue(name, out type))
            return true;
        if (!rawTypes.TryGetValue(name, out var e))
            return Builtins.TryGetValue(name, out type);

        switch (e.Name.LocalName)
        {
            case "typedef":
                type = Resolve(e.Attr("oldname"));
                break;
            case "struct":
                var (size, isFixed) = Measure(ReadMembers(e.Elements()));
                type = new(name, size, isFixed, true);
                break;
            case "union":
                type = new(name, ReadMembers(e.Elements()).Select(m => Measure([m]).Size).DefaultIfEmpty(0).Max(), true, true);
                break;
            case "eventstruct":
                type = new(name, 32, true, true);
                break;
            default: // xidtype, xidunion
                type = new(name, 4, true);
                break;
        }
        types[name] = type;
        return true;
    }

    WireType Resolve(string name) =>
        TryResolve(name.Local(), out var type) ? type : throw new KeyNotFoundException($"Unknown type '{name}'.");

    public int EnumWidth(string enumName)
    {
        if (enumWidths.TryGetValue(enumName, out var width))
            return width;

        foreach (var (en, typeName) in enumFields)
            if (en == enumName && TryResolve(typeName, out var type))
                return enumWidths[enumName] = type.Size;
        return 0;
    }

    static (int Size, bool IsFixed) Measure(IEnumerable<Member> members)
    {
        var size = 0;
        var isFixed = true;
        foreach (var member in members)
        {
            switch (member)
            {
                case Field f:
                    size += f.Type.Size;
                    isFixed &= f.Type.IsFixed;
                    break;
                case Pad p:
                    size += WireMath.PadLength(p.Bytes, p.Align, size);
                    break;
                case ListOf { Length: Const count, Type.IsFixed: true } list:
                    size += list.Type.Size * (int)count.Value;
                    break;
                default:
                    isFixed = false;
                    break;
            }
        }
        return (size, isFixed);
    }

    Member[] ReadMembers(IEnumerable<XElement> elements) =>
    [
        .. elements
            .Where(e => e.Name.LocalName is "field" or "exprfield" or "pad" or "list" or "switch"
                        && e.Attribute("type")?.Value != "fd")
            .Select<XElement, Member>(m => m.Name.LocalName switch
            {
                "pad" => new Pad(m.IntAttr("bytes"), m.IntAttr("align")),
                "field" => new Field(m.Attr("name"), TypeOf(m), m.EnumAttr(), null),
                "exprfield" => new Field(m.Attr("name"), TypeOf(m), m.EnumAttr(), ReadExpr(m.FirstChild()!)),
                "list" => new ListOf(m.Attr("name"), TypeOf(m), m.FirstChild() is { } length ? ReadExpr(length) : null),
                _ => new Switch(m.Attr("name"), ReadExpr(m.FirstChild()!),
                [
                    .. m.Elements()
                        .Where(c => c.Name.LocalName is "case" or "bitcase")
                        .Select(c =>
                        {
                            var (size, isFixed) = Measure(ReadMembers(c.Elements()));
                            var bits = c.Elements()
                                .Where(x => x.Name.LocalName is "enumref" or "value")
                                .Aggregate(0L, (all, x) => all | Number(x));
                            return new Case(c.Name.LocalName == "bitcase", bits, size, isFixed);
                        })
                ]),
            })
    ];

    WireType TypeOf(XElement e) => Resolve(e.Attr("type"));

    Expr ReadExpr(XElement e)
    {
        switch (e.Name.LocalName)
        {
            case "fieldref": return new FieldRef(e.Value.Trim());
            case "paramref": return new ParamRef(e.Value.Trim(), e.Attr("type"));
            case "value" or "enumref": return new Const(Number(e));
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

    long Number(XElement e)
    {
        if (e.Name.LocalName == "value")
            return ParseNumber(e.Value);

        var enumName = e.Attr("ref").Local();
        if (!enumItems.TryGetValue(enumName, out var items))
        {
            enumItems[enumName] = items = [];
            foreach (var item in rawEnums[enumName].SelectMany(x => x.Elements("item")))
                items[item.Attr("name")] =
                    item.Element("value") is { } value ? ParseNumber(value.Value)
                    : item.Element("bit") is { } bit ? 1L << (int)ParseNumber(bit.Value)
                    : 0;
        }
        return items[e.Value.Trim()];
    }

    static long ParseNumber(string text)
    {
        text = text.Trim();
        return text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text[2..], 16) : long.Parse(text);
    }
}


sealed class Request(string name, Member[] members, Member[]? replyMembers)
{
    static readonly (int Size, string Type)[] PadTypes = [(8, "ulong"), (4, "uint"), (2, "ushort"), (1, "byte")];

    public string Name { get; } = name;
    public Member[] Members { get; } = members;
    public Member[]? ReplyMembers { get; } = replyMembers;
    public bool HasReply => ReplyMembers is not null;

    public int StructSize { get; private set; }
    public string ParameterList { get; private set; } = "";
    public Signature Signature { get; private set; } = new([], []);
    public Declaration[] Declarations { get; private set; } = [];
    public List<(string Enum, int Size)> TypedEnums { get; } = []; // fields declared with an enum type

    ProtocolReader protocol = null!;
    readonly List<Param> parameters = [];   // constructor parameters, in field order
    readonly List<string> extras = [];      // extra int parameters (runtime sizes / implicit list lengths)
    readonly List<Declaration> body = [];   // member declarations of the struct
    int padCount;

    public void Analyze(ProtocolReader protocol)
    {
        this.protocol = protocol;
        var rest = new Queue<Member>(Members);

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
                    var storage = f.Type.CsName == "bool" ? "byte" : f.Type.CsName; // C# bool is not blittable

                    string? parameterType;
                    string fieldType, value;
                    if (f.Value is { } expr)
                        (parameterType, fieldType, value) = (null, storage, $"({storage}){Cs.Paren(Render(expr))}");
                    else if (f.Enum is { } en && protocol.EnumWidth(en) == f.Type.Size)
                    {
                        (parameterType, fieldType, value) = (en, en, name);
                        TypedEnums.Add((en, f.Type.Size));
                    }
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
                case Field field: // a struct field with a runtime size
                    length.Add(Extra($"{field.Name}_bytes"), 1);
                    break;
                case ListOf list: // list of variable-size elements
                    length.Add(Extra($"{list.Name}_bytes"), 1);
                    break;
            }
        }
        return length.Build();
    }

    string Render(Expr e)
    {
        try
        {
            return Exprs.Render(e, name =>
            {
                var field = Members.OfType<Field>().FirstOrDefault(f => f.Value is null && f.Name == name);
                if (field == null)
                    return Extra(name); // implicit list length, e.g. string_len in QueryTextExtents

                // length math is done in int, so AddPadding()/CountFlags() never run on a
                // byte/ushort operand that would overflow when padded (e.g. name_len = 65535)
                var parameter = name.Escape();
                var type = field.Type.CsName;
                if (field.Enum == null && type == "bool")
                    return $"({parameter} ? 1 : 0)";
                return field.Enum == null && type == "int" ? parameter : $"(int){parameter}";
            });
        }
        catch (NotSupportedException ex)
        {
            throw new NotSupportedException($"{ex.Message} in request {Name}");
        }
    }

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

    readonly Member[]? source;
    readonly HashSet<string> usedNames;
    readonly List<string> properties = [];                // property lines
    readonly Dictionary<string, Prop> props = [];         // by XML field name
    readonly List<string> runtime = [];                   // runtime terms of the current offset
    readonly int headerAt, bodyAt;                        // header bytes [headerAt, bodyAt) after response_type
    int offset = 1;                                       // constant part of the current offset
    int fixedEnd = -1;                                    // offset where the fixed part ends
    bool known = true;                                    // false once an offset can no longer be computed
    bool needsSumPop;

    // Members are laid out from byte 1 (right after response_type). The header (the sequence number,
    // plus length/event type for replies and XGE events) occupies [headerAt, bodyAt); a member is
    // placed in front of it only when it fits there, otherwise the layout continues at bodyAt.
    protected Message(string name, int headerAt, int bodyAt, Member[]? source)
    {
        Name = name;
        this.headerAt = headerAt;
        this.bodyAt = bodyAt;
        this.source = source;
        usedNames = ["ResponseType", "Sequence", "Number", "Length", "ByteSize", "EventType", "Extension", "ErrorCode", "Verify", "FromBytes", name];
    }

    public string Name { get; }

    public string? Todo { get; private set; }
    public int MinSize { get; private set; }
    public string[] Header { get; private set; } = [];
    public string[] Properties { get; private set; } = [];
    public bool NeedsSumPop { get; private set; }

    protected abstract void AddHeader(List<string> lines);
    protected virtual string? Intrinsic(string field) => null;

    public void Analyze()
    {
        if (source == null)
        {
            Todo = $"// TODO: {Name} is a copy of an event defined in another file";
            return;
        }

        foreach (var member in source)
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

        string count;
        try
        {
            count = list.Length is null ? throw new NotSupportedException("list without length") : Render(list.Length);
        }
        catch (NotSupportedException ex)
        {
            properties.Add($"// TODO: list '{list.Name}' has an unsupported length ({ex.Message})");
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

    string Render(Expr e) => Exprs.Render(e, PropertyRef, (string list) =>
    {
        if (!props.TryGetValue(list, out var p) || p is not { IsList: true, Type: "uint" })
            return null;
        needsSumPop = true;
        return $"SumPop({p.Name})";
    });

    string PropertyRef(string field)
    {
        if (!props.ContainsKey(field) && Intrinsic(field) is { } intrinsic)
            return intrinsic;
        if (!props.TryGetValue(field, out var p) || p.IsList)
            throw new NotSupportedException($"fieldref {field}");
        return p.Type == "bool" ? $"({p.Name} ? 1 : 0)" : $"(int){p.Name}";
    }

    void SkipHeader(Member member)
    {
        if (offset >= bodyAt || headerAt == bodyAt)
            return; // already past the header, or there is none
        int? size = member switch
        {
            Pad { Bytes: { } bytes } => bytes,
            Pad => 0,
            Field { Type.IsFixed: true } f => f.Type.Size,
            ListOf { Length: Const count, Type.IsFixed: true } list => list.Type.Size * (int)count.Value,
            _ => null, // variable size: never fits in front of the header
        };
        if (offset >= headerAt || size is not { } s || offset + s > headerAt)
            offset = bodyAt;
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

// reply: sequence at 2, length at 4, body from 8 (byte 1 is the first member's)
sealed class Reply(string name, Member[] members) : Message(name, headerAt: 2, bodyAt: 8, members)
{
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
    readonly bool isXge;
    readonly bool hasSequence;

    public int Number { get; }

    // XGE: extension 1, sequence 2, length 4, event type 8, body from 10
    // core/extension event: sequence at 2, body from 4 (byte 1 is the first member's)
    // no-sequence-number event (KeymapNotify): no header after response_type
    public Event(string xmlName, int number, bool isXge, bool hasSequence, Member[]? members)
        : base(xmlName.EndsWith("Event") ? xmlName : xmlName + "Event",
               headerAt: isXge ? 1 : 2,
               bodyAt: isXge ? 10 : hasSequence ? 4 : 2,
               members)
    {
        Number = number;
        this.isXge = isXge;
        this.hasSequence = hasSequence && !isXge;
    }

    protected override void AddHeader(List<string> lines)
    {
        if (isXge)
        {
            lines.Add("public byte Extension => _data[1];");
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
            lines.Add("public uint Length => Unsafe.As<byte, uint>(ref _data[4]);");
            lines.Add("public int ByteSize => checked(32 + (int)Length * 4);");
            lines.Add("public ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);");
        }
        else if (hasSequence)
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
    }
}


// error: error code at 1, sequence at 2, body from 4; always 32 bytes
sealed class Error(string xmlName, int number, string? copyOf, Member[]? members)
    : Message(xmlName.EndsWith("Error") ? xmlName : xmlName + "Error", headerAt: 1, bodyAt: 4, members)
{
    public int Number { get; } = number;      // glx Generic is -1
    public string? CopyOf { get; } = copyOf;  // set for <errorcopy>, may name an error of an imported module

    protected override void AddHeader(List<string> lines)
    {
        lines.Add("public byte ErrorCode => _data[1];");
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
    public static string Render(Expr e, Func<string, string> field, Func<string, string?>? sumPop = null) => e switch
    {
        Const c => c.Value.ToString(),
        FieldRef f => field(f.Name),
        Binary b => $"({Render(b.Left, field, sumPop)} {b.Op} {Render(b.Right, field, sumPop)})",
        Unary u => $"{u.Op}{Cs.Paren(Render(u.Operand, field, sumPop))}",
        PopCount p => $"{Cs.Paren(Render(p.Operand, field, sumPop))}.CountFlags()",
        SumOf { Body: PopCount { Operand: ListElement } } s when sumPop?.Invoke(s.List) is { } call => call,
        SumOf => throw new NotSupportedException("sumof"),
        UnknownExpr x => throw new NotSupportedException($"<{x.Tag}>"),
        _ => throw new NotSupportedException(e.GetType().Name),
    };

    // human-readable form for the analysis report; never throws
    public static string Describe(Expr e) => e switch
    {
        Const c => c.Value.ToString(),
        FieldRef f => f.Name,
        ParamRef p => $"parent.{p.Name}",
        Binary b => $"({Describe(b.Left)} {b.Op} {Describe(b.Right)})",
        Unary u => $"{u.Op}{Describe(u.Operand)}",
        PopCount p => $"popcount({Describe(p.Operand)})",
        SumOf s => s.Body is null ? $"sum({s.List})" : $"sum({s.List}, {Describe(s.Body)})",
        ListElement => "element",
        UnknownExpr x => $"<{x.Tag}>",
        _ => e.GetType().Name,
    };
}


static class WireMath
{
    public static int Align4(int bytes) => (bytes + 3) & ~3;

    public static int PadLength(int? bytes, int? align, int offset) =>
        bytes ?? (align!.Value - offset % align.Value) % align.Value;
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

    public static string? EnumAttr(this XElement e) => e.Attribute("enum") is { } a ? a.Value.Local() : null;

    public static XElement? FirstChild(this XElement e) => e.Elements().FirstOrDefault(c => c.Name.LocalName != "doc");
    public static string Local(this string name) => name[(name.IndexOf(':') + 1)..];
}
