using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace GameConsoleController
{
	[GeneratedCode("simple-json", "1.0.0")]
	public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		internal IDictionary<Type, ReflectionsUtils.ConstructorDelegate> ConstructorCache;

		internal IDictionary<Type, IDictionary<string, ReflectionsUtils.GetDelegate>> GetCache;

		internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>>> SetCache;

		internal static readonly Type[] EmptyTypes = new Type[0];

		internal static readonly Type[] ArrayConstructorParameterTypes = new Type[]
		{
			typeof(int)
		};

		private static readonly string[] Iso8601Format = new string[]
		{
			"yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z",
			"yyyy-MM-dd\\THH:mm:ss\\Z",
			"yyyy-MM-dd\\THH:mm:ssK"
		};

		public PocoJsonSerializerStrategy()
		{
			this.ConstructorCache = new ReflectionsUtils.ThreadSafeDictionary<Type, ReflectionsUtils.ConstructorDelegate>(new ReflectionsUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionsUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
			this.GetCache = new ReflectionsUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionsUtils.GetDelegate>>(new ReflectionsUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, ReflectionsUtils.GetDelegate>>(this.GetterValueFactory));
			this.SetCache = new ReflectionsUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>>>(new ReflectionsUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>>>(this.SetterValueFactory));
		}

		protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
		{
			return clrPropertyName;
		}

		internal virtual ReflectionsUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
		{
			return ReflectionsUtils.GetContructor(key, (!key.IsArray) ? PocoJsonSerializerStrategy.EmptyTypes : PocoJsonSerializerStrategy.ArrayConstructorParameterTypes);
		}

		internal virtual IDictionary<string, ReflectionsUtils.GetDelegate> GetterValueFactory(Type type)
		{
			IDictionary<string, ReflectionsUtils.GetDelegate> dictionary = new Dictionary<string, ReflectionsUtils.GetDelegate>();
			foreach (PropertyInfo current in ReflectionsUtils.GetProperties(type))
			{
				if (current.CanRead&& current.CanWrite)
				{
					MethodInfo getterMethodInfo = ReflectionsUtils.GetGetterMethodInfo(current);
					if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
					{
						dictionary[this.MapClrMemberNameToJsonFieldName(current.Name)] = ReflectionsUtils.GetGetMethod(current);
					}
				}
			}
			foreach (FieldInfo current2 in ReflectionsUtils.GetFields(type))
			{
				if (!current2.IsStatic && current2.IsPublic)
				{
					dictionary[this.MapClrMemberNameToJsonFieldName(current2.Name)] = ReflectionsUtils.GetGetMethod(current2);
				}
			}
			return dictionary;
		}

		internal virtual IDictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>> SetterValueFactory(Type type)
		{
			IDictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>>();
			foreach (PropertyInfo current in ReflectionsUtils.GetProperties(type))
			{
				if (current.CanWrite)
				{
					MethodInfo setterMethodInfo = ReflectionsUtils.GetSetterMethodInfo(current);
					if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
					{
						dictionary[this.MapClrMemberNameToJsonFieldName(current.Name)] = new KeyValuePair<Type, ReflectionsUtils.SetDelegate>(current.PropertyType, ReflectionsUtils.GetSetMethod(current));
					}
				}
			}
			foreach (FieldInfo current2 in ReflectionsUtils.GetFields(type))
			{
				if (!current2.IsInitOnly && !current2.IsStatic && current2.IsPublic)
				{
					dictionary[this.MapClrMemberNameToJsonFieldName(current2.Name)] = new KeyValuePair<Type, ReflectionsUtils.SetDelegate>(current2.FieldType, ReflectionsUtils.GetSetMethod(current2));
				}
			}
			return dictionary;
		}

		public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
		{
			return this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output);
		}

		public virtual object DeserializeObject(object value, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			string text = value as string;
			object result;
			if (type == typeof(Guid) && string.IsNullOrEmpty(text))
			{
				result = default(Guid);
			}
			else if (value == null)
			{
				result = null;
			}
			else
			{
				object obj = null;
				if (text != null)
				{
					if (text.Length != 0)
					{
                        if (type.IsEnum)
                        {
                            return Enum.Parse(type, text);
                        }
                        if (type == typeof(DateTime) || (ReflectionsUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
						{
                            result = DateTime.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

                            return result;
						}
						if (type == typeof(DateTimeOffset) || (ReflectionsUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset)))
						{
							result = DateTimeOffset.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
							return result;
						}
						if (type == typeof(Guid) || (ReflectionsUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
						{
							result = new Guid(text);
							return result;
						}
                        if (type == typeof(Uri))
                        {
                            result = new Uri(text);
                            return result;
                        }
						result = text;
						return result;
					}
					else
					{
                       
						 if(type == typeof(Guid))
						{
							obj = default(Guid);
						}
						else if (ReflectionsUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
						{
							obj = null;
						}
						else
						{
							obj = text;
						}
						if (!ReflectionsUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
						{
							result = text;
							return result;
						}
					}
				}
				else if (value is bool)
				{
					result = value;
					return result;
				}
				bool flag = value is long;
				bool flag2 = value is double;
				if ((flag && type == typeof(long)) || (flag2 && type == typeof(double)))
				{
					result = value;
				}
				else if ((flag2 && type != typeof(double)) || (flag && type != typeof(long)))
				{
					obj = ((!typeof(IConvertible).IsAssignableFrom(type)) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
					if (ReflectionsUtils.IsNullableType(type))
					{
						result = ReflectionsUtils.ToNullableType(obj, type);
					}
					else
					{
						result = obj;
					}
				}
				else
				{
					IDictionary<string, object> dictionary = value as IDictionary<string, object>;
					if (dictionary != null)
					{
						IDictionary<string, object> dictionary2 = dictionary;
						if (ReflectionsUtils.IsTypeDictionary(type))
						{
							Type[] genericTypeArguments = ReflectionsUtils.GetGenericTypeArguments(type);
							Type type2 = genericTypeArguments[0];
							Type type3 = genericTypeArguments[1];
							Type key = typeof(Dictionary<, >).MakeGenericType(new Type[]
							{
								type2,
								type3
							});
							IDictionary dictionary3 = (IDictionary)this.ConstructorCache[key](null);
							foreach (KeyValuePair<string, object> current in dictionary2)
							{
								dictionary3.Add(current.Key, this.DeserializeObject(current.Value, type3));
							}
							obj = dictionary3;
						}
						else if (type == typeof(object))
						{
							obj = value;
						}
						else
						{
							obj = this.ConstructorCache[type](null);
							foreach (KeyValuePair<string, KeyValuePair<Type, ReflectionsUtils.SetDelegate>> current2 in this.SetCache[type])
							{
								object value2;
								if (dictionary2.TryGetValue(current2.Key, out value2))
								{
									value2 = this.DeserializeObject(value2, current2.Value.Key);
									current2.Value.Value(obj, value2);
								}
							}
						}
					}
					else
					{
						IList<object> list = value as IList<object>;
						if (list != null)
						{
							IList<object> list2 = list;
							IList list3 = null;
							if (type.IsArray)
							{
								list3 = (IList)this.ConstructorCache[type](new object[]
								{
									list2.Count
								});
								int num = 0;
								foreach (object current3 in list2)
								{
									list3[num++] = this.DeserializeObject(current3, type.GetElementType());
								}
							}
							else if (ReflectionsUtils.IsTypeGenericeCollectionInterface(type) || ReflectionsUtils.IsAssignableFrom(typeof(IList), type))
							{
								Type type4 = ReflectionsUtils.GetGenericTypeArguments(type)[0];
								Type key2 = typeof(List<>).MakeGenericType(new Type[]
								{
									type4
								});
								list3 = (IList)this.ConstructorCache[key2](new object[]
								{
									list2.Count
								});
								foreach (object current4 in list2)
								{
									list3.Add(this.DeserializeObject(current4, type4));
								}
							}
							obj = list3;
						}
					}
					result = obj;
				}
			}
			return result;
		}

		protected virtual object SerializeEnum(Enum p)
		{
            //原版枚举转换成Int保存，改成保存字符串
            //return Convert.ToDouble(p, CultureInfo.InvariantCulture);
            return Convert.ToString(p, CultureInfo.InvariantCulture);
        }

		protected virtual bool TrySerializeKnownTypes(object input, out object output)
		{
			bool result = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is DateTimeOffset)
			{
				output = ((DateTimeOffset)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else
			{
				Enum @enum = input as Enum;
				if (@enum != null)
				{
					output = this.SerializeEnum(@enum);
				}
				else
				{
					result = false;
					output = null;
				}
			}
			return result;
		}

		protected virtual bool TrySerializeUnknownTypes(object input, out object output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			output = null;
			Type type = input.GetType();
			bool result;
			if (type.FullName == null)
			{
				result = false;
			}
			else
			{
                IDictionary<string, object> dictionary = new Dictionary<string, object>();
				IDictionary<string, ReflectionsUtils.GetDelegate> dictionary2 = this.GetCache[type];
				foreach (KeyValuePair<string, ReflectionsUtils.GetDelegate> current in dictionary2)
				{
					if (current.Value != null)
					{
						dictionary.Add(this.MapClrMemberNameToJsonFieldName(current.Key), current.Value(input));
					}
				}
				output = dictionary;
				result = true;
			}
			return result;
		}
	}
}
