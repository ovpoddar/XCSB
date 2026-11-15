#:sdk Microsoft.NET.Sdk
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Helpers;

var source = args.Length > 0 ? args[0] : "/mnt/hardDrive/testP/xcb-proto/src";
while (!Directory.Exists(source))
{
    Console.WriteLine("Put the Folder path.");
    source = Console.ReadLine();
    if (source == null) return;
}

var output = new StringBuilder("""
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Xcsb.Connection.Helpers;
    using Xcsb.Connection.Models;
    using Xcsb.Connection.Response;
    using Xcsb.Connection.Response.Contract;

    """);

foreach (var file in Directory.GetFiles(source, "*.xml").Order(StringComparer.Ordinal))
{
    var protocol = XmlParser.Parse(file);
    output.AppendLine($"namespace {protocol.Namespace}").AppendLine("{");

    List<string> voidMethods = [], replyMethods = [];
    foreach (var request in protocol.Requests)
    {
        var signature = RequestWriter.Write(protocol, request, output);
        InterfaceWriter.Add(request, signature, voidMethods, replyMethods);
    }
    foreach (var ev in protocol.Events)
        EventWriter.Write(ev, output);
    InterfaceWriter.Write(protocol, voidMethods, replyMethods, output);

    output.AppendLine("}");
}
File.WriteAllText("out.G.cs", output.ToString());
Console.WriteLine("Written");


readonly record struct WireType(string CsName, int Size, bool IsFixed, bool IsStruct = false);
sealed record Expr(string Kind, string Text = "", Expr[]? Args = null);
sealed record Case(bool IsBitCase, long Bits, int Size, bool IsFixed);
sealed record Member(
    string Kind, string Name = "", WireType Type = default, string? Enum = null, Expr? Expr = null,
    int? Bytes = null, int? Align = null, Case[]? Cases = null);
sealed record RequestModel(string Name, Member[] Members, bool HasReply);
sealed record RequestSignature(string[] Parameters, Member[] Trailing);
sealed record EventModel(string Name, int Number, bool IsXge, Member[]? Members);
sealed record ProtocolModel(
    string Namespace, bool IsCore, RequestModel[] Requests, EventModel[] Events, Dictionary<string, int> EnumWidths);


static class XmlParser
{
    static readonly Dictionary<string, WireType> BaseTypes = new()
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

    public static ProtocolModel Parse(string path)
    {
        var directory = Path.GetDirectoryName(path)!;
        var types = new Dictionary<string, WireType>(BaseTypes);
        var enumWidths = new Dictionary<string, int>();
        var enumValues = new Dictionary<(string Enum, string Item), long>();
        var loaded = new HashSet<string>();

        var root = Load(Path.GetFileNameWithoutExtension(path))!;
        var requests = root.Elements("request")
            .Select(r => new RequestModel(Attr(r, "name"), ParseMembers(r.Elements()), r.Element("reply") != null))
            .ToArray();
        return new(Attr(root, "header"), root.Attribute("extension-name") == null, requests, ParseEvents(root), enumWidths);

        EventModel[] ParseEvents(XElement xml)
        {
            var defined = xml.Elements("event")
                .ToDictionary(e => Attr(e, "name"), e => (Xge: e.Attribute("xge") != null, Members: ParseMembers(e.Elements())));
            return
            [
                .. xml.Elements().Where(e => e.Name.LocalName is "event" or "eventcopy").Select(e =>
                {
                    var name = Attr(e, "name");
                    var number = int.Parse(Attr(e, "number"));
                    var origin = e.Name.LocalName == "event" ? name : Local(Attr(e, "ref"));
                    return defined.TryGetValue(origin, out var d)
                        ? new EventModel(name, number, d.Xge, d.Members)
                        : new EventModel(name, number, false, null); // copy of an event from another file
                })
            ];
        }

        XElement? Load(string file)
        {
            if (!loaded.Add(file))
                return null;
            var xml = XDocument.Load(Path.Join(directory, file + ".xml")).Root!;
            foreach (var e in xml.Elements())
            {
                var name = e.Attribute("name")?.Value!;
                switch (e.Name.LocalName)
                {
                    case "import":
                        Load(e.Value);
                        break;
                    case "xidtype" or "xidunion":
                        types[name] = new(name, 4, true);
                        break;
                    case "typedef":
                        types[Attr(e, "newname")] = LookupType(Attr(e, "oldname"));
                        break;
                    case "struct":
                        var (size, isFixed) = Measure(e.Elements());
                        types[name] = new(name, size, isFixed, true);
                        break;
                    case "union":
                        types[name] = new(name, e.Elements().Max(m => Measure([m]).Size), true, true);
                        break;
                    case "eventstruct":
                        types[name] = new(name, 32, true, true);
                        break;
                    case "enum":
                        foreach (var item in e.Elements("item"))
                            enumValues[(name, Attr(item, "name"))] =
                                item.Element("value") is { } value ? long.Parse(value.Value)
                                : item.Element("bit") is { } bit ? 1L << int.Parse(bit.Value)
                                : 0;
                        break;
                }
            }

            foreach (var field in xml.Descendants("field"))
                if (field.Attribute("enum") is { } en && types.TryGetValue(Local(Attr(field, "type")), out var type))
                    enumWidths.TryAdd(Local(en.Value), type.Size);
            return xml;
        }

        Member[] ParseMembers(IEnumerable<XElement> elements) =>
        [
            .. elements
                .Where(m => m.Name.LocalName is "field" or "exprfield" or "pad" or "list" or "switch"
                            && m.Attribute("type")?.Value != "fd")
                .Select(ParseMember)
        ];

        Member ParseMember(XElement m) => m.Name.LocalName switch
        {
            "pad" => new("pad", Bytes: Int(m, "bytes"), Align: Int(m, "align")),
            "field" => new("field", Attr(m, "name"), TypeOf(m), EnumOf(m)),
            "exprfield" => new("field", Attr(m, "name"), TypeOf(m), EnumOf(m), ParseExpr(FirstChild(m)!)),
            "list" => new("list", Attr(m, "name"), TypeOf(m), Expr: FirstChild(m) is { } count ? ParseExpr(count) : null),
            "switch" => new("switch", Attr(m, "name"), Expr: ParseExpr(FirstChild(m)!), Cases: ParseCases(m)),
            var other => throw new NotSupportedException($"<{other}>"),
        };

        Case[] ParseCases(XElement sw) =>
        [
            .. sw.Elements().Where(c => c.Name.LocalName is "bitcase" or "case").Select(c =>
            {
                var (size, isFixed) = Measure(c.Elements());
                var bits = c.Elements()
                    .Where(x => x.Name.LocalName is "enumref" or "value")
                    .Aggregate(0L, (all, x) => all | long.Parse(ParseExpr(x).Text));
                return new Case(c.Name.LocalName == "bitcase", bits, size, isFixed);
            })
        ];

        Expr ParseExpr(XElement e)
        {
            var args = e.Elements().Where(x => x.Name.LocalName != "doc").Select(ParseExpr).ToArray();
            return e.Name.LocalName switch
            {
                "fieldref" => new("field", e.Value.Trim()),
                "value" => new("value", ParseNumber(e.Value).ToString()),
                "enumref" => new("value", enumValues[(Local(Attr(e, "ref")), e.Value.Trim())].ToString()),
                "op" => new("op", Attr(e, "op"), args),
                "unop" => new("unop", Attr(e, "op"), args),
                "popcount" => new("popcount", Args: args),
                "sumof" => new("sumof", Attr(e, "ref"), args),
                "listelement-ref" => new("listelement"),
                var tag => new("unsupported", tag),
            };
        }

        (int Size, bool IsFixed) Measure(IEnumerable<XElement> members)
        {
            var (size, isFixed) = (0, true);
            foreach (var m in members)
            {
                switch (m.Name.LocalName)
                {
                    case "field" or "exprfield":
                        size += TypeOf(m).Size;
                        isFixed &= TypeOf(m).IsFixed;
                        break;
                    case "pad":
                        size += PadLength(Int(m, "bytes"), Int(m, "align"), size);
                        break;
                    case "list" when FirstChild(m) is { Name.LocalName: "value" } count && TypeOf(m).IsFixed:
                        size += TypeOf(m).Size * int.Parse(count.Value);
                        break;
                    case "list" or "switch":
                        isFixed = false;
                        break;
                }
            }
            return (size, isFixed);
        }

        WireType LookupType(string name) => types[Local(name)];
        WireType TypeOf(XElement member) => LookupType(Attr(member, "type"));
    }
}


static class RequestWriter
{
    static readonly (int Size, string Type)[] PadTypes = [(8, "ulong"), (4, "uint"), (2, "ushort"), (1, "byte")];

    static bool IsFixed(Member m) => m.Kind == "pad" || (m.Kind == "field" && m.Type.IsFixed);
    static bool FitsInByte(Member m) => m.Kind == "pad" ? m.Bytes == 1 : IsFixed(m) && m.Type.Size == 1;
    static string Scale(string count, int factor) => factor == 1 ? count : $"{Paren(count)} * {factor}";

    public static RequestSignature Write(ProtocolModel protocol, RequestModel request, StringBuilder sb)
    {
        List<string> parameters = [], extras = [], lines = [];
        var padIndex = 0;
        var members = new Queue<Member>(request.Members);

        if (!protocol.IsCore)
        {
            parameters.Add("byte majorOpCode");
            lines.Add("public readonly byte MajorOpcode = majorOpCode;");
        }
        lines.Add($"public readonly Opcode Opcode = Opcode.{request.Name};");

        if (protocol.IsCore)
        {
            if (members.TryPeek(out var first) && FitsInByte(first))
                Emit(members.Dequeue(), 1);
            else
                lines.AddRange(Pad(1));
        }

        var lengthAt = lines.Count;
        var size = 4;
        while (members.TryPeek(out var member) && IsFixed(member))
            size += Emit(members.Dequeue(), size);
        lines.Insert(lengthAt, $"public readonly ushort Length = {Length(size, members)};");

        var structSize = members.Count == 0 ? Align4(size) : size;
        sb.AppendLine($"    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = {structSize})]");
        sb.AppendLine("    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    internal readonly struct {request.Name}Type({string.Join(", ", parameters.Concat(extras))})");
        sb.AppendLine("    {");
        lines.ForEach(line => sb.AppendLine("        " + line));
        sb.AppendLine("    }");
        return new([.. parameters.Skip(protocol.IsCore ? 0 : 1)], [.. members]);

        int Emit(Member m, int offset)
        {
            if (m.Kind != "pad")
            {
                lines.Add(Field(m));
                return m.Type.Size;
            }
            var bytes = PadLength(m.Bytes, m.Align, offset);
            lines.AddRange(Pad(bytes));
            return bytes;
        }

        IEnumerable<string> Pad(int bytes)
        {
            foreach (var (padSize, type) in PadTypes)
                for (; bytes >= padSize; bytes -= padSize)
                    yield return $"private readonly {type} _pad{padIndex++} = 0;";
        }

        string Field(Member m)
        {
            var parameter = m.Name.Escape();
            var storage = m.Type.CsName == "bool" ? "byte" : m.Type.CsName; // C# bool is not blittable

            var (parameterType, fieldType, value) = (m.Expr, m.Enum, m.Type.CsName) switch
            {
                ({ } expr, _, _) => ((string?)null, storage, $"({storage}){Paren(Expression(expr))}"),
                (_, { } en, _) when protocol.EnumWidths.GetValueOrDefault(en) == m.Type.Size => (en, en, parameter),
                (_, { } en, _) => (en, storage, $"({storage}){parameter}"),
                (_, _, "bool") => ("bool", "byte", $"(byte)({parameter} ? 1 : 0)"),
                _ => (m.Type.CsName, m.Type.CsName, parameter),
            };
            if (parameterType != null)
                parameters.Add($"{parameterType} {parameter}");

            var access = m.Name.EndsWith("_len") ? "private" : "public";
            return $"{access} readonly {fieldType} {m.Name.ToPascalCase()} = {value};";
        }

        string Length(int headerSize, IEnumerable<Member> trailing)
        {
            List<string> words = [], run = [];
            var constantWords = 0;
            var constantBytes = headerSize;

            void Add(string count, int unit)
            {
                if (int.TryParse(count, out var n)) constantBytes += n * unit;
                else if (unit % 4 == 0) words.Add(Scale(count, unit / 4));
                else run.Add(Scale(count, unit));
            }

            void EndRun()
            {
                if (run.Count == 0)
                    constantWords += Align4(constantBytes) / 4;
                else
                {
                    constantWords += constantBytes / 4;
                    var rest = constantBytes % 4;
                    var terms = rest == 0 ? run : run.Prepend(rest.ToString());
                    words.Add($"{Paren(string.Join(" + ", terms))}.AddPadding() / 4");
                }
                constantBytes = 0;
                run.Clear();
            }

            void AddSwitch(Member sw)
            {
                var cases = sw.Cases!;
                if (!cases.All(x => x.IsBitCase && x.IsFixed))
                    Add(Extra($"{sw.Name}_bytes"), 1);
                else if (sw.Expr!.Kind == "field" && cases.Select(x => x.Size).Distinct().Count() == 1)
                    Add($"{sw.Expr.Text.Escape()}.CountFlags()", cases[0].Size);
                else
                {
                    var mask = Expression(sw.Expr);
                    foreach (var bitcase in cases)
                    {
                        var unit = bitcase.Size % 4 == 0 ? 4 : 1;
                        Add($"(({mask} & {bitcase.Bits}) != 0 ? {bitcase.Size / unit} : 0)", unit);
                    }
                }
            }

            foreach (var part in trailing)
            {
                switch (part)
                {
                    case { Kind: "pad", Align: not null }:
                        EndRun();
                        break;
                    case { Kind: "pad" }:
                        constantBytes += PadLength(part.Bytes, part.Align, 0);
                        break;
                    case { Kind: "switch" }:
                        AddSwitch(part);
                        break;
                    case { Kind: "list", Type.IsFixed: true }:
                        Add(part.Expr == null ? Extra($"{part.Name}_len") : Expression(part.Expr), part.Type.Size);
                        break;
                    default: // variable-size list, or a struct field with a runtime size
                        Add(Extra($"{part.Name}_bytes"), 1);
                        break;
                }
            }
            EndRun();

            if (words.Count == 0)
                return constantWords.ToString();
            if (constantWords != 0)
                words.Insert(0, constantWords.ToString());
            return $"(ushort)({string.Join(" + ", words)})";
        }

        string Expression(Expr e) => e.Kind switch
        {
            "field" => FieldValue(e.Text),
            "value" => e.Text,
            "op" => $"({Expression(e.Args![0])} {e.Text} {Expression(e.Args[1])})",
            "unop" => $"{e.Text}{Paren(Expression(e.Args![0]))}",
            "popcount" => $"{Paren(Expression(e.Args![0]))}.CountFlags()",
            _ => throw new NotSupportedException($"<{e.Text}> in request {request.Name}"),
        };

        string FieldValue(string name)
        {
            var field = request.Members.FirstOrDefault(m => m.Kind == "field" && m.Expr == null && m.Name == name);
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
            var parameter = name.Escape();
            var declaration = $"int {parameter}";
            if (!extras.Contains(declaration))
                extras.Add(declaration);
            return parameter;
        }
    }
}


static class EventWriter
{
    static readonly HashSet<string> Primitives =
        ["byte", "ushort", "uint", "ulong", "sbyte", "short", "int", "long", "bool", "float", "double"];

    static string? Primitive(WireType t) =>
        t.IsStruct ? null
        : Primitives.Contains(t.CsName) ? t.CsName
        : t.Size switch { 1 => "byte", 2 => "ushort", 4 => "uint", 8 => "ulong", _ => null };

    public static void Write(EventModel ev, StringBuilder sb)
    {
        var name = ev.Name.EndsWith("Event") ? ev.Name : ev.Name + "Event";
        if (ev.Members == null)
        {
            sb.AppendLine($"    // TODO: {name} is a copy of an event defined in another file");
            return;
        }

        var hasSequence = !ev.IsXge && ev.Name != "KeymapNotify"; // KeymapNotify has no sequence number
        List<string> lines = [], dynamic = [];
        Dictionary<string, (string Name, string Type, bool IsList)> props = [];
        HashSet<string> used = ["ResponseType", "Sequence", "Number", "Length", "ByteSize", "EventType", "Extension", name];
        var constant = ev.IsXge ? 10 : hasSequence ? 4 : 1;
        var fixedEnd = -1;
        var known = true;
        var needsSumPop = false;
        var members = new Queue<Member>(ev.Members);

        // the first byte after the response type may carry a field of its own
        if (hasSequence && members.TryPeek(out var first)
            && (first.Kind == "pad" ? first.Bytes == 1
                : first.Kind == "field" && first.Type.Size == 1 && Primitive(first.Type) != null))
        {
            members.Dequeue();
            if (first.Kind == "field")
                EmitField(first);
        }

        foreach (var m in members)
        {
            switch (m.Kind)
            {
                case "pad" when m.Bytes != null:
                    constant += m.Bytes.Value;
                    break;
                case "pad" when known && dynamic.Count == 0:
                    constant += PadLength(null, m.Align, constant);
                    break;
                case "pad":
                    Unknown();
                    break;
                case "field":
                    EmitField(m);
                    break;
                case "list":
                    EmitList(m);
                    break;
                default:
                    lines.Add($"// TODO: '{m.Name}' ({m.Kind}) is not supported");
                    Unknown();
                    break;
            }
        }
        MarkEnd();
        var minSize = Math.Max(32, fixedEnd);

        sb.AppendLine($$"""
                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                public readonly struct {{name}} : IXEvent<{{name}}>
                {
                    private readonly byte[] _data;
                    public const byte Number = {{ev.Number}};

                    private {{name}}(byte[] data)
                    {
                        if (data is null) throw new ArgumentNullException(nameof(data));
                        if (data.Length < {{minSize}})
                            throw new ArgumentException("Buffer too small for {{name}}.", nameof(data));
                        _data = data;
                    }

                    public byte ResponseType => _data[0];
            """);
        if (ev.IsXge)
            sb.AppendLine("""
                        public byte Extension => _data[1];
                        public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);
                        public uint Length => Unsafe.As<byte, uint>(ref _data[4]);
                        public int ByteSize => checked(32 + (int)Length * 4);
                        public ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);
                """);
        else if (hasSequence)
            sb.AppendLine("        public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);");

        lines.ForEach(line => sb.AppendLine("        " + line));
        if (needsSumPop)
            sb.AppendLine("""

                        private static int SumPop(ReadOnlySpan<uint> s)
                        {
                            var n = 0;
                            foreach (var v in s) n += v.CountFlags();
                            return n;
                        }
                """);
        sb.AppendLine($$"""

                    public {{name}} FromBytes(byte[] response) => new {{name}}(response);
                }

            """);
        return;

        string At() => dynamic.Count == 0 ? constant.ToString() : $"{constant} + {string.Join(" + ", dynamic)}";

        void MarkEnd()
        {
            if (fixedEnd < 0) fixedEnd = constant;
        }

        void Unknown()
        {
            MarkEnd();
            known = false;
        }

        string PropName(string raw)
        {
            var n = raw.ToPascalCase();
            while (!used.Add(n))
                n += "Field";
            return n;
        }

        void Advance(string bytes)
        {
            if (int.TryParse(bytes, out var n))
                constant += n;
            else
            {
                MarkEnd();
                dynamic.Add(bytes);
            }
        }

        void EmitField(Member m)
        {
            var prim = Primitive(m.Type);
            if (prim == null)
            {
                lines.Add($"// TODO: field '{m.Name}' has type {m.Type.CsName} which can't be represented in c#");
                if (m.Type.IsFixed) constant += m.Type.Size;
                else Unknown();
                return;
            }
            if (!known)
            {
                lines.Add($"// TODO: field '{m.Name}' follows a member with an unknown size");
                return;
            }
            var prop = PropName(m.Name);
            props[m.Name] = (prop, prim, false);
            var access = m.Name.EndsWith("_len") ? "private" : "public";
            var at = At();
            var read = prim switch
            {
                "bool" => $"_data[{at}] != 0",
                "byte" => $"_data[{at}]",
                "sbyte" => $"(sbyte)_data[{at}]",
                _ => $"Unsafe.As<byte, {prim}>(ref _data[{at}])",
            };
            lines.Add($"{access} {prim} {prop} => {read};");
            constant += m.Type.Size;
        }

        void EmitList(Member m)
        {
            if (!known)
            {
                lines.Add($"// TODO: list '{m.Name}' follows a member with an unknown size");
                return;
            }
            string count;
            try
            {
                count = m.Expr == null ? throw new NotSupportedException("list without length") : Expression(m.Expr);
            }
            catch (NotSupportedException e)
            {
                lines.Add($"// TODO: list '{m.Name}' has an unsupported length ({e.Message})");
                Unknown();
                return;
            }

            var bytes = m.Type.Size == 1 ? count : $"{Paren(count)} * {m.Type.Size}";
            var advance = int.TryParse(count, out var n) ? (n * m.Type.Size).ToString() : bytes;
            var prim = Primitive(m.Type);
            if (prim == null)
            {
                lines.Add($"// TODO: list '{m.Name}' of {m.Type.CsName} can't be represented in c#");
                if (m.Type.IsFixed) Advance(advance);
                else Unknown();
                return;
            }

            var prop = PropName(m.Name);
            props[m.Name] = (prop, prim, true);
            var span = $"new ReadOnlySpan<byte>(_data, {At()}, {bytes})";
            var element = prim == "bool" ? "byte" : prim;
            var value = element == "byte" ? span : $"MemoryMarshal.Cast<byte, {element}>({span})";
            lines.Add($"public ReadOnlySpan<{element}> {prop} => {value};");
            Advance(advance);
        }

        string Expression(Expr e)
        {
            switch (e.Kind)
            {
                case "field":
                    if (!props.TryGetValue(e.Text, out var p) || p.IsList)
                        throw new NotSupportedException($"fieldref {e.Text}");
                    return p.Type == "bool" ? $"({p.Name} ? 1 : 0)" : $"(int){p.Name}";
                case "value":
                    return e.Text;
                case "op":
                    return $"({Expression(e.Args![0])} {e.Text} {Expression(e.Args[1])})";
                case "unop":
                    return $"{e.Text}{Paren(Expression(e.Args![0]))}";
                case "popcount":
                    return $"{Paren(Expression(e.Args![0]))}.CountFlags()";
                case "sumof" when e.Args is [{ Kind: "popcount", Args: [{ Kind: "listelement" }] }]
                                  && props.TryGetValue(e.Text, out var list) && list is { IsList: true, Type: "uint" }:
                    needsSumPop = true;
                    return $"SumPop({list.Name})";
                default:
                    throw new NotSupportedException(e.Kind == "unsupported" ? $"<{e.Text}>" : e.Kind);
            }
        }
    }
}


static class InterfaceWriter
{
    public static void Add(RequestModel request, RequestSignature signature, List<string> voidMethods, List<string> replyMethods)
    {
        var target = request.HasReply ? replyMethods : voidMethods;

        var counts = signature.Trailing
            .Where(t => t is { Kind: "list", Expr.Kind: "field" })
            .Select(t => t.Expr!.Text.ToCamelCase())
            .ToHashSet();
        var parameters = signature.Parameters
            .Select(p => p.Split(' ') is [var type, var name] ? $"{type} {name.ToCamelCase()}" : p)
            .Where(p => !counts.Contains(p[(p.LastIndexOf(' ') + 1)..]))
            .ToList();

        foreach (var part in signature.Trailing)
        {
            if (part.Kind == "pad")
                continue;
            if (part.Kind != "list")
            {
                target.Add($"// TODO: {request.Name} has a {part.Kind} '{part.Name}' which can't be represented in c#");
                return;
            }
            var element = part.Type.CsName is "bool" or "char" or "void" ? "byte" : part.Type.CsName;
            parameters.Add($"ReadOnlySpan<{element}> {part.Name.ToCamelCase()}");
        }

        var list = string.Join(", ", parameters);
        target.Add(request.HasReply
            ? $"ReplyLease<{request.Name}Reply> {request.Name}({list});"
            : $"ResponseProto {request.Name}({list});");
    }

    public static void Write(ProtocolModel protocol, List<string> voidMethods, List<string> replyMethods, StringBuilder sb)
    {
        var name = "I" + protocol.Namespace.Split('.')[^1].ToPascalCase();
        sb.AppendLine();
        Interface($"{name}VoidProto", voidMethods);
        Interface($"{name}ResponseProto", replyMethods);
        Interface($"{name}Request : {name}ResponseProto, {name}VoidProto", []);

        void Interface(string header, List<string> methods)
        {
            sb.AppendLine($"    public interface {header}").AppendLine("    {");
            methods.ForEach(m => sb.AppendLine("        " + m));
            sb.AppendLine("    }");
            if (!header.Contains(':')) sb.AppendLine();
        }
    }
}


static class Helpers
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

    public static string Local(string name) => name[(name.IndexOf(':') + 1)..];
    public static XElement? FirstChild(XElement e) => e.Elements().FirstOrDefault(c => c.Name.LocalName != "doc");
    public static string Attr(XElement e, string name) => e.Attribute(name)!.Value;
    public static int? Int(XElement e, string name) => e.Attribute(name) is { } a ? int.Parse(a.Value) : null;
    public static string? EnumOf(XElement e) => e.Attribute("enum") is { } a ? Local(a.Value) : null;

    public static long ParseNumber(string text)
    {
        text = text.Trim();
        return text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text[2..], 16) : long.Parse(text);
    }

    public static int Align4(int bytes) => (bytes + 3) & ~3;

    public static int PadLength(int? bytes, int? align, int offset) =>
        bytes ?? (align!.Value - offset % align.Value) % align.Value;

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