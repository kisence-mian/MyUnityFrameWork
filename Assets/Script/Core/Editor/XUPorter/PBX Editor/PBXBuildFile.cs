using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXBuildFile : PBXObject
	{
		private const string FILE_REF_KEY = "fileRef";
		private const string SETTINGS_KEY = "settings";
		private const string ATTRIBUTES_KEY = "ATTRIBUTES";
		private const string WEAK_VALUE = "Weak";
		private const string COMPILER_FLAGS_KEY = "COMPILER_FLAGS";
		
		public PBXBuildFile( PBXFileReference fileRef, bool weak = false ) : base()
		{
			this.Add( FILE_REF_KEY, fileRef.guid );
			SetWeakLink( weak );
			
			if (!string.IsNullOrEmpty(fileRef.compilerFlags))
			{
				foreach (var flag in fileRef.compilerFlags.Split(','))
					AddCompilerFlag(flag);
			}
		}
		
		public PBXBuildFile( string guid, PBXDictionary dictionary ) : base ( guid, dictionary )
		{
		}

		public string fileRef
		{
			get {
				return (string)_data[ FILE_REF_KEY ];
			}
		}
		
		public bool SetWeakLink( bool weak = false )
		{
			PBXDictionary settings = null;
			PBXList attributes = null;
			
			if( !_data.ContainsKey( SETTINGS_KEY ) ) {
				if( weak ) {
					attributes = new PBXList();
					attributes.Add( WEAK_VALUE );
					
					settings = new PBXDictionary();
					settings.Add( ATTRIBUTES_KEY, attributes );

					_data.Add( SETTINGS_KEY, settings );
				}
				return true;
			}
			
			settings = _data[ SETTINGS_KEY ] as PBXDictionary;
			if( !settings.ContainsKey( ATTRIBUTES_KEY ) ) {
				if( weak ) {
					attributes = new PBXList();
					attributes.Add( WEAK_VALUE );
					settings.Add( ATTRIBUTES_KEY, attributes );
					return true;
				}
				else {
					return false;
				}
			}
			else {
				attributes = settings[ ATTRIBUTES_KEY ] as PBXList;
			}
			
			if( weak ) {
				attributes.Add( WEAK_VALUE );
			}
			else {
				attributes.Remove( WEAK_VALUE );
			}
			
			settings.Add( ATTRIBUTES_KEY, attributes );
			this.Add( SETTINGS_KEY, settings );
			
			return true;
		}

		//CodeSignOnCopy
		public bool AddCodeSignOnCopy()
		{
			if( !_data.ContainsKey( SETTINGS_KEY ) )
				_data[ SETTINGS_KEY ] = new PBXDictionary();

			var settings = _data[ SETTINGS_KEY ] as PBXDictionary;
			if( !settings.ContainsKey( ATTRIBUTES_KEY ) ) {
				var attributes = new PBXList();
				attributes.Add( "CodeSignOnCopy" );
				attributes.Add( "RemoveHeadersOnCopy" );
				settings.Add( ATTRIBUTES_KEY, attributes );
			}
			else {
				var attributes = settings[ ATTRIBUTES_KEY ] as PBXList;
				attributes.Add( "CodeSignOnCopy" );
				attributes.Add( "RemoveHeadersOnCopy" );
			}
			return true;		
		}

		
		public bool AddCompilerFlag( string flag )
		{
			if( !_data.ContainsKey( SETTINGS_KEY ) )
				_data[ SETTINGS_KEY ] = new PBXDictionary();
			
			if( !((PBXDictionary)_data[ SETTINGS_KEY ]).ContainsKey( COMPILER_FLAGS_KEY ) ) {
				((PBXDictionary)_data[ SETTINGS_KEY ]).Add( COMPILER_FLAGS_KEY, flag );
				return true;
			}
			
			string[] flags = ((string)((PBXDictionary)_data[ SETTINGS_KEY ])[ COMPILER_FLAGS_KEY ]).Split( ' ' );
			foreach( string item in flags ) {
				if( item.CompareTo( flag ) == 0 )
					return false;
			}
			
			((PBXDictionary)_data[ SETTINGS_KEY ])[ COMPILER_FLAGS_KEY ] = ( string.Join( " ", flags ) + " " + flag );
			return true;
		}
		
	}
}
