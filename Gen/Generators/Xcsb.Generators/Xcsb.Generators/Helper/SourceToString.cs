using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xcsb.Generators.Helper;

internal static class SourceToString
{
    private const BindingFlags AllDeclared =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        public static string GetSource(params Type[] types)
        {
            var sb = new StringBuilder();
            sb.AppendLine("namespace Xcsb.Generators");
            sb.AppendLine("{");
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0) sb.AppendLine();
                if (types[i].IsEnum) WriteEnum(sb, types[i]);
                else WriteClass(sb, types[i]);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void WriteEnum(StringBuilder sb, Type type)
        {
            WriteAttributes(sb, type, "");
            sb.Append(Visibility(type)).Append(" enum ").Append(type.Name);

            var underlying = Enum.GetUnderlyingType(type);
            if (underlying != typeof(int)) sb.Append(" : ").Append(TypeName(underlying));
            sb.AppendLine().AppendLine("{");

            var members = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => "    " + f.Name + " = " +
                             Convert.ToString(f.GetRawConstantValue(), CultureInfo.InvariantCulture))
                .ToList();
            foreach (var member in members)
                sb.AppendLine(member + ",");
            
            sb.AppendLine("}");
        }
        private static void WriteClass(StringBuilder sb, Type type)
        {
            WriteAttributes(sb, type, "");

            sb.Append(Visibility(type));
            if (type.IsAbstract && type.IsSealed) sb.Append(" static");
            else if (type.IsSealed) sb.Append(" sealed");
            else if (type.IsAbstract) sb.Append(" abstract");
            sb.Append(" class ").Append(type.Name);

            if (type.BaseType != null && type.BaseType != typeof(object))
                sb.Append(" : ").Append(TypeName(type.BaseType));

            sb.AppendLine();
            sb.AppendLine("{");

            var first = true;

            foreach (var c in type.GetConstructors(AllDeclared).Where(c => !c.IsStatic))
            {
                if (!first) sb.AppendLine();
                first = false;

                var p = string.Join(", ", c.GetParameters().Select(x => TypeName(x.ParameterType) + " " + x.Name));
                sb.AppendLine("    " + MemberVisibility(c) + " " + type.Name + "(" + p + ")");
                sb.AppendLine("    {");
                sb.AppendLine("    }");
            }

            foreach (var f in type.GetFields(AllDeclared))
            {
                if (!first) sb.AppendLine();
                first = false;

                sb.AppendLine("    " + MemberVisibility(f) + (f.IsStatic ? " static" : "") + " " +
                              TypeName(f.FieldType) + " " + f.Name + ";");
            }

            foreach (var m in type.GetMethods(AllDeclared).Where(m => !m.IsSpecialName))
            {
                if (!first) sb.AppendLine();
                first = false;

                var p = string.Join(", ", m.GetParameters().Select(x => TypeName(x.ParameterType) + " " + x.Name));
                sb.AppendLine("    " + MemberVisibility(m) + (m.IsStatic ? " static" : "") + " " +
                              TypeName(m.ReturnType) + " " + m.Name + "(" + p + ")");
                sb.AppendLine("    {");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
        }
        private static void WriteAttributes(StringBuilder sb, MemberInfo member, string indent)
        {
            foreach (var data in CustomAttributeData.GetCustomAttributes(member))
            {
                var name = TypeName(data.AttributeType);
                const string suffix = "Attribute";
                if (name.EndsWith(suffix, StringComparison.Ordinal))
                    name = name.Substring(0, name.Length - suffix.Length);

                var args = data.ConstructorArguments.Select(FormatArg)
                    .Concat(data.NamedArguments.Select(n => n.MemberName + " = " + FormatArg(n.TypedValue)))
                    .ToList();

                sb.Append(indent).Append('[').Append(name);
                if (args.Count > 0) sb.Append('(').Append(string.Join(", ", args)).Append(')');
                sb.AppendLine("]");
            }
        }

        private static string FormatArg(CustomAttributeTypedArgument a)
        {
            if (a.Value == null) return "null";
            if (a.ArgumentType.IsEnum) return FormatEnum(a.ArgumentType, a.Value);

            if (a.Value is string s)
                return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            if (a.Value is bool b) return b ? "true" : "false";
            if (a.Value is Type t) return "typeof(" + TypeName(t) + ")";
            if (a.Value is char c) return "'" + c + "'";
            return Convert.ToString(a.Value, CultureInfo.InvariantCulture);
        }

        private static string FormatEnum(Type enumType, object value)
        {
            var text = Enum.ToObject(enumType, value).ToString();
            var parts = text.Split(new[] { ", " }, StringSplitOptions.None);

            if (char.IsDigit(parts[0][0]) || parts[0][0] == '-')
                return "(" + TypeName(enumType) + ")" + text;

            return string.Join(" | ", parts.Select(p => TypeName(enumType) + "." + p));
        }

        private static string TypeName(Type t)
        {
            if (t == typeof(void)) return "void";
            if (t == typeof(int)) return "int";
            if (t == typeof(string)) return "string";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(object)) return "object";
            if (t == typeof(long)) return "long";
            if (t == typeof(byte)) return "byte";

            var full = (t.FullName ?? t.Name).Replace('+', '.');
            return "global::" + full;
        }

        private static string Visibility(Type t) => t.IsPublic || t.IsNestedPublic ? "public" : "internal";

        private static string MemberVisibility(MethodBase m) =>
            m.IsPublic ? "public" : m.IsPrivate ? "private" : m.IsFamily ? "protected" : "internal";

        private static string MemberVisibility(FieldInfo f) =>
            f.IsPublic ? "public" : f.IsPrivate ? "private" : f.IsFamily ? "protected" : "internal";
}