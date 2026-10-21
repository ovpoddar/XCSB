#:sdk Microsoft.NET.Sdk


using System.Text;
using System.Xml;

using var file = XmlReader.Create("/mnt/hardDrive/testP/xcb-proto/src/xproto.xml");

file.ReadToFollowing("request");

do
{
    var requestStruct = new RequestStruct();
    requestStruct.Name = file.GetAttribute("name");
    using var subtree = file.ReadSubtree();
    subtree.ReadToFollowing("field");

    do
    {
        requestStruct.Fields.Add(new Fields()
        {
            Name = subtree.GetAttribute("name"),
            TypeName = subtree.GetAttribute("type"),
            Size = 4
        });
    }
    while (subtree.ReadToFollowing("field"));

    System.Console.WriteLine(requestStruct.ToString());
}
while (file.ReadToFollowing("request"));



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
        foreach(var field in Fields)
            sb.Append("    public ").Append(field.TypeName).Append(' ').Append(field.Name)
                .Append(" => ").AppendLine(field.ToView(ref writeAt));



        sb.AppendLine("}");
        return sb.ToString();
    }

}

struct Fields
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public int Size { get; set; }

    public string ToView(ref int index) => "throw new NotImplementation()";
}