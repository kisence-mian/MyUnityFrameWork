using UnityEngine;
using System.Collections;
using System.IO;

namespace UnityEditor.XCodeEditor 
{
	public class XCMod 
	{
		private Hashtable _datastore = new Hashtable();
		private ArrayList _libs = null;
		
		public string name { get; private set; }
		public string path { get; private set; }
		
		public string group {
			get {
				if (_datastore != null && _datastore.Contains("group"))
					return (string)_datastore["group"];
				return string.Empty;
			}
		}
		
		public ArrayList patches {
			get {
				return (ArrayList)_datastore["patches"];
			}
		}
		
		public ArrayList libs {
			get {
				if( _libs == null ) {
					_libs = new ArrayList( ((ArrayList)_datastore["libs"]).Count );
					foreach( string fileRef in (ArrayList)_datastore["libs"] ) {
						Debug.Log("Adding to Libs: "+fileRef);
						_libs.Add( new XCModFile( fileRef ) );
					}
				}
				return _libs;
			}
		}
		
		public ArrayList frameworks {
			get {
				return (ArrayList)_datastore["frameworks"];
			}
		}
		
		public ArrayList headerpaths {
			get {
				return (ArrayList)_datastore["headerpaths"];
			}
		}
		
		public ArrayList files {
			get {
				return (ArrayList)_datastore["files"];
			}
		}
		
		public ArrayList folders {
			get {
				return (ArrayList)_datastore["folders"];
			}
		}
		
		public ArrayList excludes {
			get {
				return (ArrayList)_datastore["excludes"];
			}
		}

		public ArrayList compiler_flags {
			get {
				return (ArrayList)_datastore["compiler_flags"];
			}
		}

		public ArrayList linker_flags {
			get {
				return (ArrayList)_datastore["linker_flags"];
			}
		}

		public ArrayList embed_binaries {
			get {
				return (ArrayList)_datastore["embed_binaries"];
			}
		}

		public Hashtable plist {
			get {
				return (Hashtable)_datastore["plist"];
			}
		}
		
		public XCMod( string filename )
		{	
			FileInfo projectFileInfo = new FileInfo( filename );
			if( !projectFileInfo.Exists ) {
				Debug.LogWarning( "File does not exist." );
			}
			
			name = System.IO.Path.GetFileNameWithoutExtension( filename );
			path = System.IO.Path.GetDirectoryName( filename );
			
			string contents = projectFileInfo.OpenText().ReadToEnd();
			Debug.Log (contents);
			_datastore = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode( contents );
			if (_datastore == null || _datastore.Count == 0) {
				Debug.Log (contents);
				throw new UnityException("Parse error in file " + System.IO.Path.GetFileName(filename) + "! Check for typos such as unbalanced quotation marks, etc.");
			}
		}
	}

	public class XCModFile
	{
		public string filePath { get; private set; }
		public bool isWeak { get; private set; }
		
		public XCModFile( string inputString )
		{
			isWeak = false;
			
			if( inputString.Contains( ":" ) ) {
				string[] parts = inputString.Split( ':' );
				filePath = parts[0];
				isWeak = ( parts[1].CompareTo( "weak" ) == 0 );	
			}
			else {
				filePath = inputString;
			}
		}
	}
}
