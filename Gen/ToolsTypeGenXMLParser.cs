#:sdk Microsoft.NET.Sdk


using System.Text;
using System.Xml;

var dict = new Dictionary<string, string>()
{
    {"BYTE", "byte"},
    {"CARD8", "byte"},
    {"INT8", "byte"},
    {"BOOL", "byte"},
    {"INT16", "short"},
    {"CARD16", "ushort"},
    {"CARD32", "uint"},
    {"INT32", "int"},
};
// typedefs
using (var file = XmlReader.Create("/mnt/hardDrive/testP/xcb-proto/src/xproto.xml"))
{
    file.ReadToFollowing("typedef");

    do
    {
        dict.Add(file.GetAttribute("newname"), file.GetAttribute("oldname"));
    }
    while (file.ReadToFollowing("typedef"));
}

// xidtype
using (var file = XmlReader.Create("/mnt/hardDrive/testP/xcb-proto/src/xproto.xml"))
{
    file.ReadToFollowing("xidtype");
    do
    {
        dict.Add(file.GetAttribute("name"), "uint");
    }
    while (file.ReadToFollowing("xidtype"));
}

using (var file = XmlReader.Create("/mnt/hardDrive/testP/xcb-proto/src/xproto.xml"))
{
    file.ReadToFollowing("request");

    do
    {
        var requestStruct = new RequestStruct
        {
            Name = file.GetAttribute("name")
        };
        using var subtree = file.ReadSubtree();
        subtree.ReadToFollowing("field");

        do
        {
            var type = subtree.GetAttribute("type") ?? string.Empty;
            requestStruct.Fields.Add(new Fields(subtree.GetAttribute("name"), type, dict.GetBaseType(type)));
        }
        while (subtree.ReadToFollowing("field"));

        System.Console.WriteLine(requestStruct.ToString());
    }
    while (file.ReadToFollowing("request"));
}


class RequestStruct
{
    public string Name { get; set; }
    public List<Fields> Fields { get; set; } = new List<Fields>();


    public override string ToString()
    {
        var size = Fields.Sum(a => a.Size);
        var sb = new StringBuilder();
        sb.Append("public struct ").AppendLine(Name);

        sb.AppendLine("{").AppendLine("    private readonly byte[] _data;");

        sb.Append("    private ").Append(Name).AppendLine("(byte[] data)");

        sb.AppendLine("        {").AppendLine("            _data = data;");

        sb.Append("        if (_data.Length < ").Append(size).AppendLine(") throw new IndexOutOfRangeException();");

        sb.AppendLine("    }");

        var writeAt = 0;
        foreach (var field in Fields)
            sb.Append("    public ").Append(field.CsType).Append(' ').Append(field.Name.FixName())
                .Append(" => ").AppendLine(field.ToView(ref writeAt));



        sb.AppendLine("}");
        return sb.ToString();
    }

}

struct Fields
{
    public string Name { get; set; }
    public string CsType { get; }
    public string TypeName { get; set; }
    public int Size { get; set; }

    public Fields(string name, string cType, string csType)
    {
        this.Name = name;
        this.TypeName = cType;
        this.CsType = csType;
    }

    public string ToView(ref int index) => this.CsType switch
    {
        "byte" => this.TypeName == "BOOL"
                            ? $" _data[{index}] == 1;"
                            : $"_data[{index++}];",
        "ushort" or "short" => $"Unsafe.As<byte, {CsType}>(ref _data.AsSpan({index += 2}, 2)[0]);",
        "int" or "uint" => $"Unsafe.As<byte, {CsType}>(ref _data.AsSpan({index += 4}, 4)[0]);",
        _ => $"throw new NotImplementation({index++});",
    };
}

static class Extensation
{
    public static string GetBaseType(this Dictionary<string, string> keyValues, string name)
    {
        while (keyValues.TryGetValue(name, out var result))
        {
            name = result;
        }
        return name;
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