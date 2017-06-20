using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXDictionary : Dictionary<string, object>
	{
		
		public void Append( PBXDictionary dictionary )
		{
			foreach( var item in dictionary) {
				this.Add( item.Key, item.Value );
			}
		}
		
		public void Append<T>( PBXDictionary<T> dictionary ) where T : PBXObject
		{
			foreach( var item in dictionary) {
				this.Add( item.Key, item.Value );
			}
		}

		public void Append( PBXSortedDictionary dictionary )
		{
			foreach( var item in dictionary) {
				this.Add( item.Key, item.Value );
			}
		}

		public void Append<T>( PBXSortedDictionary<T> dictionary ) where T : PBXObject
		{
			foreach( var item in dictionary) {
				this.Add( item.Key, item.Value );
			}
		}

		/// <summary>
		/// This allows us to use the form:
		/// "if (x)" or "if (!x)"
		/// </summary>
		public static implicit operator bool( PBXDictionary x ) {
			//if null or empty, treat us as false/null
			return (x == null) ? false : (x.Count == 0);
		}

		/// <summary>
		/// I find this handy. return our fields as comma-separated values
		/// </summary>
		public string ToCSV() {
		// TODO use a char sep argument to allow specifying separator
			string ret = string.Empty;
			foreach (KeyValuePair<string, object> item in this) {
				ret += "<";
				ret += item.Key;
				ret += ", ";
				ret += item.Value;
				ret += ">, ";
			}
			return ret;
		}

		/// <summary>
		/// Concatenate and format so appears as "{,,,}"
		/// </summary>
		public override string ToString() {
			return "{" + this.ToCSV() + "}";
		}
		
	}

	public class PBXDictionary<T> : Dictionary<string, T> where T : PBXObject
	{
		public PBXDictionary()
		{
			
		}
		
		public PBXDictionary( PBXDictionary genericDictionary )
		{
			foreach( KeyValuePair<string, object> currentItem in genericDictionary ) {
				if( ((string)((PBXDictionary)currentItem.Value)[ "isa" ]).CompareTo( typeof(T).Name ) == 0 ) {
					T instance = (T)System.Activator.CreateInstance( typeof(T), currentItem.Key, (PBXDictionary)currentItem.Value );
					this.Add( currentItem.Key, instance );
				}
			}	
		}
		
		public PBXDictionary( PBXSortedDictionary genericDictionary )
		{
			foreach( KeyValuePair<string, object> currentItem in genericDictionary ) {
				if( ((string)((PBXDictionary)currentItem.Value)[ "isa" ]).CompareTo( typeof(T).Name ) == 0 ) {
					T instance = (T)System.Activator.CreateInstance( typeof(T), currentItem.Key, (PBXDictionary)currentItem.Value );
					this.Add( currentItem.Key, instance );
				}
			}	
		}
		
		public void Add( T newObject )
		{
			this.Add( newObject.guid, newObject );
		}
		
		public void Append( PBXDictionary<T> dictionary )
		{
			foreach( KeyValuePair<string, T> item in dictionary) {
				this.Add( item.Key, (T)item.Value );
			}
		}
		
	}
}
