using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXList : ArrayList
	{
		public PBXList()
		{
			
		}
		
		public PBXList( object firstValue )
		{
			this.Add( firstValue );
		}
	
		/// <summary>
		/// This allows us to use the form:
		/// "if (x)" or "if (!x)"
		/// </summary>
		public static implicit operator bool( PBXList x ) {
			//if null or empty, treat us as false/null
			return (x == null) ? false : (x.Count == 0);
		}

		/// <summary>
		/// I find this handy. return our fields as comma-separated values
		/// </summary>
		public string ToCSV() {
		// TODO use a char sep argument to allow specifying separator
			string ret = string.Empty;
			foreach (string item in this) {
				ret += "\"";
				ret += item;
				ret += "\", ";
			}
			return ret;
		}

		/// <summary>
		/// Concatenate and format so appears as "{,,,}"
		/// </summary>
		public override string ToString() {
			return "{" + this.ToCSV() + "} ";
		}
	}
}
