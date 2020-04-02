using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;


[GeneratedCode("simple-json", "1.0.0")]
public static class SimpleJsonTool
{
    private const int TOKEN_NONE = 0;

    private const int TOKEN_CURLY_OPEN = 1;

    private const int TOKEN_CURLY_CLOSE = 2;

    private const int TOKEN_SQUARED_OPEN = 3;

    private const int TOKEN_SQUARED_CLOSE = 4;

    private const int TOKEN_COLON = 5;

    private const int TOKEN_COMMA = 6;

    private const int TOKEN_STRING = 7;

    private const int TOKEN_NUMBER = 8;

    private const int TOKEN_TRUE = 9;

    private const int TOKEN_FALSE = 10;

    private const int TOKEN_NULL = 11;

    private const int BUILDER_CAPACITY = 2000;

    private static IJsonSerializerStrategy _currentJsonSerializerStrategy;

    private static PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;

    public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
    {
        get
        {
            IJsonSerializerStrategy arg_18_0;
            if ((arg_18_0 = SimpleJsonTool._currentJsonSerializerStrategy) == null)
            {
                arg_18_0 = (SimpleJsonTool._currentJsonSerializerStrategy = SimpleJsonTool.PocoJsonSerializerStrategy);
            }
            return arg_18_0;
        }
        set
        {
            SimpleJsonTool._currentJsonSerializerStrategy = value;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
    {
        get
        {
            PocoJsonSerializerStrategy arg_18_0;
            if ((arg_18_0 = SimpleJsonTool._pocoJsonSerializerStrategy) == null)
            {
                arg_18_0 = (SimpleJsonTool._pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
            }
            return arg_18_0;
        }
    }

    public static object DeserializeObject(string json)
    {
        object result;
        if (SimpleJsonTool.TryDeserializeObject(json, out result))
        {
            return result;
        }
        throw new SerializationException("Invalid JSON string£º" + json);
    }

    public static bool TryDeserializeObject(string json, out object obj)
    {
        bool result = true;
        if (json != null)
        {
            char[] json2 = json.ToCharArray();
            int num = 0;
            obj = SimpleJsonTool.ParseValue(json2, ref num, ref result);
        }
        else
        {
            obj = null;
        }
        return result;
    }

    public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
    {
        object obj = SimpleJsonTool.DeserializeObject(json);
        return (type != null && (obj == null || !ReflectionsUtils.IsAssignableFrom(obj.GetType(), type))) ? (jsonSerializerStrategy ?? SimpleJsonTool.CurrentJsonSerializerStrategy).DeserializeObject(obj, type) : obj;
    }

    public static object DeserializeObject(string json, Type type)
    {
        return SimpleJsonTool.DeserializeObject(json, type, null);
    }

    public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
    {
        return (T)((object)SimpleJsonTool.DeserializeObject(json, typeof(T), jsonSerializerStrategy));
    }

    public static T DeserializeObject<T>(string json)
    {
        return (T)((object)SimpleJsonTool.DeserializeObject(json, typeof(T), null));
    }

    public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
    {
        StringBuilder stringBuilder = GetStringBuilder();
        bool flag = SimpleJsonTool.SerializeValue(jsonSerializerStrategy, json, stringBuilder);

        string res = (!flag) ? null : stringBuilder.ToString();
        RecycleStringBuilder(stringBuilder);
        return res;
    }

    public static string SerializeObject(object json)
    {
        return SimpleJsonTool.SerializeObject(json, SimpleJsonTool.CurrentJsonSerializerStrategy);
    }

    public static string EscapeToJavascriptString(string jsonString)
    {
        string result;
        if (string.IsNullOrEmpty(jsonString))
        {
            result = jsonString;
        }
        else
        {
            StringBuilder stringBuilder = GetStringBuilder();
            int i = 0;
            while (i < jsonString.Length)
            {
                char c = jsonString[i++];
                if (c == '\\')
                {
                    int num = jsonString.Length - i;
                    if (num >= 2)
                    {
                        char c2 = jsonString[i];
                        if (c2 == '\\')
                        {
                            stringBuilder.Append('\\');
                            i++;
                        }
                        else if (c2 == '"')
                        {
                            stringBuilder.Append("\"");
                            i++;
                        }
                        else if (c2 == 't')
                        {
                            stringBuilder.Append('\t');
                            i++;
                        }
                        else if (c2 == 'b')
                        {
                            stringBuilder.Append('\b');
                            i++;
                        }
                        else if (c2 == 'n')
                        {
                            stringBuilder.Append('\n');
                            i++;
                        }
                        else if (c2 == 'r')
                        {
                            stringBuilder.Append('\r');
                            i++;
                        }
                    }
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            result = stringBuilder.ToString();

            RecycleStringBuilder(stringBuilder);
        }
        return result;
    }

    private static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
    {
        IDictionary<string, object> dictionary = new JsonObject();
        SimpleJsonTool.NextToken(json, ref index);
        bool flag = false;
        IDictionary<string, object> result;
        while (!flag)
        {
            int num = SimpleJsonTool.LookAhead(json, index);
            if (num != 0)
            {
                if (num == 6)
                {
                    SimpleJsonTool.NextToken(json, ref index);
                }
                else
                {
                    if (num == 2)
                    {
                        SimpleJsonTool.NextToken(json, ref index);
                        result = dictionary;
                        return result;
                    }
                    string key = SimpleJsonTool.ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        result = null;
                        return result;
                    }
                    num = SimpleJsonTool.NextToken(json, ref index);
                    if (num != 5)
                    {
                        success = false;
                        result = null;
                        return result;
                    }
                    object value = SimpleJsonTool.ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        result = null;
                        return result;
                    }
                    dictionary[key] = value;
                }
                continue;
            }
            success = false;
            result = null;
            return result;
        }
        result = dictionary;
        return result;
    }

    private static JsonArray ParseArray(char[] json, ref int index, ref bool success)
    {
        JsonArray jsonArray = new JsonArray();
        SimpleJsonTool.NextToken(json, ref index);
        bool flag = false;
        JsonArray result;
        while (!flag)
        {
            int num = SimpleJsonTool.LookAhead(json, index);
            if (num != 0)
            {
                if (num == 6)
                {
                    SimpleJsonTool.NextToken(json, ref index);
                }
                else
                {
                    if (num == 4)
                    {
                        SimpleJsonTool.NextToken(json, ref index);
                        break;
                    }
                    object item = SimpleJsonTool.ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        result = null;
                        return result;
                    }
                    jsonArray.Add(item);
                }
                continue;
            }
            success = false;
            result = null;
            return result;
        }
        result = jsonArray;
        return result;
    }

    private static object ParseValue(char[] json, ref int index, ref bool success)
    {
        object result;
        switch (SimpleJsonTool.LookAhead(json, index))
        {
            case 1:
                result = SimpleJsonTool.ParseObject(json, ref index, ref success);
                return result;
            case 3:
                result = SimpleJsonTool.ParseArray(json, ref index, ref success);
                return result;
            case 7:
                result = SimpleJsonTool.ParseString(json, ref index, ref success);
                return result;
            case 8:
                result = SimpleJsonTool.ParseNumber(json, ref index, ref success);
                return result;
            case 9:
                SimpleJsonTool.NextToken(json, ref index);
                result = true;
                return result;
            case 10:
                SimpleJsonTool.NextToken(json, ref index);
                result = false;
                return result;
            case 11:
                SimpleJsonTool.NextToken(json, ref index);
                result = null;
                return result;
        }
        success = false;
        result = null;
        return result;
    }
    private static string ParseString(char[] json, ref int index, ref bool success)
    {
        StringBuilder stringBuilder = GetStringBuilder(); //new StringBuilder(2000);

        SimpleJsonTool.EatWhitespace(json, ref index);
        char c = json[index++];
        bool flag = false;
        string result;
        while (!flag)
        {
            if (index == json.Length)
            {
                break;
            }
            c = json[index++];
            if (c == '"')
            {
                flag = true;
                break;
            }
            if (c == '\\')
            {
                if (index == json.Length)
                {
                    break;
                }
                c = json[index++];
                if (c == '"')
                {
                    stringBuilder.Append('"');
                }
                else if (c == '\\')
                {
                    stringBuilder.Append('\\');
                }
                else if (c == '/')
                {
                    stringBuilder.Append('/');
                }
                else if (c == 'b')
                {
                    stringBuilder.Append('\b');
                }
                else if (c == 'f')
                {
                    stringBuilder.Append('\f');
                }
                else if (c == 'n')
                {
                    stringBuilder.Append('\n');
                }
                else if (c == 'r')
                {
                    stringBuilder.Append('\r');
                }
                else if (c == 't')
                {
                    stringBuilder.Append('\t');
                }
                else if (c == 'u')
                {
                    int num = json.Length - index;
                    if (num >= 4)
                    {
                        uint num2;
                        if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
                        {
                            result = "";
                        }
                        else
                        {
                            if (55296u > num2 || num2 > 56319u)
                            {
                                stringBuilder.Append(SimpleJsonTool.ConvertFromUtf32((int)num2));
                                index += 4;
                                continue;
                            }
                            index += 4;
                            num = json.Length - index;
                            if (num >= 6)
                            {
                                uint num3;
                                if (new string(json, index, 2) == "\\u" && uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3))
                                {
                                    if (56320u <= num3 && num3 <= 57343u)
                                    {
                                        stringBuilder.Append((char)num2);
                                        stringBuilder.Append((char)num3);
                                        index += 6;
                                        continue;
                                    }
                                }
                            }
                            success = false;
                            result = "";
                        }
                        return result;
                    }
                    break;
                }
            }
            else
            {
                stringBuilder.Append(c);
            }
        }
        if (!flag)
        {
            success = false;
            result = null;
            return result;
        }
        result = stringBuilder.ToString();

        RecycleStringBuilder(stringBuilder);

        return result;
    }

    private static string ConvertFromUtf32(int utf32)
    {
        if (utf32 < 0 || utf32 > 1114111)
        {
            throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
        }
        if (55296 <= utf32 && utf32 <= 57343)
        {
            throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
        }
        string result;
        if (utf32 < 65536)
        {
            result = new string((char)utf32, 1);
        }
        else
        {
            utf32 -= 65536;
            result = new string(new char[]
            {
                    (char)((utf32 >> 10) + 55296),
                    (char)(utf32 % 1024 + 56320)
            });
        }
        return result;
    }

    private static object ParseNumber(char[] json, ref int index, ref bool success)
    {
        SimpleJsonTool.EatWhitespace(json, ref index);
        int lastIndexOfNumber = SimpleJsonTool.GetLastIndexOfNumber(json, index);
        int length = lastIndexOfNumber - index + 1;
        string text = new string(json, index, length);
        object result;
        if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
        {
            double num;
            success = double.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num);
            result = num;
        }
        else
        {
            long num2;
            success = long.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
            result = num2;
        }
        index = lastIndexOfNumber + 1;
        return result;
    }

    private static int GetLastIndexOfNumber(char[] json, int index)
    {
        int i;
        for (i = index; i < json.Length; i++)
        {
            if ("0123456789+-.eE".IndexOf(json[i]) == -1)
            {
                break;
            }
        }
        return i - 1;
    }

    private static void EatWhitespace(char[] json, ref int index)
    {
        while (index < json.Length)
        {
            if (" \t\n\r\b\f".IndexOf(json[index]) == -1)
            {
                break;
            }
            index++;
        }
    }

    private static int LookAhead(char[] json, int index)
    {
        int num = index;
        return SimpleJsonTool.NextToken(json, ref num);
    }

    private static int NextToken(char[] json, ref int index)
    {
        SimpleJsonTool.EatWhitespace(json, ref index);
        int result;
        if (index != json.Length)
        {
            char c = json[index];
            index++;
            switch (c)
            {
                case ',':
                    result = 6;
                    return result;
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result = 8;
                    return result;
                case '.':
                case '/':
                    IL_69:
                    switch (c)
                    {
                        case '[':
                            result = 3;
                            return result;
                        case '\\':
                            IL_7E:
                            switch (c)
                            {
                                case '{':
                                    result = 1;
                                    return result;
                                case '|':
                                    IL_93:
                                    if (c != '"')
                                    {
                                        index--;
                                        int num = json.Length - index;
                                        if (num >= 5)
                                        {
                                            if (json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
                                            {
                                                index += 5;
                                                result = 10;
                                                return result;
                                            }
                                        }
                                        if (num >= 4)
                                        {
                                            if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
                                            {
                                                index += 4;
                                                result = 9;
                                                return result;
                                            }
                                        }
                                        if (num >= 4)
                                        {
                                            if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
                                            {
                                                index += 4;
                                                result = 11;
                                                return result;
                                            }
                                        }
                                        result = 0;
                                        return result;
                                    }
                                    result = 7;

                                    return result;

                                case '}':
                                    result = 2;
                                    return result;
                                default:
                                    goto IL_93;
                            }


                        case ']':
                            result = 4;
                            return result;
                        default:
                            goto IL_7E;
                    }

                case ':':
                    result = 5;
                    return result;
                default:
                    goto IL_69;
            }

        }
        result = 0;
        return result;
    }

    private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
    {
        bool flag = true;
        string text = value as string;
        if (text != null)
        {
            flag = SimpleJsonTool.SerializeString(text, builder);
        }
        else
        {
            IDictionary<string, object> dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                flag = SimpleJsonTool.SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder);
            }
            else
            {
                IDictionary<string, string> dictionary2 = value as IDictionary<string, string>;
                if (dictionary2 != null)
                {
                    flag = SimpleJsonTool.SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder);
                }
                else
                {
                    IEnumerable enumerable = value as IEnumerable;
                    if (enumerable != null)
                    {
                        flag = SimpleJsonTool.SerializeArray(jsonSerializerStrategy, enumerable, builder);
                    }
                    else if (SimpleJsonTool.IsNumeric(value))
                    {
                        flag = SimpleJsonTool.SerializeNumber(value, builder);
                    }
                    else if (value is bool)
                    {
                        builder.Append((!(bool)value) ? "false" : "true");
                    }
                    else if (value == null)
                    {
                        builder.Append("null");
                    }
                    else
                    {
                        object value2;
                        flag = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out value2);
                        if (flag)
                        {
                            SimpleJsonTool.SerializeValue(jsonSerializerStrategy, value2, builder);
                        }
                    }
                }
            }
        }
        return flag;
    }

    private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
    {
        builder.Append("{");
        IEnumerator enumerator = keys.GetEnumerator();
        IEnumerator enumerator2 = values.GetEnumerator();
        bool flag = true;
        bool result;
        while (enumerator.MoveNext() && enumerator2.MoveNext())
        {
            object current = enumerator.Current;
            object current2 = enumerator2.Current;
            if (!flag)
            {
                builder.Append(",");
            }
            string text = current as string;
            if (text != null)
            {
                SimpleJsonTool.SerializeString(text, builder);
            }
            else if (!SimpleJsonTool.SerializeValue(jsonSerializerStrategy, current2, builder))
            {
                result = false;
                return result;
            }
            builder.Append(":");
            if (SimpleJsonTool.SerializeValue(jsonSerializerStrategy, current2, builder))
            {
                flag = false;
                continue;
            }
            result = false;
            return result;
        }
        builder.Append("}");
        result = true;
        return result;
    }

    private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
    {
        builder.Append("[");
        bool flag = true;
        IEnumerator enumerator = anArray.GetEnumerator();
        bool result;
        try
        {
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (!flag)
                {
                    builder.Append(",");
                }
                if (!SimpleJsonTool.SerializeValue(jsonSerializerStrategy, current, builder))
                {
                    result = false;
                    return result;
                }
                flag = false;
            }
        }
        finally
        {
            IDisposable disposable;
            if ((disposable = (enumerator as IDisposable)) != null)
            {
                disposable.Dispose();
            }
        }
        builder.Append("]");
        result = true;
        return result;
    }

    private static bool SerializeString(string aString, StringBuilder builder)
    {
        builder.Append("\"");
        char[] array = aString.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            char c = array[i];
            if (c == '"')
            {
                builder.Append("\\\"");
            }
            else if (c == '\\')
            {
                builder.Append("\\\\");
            }
            else if (c == '\b')
            {
                builder.Append("\\b");
            }
            else if (c == '\f')
            {
                builder.Append("\\f");
            }
            else if (c == '\n')
            {
                builder.Append("\\n");
            }
            else if (c == '\r')
            {
                builder.Append("\\r");
            }
            else if (c == '\t')
            {
                builder.Append("\\t");
            }
            else
            {
                builder.Append(c);
            }
        }
        builder.Append("\"");
        return true;
    }

    private static bool SerializeNumber(object number, StringBuilder builder)
    {
        if (number is long)
        {
            builder.Append(((long)number).ToString(CultureInfo.InvariantCulture));
        }
        else if (number is ulong)
        {
            builder.Append(((ulong)number).ToString(CultureInfo.InvariantCulture));
        }
        else if (number is int)
        {
            builder.Append(((int)number).ToString(CultureInfo.InvariantCulture));
        }
        else if (number is uint)
        {
            builder.Append(((uint)number).ToString(CultureInfo.InvariantCulture));
        }
        else if (number is decimal)
        {
            builder.Append(((decimal)number).ToString(CultureInfo.InvariantCulture));
        }
        else if (number is float)
        {
            builder.Append(((float)number).ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
        }
        return true;
    }

    private static bool IsNumeric(object value)
    {
        return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
    }

    private static Queue<StringBuilder> stringBuilders = new Queue<StringBuilder>();

    private static StringBuilder GetStringBuilder()
    {
        StringBuilder builder = null;
        if (stringBuilders.Count <= 0)
            builder = new StringBuilder(2000);
        else
        {
            builder = stringBuilders.Dequeue();
            if (builder.Length > 0)
                builder.Remove(0, builder.Length);
        }

        return builder;
    }
    private static void RecycleStringBuilder(StringBuilder builder)
    {
        stringBuilders.Enqueue(builder);
    }

}
