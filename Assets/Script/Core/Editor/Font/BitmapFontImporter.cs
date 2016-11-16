#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

public static class BitmapFontImporter
{
	
	[MenuItem("Tool/Generate Bitmap Font")]
	public static void GenerateFont ()
	{
		TextAsset selected = (TextAsset)Selection.activeObject;
		string rootPath = Path.GetDirectoryName (AssetDatabase.GetAssetPath (selected));
		
		Texture2D texture = AssetDatabase.LoadAssetAtPath (rootPath + "/" + selected.name + ".png", typeof(Texture2D)) as Texture2D;
		if (!texture)
			throw new UnityException ("Texture2d asset doesn't exist for " + selected.name);
		
		string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension (selected.name);
		
		Work (selected, exportPath, texture);
	}
	
	
	private static void Work (TextAsset import, string exportPath, Texture2D texture)
	{
		if (!import)
			throw new UnityException (import.name + "is not a valid font-xml file");
		
		
		XmlDocument xml = new XmlDocument ();
		xml.LoadXml (import.text);
		
		XmlNode info = xml.GetElementsByTagName ("info") [0];
        XmlNode common = xml.GetElementsByTagName("common")[0];
		XmlNodeList chars = xml.GetElementsByTagName ("chars") [0].ChildNodes;
		
		float texW = texture.width;
		float texH = texture.height;
		
		CharacterInfo[] charInfos = new CharacterInfo[chars.Count];
		Rect r;
		
		for (int i=0; i<chars.Count; i++) {
			XmlNode charNode = chars [i];
			if (charNode.Attributes != null) {
				CharacterInfo charInfo = new CharacterInfo ();
			
				charInfo.index = (int)ToFloat (charNode, "id");
				charInfo.advance = (int)ToFloat (charNode, "xadvance");
			
				r = new Rect ();
				r.x = ((float)ToFloat (charNode, "x")) / texW;
				r.y = ((float)ToFloat (charNode, "y")) / texH;
				r.width = ((float)ToFloat (charNode, "width")) / texW;
				r.height = ((float)ToFloat (charNode, "height")) / texH;
				r.y = 1f - r.y - r.height;
				charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
				charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
				charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
				charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);
			
			
				r = new Rect ();
				r.x = (float)ToFloat (charNode, "xoffset");
				r.y = (float)ToFloat (charNode, "yoffset");
				r.width = (float)ToFloat (charNode, "width");
				r.height = (float)ToFloat (charNode, "height");
				r.y = -r.y;
				r.height = -r.height;
				charInfo.minX = (int)r.xMin;
				charInfo.maxX = (int)r.xMax;
				charInfo.minY = (int)r.yMax;
				charInfo.maxY = (int)r.yMin;
			
				charInfos [i] = charInfo;
			}
		}
		
		// Create material
		Shader shader = Shader.Find ("UI/Default");
		Material material = new Material (shader);
		material.mainTexture = texture;
		AssetDatabase.CreateAsset (material, exportPath + ".mat");
		
		// Create font
		Font font = new Font ();
		font.material = material;
		font.name = info.Attributes.GetNamedItem ("face").InnerText;
		font.characterInfo = charInfos;

        SerializedObject mFont = new SerializedObject(font);
        mFont.FindProperty("m_FontSize").floatValue = float.Parse(common.Attributes.GetNamedItem("base").InnerText);
        mFont.FindProperty("m_LineSpacing").floatValue = float.Parse(common.Attributes.GetNamedItem("lineHeight").InnerText);

        /* Don't work yet
        int kerningsCount = int.Parse(kernings.Attributes.GetNamedItem("count").InnerText);
        if (kerningsCount > 0)
        {
            SerializedProperty kerningsProp = mFont.FindProperty("m_KerningValues");
            for (int i = 0; i < kerningsCount; i++)
            {
                kerningsProp.InsertArrayElementAtIndex(i);

                XmlNode kerning = kernings.ChildNodes[i];

                SerializedProperty kern = kerningsProp.GetArrayElementAtIndex(i);

                kern.FindPropertyRelative("second").floatValue = float.Parse(kerning.Attributes.GetNamedItem("amount").InnerText); ;
            }
        }*/


        mFont.ApplyModifiedProperties();

		AssetDatabase.CreateAsset (font, exportPath + ".fontsettings");
	}
	
	private static float ToFloat (XmlNode node, string name)
	{
		return float.Parse (node.Attributes.GetNamedItem (name).InnerText);
	}
}
#endif