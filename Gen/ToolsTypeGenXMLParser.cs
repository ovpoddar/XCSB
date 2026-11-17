#:sdk Microsoft.NET.Sdk
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string I1 = "    ";
const string I2 = "        ";
const string I3 = "            ";
var source = args.Length > 0 ? args[0] : "/mnt/hardDrive/testP/xcb-proto/src";
while (!Directory.Exists(source))
{
    Console.WriteLine("Put the Folder path.");
    source = Console.ReadLine();
    if (source == null) return;
}

using (var stream = new StreamWriter("out.G.cs"))
{
    stream.WriteLine("using System;");
    stream.WriteLine("using System.Runtime.CompilerServices;");
    stream.WriteLine("using System.Runtime.InteropServices;");
    stream.WriteLine("using Xcsb.Connection.Helpers;");
    stream.WriteLine("using Xcsb.Connection.Models;");
    stream.WriteLine("using Xcsb.Connection.Response;");
    stream.WriteLine("using Xcsb.Connection.Response.Contract;");

    foreach (var item in Directory.GetFiles(source, "*.xml").Order(StringComparer.Ordinal))
    {
        var protocol = ProtocolReader.Read(item);
        stream.WriteLine($"namespace {protocol.Namespace}");
        stream.WriteLine("{");
        foreach (var request in protocol.Requests)
        {
            stream.WriteLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1, Size = {request.StructSize})]");
            stream.WriteLine($"{I1}[method: MethodImpl(MethodImplOptions.AggressiveInlining)]");
            stream.WriteLine($"{I1}internal readonly struct {request.Name}Type({request.ParameterList})");
            stream.WriteLine($"{I1}{{");
            foreach (var m in request.Members)
                stream.WriteLine($"{I2}{m.Access} readonly {m.Type} {m.Name} = {m.Value};");
            stream.WriteLine($"{I1}}}");
        }
        foreach (var reply in protocol.Replies)
        {
            stream.WriteLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1)]");
            stream.WriteLine($"{I1}public readonly struct {reply.Name} : IXReply<{reply.Name}, {reply.Name}>, IVerify");
            stream.WriteLine($"{I1}{{");
            stream.WriteLine($"{I2}private readonly byte[] _data;");
            stream.WriteLine();
            stream.WriteLine($"{I2}private {reply.Name}(byte[] data)");
            stream.WriteLine($"{I2}{{");
            stream.WriteLine($"{I3}if (data is null) throw new ArgumentNullException(nameof(data));");
            stream.WriteLine($"{I3}if (data.Length < {reply.MinSize})");
            stream.WriteLine($"{I3}    throw new ArgumentException(\"Buffer too small for {reply.Name}.\", nameof(data));");
            stream.WriteLine($"{I3}_data = data;");
            stream.WriteLine($"{I2}}}");
            stream.WriteLine();

            foreach (var line in reply.Header)
                stream.WriteLine(I2 + line);
            foreach (var line in reply.Members)
                stream.WriteLine(I2 + line);

            if (reply.NeedsSumPop)
            {
                stream.WriteLine();
                stream.WriteLine($"{I2}private static int SumPop(ReadOnlySpan<uint> s)");
                stream.WriteLine($"{I2}{{");
                stream.WriteLine($"{I3}var n = 0;");
                stream.WriteLine($"{I3}foreach (var v in s) n += v.CountFlags();");
                stream.WriteLine($"{I3}return n;");
                stream.WriteLine($"{I2}}}");
            }

            stream.WriteLine();
            stream.WriteLine($"{I2}public {reply.Name} FromBytes(byte[] response) => new {reply.Name}(response);");
            stream.WriteLine($"{I1}}}");
            stream.WriteLine();
        }
        foreach (var ev in protocol.Events)
        {
            if (ev.Todo != null)
            {
                stream.WriteLine($"{I1}{ev.Todo}");
                return;
            }

            stream.WriteLine($"{I1}[StructLayout(LayoutKind.Sequential, Pack = 1)]");
            stream.WriteLine($"{I1}public readonly struct {ev.Name} : IXEvent<{ev.Name}>");
            stream.WriteLine($"{I1}{{");
            stream.WriteLine($"{I2}private readonly byte[] _data;");
            stream.WriteLine($"{I2}public const byte Number = {ev.Number};");
            stream.WriteLine();
            stream.WriteLine($"{I2}private {ev.Name}(byte[] data)");
            stream.WriteLine($"{I2}{{");
            stream.WriteLine($"{I3}if (data is null) throw new ArgumentNullException(nameof(data));");
            stream.WriteLine($"{I3}if (data.Length < {ev.MinSize})");
            stream.WriteLine($"{I3}    throw new ArgumentException(\"Buffer too small for {ev.Name}.\", nameof(data));");
            stream.WriteLine($"{I3}_data = data;");
            stream.WriteLine($"{I2}}}");
            stream.WriteLine();

            foreach (var line in ev.Header)
                stream.WriteLine(I2 + line);
            foreach (var line in ev.Members)
                stream.WriteLine(I2 + line);

            if (ev.NeedsSumPop)
            {
                stream.WriteLine();
                stream.WriteLine($"{I2}private static int SumPop(ReadOnlySpan<uint> s)");
                stream.WriteLine($"{I2}{{");
                stream.WriteLine($"{I3}var n = 0;");
                stream.WriteLine($"{I3}foreach (var v in s) n += v.CountFlags();");
                stream.WriteLine($"{I3}return n;");
                stream.WriteLine($"{I2}}}");
            }

            stream.WriteLine();
            stream.WriteLine($"{I2}public {ev.Name} FromBytes(byte[] response) => new {ev.Name}(response);");
            stream.WriteLine($"{I1}}}");
            stream.WriteLine();
        }

        stream.WriteLine($"{I1}public interface I{protocol.Namespace.Split('.')[^1].ToPascalCase()}VoidProto");
        stream.WriteLine($"{I1}{{");
        foreach (var r in protocol.Requests.Where(r => !r.HasReply))
            stream.WriteLine(I2 + OutputWriter.InterfaceMethod(r));
        stream.WriteLine($"{I1}}}");

        stream.WriteLine($"{I1}public interface I{protocol.Namespace.Split('.')[^1].ToPascalCase()}ResponseProto");
        stream.WriteLine($"{I1}{{");
        foreach (var r in protocol.Requests.Where(r => r.HasReply))
            stream.WriteLine(I2 + OutputWriter.InterfaceMethod(r));
        stream.WriteLine($"{I1}}}");
        
        stream.WriteLine($"{I1}public interface I{protocol.Namespace.Split('.')[^1].ToPascalCase()}Request : I{protocol.Namespace.Split('.')[^1].ToPascalCase()}ResponseProto, I{protocol.Namespace.Split('.')[^1].ToPascalCase()}VoidProto");
        stream.WriteLine($"{I1}{{{Environment.NewLine}{I1}}}");
        stream.WriteLine("}");
    }

}

Console.WriteLine("Written");

sealed record Declaration(string Access, string Type, string Name, string Value);

sealed record RequestModel(string Name, int StructSize, string ParameterList, Declaration[] Members,
    bool HasReply, Signature Signature);
sealed record EventModel(string Name, string? Todo, int Number, int MinSize, string[] Header,
    string[] Members, bool NeedsSumPop, bool IsReply = false);
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

abstract record Member { }
sealed record Pad(int? Bytes, int? Align) : Member;
sealed record Field(string Name, WireType Type, string? Enum, Expr? Value) : Member;
sealed record ListOf(string Name, WireType Type, Expr? Length) : Member;
sealed record Switch(string Name, Expr Selector, Case[] Cases) : Member;
sealed record Case(bool IsBitCase, long Bits, int Size, bool IsFixed);

sealed record Request(string Name, Member[] Members, Member[]? ReplyMembers)
{
    public bool HasReply => ReplyMembers is not null;
}
sealed record EventDef(string Name, int Number, bool IsXge, Member[]? Members);

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
    readonly Dictionary<string, WireType> types = new(Builtins);
    readonly Dictionary<(string Enum, string Item), long> enumItems = [];
    readonly Dictionary<string, int> enumWidths = [];
    readonly HashSet<string> loaded = [];

    // ---- everything learned about the protocol; analyzers and writers read it from here ----
    public string Namespace { get; private set; } = "";
    public bool IsCore { get; private set; }
    public IReadOnlyDictionary<string, WireType> Types => types;
    public IReadOnlyDictionary<(string Enum, string Item), long> EnumItems => enumItems;
    public IReadOnlyDictionary<string, int> EnumWidths => enumWidths;
    public RequestModel[] Requests { get; private set; } = [];
    public EventModel[] Events { get; private set; } = [];
    public EventModel[] Replies { get; private set; } = [];

    ProtocolReader(string directory) => this.directory = directory;

    public static ProtocolReader Read(string path)
    {
        var reader = new ProtocolReader(Path.GetDirectoryName(path)!);
        reader.ReadRoot(Path.GetFileNameWithoutExtension(path));
        return reader;
    }

    void ReadRoot(string file)
    {
        var root = Load(file)!;

        Namespace = root.Attr("header");
        IsCore = root.Attribute("extension-name") is null;
        List<RequestModel> requests = [];
        List<EventModel> replies = [];
        foreach (var r in root.Elements("request"))
        {
            var reply = r.Element("reply");
            var request = new Request(r.Attr("name"), ReadMembers(r.Elements()),
                reply is null ? null : ReadMembers(reply.Elements()));
            requests.Add(new RequestAnalyzer(this, request).Analyze());
            if (request.ReplyMembers is { } members)
                replies.Add(new EventAnalyzer(new EventDef(request.Name + "Reply", 1, false, members), isReply: true).Analyze());
        }
        Replies = [.. replies];

        var eventElements = root.Elements().Where(e => e.Name.LocalName is "event" or "eventcopy").ToArray();
        Dictionary<string, (bool Xge, Member[] Members)> defined = [];
        foreach (var e in eventElements)
            if (e.Name.LocalName == "event")
                defined[e.Attr("name")] = (e.Attribute("xge") is not null, ReadMembers(e.Elements()));

        var events = new EventModel[eventElements.Length];
        for (var i = 0; i < events.Length; i++)
        {
            var e = eventElements[i];
            var name = e.Attr("name");
            var origin = e.Name.LocalName == "event" ? name : e.Attr("ref").Local();
            defined.TryGetValue(origin, out var def);
            events[i] = new EventAnalyzer(new EventDef(name, int.Parse(e.Attr("number")), def.Xge, def.Members)).Analyze();
        }
        Events = events;

        Requests = [.. requests];
    }
    XElement? Load(string file)
    {
        if (!loaded.Add(file))
            return null;

        // Parsed XML is shared across readers: every extension imports xproto, so without
        // the cache it would be re-read and re-parsed once per protocol file.
        var xml = XmlCache.GetOrAdd(Path.Join(directory, file + ".xml"), p => XDocument.Load(p).Root!);

        foreach (var import in xml.Elements("import"))
            Load(import.Value);

        var elements = xml.Elements().ToArray();   // enumerate the root once
        foreach (var e in elements)
            if (e.Name.LocalName == "enum")
                DefineEnum(e);
        foreach (var e in elements)
            DefineType(e);

        foreach (var field in xml.Descendants("field"))
            if (field.EnumAttr() is { } enumName && types.TryGetValue(field.Attr("type").Local(), out var type))
                enumWidths.TryAdd(enumName, type.Size);

        return xml;
    }

    void DefineEnum(XElement e)
    {
        var name = e.Attr("name");
        foreach (var item in e.Elements("item"))
            enumItems[(name, item.Attr("name"))] =
                item.Element("value") is { } value ? ParseNumber(value.Value)
                : item.Element("bit") is { } bit ? 1L << (int)ParseNumber(bit.Value)
                : 0;
    }

    void DefineType(XElement e)
    {
        var name = e.Attribute("name")?.Value ?? "";
        switch (e.Name.LocalName)
        {
            case "xidtype" or "xidunion":
                types[name] = new(name, 4, true);
                break;
            case "typedef":
                types[e.Attr("newname")] = Resolve(e.Attr("oldname"));
                break;
            case "struct":
                {
                    var (size, isFixed) = Measure(ReadMembers(e.Elements()));
                    types[name] = new(name, size, isFixed, true);
                    break;
                }
            case "union":
                {
                    var largest = 0;
                    foreach (var m in ReadMembers(e.Elements()))
                        largest = Math.Max(largest, Measure([m]).Size);
                    types[name] = new(name, largest, true, true);
                    break;
                }
            case "eventstruct":
                types[name] = new(name, 32, true, true);
                break;
        }
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

    WireType Resolve(string name) => types[name.Local()];
    WireType TypeOf(XElement e) => Resolve(e.Attr("type"));

    static bool IsMember(XElement e) =>
        e.Name.LocalName is "field" or "exprfield" or "pad" or "list" or "switch"
        && e.Attribute("type")?.Value != "fd";

    Member[] ReadMembers(IEnumerable<XElement> elements) => [.. elements.Where(IsMember).Select(ReadMember)];

    Member ReadMember(XElement m) => m.Name.LocalName switch
    {
        "pad" => new Pad(m.IntAttr("bytes"), m.IntAttr("align")),
        "field" => new Field(m.Attr("name"), TypeOf(m), m.EnumAttr(), null),
        "exprfield" => new Field(m.Attr("name"), TypeOf(m), m.EnumAttr(), ReadExpr(m.FirstChild()!)),
        "list" => new ListOf(m.Attr("name"), TypeOf(m), m.FirstChild() is { } length ? ReadExpr(length) : null),
        _ => new Switch(m.Attr("name"), ReadExpr(m.FirstChild()!), ReadCases(m)),
    };

    Case[] ReadCases(XElement sw) =>
    [
        .. sw.Elements()
            .Where(c => c.Name.LocalName is "case" or "bitcase")
            .Select(c =>
            {
                var (size, isFixed) = Measure(ReadMembers(c.Elements()));
                var bits = c.Elements()
                    .Where(x => x.Name.LocalName is "enumref" or "value")
                    .Aggregate(0L, (all, x) => all | Number(x));
                return new Case(c.Name.LocalName == "bitcase", bits, size, isFixed);
            })
    ];

    Expr ReadExpr(XElement e)
    {
        var args = e.Elements().Where(x => x.Name.LocalName != "doc").Select(ReadExpr).ToArray();
        return e.Name.LocalName switch
        {
            "fieldref" => new FieldRef(e.Value.Trim()),
            "value" or "enumref" => new Const(Number(e)),
            "op" => new Binary(e.Attr("op"), args[0], args[1]),
            "unop" => new Unary(e.Attr("op"), args[0]),
            "popcount" => new PopCount(args[0]),
            "sumof" => new SumOf(e.Attr("ref"), args.FirstOrDefault()),
            "listelement-ref" => new ListElement(),
            var tag => new UnknownExpr(tag),
        };
    }
    long Number(XElement e) =>
        e.Name.LocalName == "value" ? ParseNumber(e.Value) : enumItems[(e.Attr("ref").Local(), e.Value.Trim())];

    static long ParseNumber(string text)
    {
        text = text.Trim();
        return text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text[2..], 16) : long.Parse(text);
    }
}



sealed class RequestAnalyzer
{
    static readonly (int Size, string Type)[] PadTypes = [(8, "ulong"), (4, "uint"), (2, "ushort"), (1, "byte")];

    readonly ProtocolReader protocol;
    readonly Request request;
    readonly List<Param> parameters = [];   // constructor parameters, in field order
    readonly List<string> extras = [];      // extra int parameters (runtime sizes / implicit list lengths)
    readonly List<Declaration> body = [];   // member declarations of the struct
    int padCount;

    public RequestAnalyzer(ProtocolReader protocol, Request request)
    {
        this.protocol = protocol;
        this.request = request;
    }

    static bool IsFixed(Member m) => m is Pad or Field { Type.IsFixed: true };
    static bool FitsInByte(Member m) => m is Pad { Bytes: 1 } or Field { Type: { IsFixed: true, Size: 1 } };

    public RequestModel Analyze()
    {
        var rest = new Queue<Member>(request.Members);

        if (!protocol.IsCore)
        {
            parameters.Add(new("byte", "majorOpCode"));
            body.Add(new("public", "byte", "MajorOpcode", "majorOpCode"));
        }
        body.Add(new("public", "Opcode", "Opcode", $"Opcode.{request.Name}"));

        if (protocol.IsCore)
        {
            if (rest.TryPeek(out var first) && FitsInByte(first))
                Emit(rest.Dequeue(), 1);
            else
                body.AddRange(Pads(1));
        }

        var lengthAt = body.Count;
        var size = 4;
        while (rest.TryPeek(out var next) && IsFixed(next))
            size += Emit(rest.Dequeue(), size);
        body.Insert(lengthAt, new("public", "ushort", "Length", Length(size, rest)));

        var structSize = rest.Count == 0 ? WireMath.Align4(size) : size;
        var ctor = parameters.Select(p => $"{p.Type} {p.Name.Escape()}")
            .Concat(extras.Select(x => $"int {x.Escape()}"));

        var signature = new Signature([.. parameters.Skip(protocol.IsCore ? 0 : 1)], [.. rest]);
        return new RequestModel(request.Name, structSize, string.Join(", ", ctor), [.. body], request.HasReply, signature);
    }

    int Emit(Member m, int offset)
    {
        switch (m)
        {
            case Field f:
                body.Add(Declare(f));
                return f.Type.Size;
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

    Declaration Declare(Field f)
    {
        var name = f.Name.Escape();
        var storage = f.Type.CsName == "bool" ? "byte" : f.Type.CsName; // C# bool is not blittable

        string? parameterType;
        string fieldType, value;
        if (f.Value is { } expr)
            (parameterType, fieldType, value) = (null, storage, $"({storage}){Cs.Paren(Render(expr))}");
        else if (f.Enum is { } en && protocol.EnumWidths.GetValueOrDefault(en) == f.Type.Size)
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
        return new(access, fieldType, f.Name.ToPascalCase(), value);
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
                    AddSwitch(length, sw);
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

    void AddSwitch(LengthBuilder length, Switch sw)
    {
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
    }

    string Render(Expr e)
    {
        try
        {
            return Exprs.Render(e, FieldValue);
        }
        catch (NotSupportedException ex)
        {
            throw new NotSupportedException($"{ex.Message} in request {request.Name}");
        }
    }

    string FieldValue(string name)
    {
        var field = request.Members.OfType<Field>().FirstOrDefault(f => f.Value is null && f.Name == name);
        if (field == null)
            return Extra(name); // implicit list length, e.g. string_len in QueryTextExtents

        var parameter = name.Escape();
        var type = field.Type.CsName;
        if (field.Enum != null || type is "uint" or "long" or "ulong")
            return $"(int){parameter}";
        return type == "bool" ? $"({parameter} ? 1 : 0)" : parameter;
    }

    string Extra(string name)
    {
        if (!extras.Contains(name))
            extras.Add(name);
        return name.Escape();
    }
}



sealed class EventAnalyzer
{
    sealed record Prop(string Name, string Type, bool IsList);

    static readonly HashSet<string> Primitives =
        ["byte", "ushort", "uint", "ulong", "sbyte", "short", "int", "long", "bool", "float", "double"];

    static string? Primitive(WireType t) =>
        t.IsStruct ? null
        : Primitives.Contains(t.CsName) ? t.CsName
        : t.Size switch { 1 => "byte", 2 => "ushort", 4 => "uint", 8 => "ulong", _ => null };

    readonly EventDef ev;
    readonly string name;
    readonly bool hasSequence;
    readonly HashSet<string> usedNames;
    readonly List<string> members = [];                   // property lines
    readonly Dictionary<string, Prop> props = [];         // by XML field name
    readonly List<string> runtime = [];                   // runtime terms of the current offset
    int offset;                                           // constant part of the current offset
    int fixedEnd = -1;                                    // offset where the fixed part ends
    bool known = true;                                    // false once an offset can no longer be computed
    bool needsSumPop;
    readonly bool isReply;

    public EventAnalyzer(EventDef ev, bool isReply = false)
    {
        this.ev = ev;
        this.isReply = isReply;
        name = isReply || ev.Name.EndsWith("Event") ? ev.Name : ev.Name + "Event";
        hasSequence = !ev.IsXge && ev.Name != "KeymapNotify";
        usedNames = ["ResponseType", "Sequence", "Number", "Length", "ByteSize", "EventType", "Extension", "Verify", "FromBytes", name];
        offset = ev.IsXge ? 10 : isReply ? 8 : hasSequence ? 4 : 1;
    }

    public EventModel Analyze()
    {
        if (ev.Members == null)
            return new(name, $"// TODO: {name} is a copy of an event defined in another file", ev.Number, 0, [], [], false);

        Layout(ev.Members);
        return new(name, null, ev.Number, Math.Max(32, fixedEnd), Header(), [.. members], needsSumPop, isReply);
    }

    string[] Header()
    {
        List<string> lines = ["public byte ResponseType => _data[0];"];
        if (ev.IsXge)
        {
            lines.Add("public byte Extension => _data[1];");
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
            lines.Add("public uint Length => Unsafe.As<byte, uint>(ref _data[4]);");
            lines.Add("public int ByteSize => checked(32 + (int)Length * 4);");
            lines.Add("public ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);");
        }
        else if (isReply)
        {
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
            lines.Add("public uint Length => Unsafe.As<byte, uint>(ref _data[4]);");
            lines.Add("public int ByteSize => checked(32 + (int)Length * 4);");
            lines.Add("public bool Verify(in ushort sequence) => ResponseType == 1 && Sequence == sequence;");
        }
        else if (hasSequence)
            lines.Add("public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");
        return [.. lines];
    }

    void Layout(Member[] all)
    {
        var rest = new Queue<Member>(all);

        if (!isReply && hasSequence && rest.TryPeek(out var first) && CarriesFirstByte(first))
        {
            rest.Dequeue();
            if (first is Field f)
            {
                var after = offset;
                offset = 1; // the first byte sits before the sequence number
                AddField(f);
                offset = after;
            }
        }

        foreach (var member in rest)
        {
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
                    members.Add($"// TODO: '{sw.Name}' (switch) is not supported");
                    Unknown();
                    break;
            }
        }
        MarkEnd();
    }

    static bool CarriesFirstByte(Member m) =>
        m is Pad { Bytes: 1 } || (m is Field f && f.Type.Size == 1 && Primitive(f.Type) != null);

    void AddField(Field f)
    {
        var primitive = Primitive(f.Type);
        if (primitive == null)
        {
            members.Add($"// TODO: field '{f.Name}' has type {f.Type.CsName} which can't be represented in c#");
            if (f.Type.IsFixed) offset += f.Type.Size;
            else Unknown();
            return;
        }
        if (!known)
        {
            members.Add($"// TODO: field '{f.Name}' follows a member with an unknown size");
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
        members.Add($"{access} {primitive} {prop} => {read};");
        offset += f.Type.Size;
    }

    void AddList(ListOf list)
    {
        if (!known)
        {
            members.Add($"// TODO: list '{list.Name}' follows a member with an unknown size");
            return;
        }

        string count;
        try
        {
            count = list.Length is null ? throw new NotSupportedException("list without length") : Render(list.Length);
        }
        catch (NotSupportedException ex)
        {
            members.Add($"// TODO: list '{list.Name}' has an unsupported length ({ex.Message})");
            Unknown();
            return;
        }

        var bytes = list.Type.Size == 1 ? count : $"{Cs.Paren(count)} * {list.Type.Size}";
        var advance = int.TryParse(count, out var n) ? (n * list.Type.Size).ToString() : bytes;

        var primitive = Primitive(list.Type);
        if (primitive == null)
        {
            members.Add($"// TODO: list '{list.Name}' of {list.Type.CsName} can't be represented in c#");
            if (list.Type.IsFixed) Advance(advance);
            else Unknown();
            return;
        }

        var prop = PropName(list.Name);
        props[list.Name] = new(prop, primitive, true);
        var span = $"new ReadOnlySpan<byte>(_data, {At()}, {bytes})";
        var element = primitive == "bool" ? "byte" : primitive;
        var value = element == "byte" ? span : $"MemoryMarshal.Cast<byte, {element}>({span})";
        members.Add($"public ReadOnlySpan<{element}> {prop} => {value};");
        Advance(advance);
    }

    string Render(Expr e) => Exprs.Render(e, PropertyRef, SumPopCall);
    string PropertyRef(string field)
    {
        if (isReply && field == "length" && !props.ContainsKey(field))
            return "(int)Length";
        if (!props.TryGetValue(field, out var p) || p.IsList)
            throw new NotSupportedException($"fieldref {field}");
        return p.Type == "bool" ? $"({p.Name} ? 1 : 0)" : $"(int){p.Name}";
    }

    string? SumPopCall(string list)
    {
        if (!props.TryGetValue(list, out var p) || p is not { IsList: true, Type: "uint" })
            return null;
        needsSumPop = true;
        return $"SumPop({p.Name})";
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

static class OutputWriter
{
    public static string InterfaceMethod(RequestModel request)
    {
        var signature = request.Signature;
        var counts = signature.Trailing
            .OfType<ListOf>()
            .Select(l => l.Length)
            .OfType<FieldRef>()
            .Select(f => f.Name.ToCamelCase())
            .ToHashSet();

        var parameters = signature.Parameters
            .Select(p => (p.Type, Name: p.Name.ToCamelCase()))
            .Where(p => !counts.Contains(p.Name))
            .Select(p => $"{p.Type} {p.Name}")
            .ToList();

        foreach (var part in signature.Trailing)
        {
            switch (part)
            {
                case Pad:
                    break;
                case ListOf list:
                    var element = list.Type.CsName is "bool" or "char" or "void" ? "byte" : list.Type.CsName;
                    parameters.Add($"ReadOnlySpan<{element}> {list.Name.ToCamelCase()}");
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

        var args = string.Join(", ", parameters);
        return request.HasReply
            ? $"ReplyLease<{request.Name}Reply> {request.Name}({args});"
            : $"ResponseProto {request.Name}({args});";
    }
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

    public static string ToPascalCase(this string name) =>
        string.Concat(name.Split('_', StringSplitOptions.RemoveEmptyEntries).Select(p => char.ToUpperInvariant(p[0]) + p[1..]));

    public static string ToCamelCase(this string name)
    {
        var pascal = name.ToPascalCase();
        return (char.ToLowerInvariant(pascal[0]) + pascal[1..]).Escape();
    }
    public static string Paren(string expression) =>
        Regex.IsMatch(expression, @"^[\w@.]+(\(\))?$") || IsWrapped(expression) ? expression : $"({expression})";

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