using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;


[GeneratedCode("simple-json", "1.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
internal class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
{
    private readonly Dictionary<string, object> _members;

    public object this[int index]
    {
        get
        {
            return JsonObject.GetAtIndex(this._members, index);
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return this._members.Keys;
        }
    }

    public ICollection<object> Values
    {
        get
        {
            return this._members.Values;
        }
    }

    public object this[string key]
    {
        get
        {
            return this._members[key];
        }
        set
        {
            this._members[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return this._members.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public JsonObject()
    {
        this._members = new Dictionary<string, object>();
    }

    public JsonObject(IEqualityComparer<string> comparer)
    {
        this._members = new Dictionary<string, object>(comparer);
    }

    internal static object GetAtIndex(IDictionary<string, object> obj, int index)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("obj");
        }
        if (index >= obj.Count)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        int num = 0;
        object result;
        foreach (KeyValuePair<string, object> current in obj)
        {
            if (num++ == index)
            {
                result = current.Value;
                return result;
            }
        }
        result = null;
        return result;
    }

    public void Add(string key, object value)
    {
        this._members.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return this._members.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return this._members.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return this._members.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<string, object> item)
    {
        this._members.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        this._members.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException("array");
        }
        int num = this.Count;
        foreach (KeyValuePair<string, object> current in this)
        {
            array[arrayIndex++] = current;
            if (--num <= 0)
            {
                break;
            }
        }
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return this._members.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return this._members.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._members.GetEnumerator();
    }

    public override string ToString()
    {
        return SimpleJsonTool.SerializeObject(this);
    }
}
