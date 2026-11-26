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
                        todo ??= ($"// TODO: {r.Name} has a {what} which can't be represented in c#");
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
                        todo ??= ($"// TODO: {r.Name} has a {what} which can't be represented in c#");
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

abstract record Definition(string Name);
sealed record EnumDef(string Name, EnumItem[] Items, Doc? Doc) : Definition(Name);
sealed record EnumItem(string Name, long Value, int? Bit); // Bit is set for a <bit> item, whose Value is 1 << Bit
sealed record TypedefDef(string Name, string OldName) : Definition(Name); // newname, oldname
sealed record XidTypeDef(string Name) : Definition(Name);
sealed record XidUnionDef(string Name, string[] Types) : Definition(Name);
sealed record StructDef(string Name, Member[] Members, int Size, bool IsFixed) : Definition(Name);
sealed record UnionDef(string Name, Member[] Members, int Size, bool IsFixed) : Definition(Name);
sealed record EventStructDef(string Name, AllowedEvents[] Allowed) : Definition(Name);
sealed record AllowedEvents(string Extension, bool Xge, int OpcodeMin, int OpcodeMax);
sealed record RequestDef(string Name, int Opcode, bool CombineAdjacent, Member[] Members,
    StartAlign? RequiredStartAlign, ReplyDef? Reply, Doc? Doc) : Definition(Name);
sealed record ReplyDef(Member[] Members, StartAlign? RequiredStartAlign, Doc? Doc);
sealed record EventDef(string Name, int Number, bool Xge, bool NoSequenceNumber, Member[] Members,
    StartAlign? RequiredStartAlign, Doc? Doc) : Definition(Name);
sealed record EventCopyDef(string Name, int Number, string Ref) : Definition(Name);
sealed record ErrorDef(string Name, int Number, Member[] Members) : Definition(Name);
sealed record ErrorCopyDef(string Name, int Number, string Ref) : Definition(Name);

sealed record StartAlign(int Align, int Offset);

sealed record Doc(string? Brief, string? Description, string? Example, DocField[] Fields, DocError[] Errors, DocSee[] See);
sealed record DocField(string Name, string Text);
sealed record DocError(string Type, string Text);
sealed record DocSee(string Type, string Name);

abstract record Expr { }
sealed record Const(long Value) : Expr;                              // <value>, or <bit> as 1 << bit
sealed record EnumRef(string Enum, string Item, long Value) : Expr;  // <enumref ref="Enum">Item</enumref>
sealed record FieldRef(string Name) : Expr;
sealed record ParamRef(string Name, string Type) : Expr; // a field of the enclosing request/reply
sealed record Binary(string Op, Expr Left, Expr Right) : Expr;
sealed record Unary(string Op, Expr Operand) : Expr;
sealed record PopCount(Expr Operand) : Expr;
sealed record SumOf(string List, Expr? Body) : Expr;
sealed record ListElement : Expr { }
sealed record UnknownExpr(string Tag) : Expr;

sealed record EnumUse(string Kind, string Name);

abstract record Member { }
sealed record Pad(int? Bytes, int? Align, bool Serialize = false) : Member;
sealed record Field(string Name, string TypeName, WireType Type, EnumUse? EnumUse, Expr? Value) : Member // Value: <exprfield>
{
    public string? Enum => EnumUse is { Kind: "enum" } use ? use.Name.Local() : null; // an exhaustive enum, unprefixed
}
sealed record ListOf(string Name, string TypeName, WireType Type, EnumUse? EnumUse, Expr? Length) : Member;
sealed record Fd(string Name) : Member;                       // <fd>: a file descriptor, passed out of band
sealed record FdList(string Name, Expr? Length) : Member;     // <list type="fd">
sealed record ValueParam(string MaskType, string MaskName, string ListName) : Member; // documented, no file uses it
sealed record Switch(string Name, Expr Selector, Case[] Cases, StartAlign? RequiredStartAlign) : Member;
sealed record Case(string? Name, bool IsBitCase, Expr[] Values, long Bits, Member[] Members,
    StartAlign? RequiredStartAlign, int Size, bool IsFixed);

sealed record Param(string Type, string Name);
sealed record Signature(Param[] Parameters, Member[] Trailing);


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

    public string Namespace { get; }                // the header attribute: the file's module name
    public ExtensionInfo? Extension { get; }        // null for xproto, the core protocol
    public bool IsCore => Extension is null;
    public string[] Imports { get; }

    public Request[] Requests { get; }
    public Reply[] Replies { get; }
    public Event[] Events { get; }

    public ProtocolReader(string path)
    {
        directory = Path.GetDirectoryName(path)!;
        Collect("xproto"); // xproto's types are available without an <import>
        var root = Collect(Path.GetFileNameWithoutExtension(path));
        Namespace = root.Attr("header");
        Extension = root.Attribute("extension-name") is null ? null : new ExtensionInfo(root.Attr("extension-xname"),
            root.Attr("extension-name"), root.BoolAttr("extension-multiword"),
            root.IntAttr("major-version") ?? 0, root.IntAttr("minor-version") ?? 0);
        Imports = [.. root.Elements("import").Select(i => i.Value.Trim())];
        var definitions = root.Elements()
            .Where(e => e.Name.LocalName != "import")
            .Select<XElement, Definition>(e => e.Name.LocalName switch
            {
                "enum" => new EnumDef(e.Attr("name"), ReadItems(e), DocOf(e)),
                "typedef" => new TypedefDef(e.Attr("newname"), e.Attr("oldname")),
                "xidtype" => new XidTypeDef(e.Attr("name")),
                "xidunion" => new XidUnionDef(e.Attr("name"), [.. e.Elements("type").Select(t => t.Value.Trim())]),
                "struct" => new StructDef(e.Attr("name"), ReadMembers(e.Elements()),
                    Resolve(e.Attr("name")).Size, Resolve(e.Attr("name")).IsFixed),
                "union" => new UnionDef(e.Attr("name"), ReadMembers(e.Elements()),
                    Resolve(e.Attr("name")).Size, Resolve(e.Attr("name")).IsFixed),
                "eventstruct" => new EventStructDef(e.Attr("name"), [.. e.Elements("allowed").Select(a => new AllowedEvents(
                    a.Attr("extension"), a.BoolAttr("xge"), int.Parse(a.Attr("opcode-min")), int.Parse(a.Attr("opcode-max"))))]),
                "request" => new RequestDef(e.Attr("name"), int.Parse(e.Attr("opcode")), e.BoolAttr("combine-adjacent"),
                    ReadMembers(e.Elements()), StartAlignOf(e),
                    e.Element("reply") is { } reply
                        ? new ReplyDef(ReadMembers(reply.Elements()), StartAlignOf(reply), DocOf(reply))
                        : null,
                    DocOf(e)),
                "event" => new EventDef(e.Attr("name"), int.Parse(e.Attr("number")), e.BoolAttr("xge"),
                    e.BoolAttr("no-sequence-number"), ReadMembers(e.Elements()), StartAlignOf(e), DocOf(e)),
                "eventcopy" => new EventCopyDef(e.Attr("name"), int.Parse(e.Attr("number")), e.Attr("ref")),
                "error" => new ErrorDef(e.Attr("name"), int.Parse(e.Attr("number")), ReadMembers(e.Elements())),
                "errorcopy" => new ErrorCopyDef(e.Attr("name"), int.Parse(e.Attr("number")), e.Attr("ref")),
                var tag => throw new NotSupportedException($"<{tag}> in {Namespace} is not a known top-level element"),
            });

        Requests = [.. definitions.OfType<RequestDef>().Select(r =>
        {
            var request = new Request(r.Name, r.Members.OnWire(), r.Reply?.Members.OnWire());
            request.Analyze(this);
            return request;
        })];

        Replies = [.. Requests.Where(r => r.HasReply).Select(r =>
        {
            var reply = new Reply(r.Name + "Reply", r.ReplyMembers!);
            reply.Analyze();
            return reply;
        })];

        Events = [.. definitions.Select(d => d switch
        {
            EventDef e => new Event(e.Name, e.Number, e.Xge, !e.NoSequenceNumber, e.Members.OnWire()),
            EventCopyDef c => definitions.OfType<EventDef>().LastOrDefault(e => e.Name == c.Ref.Local()) is { } source
                ? new Event(c.Name, c.Number, source.Xge, !source.NoSequenceNumber, source.Members.OnWire())
                : new Event(c.Name, c.Number, false, false, null),
            _ => null,
        }).OfType<Event>()];
        foreach (var ev in Events)
            ev.Analyze();
    }

    static EnumItem[] ReadItems(XElement e)
    {
        List<EnumItem> items = [];
        long next = 0;
        foreach (var item in e.Elements("item"))
        {
            int? bit = item.Element("bit") is { } b ? (int)ParseNumber(b.Value) : null;
            var value = bit is { } n ? 1L << n : item.Element("value") is { } v ? ParseNumber(v.Value) : next;
            next = value + 1;
            items.Add(new(item.Attr("name"), value, bit));
        }
        return [.. items];
    }

    static Doc? DocOf(XElement e) => e.Element("doc") is not { } d ? null : new(
        d.Element("brief")?.Value.Trim(),
        d.Element("description")?.Value.Trim(),
        d.Element("example")?.Value.Trim(),
        [.. d.Elements("field").Select(f => new DocField(f.Attr("name"), f.Value.Trim()))],
        [.. d.Elements("error").Select(x => new DocError(x.Attr("type"), x.Value.Trim()))],
        [.. d.Elements("see").Select(s => new DocSee(s.Attr("type"), s.Attr("name")))]);

    static StartAlign? StartAlignOf(XElement e) =>
        e.Element("required_start_align") is { } a ? new(int.Parse(a.Attr("align")), a.IntAttr("offset") ?? 0) : null;

    static EnumUse? EnumUseOf(XElement e) => e.Attributes()
        .Where(a => a.Name.LocalName is "enum" or "altenum" or "mask" or "altmask")
        .Select(a => new EnumUse(a.Name.LocalName, a.Value))
        .FirstOrDefault();

    XElement Collect(string file)
    {
        var xml = XmlCache.GetOrAdd(Path.Join(directory, file + ".xml"), p => XDocument.Load(p).Root!);
        if (!loaded.Add(file))
            return xml;

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
            if (field.Attribute("enum") is { } enumName)
                enumFields.Add((enumName.Value.Local(), field.Attr("type").Local()));

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
            case "union": // as big as its biggest member, and (as in xcbgen) only fixed-size when every member is
                var parts = ReadMembers(e.Elements()).Select(m => Measure([m])).ToArray();
                type = new(name, parts.Select(p => p.Size).DefaultIfEmpty(0).Max(), parts.All(p => p.IsFixed), true);
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
        foreach (var member in members.OnWire())
        {
            var bytes = WireMath.SizeOf(member, size);
            size += bytes ?? (member as Field)?.Type.Size ?? 0; // a variable-size struct: its static bytes
            isFixed &= bytes is not null;
        }
        return (size, isFixed);
    }

    Member[] ReadMembers(IEnumerable<XElement> elements) =>
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

    long Number(XElement e)
    {
        switch (e.Name.LocalName)
        {
            case "value": return ParseNumber(e.Value);
            case "bit": return 1L << (int)ParseNumber(e.Value);
        }

        var enumName = e.Attr("ref").Local();
        if (!enumItems.TryGetValue(enumName, out var items))
        {
            enumItems[enumName] = items = [];
            foreach (var item in rawEnums[enumName].SelectMany(ReadItems))
                items[item.Name] = item.Value;
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
        if (offset >= headerAt || WireMath.SizeOf(member, offset) is not { } s || offset + s > headerAt)
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
        EnumRef r => r.Value.ToString(),
        FieldRef f => field(f.Name),
        Binary b => $"({Render(b.Left, field, sumPop)} {b.Op} {Render(b.Right, field, sumPop)})",
        Unary u => $"{u.Op}{Cs.Paren(Render(u.Operand, field, sumPop))}",
        PopCount p => $"{Cs.Paren(Render(p.Operand, field, sumPop))}.CountFlags()",
        SumOf { Body: PopCount { Operand: ListElement } } s when sumPop?.Invoke(s.List) is { } call => call,
        SumOf => throw new NotSupportedException("sumof"),
        UnknownExpr x => throw new NotSupportedException($"<{x.Tag}>"),
        _ => throw new NotSupportedException(e.GetType().Name),
    };
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
