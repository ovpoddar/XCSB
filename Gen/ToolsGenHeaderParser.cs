#:sdk Microsoft.NET.Sdk
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

var location = "/usr/include/xcb/";
if (!Directory.Exists(location))
{
    Console.WriteLine("Put the xproto.h path.");
    location = Console.ReadLine();
    if (location == null) return;
}

var pathAttributes = File.GetAttributes(location);
if ((pathAttributes & FileAttributes.Directory) == FileAttributes.Directory)
{
    foreach (var file in Directory.EnumerateFiles(location, "*.h", SearchOption.AllDirectories))
    {
        Generate(file);
    }
}
else if (File.Exists(location))
{
    Generate(location);
}
else
{
    Console.WriteLine("Fail to process the location.");
}

Console.WriteLine("Parsing completed.");
return;

static void Generate(string path)
{
    using var readStream = File.OpenRead(path);
    var headerParser = new Parser(readStream);
    headerParser.Parse();

    var typeName = Path.GetFileNameWithoutExtension(path);
    using var writeStream = File.OpenWrite(Path.Join(Environment.CurrentDirectory, "Gen/Generated", typeName + ".generated.cs"));
    writeStream.Position = 0;

    var requestItems = headerParser.TypeDefinitions
        .Where(x => x.Name != null && x.Name.EndsWith("_request_t"))
        .ToArray();
    var responseItems = headerParser.TypeDefinitions
        .Where(x => x.Name != null && x.Name.EndsWith("_response_t"))
        .ToArray();
    writeStream.Write("namespace Xcsb."u8);
    writeStream.Write(Encoding.UTF8.GetBytes(typeName + ';'));
    writeStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));

    // attempt of writting requests only
    foreach (var item in requestItems)
    {
        writeStream.Write("internal sealed struct "u8);
        writeStream.Write(Encoding.UTF8.GetBytes(item.Name!
            .FixName("xcb_") + Environment.NewLine + '{'));

        foreach (var field in item.Fields)
        {
            writeStream.Write("\tpublic "u8);
            writeStream.Write(field.GetFieldType.MapCsType());
            writeStream.Write(" "u8);
            writeStream.Write(field.GetFieldName.FixName());
            writeStream.Write(Encoding.UTF8.GetBytes(";" + Environment.NewLine));
        }

        writeStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine + '}' + Environment.NewLine));
    }
    // attempt of writting response only
    foreach (var item in responseItems)
    {
        writeStream.Write("internal sealed struct "u8);
        writeStream.Write(Encoding.UTF8.GetBytes(item.Name!.FixName("xcb_") + Environment.NewLine + '{'));

        foreach (var field in item.Fields)
        {
            writeStream.Write("\tpublic "u8);
            writeStream.Write(field.GetFieldType.MapCsType());
            writeStream.Write(" "u8);
            writeStream.Write(field.GetFieldName.FixName());
            writeStream.Write(Encoding.UTF8.GetBytes(";" + Environment.NewLine));
        }

        writeStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine + '}' + Environment.NewLine));
    }
    

    // base version
    writeStream.Write("internal interface "u8);
    writeStream.Write(Encoding.UTF8.GetBytes("I" + typeName));
    writeStream.Write("\n{\n"u8);
    foreach (var item in requestItems)
    {
        var itemName = item.Name.FixName("xcb_", "_request_t");
        var responseName = item.Name.Trim(string.Empty, "_request_t") + "_response_t";
        var responseType = responseItems.FirstOrDefault(a => a.Name != null
                && a.Name == responseName);
        writeStream.Write("\t"u8);
        if (responseType == null)
            writeStream.Write("ResponseProto "u8);
        else
            writeStream.Write(Encoding.UTF8.GetBytes(itemName + "Request"));
        writeStream.Write(Encoding.UTF8.GetBytes(itemName + "("));

        var isfirst = true;

        foreach (var field in item.Fields)
        {
            if (!isfirst)
            {
                writeStream.Write(", "u8);
            }

            writeStream.Write(field.GetFieldType.MapCsType());
            writeStream.Write(" "u8);
            writeStream.Write(field.GetFieldName.FixName());
            isfirst = false;
        }

        writeStream.Write(");\n"u8);
    }
    writeStream.Write("\n}\n"u8);

    // unchecked version
    writeStream.Write("internal interface "u8);
    writeStream.Write(Encoding.UTF8.GetBytes("I" + typeName + "Unchecked"));
    writeStream.Write("\n{\n"u8);
    foreach (var item in requestItems)
    {
        var itemName = item.Name.FixName("xcb_", "_request_t");
        var responseName = item.Name.Trim(string.Empty, "_request_t") + "_response_t";
        var responseType = responseItems.FirstOrDefault(a => a.Name != null
                                                             && a.Name == responseName);
        writeStream.Write("\t"u8);
        if (responseType == null)
            writeStream.Write("void "u8);
        else
            writeStream.Write(Encoding.UTF8.GetBytes(itemName + "Request"));
        writeStream.Write(Encoding.UTF8.GetBytes(itemName + "Unchecked("));

        var isfirst = true;

        foreach (var field in item.Fields)
        {
            if (!isfirst)
            {
                writeStream.Write(", "u8);
            }

            writeStream.Write(field.GetFieldType.MapCsType());
            writeStream.Write(" "u8);
            writeStream.Write(field.GetFieldName.FixName());
            isfirst = false;
        }

        writeStream.Write(");\n"u8);
    }
    writeStream.Write("\n}\n"u8);

    // checked version
    writeStream.Write("internal interface "u8);
    writeStream.Write(Encoding.UTF8.GetBytes("I" + typeName + "Checked"));
    writeStream.Write("\n{\n"u8);
    foreach (var item in requestItems)
    {
        var itemName = item.Name.FixName("xcb_", "_request_t");
        var responseName = item.Name.Trim(string.Empty, "_request_t") + "_response_t";
        var responseType = responseItems.FirstOrDefault(a => a.Name != null
                                                             && a.Name == responseName);
        writeStream.Write("\t"u8);
        if (responseType == null)
            writeStream.Write("void "u8);
        else
            writeStream.Write(Encoding.UTF8.GetBytes(itemName + "Request"));
        writeStream.Write(Encoding.UTF8.GetBytes(itemName + "Checked("));

        var isfirst = true;

        foreach (var field in item.Fields)
        {
            if (!isfirst)
            {
                writeStream.Write(", "u8);
            }

            writeStream.Write(field.GetFieldType.MapCsType());
            writeStream.Write(" "u8);
            writeStream.Write(field.GetFieldName.FixName());
            isfirst = false;
        }

        writeStream.Write(");\n"u8);
    }
    writeStream.Write("\n}\n"u8);
}

public class Parser
{
    private readonly Lexer _lexer;
    public List<TypeDefinition> TypeDefinitions { get; set; }

    public Parser(Stream stream)
    {
        this._lexer = new Lexer(stream);
        this.TypeDefinitions = new List<TypeDefinition>();
    }

    public void Parse()
    {
        var cb = new StringBuilder();
        while (true)
        {
            var token = _lexer.NextToken();
            if (token.Value == TokenType.TokenEnd)
                break;

            if (token.Value == TokenType.Comment)
            {
                cb.AppendLine(token.Text);
            }

            if (token.Value == TokenType.Symbol)
            {
                if (token.Text == "typedef")
                {
                    token = _lexer.NextToken();
                    if (token.Value != TokenType.Symbol ||
                        token.Text == "struct" || token.Text == "union" || token.Text == "enum")
                    {
                        var typeDefination = GetTypeDefination(token);
                        token = _lexer.NextToken();
                        if (token.Value != TokenType.Semicolon)
                        {
                            if (token.Value != TokenType.Symbol)
                                throw new Exception("Expected a symbol token or a semicolon");
                            if (token.Text != typeDefination.Name)
                                typeDefination.Aliases.Add(token.Text);
                            ExpextedToken(TokenType.Semicolon, ";");
                        }

                        typeDefination.Comments = cb.ToString();
                        cb.Clear();
                        this.TypeDefinitions.Add(typeDefination);
                    }
                    else
                    {
                        var definationType = _lexer.NextToken();

                        var isFound = false;
                        for (int i = 0; i < TypeDefinitions.Count; i++)
                        {
                            TypeDefinition typeDef = this.TypeDefinitions[i];
                            if (typeDef.Name != definationType.Text) continue;

                            cb.AppendLine(typeDef.Comments);
                            typeDef.Comments = cb.ToString();
                            cb.Clear();
                            if (definationType.Text != typeDef.Name)
                                typeDef.Aliases.Add(definationType.Text);

                            isFound = true;
                            break;
                        }

                        if (!isFound)
                        {
                            TypeDefinition typeDef = new TypeDefinition
                            {
                                Name = token.Text,
                                Comments = cb.ToString(),
                            };
                            typeDef.Aliases.Add(definationType.Text);
                            cb.Clear();

                            this.TypeDefinitions.Add(typeDef);
                        }

                        ExpextedToken(TokenType.Semicolon, ";");
                    }
                }

                if (token.Text is "struct" or "union" or "enum")
                {
                    var typeDefination = GetTypeDefination(token);
                    typeDefination.Comments = cb.ToString();
                    this.TypeDefinitions.Add(typeDefination);
                    cb.Clear();
                }
            }
        }
    }

    private TypeDefinition GetTypeDefination(Token token)
    {
        if (token.Value != TokenType.Symbol)
            throw new Exception("Expected a symbol token");
        if (token.Text != "struct" && token.Text != "union" && token.Text != "enum")
            throw new Exception("Unexpected a symbol token");
        var typeDefination = new TypeDefinition
        {
            Type = token.Text switch
            {
                "struct" => TypeDefinition.TypeKind.Struct,
                "union" => TypeDefinition.TypeKind.Union,
                "enum" => TypeDefinition.TypeKind.Enum,
                _ => throw new Exception("Unexpected type")
            }
        };

        var brases = 1;
        token = _lexer.NextToken();
        if (token.Value == TokenType.Symbol)
        {
            typeDefination.Name = token.Text;

            token = _lexer.NextToken();
            if (token.Value != TokenType.OpenCurly)
            {
                return typeDefination;
            }
        }
        else if (token.Value == TokenType.OpenCurly)
        {
            typeDefination.Name = null;
        }
        else
        {
            throw new Exception("Unexpected token after struct/union/enum");
        }

        var tokens = new List<Token>();
        while (true)
        {
            token = _lexer.NextToken();
            if (token.Value == TokenType.OpenCurly)
                brases++;
            else if (token.Value == TokenType.CloseCurly)
                brases--;


            if (token.Value == TokenType.Semicolon && brases == 1)
            {
                typeDefination.Fields.Add(new TypeDefinition.FieldDetails(tokens.ToList()));
                tokens.Clear();
            }
            else
            {
                tokens.Add(token);
            }

            if (token.Value == TokenType.CloseCurly && brases == 0)
                break;
        }

        return typeDefination;
    }

    private void ExpextedToken(TokenType expected, string expectedText)
    {
        var token = _lexer.NextToken();
        if (token.Value != expected || token.Text != expectedText)
            throw new Exception($"Expected token {expectedText} but got {token.Text}");
    }


    private class Lexer(Stream file)
    {
        private long _line = 1;
        private long _startOfLine = 0;

        public Token NextToken()
        {
            if (!file.CanRead || !file.CanSeek) throw new InvalidOperationException("Stream must be readable.");
            if (file.Position >= file.Length)
                return new Token(TokenType.TokenEnd, string.Empty, _line, file.Position - _startOfLine);
            Span<byte> buffer = stackalloc byte[1];
            file.ReadExactly(buffer);


            switch (buffer[0])
            {
                case (byte)'/':
                    {
                        if (file.Position >= file.Length)
                            return new Token(TokenType.Operator, "/", _line, file.Position - _startOfLine);
                        file.ReadExactly(buffer);
                        switch (buffer[0])
                        {
                            case (byte)'/':
                                {
                                    var sb = new StringBuilder();
                                    sb.Append("//");
                                    while (file.Position < file.Length)
                                    {
                                        file.ReadExactly(buffer);
                                        if (buffer[0] == (byte)'\n')
                                        {
                                            _line++;
                                            _startOfLine = file.Position;
                                            break;
                                        }

                                        sb.Append((char)buffer[0]);
                                    }

                                    return new Token(TokenType.Comment, sb.ToString(), _line, file.Position - _startOfLine);
                                }
                            case (byte)'*':
                                {
                                    var sb = new StringBuilder();
                                    sb.Append("/*");
                                    while (file.Position < file.Length)
                                    {
                                        file.ReadExactly(buffer);
                                        sb.Append((char)buffer[0]);
                                        if (buffer[0] == (byte)'\n')
                                        {
                                            _line++;
                                            _startOfLine = file.Position;
                                        }

                                        if (buffer[0] != (byte)'*') continue;

                                        file.ReadExactly(buffer);
                                        if (buffer[0] == (byte)'/')
                                        {
                                            sb.Append((char)buffer[0]);
                                            break;
                                        }

                                        file.Seek(-1, SeekOrigin.Current);
                                    }

                                    return new Token(TokenType.Comment, sb.ToString(), _line, file.Position - _startOfLine);
                                }
                            default:
                                file.Seek(-1, SeekOrigin.Current);
                                return new Token(TokenType.Operator, "/", _line, file.Position - _startOfLine);
                        }
                    }
                case (byte)'#':
                    {
                        var sb = new StringBuilder();
                        while (file.Position < file.Length)
                        {
                            sb.Append((char)buffer[0]);
                            file.ReadExactly(buffer);
                            if (buffer[0] == (byte)'\\')
                            {
                                file.ReadExactly(buffer);
                                if (buffer[0] != (byte)'\n')
                                    file.Seek(-1, SeekOrigin.Current);
                                else
                                {
                                    _line++;
                                    _startOfLine = file.Position;
                                    continue;
                                }
                            }

                            if (buffer[0] == (byte)'\n')
                            {
                                _line++;
                                _startOfLine = file.Position;
                                break;
                            }
                        }

                        return new Token(TokenType.PreprocessorDirective, sb.ToString(), _line,
                            file.Position - _startOfLine);
                    }
                case (byte)'\n':
                    _line++;
                    _startOfLine = file.Position;
                    return NextToken();
                case (byte)' ':
                case (byte)'\t':
                    return NextToken();
                case (byte)'\'':
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)buffer[0]);
                        while (file.Position < file.Length)
                        {
                            file.ReadExactly(buffer);
                            sb.Append((char)buffer[0]);
                            if (buffer[0] == (byte)'\\')
                            {
                                file.ReadExactly(buffer);
                                sb.Append((char)buffer[0]);
                                continue;
                            }

                            if (buffer[0] == (byte)'\'')
                                break;
                        }

                        return new Token(TokenType.StringInSingleQuotes, sb.ToString(), _line,
                            file.Position - _startOfLine);
                    }
                case (byte)'"':
                    {
                        var sb = new StringBuilder();
                        sb.Append('"');
                        while (file.Position < file.Length)
                        {
                            file.ReadExactly(buffer);
                            sb.Append((char)buffer[0]);

                            if (buffer[0] == (byte)'\\')
                            {
                                file.ReadExactly(buffer);
                                sb.Append((char)buffer[0]);
                            }

                            if (buffer[0] == (byte)'"')
                                break;
                        }

                        return new Token(TokenType.StringInDoubleQuotes, sb.ToString(), _line,
                            file.Position - _startOfLine);
                    }
                case (byte)'+':
                case (byte)'-':
                case (byte)'*':
                case (byte)'%':
                case (byte)'=':
                case (byte)'!':
                case (byte)'<':
                case (byte)'>':
                case (byte)'&':
                case (byte)'|':
                case (byte)'^':
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)buffer[0]);
                        if (file.Position < file.Length)
                        {
                            file.ReadExactly(buffer);
                            if ((buffer[0] == (byte)'=' || buffer[0] == (byte)'&' || buffer[0] == (byte)'|') &&
                                (sb[0] == '+' || sb[0] == '-' || sb[0] == '*' || sb[0] == '%' || sb[0] == '=' ||
                                 sb[0] == '!' || sb[0] == '<' || sb[0] == '>'))
                            {
                                sb.Append((char)buffer[0]);
                            }
                            else
                            {
                                file.Seek(-1, SeekOrigin.Current);
                            }
                        }

                        return new Token(TokenType.Operator, sb.ToString(), _line, file.Position - _startOfLine);
                    }
                case (byte)'?':
                    return new Token(TokenType.TernaryQuestion, "?", _line, file.Position - _startOfLine);
                case (byte)':':
                    return new Token(TokenType.TernaryColon, ":", _line, file.Position - _startOfLine);

                case (byte)'(':
                    return new Token(TokenType.OpenParen, "(", _line, file.Position - _startOfLine);
                case (byte)')':
                    return new Token(TokenType.CloseParen, ")", _line, file.Position - _startOfLine);
                case (byte)'{':
                    return new Token(TokenType.OpenCurly, "{", _line, file.Position - _startOfLine);
                case (byte)'}':
                    return new Token(TokenType.CloseCurly, "}", _line, file.Position - _startOfLine);
                case (byte)'[':
                    return new Token(TokenType.OpenSquare, "[", _line, file.Position - _startOfLine);
                case (byte)']':
                    return new Token(TokenType.CloseSquare, "]", _line, file.Position - _startOfLine);
                case (byte)';':
                    return new Token(TokenType.Semicolon, ";", _line, file.Position - _startOfLine);
                case (byte)',':
                    return new Token(TokenType.Comma, ",", _line, file.Position - _startOfLine);
                case (byte)'.':
                    return new Token(TokenType.Dot, ".", _line, file.Position - _startOfLine);
                case >= (byte)'0' and <= (byte)'9':
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)buffer[0]);
                        while (file.Position < file.Length)
                        {
                            file.ReadExactly(buffer);
                            if ((buffer[0] >= (byte)'A' && buffer[0] <= (byte)'Z') ||
                                (buffer[0] >= (byte)'a' && buffer[0] <= (byte)'z') ||
                                (buffer[0] >= (byte)'0' && buffer[0] <= (byte)'9') ||
                                buffer[0] == (byte)'.' || buffer[0] == (byte)'\'')
                            {
                                sb.Append((char)buffer[0]);
                            }
                            else
                            {
                                file.Seek(-1, SeekOrigin.Current);
                                break;
                            }
                        }

                        return new Token(TokenType.Number, sb.ToString(), _line, file.Position - _startOfLine);
                    }
                case >= (byte)'A' and <= (byte)'Z':
                case >= (byte)'a' and <= (byte)'z':
                case (byte)'_':
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)buffer[0]);
                        while (file.Position < file.Length)
                        {
                            file.ReadExactly(buffer);
                            if ((buffer[0] >= (byte)'A' && buffer[0] <= (byte)'Z') ||
                                (buffer[0] >= (byte)'a' && buffer[0] <= (byte)'z') ||
                                (buffer[0] >= (byte)'0' && buffer[0] <= (byte)'9') ||
                                buffer[0] == (byte)'_')
                            {
                                sb.Append((char)buffer[0]);
                            }
                            else
                            {
                                file.Seek(-1, SeekOrigin.Current);
                                break;
                            }
                        }

                        return new Token(TokenType.Symbol, sb.ToString(), _line, file.Position - _startOfLine);
                    }
                default:
                    return new Token(TokenType.Unknown, ((char)buffer[0]).ToString(), _line,
                        file.Position - _startOfLine);
            }
        }
    }
}

public struct Token(TokenType value, string text, long line, long column)
{
    public TokenType Value { get; } = value;
    public string Text { get; set; } = text;
    public Location Location { get; } = new(line, column);
}

public enum TokenType
{
    TokenEnd,
    PreprocessorDirective,
    Symbol,
    Comment,
    Unknown,
    OpenParen,
    CloseParen,
    OpenCurly,
    CloseCurly,
    OpenSquare,
    CloseSquare,
    Semicolon,
    Number,
    Operator,
    StringInDoubleQuotes,
    StringInSingleQuotes,
    Comma,
    Dot,
    TernaryQuestion,
    TernaryColon,
}

public struct Location(long row, long column)
{
    public long Row { get; set; } = row;
    public long Column { get; set; } = column;
}

public sealed class TypeDefinition
{
    public string? Name { get; set; }
    public List<string> Aliases { get; set; } = new List<string>();
    public string? Comments { get; set; }
    public List<FieldDetails> Fields { get; set; } = new List<FieldDetails>();
    public TypeKind Type { get; set; }

    public sealed class FieldDetails
    {
        private readonly byte[] _fieldName;
        private readonly byte[] _fieldType;

        public FieldDetails(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("Tokens cannot be null or empty.", nameof(tokens));

            int symbolIndex = -1;

            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Value == TokenType.Symbol)
                {
                    symbolIndex = i;
                    break;
                }
            }

            if (symbolIndex == -1)
                throw new InvalidOperationException("No symbol token found.");

            string fieldName = tokens[symbolIndex].Text;
            _fieldName = Encoding.UTF8.GetBytes(fieldName);

            if (symbolIndex == 0)
                _fieldType = Array.Empty<byte>();
            else
            {
                var sb = new StringBuilder();

                for (int i = 0; i < symbolIndex; i++)
                {
                    if (i > 0)
                        sb.Append(' ');

                    sb.Append(tokens[i].Text);
                }

                _fieldType = Encoding.UTF8.GetBytes(sb.ToString());
            }
        }

        public ReadOnlySpan<byte> GetFieldName => _fieldName;

        public ReadOnlySpan<byte> GetFieldType => _fieldType;
    }

    public enum TypeKind
    {
        Unknown,
        Struct,
        Enum,
        Union,
    }
}

public static class Helpers
{
    public static string FixName(this string value, string startsWith)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        var sb = new StringBuilder();
        Debug.Assert(value.StartsWith(startsWith));
        Debug.Assert(value.EndsWith("_t"));
        var isUpperCase = true;
        for (var i = startsWith.Length; i < value.Length - 2; i++)
        {
            if (value[i] == '_')
            {
                isUpperCase = true;
                continue;
            }
            if (isUpperCase)
            {
                sb.Append(char.ToUpper(value[i]));
                isUpperCase = false;
            }
            else
            {
                sb.Append(value[i]);
            }
        }

        return sb.ToString();
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

    public static string FixName(this string value, string startsWith, string endWith)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        var sb = new StringBuilder();
        Debug.Assert(value.StartsWith(startsWith));
        Debug.Assert(value.EndsWith(endWith));
        var isUpperCase = true;
        for (var i = startsWith.Length; i < value.Length - endWith.Length; i++)
        {
            if (value[i] == '_')
            {
                isUpperCase = true;
                continue;
            }
            if (isUpperCase)
            {
                sb.Append(char.ToUpper(value[i]));
                isUpperCase = false;
            }
            else
            {
                sb.Append(value[i]);
            }
        }

        return sb.ToString();
    }
    public static string Trim(this string value, string startsWith, string endWith)
    {
        Span<char> result = stackalloc char[value.Length - (startsWith.Length + endWith.Length)];

        for (int i = 0; i < result.Length; i++)
            result[i] = value[i + startsWith.Length];

        return new string(result);
    }
    private static readonly FrozenDictionary<string, byte[]> _map = new Dictionary<string, byte[]>
        {
            {"uint8_t", "byte"u8.ToArray()},
            {"uint16_t", "ushort"u8.ToArray()},
            {"int16_t", "short"u8.ToArray()},
            {"xcb_window_t", "uint"u8.ToArray()},
        }.ToFrozenDictionary();

    public static ReadOnlySpan<byte> MapCsType(this ReadOnlySpan<byte> value)
    {
        string key = Encoding.UTF8.GetString(value);
        if (_map.TryGetValue(key, out var result))
            return result;
        return value;
    }
}