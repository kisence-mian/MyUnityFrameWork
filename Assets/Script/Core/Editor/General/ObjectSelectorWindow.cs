
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 资源选择窗口
/// </summary>
public class ObjectSelectorWindow : EditorWindow
{
    private class Styles
    {
        public GUIStyle smallStatus = "ObjectPickerSmallStatus";

        public GUIStyle largeStatus = "ObjectPickerLargeStatus";

        public GUIStyle toolbarBack = "ObjectPickerToolbar";

        public GUIStyle tab = "ObjectPickerTab";

        public GUIStyle bottomResize = "WindowBottomResize";

        public GUIStyle previewBackground = "PopupCurveSwatchBackground";

        public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";
    }

    private Styles m_Styles ;

    private float m_ToolbarHeight = 20f;
    private const float minGridSize=40;
    private const float maxGridSize=120;
    private float selectAreaSizeHeight = 120;

    private static ObjectSelectorWindow win;
    private List<string> allFilePath = new List<string>();
    //private string assetName;
    CallBack<string, UnityEngine.Object> selectFileCallBack;
    private string searchName;
    private ObjectAssets selectAsset;

    private EditorWindow otherWindow;
    /// <summary>
    /// 显示资源选择窗口
    /// </summary>
    /// <param name="otherWindow">选择了资源后需要刷新的界面</param>
    /// <param name="assetName">当前选择的资源名字</param>
    /// <param name="paths">选择当前文件夹下资源(包含子文件夹)</param>
    /// <param name="assetType">资源类型</param>
    /// <param name="selectFileCallBack">选择后的回调</param>
    public static void Show(EditorWindow otherWindow, string assetName,string[] paths,Type assetType,  CallBack<string, UnityEngine.Object> selectFileCallBack)
    {
        if (win == null)
            win = GetWindow<ObjectSelectorWindow>();
        win.titleContent = new GUIContent("Select "+assetType.Name);
        win.Init(otherWindow, assetName, paths,assetType, selectFileCallBack);
        
    }

    private void Init(EditorWindow otherWindow, string assetName, string[] paths, Type assetType, CallBack<string, UnityEngine.Object> selectFileCallBack)
    {
        this.otherWindow = otherWindow;
        //this.assetName = assetName;
        this.selectFileCallBack = selectFileCallBack;

        allFilePath.Clear();

        allFilePath.AddRange(paths);

        objectAssets.Clear();

      string[] tempGIDs=  AssetDatabase.FindAssets("t:" + assetType.Name, allFilePath.ToArray());

        foreach (var id in tempGIDs)
        {
            string p = AssetDatabase.GUIDToAssetPath(id);
            UnityEngine.Object obj= AssetDatabase.LoadAssetAtPath(p, assetType);
            ObjectAssets oa = new ObjectAssets();
            oa.name = Path.GetFileNameWithoutExtension(p);
            oa.type = assetType;
            oa.assetObject = obj;
            oa.path = p;
            oa.previewEditor = Editor.CreateEditor(obj);
            oa.previewIcon = oa.previewEditor.RenderStaticPreview(p, null, (int)maxGridSize, (int)maxGridSize); //AssetPreview.GetAssetPreview(obj);
            oa.miniThumbnailIcon = AssetPreview.GetMiniThumbnail(obj);
            
            if (oa.previewIcon == null)
            {
                Debug.Log("Path ;" + p);
                oa.previewIcon = new Texture2D((int)maxGridSize, (int)maxGridSize);

                for (int i = 0; i < oa.previewIcon.width; i++)
                {
                    for (int j = 0; j < oa.previewIcon.height; j++)
                    {
                        oa.previewIcon.SetPixel(i, j, Color.black);
                    }
                }
                oa.previewIcon.Apply();

            }
            int w = (int)position.width / 5;
            TextureScale.Bilinear(oa.previewIcon, w, w);


            objectAssets.Add(oa);

            if (oa.name == assetName)
                selectAsset = oa;
        }

    }

    private List<ObjectAssets> objectAssets = new List<ObjectAssets>();
    private void OnGUI()
    {
        if (m_Styles == null)
            m_Styles = new Styles();

        

        if (position.width < maxGridSize + 40)
            this.minSize = new Vector2(maxGridSize + 40, 100);
        SearchArea();

        List<ObjectAssets> tempAssets = GetSearchResult(searchName);

        DrawGridIcon(tempAssets);

        DrawSelectItem();
    }

    private void DrawSelectItem()
    {
        GUILayout.BeginArea(new Rect(0,position.height- selectAreaSizeHeight,position.width,selectAreaSizeHeight), m_Styles. previewTextureBackground);
        if (selectAsset != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(selectAsset.previewIcon,GUILayout.Width(selectAreaSizeHeight-5),GUILayout.Height(selectAreaSizeHeight-5));

            GUILayout.BeginVertical();
            GUILayout.Label(selectAsset.name, m_Styles.largeStatus);
            GUILayout.Label(selectAsset.type.Name, m_Styles.smallStatus);
            GUILayout.Label(selectAsset.path, m_Styles.smallStatus);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorDrawGUIUtil.DrawHorizontalCenter(() =>
            {
                if (GUILayout.Button("Ok",GUILayout.Width(70),GUILayout.Height(45)))
                {
                    if (selectFileCallBack != null)
                        selectFileCallBack(selectAsset .name, selectAsset.assetObject);

                    if (otherWindow != null)
                        otherWindow.Repaint();
                    Close();
                }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndArea();
    }

    private float gridSize= maxGridSize;
    private void SearchArea()
    {
        searchName = EditorDrawGUIUtil.DrawSearchField(searchName);
         
        GUILayout.BeginArea(new Rect(0f, 20f, base.position.width, m_ToolbarHeight));
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
         gridSize = GUILayout.HorizontalSlider(gridSize,  minGridSize,  maxGridSize,GUILayout.Width(60));
        GUILayout.Space(4);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();


           
        
    }
    private Vector2 space = new Vector2(10, 10);
    private void DrawGridIcon(List<ObjectAssets> tempAssets)
    {
        GUILayout.BeginArea(new Rect(0, 44, position.width, position.height - 24f- m_ToolbarHeight - selectAreaSizeHeight));

        if (gridSize != minGridSize)
        {
            int rowCount = (int)(position.width / (gridSize + space.x));
            EditorDrawGUIUtil.DrawGrid(tempAssets, rowCount, space, (item) =>
             {

                 GUIContent content = new GUIContent(item.name, item.previewIcon);
                 GUIStyle style = "TL SelectionButton";
                 if (item == selectAsset)
                 {
                     style = "TL SelectionButton PreDropGlow";
                 }
                 style.imagePosition = ImagePosition.ImageAbove;
                 style.stretchHeight = true;
                 style.stretchWidth = true;
                 style.alignment = TextAnchor.LowerCenter;
                 if (GUILayout.Button(content, style, GUILayout.Width(gridSize), GUILayout.Height(gridSize + 10)))
                 {
                     selectAsset = item;
                 }

             });
        }
        else
        {
            EditorDrawGUIUtil.DrawScrollView(tempAssets, () =>
             {
                 foreach (var item in tempAssets)
                 {
                     GUIContent content = new GUIContent(item.name, item.miniThumbnailIcon);
                     GUIStyle style = "TL SelectionButton";
                     if (item == selectAsset)
                     {
                         style = "TL SelectionButton PreDropGlow";
                     }
                     style.imagePosition = ImagePosition.ImageLeft;
                     style.alignment = TextAnchor.MiddleLeft;
                     //style.stretchHeight = true;
                     //style.stretchWidth = true;

                     if (GUILayout.Button(content, style, GUILayout.Height(gridSize / 3 * 2)))
                     {
                         selectAsset = item;
                     }
                 }
             });
           
        }
        GUILayout.EndArea();
        
    }

    List<ObjectAssets> tempGetAssets = new List<ObjectAssets>();


    private List<ObjectAssets> GetSearchResult(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return objectAssets;
        }
        else
        {
            tempGetAssets.Clear();

            foreach (var o in objectAssets)
            {
                if (o.name.Contains(name))
                    tempGetAssets.Add(o);
            }
            return tempGetAssets;
        }

         
    }

    private class ObjectAssets
    {
        public string name;
        public Type type;
        public UnityEngine.Object assetObject;
        public Texture2D previewIcon;
        public Texture2D miniThumbnailIcon;
        public string path;
        public Editor previewEditor;
    }
}
