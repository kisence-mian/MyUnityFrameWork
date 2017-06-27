using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

public class UnityInternalIconWindow : EditorWindow
{
    [MenuItem("Window/Unity内建图标查看器", priority = 1102)]
    static void Open()
    {
        GetWindow<UnityInternalIconWindow>();
    }

    void OnEnable()
    {
        Init();
    }
    GUIContent[] findTextureIcons;
    GUIContent[] iconContentIcons;
    GUIContent[] loadIconIcons;
    GUIContent[] internalWindowIcons;
    GUIContent[] allIcons;
    private void Init()
    {
        findTextureIcons = GetIconContent("FindTexture获取");
        iconContentIcons = GetIconContent("IconContent获取");
        loadIconIcons = GetIconContent("LoadIcon获取");
        internalWindowIcons = GetIconContent("内置窗口图标");
        List<GUIContent> list = new List<GUIContent>();
        list.AddRange(findTextureIcons);
        list.AddRange(iconContentIcons);
        list.AddRange(loadIconIcons);
        list.AddRange(internalWindowIcons);
        allIcons = list.ToArray();
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "全部内置图标", "搜索" };

    private int toolbarOptionSec = 0;
    private string[] toolbarTextsSec = { "传递给 EditorGUIUtility.FindTexture 的参数", "IconContent获取的", "传递给 EditorGUIUtility.LoadIcon 的参数" , "添加EditorWindowTitleAttribute 特性的窗口的图标" };
    Vector2 scrollPosition = new Vector2(0, 0);
    string search = "";
    void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                toolbarOptionSec = GUILayout.Toolbar(toolbarOptionSec, toolbarTextsSec, GUILayout.Width(Screen.width - 40));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                switch (toolbarOptionSec)
                {
                    case 0:
                        GUILayout.Space(10);
                        foreach (GUIContent content in findTextureIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                    case 1:
                        GUILayout.Space(10);
                        foreach (GUIContent content in iconContentIcons)
                        {
                            ShowStyleGUI(content);
                        }

                        GUILayout.FlexibleSpace();
                        break;
                    case 2:
                        GUILayout.Space(10);
                        foreach (GUIContent content in loadIconIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                    case 3:
                        GUILayout.Space(10);
                        foreach (GUIContent content in internalWindowIcons)
                        {
                            ShowStyleGUI(content);
                        }
                        break;
                }
                break;
            case 1:
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label("Click a right Button to copy its Name to your Clipboard", "MiniBoldLabel");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Search:");
                search = EditorGUILayout.TextField(search);

                GUILayout.EndHorizontal();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                foreach (GUIContent content in allIcons)
                {

                    if (content.text.ToLower().Contains(search.ToLower()))
                    {
                        ShowStyleGUI(content);
                    }
                }
                break;


        }
        GUILayout.EndScrollView();
    }
    void ShowStyleGUI( GUIContent content)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Space(40);
        GUILayout.Label(content);
        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(content.text);
        GUILayout.Space(6);
        if (GUILayout.Button("复制到剪贴板"))
        {
           // EditorGUIUtility.systemCopyBuffer = style.text;
            TextEditor tx = new TextEditor();
            tx.text = content.text;
            tx.OnFocus();
            tx.Copy();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(11);
    }

    GUIContent[] GetIconContent(string fileName)
    {
        string[] ss = TextLoad(fileName);
        return GetGUIContent(ss);
    }
    private string[] TextLoad(string fileName)
    {
        string tt = ResourceIOTool.ReadStringByFile(Application.dataPath + "/Script/Core/Editor/res/EditorWindow/" + fileName + ".txt");

        return tt.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
    private GUIContent[] GetGUIContent(string[] iconNames)
    {
        List<GUIContent> list = new List<GUIContent>();
        for (int i = 0; i < iconNames.Length; i++)
        {
            try
            {
                GUIContent cc = EditorGUIUtility.IconContent(iconNames[i].Trim());
                cc.text = iconNames[i];
                list.Add(cc);
            }
            catch
            {

            }
        }
        return list.ToArray();
    }


    //private List<GUIContent> lstWindowIcons, lstLoadIconParmContents, lstFindTextureParmContents;
    //private Vector2 vct2LoadIconParmScroll;
    //private Rect rectScrollViewPos = new Rect(), rectScrollViewRect = new Rect();
    //private Rect headerRct = new Rect();
    //private Rect rectLoadIcon = new Rect(0, 0, 300, 35);


    //private MethodInfo loadIconMethodInfo, findTextureMethodInfo;
    //private IEnumerator enumeratorLoadIcon, enumeratorFindTexture;

    //void Awake()
    //{
    //    lstWindowIcons = new List<GUIContent>();
    //    lstLoadIconParmContents = new List<GUIContent>();
    //    lstFindTextureParmContents = new List<GUIContent>();

    //    loadIconMethodInfo = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
    //    findTextureMethodInfo = typeof(EditorGUIUtility).GetMethod("FindTexture", BindingFlags.Static | BindingFlags.Public);

    //    InitWindowsIconList();
    //    enumeratorLoadIcon = MethodParamEnumerator("EditorGUIUtility.LoadIcon", loadIconMethodInfo);
    //    enumeratorFindTexture = MethodParamEnumerator("EditorGUIUtility.FindTexture", findTextureMethodInfo);

    //    // LoadIcon 的实参有的是字符串拼接的。。这种我就没有加载出来，可以到UnityEditor.dll源码中查看如何凭借
    //    // 这里我用一个源码中拼接的图标作为该窗口的图标
    //    titleContent = new GUIContent("InternalIcon", loadIconMethodInfo.Invoke(null, new object[] { "WaitSpin00" }) as Texture);
    //    minSize = new Vector2(512, 320);
    //}
    //void OnGUI()
    //{
    //    // Don't use yield in OnGUI() between GUILayout.BeginArea() and GUILayout.EndArea()
    //    if (null != enumeratorLoadIcon && enumeratorLoadIcon.MoveNext() && null != enumeratorLoadIcon.Current)
    //    {
    //        lstLoadIconParmContents.Add(enumeratorLoadIcon.Current as GUIContent);
    //        Repaint();
    //    }
    //    if (null != enumeratorFindTexture && enumeratorFindTexture.MoveNext() && null != enumeratorFindTexture.Current)
    //    {
    //        lstFindTextureParmContents.Add(enumeratorFindTexture.Current as GUIContent);
    //        Repaint();
    //    }

    //    headerRct.x = headerRct.y = 0;
    //    headerRct.width = position.width;
    //    headerRct.height = 30;

    //    int colCount = Mathf.Max(1, (int)(position.width / rectLoadIcon.width));
    //    int rowCount = (lstWindowIcons.Count + lstLoadIconParmContents.Count + lstFindTextureParmContents.Count) / colCount + 2;

    //    rectScrollViewRect.width = colCount * rectLoadIcon.width;
    //    rectScrollViewRect.height = rowCount * rectLoadIcon.height + 3 * headerRct.height;
    //    rectScrollViewPos.width = position.width;
    //    rectScrollViewPos.height = position.height;

    //    vct2LoadIconParmScroll = GUI.BeginScrollView(rectScrollViewPos, vct2LoadIconParmScroll, rectScrollViewRect);
    //    {
    //        float offsetY = 0;
    //        string headerText = "添加EditorWindowTitleAttribute 特性的窗口的图标：" + lstWindowIcons.Count + " 个";
    //        offsetY = DrawList(headerText, offsetY, colCount, lstWindowIcons, false);

    //        headerRct.y = offsetY;
    //        headerText = "传递给 EditorGUIUtility.LoadIcon 的参数：" + lstLoadIconParmContents.Count + " 个";
    //        offsetY = DrawList(headerText, offsetY, colCount, lstLoadIconParmContents, true);

    //        headerRct.y = offsetY;
    //        headerText = "传递给 EditorGUIUtility.FindTexture 的参数：" + lstFindTextureParmContents.Count + " 个";
    //        offsetY = DrawList(headerText, offsetY, colCount, lstFindTextureParmContents, true);
    //    }
    //    GUI.EndScrollView();
    //}
    ///// <summary>
    ///// 绘制 GUIContent list
    ///// </summary>
    ///// <param name="headerText">标头</param>
    ///// <param name="offsetY">绘制区域的垂直偏移量</param>
    ///// <param name="colCount">一行绘制几个</param>
    ///// <param name="lstGUIContent">将要绘制的 GUIContent list</param>
    ///// <returns>返回 结束后的偏移量</returns>
    //private float DrawList(string headerText, float offsetY, int colCount, List<GUIContent> lstGUIContent, bool isRemoveReturn)
    //{
    //    GUI.Label(headerRct, headerText);
    //    offsetY += headerRct.height;
    //    for (int i = 0; i < lstGUIContent.Count; ++i)
    //    {
    //        rectLoadIcon.x = (int)(rectLoadIcon.width * (i % colCount));
    //        rectLoadIcon.y = (int)(rectLoadIcon.height * (i / colCount)) + offsetY;

    //        if (GUI.Button(rectLoadIcon, lstGUIContent[i]))
    //        {
    //            string str = lstGUIContent[i].text;
    //            if (isRemoveReturn)
    //            {
    //                str = str.Replace("\r", "");
    //                str = str.Replace("\n", "");
    //            }
    //            Debug.Log(str);
    //        }
    //    }
    //    return offsetY + (lstGUIContent.Count / colCount + 1) * rectLoadIcon.height;
    //}
    ///// <summary>
    ///// 通过反射得到 EditorWindowTitleAttribute 特性标记的 EditorWindow 子类
    ///// 并通过这个特性中的属性得到 图标的名字，
    ///// 然后继续通过反射调用内部方法 EditorGUIUtility.LoadIcon 来得到 图标的 Texture 实例
    ///// </summary>
    //private void InitWindowsIconList()
    //{
    //    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

    //    Type editorWindowTitleAttrType = typeof(EditorWindow).Assembly.GetType("UnityEditor.EditorWindowTitleAttribute");

    //    foreach (Assembly assembly in assemblies)
    //    {
    //        Type[] types = assembly.GetTypes();
    //        foreach (Type type in types)
    //        {
    //            if (!type.IsSubclassOf(typeof(EditorWindow)))
    //                continue;

    //            object[] attrs = type.GetCustomAttributes(editorWindowTitleAttrType, true);
    //            for (int i = 0; i < attrs.Length; ++i)
    //            {
    //                if (attrs[i].GetType() == editorWindowTitleAttrType)
    //                {
    //                    string icon = GetPropertyValue<string>(editorWindowTitleAttrType, attrs[i], "icon");
    //                    if (string.IsNullOrEmpty(icon))
    //                    {
    //                        bool useTypeNameAsIconName = GetPropertyValue<bool>(editorWindowTitleAttrType, attrs[i], "useTypeNameAsIconName");
    //                        if (useTypeNameAsIconName)
    //                            icon = type.ToString();
    //                    }

    //                    if (!string.IsNullOrEmpty(icon) && null != loadIconMethodInfo)
    //                    {
    //                        var iconTexture = loadIconMethodInfo.Invoke(null, new object[] { icon }) as Texture2D;
    //                        if (null != iconTexture)
    //                            lstWindowIcons.Add(new GUIContent(type.Name + "\n" + icon, iconTexture));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    ///// <summary>
    ///// 通过将 Editor.dll 反编译出来，遍历反编译出来的所有文件，
    ///// 通过正则找出所有 调用 EditorGUIUtility.LoadIcon 时传递 的参数
    ///// </summary>
    ///// <param name="methodName">加载贴图的函数名</param>
    ///// <param name="loadTextureAction">加载贴图的函数</param>
    ///// <returns></returns>
    //private IEnumerator MethodParamEnumerator(string methodName, MethodInfo loadTextureMethodInfo)
    //{
    //    Type editorResourcesUtility = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.EditorResourcesUtility");

    //    //Regex regex = new Regex(@"(?<=EditorGUIUtility.LoadIcon\("")[^""]+(?=""\))");
    //    Regex regex = new Regex(@"(?<=" + methodName + @"\()[^\(\)]*(((?'Open'\()[^\(\)]*)+((?'-Open'\))[^\(\)]*)+)*(?=\))(?(Open)(?!))");
    //    Regex quatRegex = new Regex(@"(?<=^"")[^""]+(?=""$)");

    //    // 这里是反编译 UnityEditor.dll 导出来的文件夹
    //    string[] files = Directory.GetFiles(@"D:\Unity5\UnityEditor", "*.cs", SearchOption.AllDirectories);

    //    var enumerable = from matchCollection in
    //                        (from content in
    //                            (from file in files select File.ReadAllText(file))
    //                         select regex.Matches(content))
    //                     select matchCollection;

    //    foreach (MatchCollection matchCollection in enumerable)
    //    {
    //        for (int i = 0; i < matchCollection.Count; ++i)
    //        {
    //            Match match = matchCollection[i];
    //            string iconName = ((Match)match).Groups[0].Value;

    //            if (string.IsNullOrEmpty(iconName) || null == loadTextureMethodInfo)
    //                continue;

    //            bool isDispatchMethod = false;
    //            Texture iconTexture = null;
    //            if (quatRegex.IsMatch(iconName))
    //            {
    //                isDispatchMethod = true;
    //                iconName = iconName.Replace("\"", "");
    //            }
    //            else if (iconName.StartsWith("EditorResourcesUtility."))
    //            {
    //                string resName = GetPropertyValue<string>(editorResourcesUtility, null, iconName.Replace("EditorResourcesUtility.", ""));
    //                if (!string.IsNullOrEmpty(resName))
    //                {
    //                    isDispatchMethod = true;
    //                    iconName = resName;
    //                }
    //            }

    //            if (isDispatchMethod)
    //            {
    //                try
    //                {
    //                    iconTexture = loadTextureMethodInfo.Invoke(null, new object[] { iconName }) as Texture2D;
    //                }
    //                catch (Exception e)
    //                {
    //                    Debug.LogError(iconName + "\n" + e);
    //                }
    //            }

    //            if (null != iconTexture)
    //                yield return new GUIContent(InsertReturn(iconName, 20), iconTexture);
    //            else
    //                yield return new GUIContent(InsertReturn(iconName, 30));
    //        }
    //    }
    //}
    ///// <summary>
    ///// 反射得到属性值
    ///// </summary>
    ///// <typeparam name="T">属性类型</typeparam>
    ///// <param name="type">属性所在的类型</param>
    ///// <param name="obj">类型实例，若是静态属性，则obj传null即可</param>
    ///// <param name="propertyName">属性名</param>
    ///// <returns>属性值</returns>
    //private T GetPropertyValue<T>(Type type, object obj, string propertyName)
    //{
    //    T result = default(T);
    //    PropertyInfo propertyInfo = type.GetProperty(propertyName);
    //    if (null != propertyInfo)
    //    {
    //        result = (T)propertyInfo.GetValue(obj, null);
    //    }
    //    return result;
    //}
    ///// <summary>
    ///// 对字符串插入 换行符
    ///// </summary>
    ///// <param name="str">待处理的字符串</param>
    ///// <param name="interval">每几个字符插入一个 换行符</param>
    ///// <returns></returns>
    //private string InsertReturn(string str, int interval)
    //{
    //    if (string.IsNullOrEmpty(str) || str.Length <= interval)
    //        return str;

    //    StringBuilder sb = new StringBuilder();
    //    int index = 0;
    //    while (index < str.Length)
    //    {
    //        if (0 != index)
    //            sb.Append("\r\n");

    //        int len = index + interval >= str.Length ? str.Length - index : interval;
    //        sb.Append(str.Substring(index, len));
    //        index += len;
    //    }
    //    return sb.ToString();
    //}
}