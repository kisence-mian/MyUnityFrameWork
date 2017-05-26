using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackageService
{
    static BuildAssetBundleOptions relyBuildOption; //依赖包打包设置
    static BuildTarget GetTargetPlatform
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

#pragma warning disable

    public static IEnumerator Package(List<EditPackageConfig> relyPackages, List<EditPackageConfig> bundles, PackageCallBack callBack)
    {
        relyBuildOption = BuildAssetBundleOptions.DeterministicAssetBundle //每次二进制一致
               | BuildAssetBundleOptions.CollectDependencies   //收集依赖
               | BuildAssetBundleOptions.CompleteAssets;      //完整资源
                                                              //| BuildAssetBundleOptions.UncompressedAssetBundle //不压缩

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

        //for (int i = 0; i < m_NoPackagekFile.Count; i++)
        //{
        //    CopyFile(m_NoPackagekFile[i]);
        //}

        BuildPipeline.PopAssetDependencies();

        AssetDatabase.Refresh();
    }

    public static void Package(List<EditPackageConfig> relyPackages, List<EditPackageConfig> bundles)
    {
        relyBuildOption = BuildAssetBundleOptions.DeterministicAssetBundle //每次二进制一致
               | BuildAssetBundleOptions.CollectDependencies   //收集依赖
               | BuildAssetBundleOptions.CompleteAssets;      //完整资源
                                                              //| BuildAssetBundleOptions.UncompressedAssetBundle //不压缩

        BuildPipeline.PushAssetDependencies();


        //删除streaming下所有旧资源
        if (Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");
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

        //for (int i = 0; i < m_NoPackagekFile.Count; i++)
        //{
        //    CopyFile(m_NoPackagekFile[i]);
        //}

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

        BuildPipeline.BuildAssetBundle(null, res, path, relyBuildOption, GetTargetPlatform);
    }

    static void PackageBundle(EditPackageConfig package)
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

        BuildPipeline.BuildAssetBundle(package.mainObject.obj, res, path, relyBuildOption, GetTargetPlatform);

        BuildPipeline.PopAssetDependencies();
    }

    static string GetExportPath(string path, string name)
    {
        return Application.dataPath + "/StreamingAssets/" + BundleConfigEditorWindow. GetRelativePath(FileTool.RemoveExpandName(path)) + "." + AssetsBundleManager.c_AssetsBundlesExpandName;
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
}
