using UnityEngine;
using System.Collections;

namespace UnityEditor.XCodeEditor
{
	public class XCBuildConfiguration : PBXObject
	{
		protected const string BUILDSETTINGS_KEY = "buildSettings";
		protected const string HEADER_SEARCH_PATHS_KEY = "HEADER_SEARCH_PATHS";
		protected const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";
		protected const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";
		protected const string OTHER_C_FLAGS_KEY = "OTHER_CFLAGS";
		protected const string OTHER_LDFLAGS_KEY = "OTHER_LDFLAGS";
		
		public XCBuildConfiguration( string guid, PBXDictionary dictionary ) : base( guid, dictionary )
		{
			
		}
		
		public PBXSortedDictionary buildSettings {
			get {
				if( ContainsKey( BUILDSETTINGS_KEY ) ) {
					if (_data[BUILDSETTINGS_KEY].GetType() == typeof(PBXDictionary)) {
						PBXSortedDictionary ret = new PBXSortedDictionary();
						ret.Append((PBXDictionary)_data[BUILDSETTINGS_KEY]);						
						return ret;
					}
					return (PBXSortedDictionary)_data[BUILDSETTINGS_KEY];	
				}
				return null;
			}
		}
		
		protected bool AddSearchPaths( string path, string key, bool recursive = true )
		{
			PBXList paths = new PBXList();
			paths.Add( path );
			return AddSearchPaths( paths, key, recursive );
		}
		
		protected bool AddSearchPaths( PBXList paths, string key, bool recursive = true, bool quoted = false ) //we want no quoting whenever we can get away with it
		{	
			//Debug.Log ("AddSearchPaths " + paths + key + (recursive?" recursive":"") + " " + (quoted?" quoted":""));
			bool modified = false;
			
			if( !ContainsKey( BUILDSETTINGS_KEY ) )
				this.Add( BUILDSETTINGS_KEY, new PBXSortedDictionary() );
			
			foreach( string path in paths ) {
				string currentPath = path;
				//Debug.Log ("path " + currentPath);
				if( !((PBXDictionary)_data[BUILDSETTINGS_KEY]).ContainsKey( key ) ) {
					((PBXDictionary)_data[BUILDSETTINGS_KEY]).Add( key, new PBXList() );
				}
				else if( ((PBXDictionary)_data[BUILDSETTINGS_KEY])[key] is string ) {
					PBXList list = new PBXList();
					list.Add( ((PBXDictionary)_data[BUILDSETTINGS_KEY])[key] );
					((PBXDictionary)_data[BUILDSETTINGS_KEY])[key] = list;
				}
				
				//Xcode uses space as the delimiter here, so if there's a space in the filename, we *must* quote. Escaping with slash may work when you are in the Xcode UI, in some situations, but it doesn't work here.
				if (currentPath.Contains(@" ")) quoted = true;
				
				if (quoted) {
					//if it ends in "/**", it wants to be recursive, and the "/**" needs to be _outside_ the quotes
					if (currentPath.EndsWith("/**")) {
						currentPath = "\\\"" + currentPath.Replace("/**", "\\\"/**");
					} else {
						currentPath = "\\\"" + currentPath + "\\\"";
					}
				}
				//Debug.Log ("currentPath = " + currentPath);
				if( !((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[key]).Contains( currentPath ) ) {
					((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[key]).Add( currentPath );
					modified = true;
				}
			}
		
			return modified;
		}
		
		public bool AddHeaderSearchPaths( PBXList paths, bool recursive = true )
		{
			return this.AddSearchPaths( paths, HEADER_SEARCH_PATHS_KEY, recursive );
		}
		
		public bool AddLibrarySearchPaths( PBXList paths, bool recursive = true )
		{
			Debug.Log ("AddLibrarySearchPaths " + paths);
			return this.AddSearchPaths( paths, LIBRARY_SEARCH_PATHS_KEY, recursive );
		}

		public bool AddFrameworkSearchPaths( PBXList paths, bool recursive = true )
		{
			return this.AddSearchPaths( paths, FRAMEWORK_SEARCH_PATHS_KEY, recursive );
		}
		
		public bool AddOtherCFlags( string flag )
		{
			//Debug.Log( "INIZIO 1" );
			PBXList flags = new PBXList();
			flags.Add( flag );
			return AddOtherCFlags( flags );
		}
		
		public bool AddOtherCFlags( PBXList flags )
		{
			//Debug.Log( "INIZIO 2" );
			
			bool modified = false;
			
			if( !ContainsKey( BUILDSETTINGS_KEY ) )
				this.Add( BUILDSETTINGS_KEY, new PBXSortedDictionary() );
			
			foreach( string flag in flags ) {
				
				if( !((PBXDictionary)_data[BUILDSETTINGS_KEY]).ContainsKey( OTHER_C_FLAGS_KEY ) ) {
					((PBXDictionary)_data[BUILDSETTINGS_KEY]).Add( OTHER_C_FLAGS_KEY, new PBXList() );
				}
				else if ( ((PBXDictionary)_data[BUILDSETTINGS_KEY])[ OTHER_C_FLAGS_KEY ] is string ) {
					string tempString = (string)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_C_FLAGS_KEY];
					((PBXDictionary)_data[BUILDSETTINGS_KEY])[ OTHER_C_FLAGS_KEY ] = new PBXList();
					((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_C_FLAGS_KEY]).Add( tempString );
				}
				
				if( !((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_C_FLAGS_KEY]).Contains( flag ) ) {
					((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_C_FLAGS_KEY]).Add( flag );
					modified = true;
				}
			}
			
			return modified;
		}

		public bool AddOtherLinkerFlags( string flag )
		{
			PBXList flags = new PBXList();
			flags.Add( flag );
			return AddOtherLinkerFlags( flags );
		}

		public bool AddOtherLinkerFlags( PBXList flags )
		{
			bool modified = false;

			if( !ContainsKey( BUILDSETTINGS_KEY ) )
				this.Add( BUILDSETTINGS_KEY, new PBXSortedDictionary() );

			foreach( string flag in flags ) {
				
				if( !((PBXDictionary)_data[BUILDSETTINGS_KEY]).ContainsKey( OTHER_LDFLAGS_KEY ) ) {
					((PBXDictionary)_data[BUILDSETTINGS_KEY]).Add( OTHER_LDFLAGS_KEY, new PBXList() );
				}
				else if ( ((PBXDictionary)_data[BUILDSETTINGS_KEY])[ OTHER_LDFLAGS_KEY ] is string ) {
					string tempString = (string)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_LDFLAGS_KEY];
					((PBXDictionary)_data[BUILDSETTINGS_KEY])[ OTHER_LDFLAGS_KEY ] = new PBXList();
					if( !string.IsNullOrEmpty(tempString) ) {
						((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_LDFLAGS_KEY]).Add( tempString );
					}
				}
				
				if( !((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_LDFLAGS_KEY]).Contains( flag ) ) {
					((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[OTHER_LDFLAGS_KEY]).Add( flag );
					modified = true;
				}
			}

			return modified;
		}
		
		public bool overwriteBuildSetting(string settingName, string settingValue) {
			Debug.Log ("overwriteBuildSetting " + settingName + " " + settingValue);
			bool modified = false;
			
			if( !ContainsKey( BUILDSETTINGS_KEY ) ) {
				Debug.Log ("creating key " + BUILDSETTINGS_KEY);
				this.Add( BUILDSETTINGS_KEY, new PBXSortedDictionary() );
			}
				
			if( !((PBXDictionary)_data[BUILDSETTINGS_KEY]).ContainsKey( settingName ) ) {
				Debug.Log("adding key " + settingName);
				 ((PBXDictionary)_data[BUILDSETTINGS_KEY]).Add( settingName, new PBXList() );
			}
			else if ( ((PBXDictionary)_data[BUILDSETTINGS_KEY])[ settingName ] is string ) {
				//Debug.Log("key is string:" + settingName);
				//string tempString = (string)((PBXDictionary)_data[BUILDSETTINGS_KEY])[settingName];
				((PBXDictionary)_data[BUILDSETTINGS_KEY])[ settingName ] = new PBXList();
				//((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[settingName]).Add( tempString );
			}
			
			if( !((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[settingName]).Contains( settingValue ) ) {
				Debug.Log("setting " + settingName + " to " + settingValue);
				((PBXList)((PBXDictionary)_data[BUILDSETTINGS_KEY])[settingName]).Add( settingValue );
				modified = true;
				}
			
			return modified;
		}			
	}
}