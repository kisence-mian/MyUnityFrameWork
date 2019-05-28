using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#pragma warning disable
public static class PackageService
{
    #region 4.0旧版本打包

    static BuildAssetBundleOptions relyBuildOption = BuildAssetBundleOptions.AppendHashToAssetBundleName;    //每次二进制一致  //依赖包打包设置   
                                                                                                             // | BuildAssetBundleOptions.ForceRebuildAssetBundle;
                                                                                                             // | BuildAssetBundleOptions.CollectDependencies;   //收集依赖
                                                                                                             // | BuildAssetBundleOptions.CompleteAssets;         //完整资源    
                                                                                                             // | BuildAssetBundleOptions.ChunkBasedCompression;   //块压缩 

    static BuildAssetBundleOptions bundleBuildOption = BuildAssetBundleOptions.DeterministicAssetBundle  //每次二进制一致  //Bundle打包设置
                                                       | BuildAssetBundleOptions.CollectDependencies     //收集依赖
                                                       | BuildAssetBundleOptions.CompleteAssets;         //完整资源
                                                                                                         //| BuildAssetBundleOptions.ChunkBasedCompression; //块压缩
    public static BuildTarget GetTargetPlatform
    {
        get
        {
            BuildTarget target = BuildTarget.StandaloneWindows;

#if UNITY_ANDROID //安卓
            target = BuildTarget.Android;
#elif UNITY_IOS //iPhone
                target = BuildTarget.iOS;
#elif UNITY_WEBGL //WebGL
            target = BuildTarget.WebGL;
#endif

            return target;
        }
    }

    public static IEnumerator Package(List<EditPackageConfig> relyPackages, List<EditPackageConfig> bundles, PackageCallBack callBack)
    {
        BuildPipeline.PushAssetDependencies();

        float sumCount = relyPackages.Count + bundles.Count;
        float currentCount = 0;

        callBack(0, "删除旧资源");
        yield return 0;

        //删除streaming下所有旧资源
        if (Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");
        }

        callBack(0, "开始打包");
        yield return 0;

        //先打依赖包
        for (int i = 0; i < relyPackages.Count; i++)
        {
            PackageRelyPackage(relyPackages[i]);

            currentCount++;
            callBack(currentCount / sumCount, "打包依赖包 第" + i + "个 共" + relyPackages.Count + "个");

            yield return 0;
        }

        //再打普通包
        for (int i = 0; i < bundles.Count; i++)
        {
            PackageBundle(bundles[i]);
            currentCount++;
            callBack(currentCount / sumCount, "打包普通包 第" + i + "个 共" + bundles.Count + "个");

            yield return 0;
        }

        BuildPipeline.PopAssetDependencies();

        AssetDatabase.Refresh();
    }

    public static void Package(List<EditPackageConfig> relyPackages, List<EditPackageConfig> bundles)
    {
        BuildPipeline.PushAssetDependencies();


        //删除streaming下所有旧资源
        if (Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            FileTool.SafeDeleteDirectory(Application.dataPath + "/StreamingAssets");
        }

        //先打依赖包
        for (int i = 0; i < relyPackages.Count; i++)
        {
            PackageRelyPackage(relyPackages[i]);
        }

        //再打普通包
        for (int i = 0; i < bundles.Count; i++)
        {
            PackageBundle(bundles[i]);
        }

        BuildPipeline.PopAssetDependencies();

        AssetDatabase.Refresh();
    }


    static void PackageRelyPackage(EditPackageConfig package)
    {
        //BuildPipeline.PushAssetDependencies();

        if (package.objects.Count == 0)
        {
            Debug.LogError(package.name + " 没有资源！");
        }

        Object[] res = new Object[package.objects.Count];

        for (int i = 0; i < package.objects.Count; i++)
        {
            res[i] = package.objects[i].obj;
        }

        string path = GetExportPath(package.path, package.name);

        FileTool.CreatFilePath(path);

        if (package.isCollectDependencies)
        {
            BuildPipeline.BuildAssetBundle(null, res, path, relyBuildOption, GetTargetPlatform);
        }
        else
        {
            BuildAssetBundleOptions bbo = BuildAssetBundleOptions.DeterministicAssetBundle;

            BuildPipeline.BuildAssetBundle(null, res, path, bbo, GetTargetPlatform);
        }
    }

    public static void PackageBundle(EditPackageConfig package)
    {
        //导入资源包
        BuildPipeline.PushAssetDependencies();

        //打包
        Object[] res = new Object[package.objects.Count];

        for (int i = 0; i < package.objects.Count; i++)
        {
            res[i] = package.objects[i].obj;
        }

        string path = GetExportPath(package.path, package.name);

        FileTool.CreatFilePath(path);

        BuildPipeline.BuildAssetBundle(package.mainObject.obj, res, path, bundleBuildOption, GetTargetPlatform);

        BuildPipeline.PopAssetDependencies();
    }

    static string GetExportPath(string path, string name)
    {
        return Application.dataPath + "/StreamingAssets/" + BundleConfigEditorWindow.GetRelativePath(FileTool.RemoveExpandName(path)).ToLower();
    }

    static void CopyFile(string fileName)
    {
        string filePath = PathTool.GetAbsolutePath(ResLoadLocation.Resource, fileName + "." + ConfigManager.c_expandName);
        string exportPath = PathTool.GetAbsolutePath(ResLoadLocation.Streaming, fileName + "." + ConfigManager.c_expandName);

        if (File.Exists(filePath))
        {
            Debug.Log("导出 " + exportPath);
            File.Copy(filePath, exportPath, true);
        }
        else
        {
            Debug.Log(filePath + " 不存在");
        }
    }

    public delegate void PackageCallBack(float progress, string content);

    #endregion

    #region 5.0新版本打包

    public const string c_StreamingAssetsPath = "/StreamingAssets/";
    public const string c_ResourceParentPath = "/Resources/";
    public const string c_AssetsParentPath = "Assets/";

    public static void Package_5_0(bool deleteManifestFile)
    {
        string streamingPath = Application.dataPath + c_StreamingAssetsPath;

        //删除streaming下所有旧资源
        if (Directory.Exists(streamingPath))
        {
            FileTool.DeleteDirectory(streamingPath);
        }
        else
        {
            FileTool.CreatPath(streamingPath);
        }

        BuildPipeline.BuildAssetBundles(streamingPath, BuildAssetBundleOptions.None, PackageService.GetTargetPlatform);

        //删除冗余的清单文件
        if (deleteManifestFile)
        {
            DeleteManifestFile(streamingPath);
        }
    }

    /// <summary>
    /// 清除所有AssetsBundle设置
    /// </summary>
    public static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("清除前 bundle数目 " + length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("清除后 bundle数目 " + length);
    }

    //构造相对路径使用
    static int direIndex = 0;
    static int assetsIndex = 0;
    static string resourcePath;

    public static void SetAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("生成前 bundle数目 " + length);

        //构造相对路径使用
        resourcePath = Application.dataPath + c_ResourceParentPath;

        direIndex = resourcePath.LastIndexOf(c_ResourceParentPath);
        direIndex += c_ResourceParentPath.Length;

        assetsIndex = resourcePath.LastIndexOf(c_AssetsParentPath);

        RecursionDirectory(Application.dataPath + "/Resources/");

        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("生成后 bundle数目 " + length);
    }

    //递归所有目录
    static void RecursionDirectory(string path)
    {
        if (!File.Exists(path))
        {
            FileTool.CreatPath(path);
        }

        string[] dires = Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length; i++)
        {
            RecursionDirectory(dires[i]);
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;
            else
            {
                string relativePath = FileTool.RemoveExpandName(files[i].Substring(direIndex));
                string assetsPath = files[i].Substring(assetsIndex);
                Object obj = Resources.Load(relativePath);
                if (obj == null)
                {
                    Debug.LogError("Resources obj is null ->" + relativePath);
                }

                SetAssetBundle(obj, assetsPath);
            }
        }
    }

    /// <summary>
    /// 获取所有相关资源
    /// </summary>
    /// <param name="go">目标对象</param>
    /// <returns>所有相关资源</returns>
    static Object[] GetCorrelationResource(Object go)
    {
        Object[] roots = new Object[] { go };
        return EditorUtility.CollectDependencies(roots);
    }

    static void SetAssetBundle(Object obj,string path)
    {
        //寻找资源的依赖，将其设为ABN
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);

        Object[] objs = GetCorrelationResource(obj);
        for (int i = 0; i < objs.Length; i++)
        {
            if(!ComponentFilter(objs[i]))
            {
                string tmp = AssetDatabase.GetAssetPath(objs[i]);
                SetAssetsBundleName(tmp);
            }
        }

        //将资源设为ABN
        SetAssetsBundleName(path);
    }

    static void SetAssetsBundleName(string path)
    {
        //Resources下的资源单独打包
        //Res下的资源以文件夹为单位打包
        string name = FileTool.RemoveExpandName(path).ToLower().Replace("assets/", "");

        if (name.Contains("resources/"))
        {
            name = name.Replace("resources/", "");
        }
        else
        {
            name = FileTool.GetUpperPath(name) + "_rely";//使用文件夹名加依赖后缀
        }

        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter != null)
        {
            assetImporter.assetBundleName = name;
        }
        else
        {
            Debug.LogError("SetAssetsInfo relyPackages error :->" + path);
        }
    }

    static bool ComponentFilter(Object comp)
    {
        //过滤掉unity自带对象
        string path = AssetDatabase.GetAssetPath(comp);
        if (path.IndexOf("Assets") != 0)
        {
            return true;
        }

        ////过滤掉所有shander
        //if (comp as Shader != null)
        //{
        //    if (!shaderFilter.ContainsKey(comp.ToString()))
        //    {
        //        shaderFilter.Add(comp.ToString(), (Shader)comp);
        //        Debug.LogWarning("包含 Shader! :" + comp.ToString());
        //    }

        //    return true;
        //}

        if (comp is MonoScript)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 删除打包后冗余的Manifest文件
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteManifestFile(string path)
    {
        string[] dires = Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length; i++)
        {
            DeleteManifestFile(dires[i]);
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".manifest")
                || files[i].EndsWith(".meta"))
            {
                File.Delete(files[i]);
            }
        }
    }

    #endregion
}