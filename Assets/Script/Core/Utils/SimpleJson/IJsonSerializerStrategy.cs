using System;
using System.CodeDom.Compiler;


[GeneratedCode("simple-json", "1.0.0")]
public interface IJsonSerializerStrategy
{
    bool TrySerializeNonPrimitiveObject(object input, out object output);

    object DeserializeObject(object value, Type type);
}
