/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using FrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Deserializer {

    /// <summary>
    /// A Class used to direct which class to make when it's not obvious, like if you have a class
    /// with a member that's a base class but the actual class could be one of many derived classes.
    /// </summary>
    public abstract class CustomCreator {

        /// <summary>
        /// Creates an new derived class when a base is expected
        /// </summary>
        /// <param name="src">A dictionary of the json fields that belong to the object to be created.</param>
        /// <param name="parentSrc">A dictionary of the json fields that belong to the object that is the parent of the object to be created.</param>
        /// <example>
        /// Example: Assume you have the following classes
        /// <code>
        ///     class Fruit { public int type; }
        ///     class Apple : Fruit { public float height; public float radius; };
        ///     class Raspberry : Fruit { public int numBulbs; }
        /// </code>
        /// You'd register a dervied CustomCreator for type `Fruit`. When the Deserialize needs to create
        /// a `Fruit` it will call your Create function. Using `src` you could look at `type` and
        /// decide whether to make an Apple or a Raspberry.
        /// <code>
        ///     int type = src["type"];
        ///     if (type == 0) { return new Apple; }
        ///     if (type == 1) { return new Raspberry; }
        ///     .
        /// </code>
        /// If the parent has info on the type you can do this
        /// <code>
        ///     class Fruit { }
        ///     class Apple : Fruit { public float height; public float radius; };
        ///     class Raspberry : Fruit { public int numBulbs; }
        ///     class Basket { public int typeInBasket; Fruit fruit; }
        /// </code>
        /// In this case again, when trying to create a `Fruit` your CustomCreator.Create function
        /// will be called. You can use `'parentSrc`' to look at the fields from 'Basket' as in
        /// <code>
        ///     int typeInBasket = parentSrc['typeInBasket'];
        ///     if (type == 0) { return new Apple; }
        ///     if (type == 1) { return new Raspberry; }
        ///     .
        /// </code>
        /// </example>
        /// <returns>The created object</returns>
        public abstract object Create(Dictionary<string, object> src, Dictionary<string, object> parentSrc);

        /// <summary>
        /// The base type this CustomCreator makes.
        /// </summary>
        /// <returns>The type this CustomCreator makes.</returns>
        public abstract System.Type TypeToCreate();
    }

    static public System.Type GetTypeByString(string typeStr) {
		return System.Type.GetType (typeStr);
	}

    /// <summary>
    /// Deserializer for Json to your classes.
    /// </summary>
    public Deserializer(bool includePrivateFields = false) {
        m_creators = new Dictionary<System.Type, CustomCreator>();
        m_fieldFlags =
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            (includePrivateFields ? System.Reflection.BindingFlags.NonPublic : 0);
    }

    /// <summary>
    /// Deserializes a json string into classes.
    /// </summary>
    /// <param name="json">String containing JSON</param>
    /// <returns>An instance of class T.</returns>
    /// <example>
    /// <code>
    ///     public class Foo {
    ///         public int num;
    ///         public string name;
    ///         public float weight;
    ///     };
    ///
    ///     public class Bar {
    ///         public int hp;
    ///         public Foo someFoo;
    ///     };
    /// ..
    ///     Deserializer deserializer = new Deserializer();
    ///
    ///     string json = "{\"hp\":123,\"someFoo\":{\"num\":456,\"name\":\"gman\",\"weight\":156.4}}";
    ///
    ///     Bar bar = deserializer.Deserialize<Bar>(json);
    ///
    ///     print("bar.hp: " + bar.hp);
    ///     print("bar.someFoo.num: " + bar.someFoo.num);
    ///     print("bar.someFoo.name: " + bar.someFoo.name);
    ///     print("bar.someFoo.weight: " + bar.someFoo.weight);
    ///
    /// </code>
    /// </example>
    public T Deserialize<T>(string json) {
        object o = Json.Deserialize(json);
        return Deserialize<T>(o);
    }

    /// <summary>
    /// Deserializes a object into classes.
    /// </summary>
    /// <param name="o">Object containing data</param>
    /// <returns>An instance of class T.</returns>
    /// <example>
    /// <code>
    ///     public class Foo {
    ///         public int num;
    ///         public string name;
    ///         public float weight;
    ///     };
    ///
    ///     public class Bar {
    ///         public int hp;
    ///         public Foo someFoo;
    ///     };
    /// ..
    ///     Deserializer deserializer = new Deserializer();
    ///
    ///     Dictionary<string, object> d = new Dictionary<string, object>
    ///     d["num"] = 123;
    ///     d["name"] = "Bob";
    ///     d["weight] = 4.5f;
    ///
    ///     Foo foo = deserializer.Deserialize<Foo>(d);
    ///
    ///     print("foo.num: " + foo.num);
    ///     print("foo.name: " + foo.name);
    ///     print("foo.weight: " + foo.weight);
    ///
    /// </code>
    /// </example>
    public T Deserialize<T>(object o) {
        return (T)ConvertToType(o, typeof(T), null);
    }

        public object Deserialize(Type type, string json)
        {
            object o = Json.Deserialize(json);
            return ConvertToType(o, type, null);
        }

        public object Deserialize(string typeName, string json)
    {
        object o = Json.Deserialize(json);
        Type type = Type.GetType(typeName);
        return ConvertToType(o, type, null);
    }

    /// <summary>
    /// Registers a CustomCreator.
    /// </summary>
    /// <param name="creator">The creator to register</param>
        public void RegisterCreator(CustomCreator creator) {
        System.Type t = creator.TypeToCreate();
        m_creators[t] = creator;
    }

    private object DeserializeO(Type destType, Dictionary<string, object> src, Dictionary<string, object> parentSrc) {
        object dest = null;

        // This seems like a hack but for now maybe it's the right thing?
        // Basically if the thing you want is a Dictionary<stirng, object>
        // Then just give it to you since that's the source. No need
        // to try to copy it.
        if (destType == typeof(System.Object) ||
            destType == typeof(System.Collections.Generic.Dictionary<string, object>)) {
            return src;
        }

        // First see if there is a CustomCreator for this type.
        CustomCreator creator;
        if (m_creators.TryGetValue(destType, out creator)) {
            dest = creator.Create(src, parentSrc);
        }

        if (dest == null) {
            // Check if there is a type serialized for this
            object typeNameObject;
            if (src.TryGetValue("$dotNetType", out typeNameObject)) {
                destType = System.Type.GetType((string)typeNameObject);
            }
            dest = Activator.CreateInstance(destType);
        }

        DeserializeIt(dest, src);
        return dest;
    }

    private void DeserializeIt(object dest, Dictionary<string, object> src) {
        System.Type type = dest.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields(m_fieldFlags);
        DeserializeClassFields(dest, fields, src);
    }

    private void DeserializeClassFields(object dest, System.Reflection.FieldInfo[] fields, Dictionary<string, object> src) {
        foreach (System.Reflection.FieldInfo info in fields) {
            if (info.IsStatic) {
                continue;
            }
            object value;
            if (src.TryGetValue(info.Name, out value)) {
                DeserializeField(dest, info, value, src);
            }
        }
    }

    private void DeserializeField(object dest, System.Reflection.FieldInfo info, object value, Dictionary<string, object> src) {
        Type fieldType = info.FieldType;
        object o = ConvertToType(value, fieldType, src);
        if (fieldType.IsAssignableFrom(o.GetType())) {
            info.SetValue(dest, o);
        }
    }

    public static bool IsGenericList(System.Type type)
    {
        return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
    }

    public static bool IsGenericDictionary(System.Type type)
    {
        return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
    }

    private object ConvertToType(object value, System.Type type, Dictionary<string, object> src) {
        if (type.IsArray) {
            return ConvertToArray(value, type, src);
        } else if (Deserializer.IsGenericList(type)) {
            return ConvertToList(value, type, src);
        } else if (Deserializer.IsGenericDictionary(type)) {
            return ConvertToDictionary(value, type, src);
//        } else if (type == typeof(List<object>)) {
//            object[] oArray = ((List<object>)value).ToArray();
//            return ConvertToArray(oArray, type, src); 
//        } else if (type.IsPrimitive) {
//           return System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value.ToString ());
        } else if (type.IsEnum) {
            return System.Enum.Parse(type, Convert.ToString(value));
        } else if (type == typeof(string)) {
            return Convert.ToString(value);
        } else if (type == typeof(Byte)) {
            return Convert.ToByte(value);
        } else if (type == typeof(SByte)) {
            return Convert.ToSByte(value);
        } else if (type == typeof(Int16)) {
            return Convert.ToInt16(value);
        } else if (type == typeof(UInt16)) {
            return Convert.ToUInt16(value);
        } else if (type == typeof(Int32)) {
            return Convert.ToInt32(value);
        } else if (type == typeof(UInt32)) {
            return Convert.ToUInt32(value);
        } else if (type == typeof(Int64)) {
            return Convert.ToInt64(value);
        } else if (type == typeof(UInt64)) {
            return Convert.ToUInt64(value);
        } else if (type == typeof(Char)) {
            return Convert.ToChar(value);
        } else if (type == typeof(Double)) {
            return Convert.ToDouble(value);
        } else if (type == typeof(Single)) {
            return Convert.ToSingle(value);
        } else if (type == typeof(int)) {
            return Convert.ToInt32(value);
        } else if (type == typeof(float)) {
            return Convert.ToSingle(value);
        } else if (type == typeof(double)) {
            return Convert.ToDouble(value);
        } else if (type == typeof(bool)) {
            return Convert.ToBoolean(value);
        } else if (type == typeof(Boolean)) {
            return Convert.ToBoolean(value);
        } else if (type.IsValueType) {
            return DeserializeO(type, (Dictionary<string, object>)value, src);
        } else if (type == typeof(System.Object)) {
            return value;
        } else if (type.IsClass) {
            return DeserializeO(type, (Dictionary<string, object>)value, src);
        } else {
            // Should we throw here?
        }
        return value;
    }

    private object ConvertToDictionary(object value, System.Type type, Dictionary<string, object> src) {
        Type typeDef = type.GetGenericTypeDefinition();
        Type[] typeArgs = type.GetGenericArguments();
        Type constructed = typeDef.MakeGenericType(typeArgs);
        object dict = Activator.CreateInstance(constructed);
        Dictionary<string, object> srcDict = (Dictionary<string, object>)value;
        foreach(KeyValuePair<string, object> entry in srcDict)
        {
            object elementKey = ConvertToType(entry.Key, typeArgs[0], src);
            object elementValue = ConvertToType(entry.Value, typeArgs[1], src);
            dict.GetType().GetMethod("Add").Invoke(dict, new[] {elementKey, elementValue});
        }
        return dict;
    }

    private object ConvertToList(object value, System.Type type, Dictionary<string, object> src) {
        Type typeDef = type.GetGenericTypeDefinition();
        Type[] typeArgs = type.GetGenericArguments();
        Type constructed = typeDef.MakeGenericType(typeArgs);
        object list = Activator.CreateInstance(constructed);
        List<object> elements = (List<object>)value;
        int index = 0;
        foreach (object elementValue in elements) {
            object o = ConvertToType(elementValue, typeArgs[0], src);
            list.GetType().GetMethod("Add").Invoke(list, new[] {o});
            ++index;
        }
        return list;
    }

    private object ConvertToArray(object value, System.Type type, Dictionary<string, object> src) {
        List<object> elements = (List<object>)value;
        int numElements = elements.Count;
        Type elementType = type.GetElementType();
        Array array = Array.CreateInstance(elementType, numElements);
        int index = 0;
        foreach (object elementValue in elements) {
            object o = ConvertToType(elementValue, elementType, src);
            array.SetValue(o, index);
            ++index;
        }
        return array;
    }

    private Dictionary<System.Type, CustomCreator> m_creators;
    private System.Reflection.BindingFlags m_fieldFlags;
};

/// <summary>
/// At this point this is just a namespace.
/// See Serializer.Serialize
/// </summary>
public class Serializer {

    /// <summary>
    /// Serializes an object to JSON.
    ///
    /// Only Public fields will be serialized.
    /// </summary>
    /// <param name="obj">Object to serialized</param>
    /// <param name="includeTypeInfoForDerivedTypes">true if you want it to add type data for derived types</param>
    /// <param name="prettyPrint">try if you want whitespace, linebreaks, and indention</param>
    /// <returns>a json string representing the object passed in.</returns>
    public static string Serialize(object obj, bool includeTypeInfoForDerivedTypes = false, bool prettyPrint = false, bool includePrivateFields = false) {
        Serializer s = new Serializer(includeTypeInfoForDerivedTypes, prettyPrint, includePrivateFields);
        s.SerializeValue(obj);
        return s.GetJson();
    }

    private Serializer(bool includeTypeInfoForDerivedTypes, bool prettyPrint, bool includePrivateFields) {
        m_builder = new StringBuilder();
        m_includeTypeInfoForDerivedTypes = includeTypeInfoForDerivedTypes;
        m_prettyPrint = prettyPrint;
        m_includePrivateFields = includePrivateFields;
        m_prefix = "";

        m_fieldFlags =
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            (m_includePrivateFields ? System.Reflection.BindingFlags.NonPublic : 0);
    }

    private string GetJson() {
        return m_builder.ToString();
    }

    private StringBuilder m_builder;
    private bool m_includeTypeInfoForDerivedTypes;
    private bool m_prettyPrint;
    private bool m_includePrivateFields;
    private System.Reflection.BindingFlags m_fieldFlags;
    private string m_prefix;

    private void Indent() {
        if (m_prettyPrint) {
            m_prefix = m_prefix + "  ";
        }
    }

    private void Outdent() {
        if (m_prettyPrint) {
            m_prefix = m_prefix.Substring(2);
        }
    }

    private void AddIndent() {
        if (m_prettyPrint) {
            m_builder.Append(m_prefix);
        }
    }

    private void AddLine() {
        if (m_prettyPrint) {
            m_builder.Append("\n");
        }
    }

    private void AddSpace() {
        if (m_prettyPrint) {
            m_builder.Append(" ");
        }
    }

    private void SerializeValue(object obj) {
        if (obj == null) {
            m_builder.Append("undefined");
            return;
        }
        System.Type type = obj.GetType();

        if (type.IsArray) {
            SerializeArray(obj);
        } else if (Deserializer.IsGenericList(type)) { //(type == typeof(List<Object>)) {
                Type elementType = type.GetGenericArguments()[0];
                System.Reflection.MethodInfo castMethod = typeof(System.Linq.Enumerable).GetMethod("Cast").MakeGenericMethod( new System.Type[]{ elementType } );
                System.Reflection.MethodInfo toArrayMethod = typeof(System.Linq.Enumerable).GetMethod("ToArray").MakeGenericMethod( new System.Type[]{ elementType } );
            var castedObjectEnum = castMethod.Invoke(null, new object[] { obj });
            var castedObject = toArrayMethod.Invoke(null, new object[] { castedObjectEnum });
//            object[] oArray = ((List<object>)obj).ToArray();
//            SerializeArray(oArray);
            SerializeArray(castedObject);
        } else if (type.IsEnum) {
            SerializeString(obj.ToString());
        } else if (type == typeof(string)) {
            SerializeString(obj as string);
        } else if (type == typeof(Char)) {
            SerializeString(obj.ToString());
        } else if (type == typeof(bool)) {
            m_builder.Append((bool)obj ? "true" : "false");
        } else if (type == typeof(Boolean)) {
            m_builder.Append((Boolean)obj ? "true" : "false");
//        } else if (type.IsPrimitive) {
//            m_builder.Append(System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertToInvariantString(obj));

            m_builder.Append(Convert.ChangeType(obj, typeof(string)));
        } else if (type == typeof(int)) {
            m_builder.Append(obj);
        } else if (type == typeof(Byte)) {
            m_builder.Append(obj);
        } else if (type == typeof(SByte)) {
            m_builder.Append(obj);
        } else if (type == typeof(Int16)) {
            m_builder.Append(obj);
        } else if (type == typeof(UInt16)) {
            m_builder.Append(obj);
        } else if (type == typeof(Int32)) {
            m_builder.Append(obj);
        } else if (type == typeof(UInt32)) {
            m_builder.Append(obj);
        } else if (type == typeof(Int64)) {
            m_builder.Append(obj);
        } else if (type == typeof(UInt64)) {
            m_builder.Append(obj);
        } else if (type == typeof(Single)) {
            m_builder.Append(((Single)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        } else if (type == typeof(Double)) {
            m_builder.Append(((Double)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        } else if (type == typeof(float)) {
            m_builder.Append(((float)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        } else if (type == typeof(double)) {
            m_builder.Append(((double)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        } else if (type.IsValueType) {
            SerializeObject(obj);
        } else if (type.IsClass) {
            SerializeObject(obj);
        } else {
            throw new System.InvalidOperationException("unsupport type: " + type.Name);
        }
    }

    private void SerializeArray(object obj) {
        m_builder.Append("[");
        AddLine();
        Indent();
        Array array = obj as Array;
        bool first = true;
        foreach (object element in array) {
            if (!first) {
                m_builder.Append(",");
                AddLine();
            }
            AddIndent();
            SerializeValue(element);
            first = false;
        }
        AddLine();
        Outdent();
        AddIndent();
        m_builder.Append("]");
    }

    private void SerializeDictionary(IDictionary obj) {
        bool first = true;
        foreach (object key in obj.Keys) {
            if (!first) {
                m_builder.Append(',');
                AddLine();
            }

            AddIndent();
            SerializeString(key.ToString());
            m_builder.Append(':');
            AddSpace();

            SerializeValue(obj[key]);

            first = false;
        }
    }

    private void SerializeObject(object obj) {
        m_builder.Append("{");
        AddLine();
        Indent();
        bool first = true;
        if (m_includeTypeInfoForDerivedTypes) {
            // Only inlcude type info for derived types.
            System.Type type = obj.GetType();
            System.Type baseType = type.BaseType;
            if (baseType != null && baseType != typeof(System.Object)) {
                AddIndent();
                SerializeString("$dotNetType");  // assuming this won't clash with user's properties.
                m_builder.Append(":");
                AddSpace();
                SerializeString(type.AssemblyQualifiedName);
            }
        }

        IDictionary asDict;
        if ((asDict = obj as IDictionary) != null) {
            SerializeDictionary(asDict);
        } else {
            System.Reflection.FieldInfo[] fields = obj.GetType().GetFields(m_fieldFlags);
            foreach (System.Reflection.FieldInfo info in fields) {
                if (info.IsStatic) {
                    continue;
                }
                object fieldValue = info.GetValue(obj);
                if (fieldValue != null) {
                    if (!first) {
                        m_builder.Append(",");
                        AddLine();
                    }
                    AddIndent();
                    SerializeString(info.Name);
                    m_builder.Append(":");
                    AddSpace();
                    SerializeValue(fieldValue);
                    first = false;
                }
            }
        }
        AddLine();
        Outdent();
        AddIndent();
        m_builder.Append("}");
    }

    private void SerializeString(string str) {
        m_builder.Append('\"');

        char[] charArray = str.ToCharArray();
        foreach (var c in charArray) {
            switch (c) {
            case '"':
                m_builder.Append("\\\"");
                break;
            case '\\':
                m_builder.Append("\\\\");
                break;
            case '\b':
                m_builder.Append("\\b");
                break;
            case '\f':
                m_builder.Append("\\f");
                break;
            case '\n':
                m_builder.Append("\\n");
                break;
            case '\r':
                m_builder.Append("\\r");
                break;
            case '\t':
                m_builder.Append("\\t");
                break;
            default:
                int codepoint = Convert.ToInt32(c);
                if ((codepoint >= 32) && (codepoint <= 126)) {
                    m_builder.Append(c);
                } else {
                    m_builder.Append("\\u");
                    m_builder.Append(codepoint.ToString("x4"));
                }
                break;
            }
        }

        m_builder.Append('\"');
    }
}

