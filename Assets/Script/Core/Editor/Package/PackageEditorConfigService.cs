using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackageEditorConfigService
{
    //所有依赖包
    private static List<EditPackageConfig> relyPackages = new List<EditPackageConfig>();
    //所有普通包
    private static List<EditPackageConfig> bundles = new List<EditPackageConfig>();

    static bool s_isInit;

    public static List<EditPackageConfig> RelyPackages
    {
        get
        {
            if(!s_isInit)
            {
                s_isInit = true;
                LoadPackageEditorConfig();
            }

            return relyPackages;
        }

        set
        {
            if (!s_isInit)
            {
                s_isInit = true;
                LoadPackageEditorConfig();
            }

            relyPackages = value;
        }
    }

    public static List<EditPackageConfig> Bundles
    {
        get
        {
            if (!s_isInit)
            {
                s_isInit = true;
                LoadPackageEditorConfig();
            }

            return bundles;
        }

        set
        {
            if (!s_isInit)
            {
                s_isInit = true;
                LoadPackageEditorConfig();
            }

            bundles = value;
        }
    }

    static void LoadPackageEditorConfig()
    {
        Dictionary<string, object> final = ConfigEditorWindow.GetEditorConfigData(BundleConfigEditorWindow.c_configFileName);

        if (final == null)
        {
            Debug.Log(BundleConfigEditorWindow.c_configFileName + " ConfigData dont Exits");
            return;
        }

        //依赖包
        relyPackages = JsonTool.Json2List<EditPackageConfig>((string)final["relyBundles"]);

        for (int i = 0; i < relyPackages.Count; i++)
        {
            //重新加载Object
            ReLoadGameObject(relyPackages[i]);
        }

        //Bundle包
        bundles = JsonTool.Json2List<EditPackageConfig>((string)final["AssetsBundles"]);

        for (int i = 0; i < bundles.Count; i++)
        {
            //重新加载Object
            ReLoadGameObject(bundles[i]);
        }
    }

    public static void SavePackageEditorConfig()
    {
        //生成编辑器配置文件
        Dictionary<string, object> editorConfig = new Dictionary<string, object>();

        editorConfig.Add(BundleConfigEditorWindow.c_keyRelyPackages, JsonTool.List2Json<EditPackageConfig>(relyPackages)); //依赖包
        editorConfig.Add(BundleConfigEditorWindow.c_keyBundles, JsonTool.List2Json<EditPackageConfig>(bundles));    //Bundle包

        //保存编辑器配置文件
        ConfigEditorWindow.SaveEditorConfigData(BundleConfigEditorWindow.c_configFileName, editorConfig);
    }

    //重新加载Object
    static void ReLoadGameObject(EditPackageConfig pack)
    {
        if (pack.mainObject != null)
        {
            ReLoadEditObject(pack.mainObject);
        }

        for (int i = 0; i < pack.objects.Count; i++)
        {
            ReLoadEditObject(pack.objects[i]);
        }
    }

    static void ReLoadEditObject(EditorObject editObj)
    {
        if (editObj.obj == null)
        {
            editObj.obj = AssetDatabase.LoadAssetAtPath<Object>(editObj.path);
        }
    }
}
