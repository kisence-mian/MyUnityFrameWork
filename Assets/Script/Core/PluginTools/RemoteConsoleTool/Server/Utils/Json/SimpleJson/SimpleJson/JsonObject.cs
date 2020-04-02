using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameConsoleController
{
	[GeneratedCode("simple-json", "1.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
	internal class JsonObject : IDictionary<object, object>, ICollection<KeyValuePair<object, object>>, IEnumerable<KeyValuePair<object, object>>, IEnumerable
	{
		private readonly Dictionary<object, object> _members;

		public object this[int index]
		{
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		public ICollection<object> Keys
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

		public object this[object key]
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
			this._members = new Dictionary<object, object>();
		}

		public JsonObject(IEqualityComparer<object> comparer)
		{
			this._members = new Dictionary<object, object>(comparer);
		}

		internal static object GetAtIndex(IDictionary<object, object> obj, int index)
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
			foreach (KeyValuePair<object, object> current in obj)
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

		public void Add(object key, object value)
		{
			this._members.Add(key, value);
		}

		public bool ContainsKey(object key)
		{
			return this._members.ContainsKey(key);
		}

		public bool Remove(object key)
		{
			return this._members.Remove(key);
		}

		public bool TryGetValue(object key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<object, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			this._members.Clear();
		}

		public bool Contains(KeyValuePair<object, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = this.Count;
			foreach (KeyValuePair<object, object> current in this)
			{
				array[arrayIndex++] = current;
				if (--num <= 0)
				{
					break;
				}
			}
		}

		public bool Remove(KeyValuePair<object, object> item)
		{
			return this._members.Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
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
}
