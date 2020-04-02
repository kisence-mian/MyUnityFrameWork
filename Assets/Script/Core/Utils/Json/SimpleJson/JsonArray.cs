using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;


[GeneratedCode("simple-json", "1.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
internal class JsonArray : List<object>
{
    public JsonArray()
    {
    }

    public JsonArray(int capacity) : base(capacity)
    {
    }

    public override string ToString()
    {
        return SimpleJsonTool.SerializeObject(this) ?? string.Empty;
    }
}
