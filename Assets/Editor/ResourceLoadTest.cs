using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ResourceLoadTest 
{

	[Test]
    public void LoadByResource()
	{
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Resource;

        BundleConfig packConfig =  BundleConfigManager.GetBundleConfig("GameObject_adasd");

        GameObject testTmp = (GameObject)ResourceManager.Load("GameObject_adasd");

        Assert.NotNull(testTmp);
	}

    [Test]
    public void LoadByBundle()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;

        GameObject testTmp = (GameObject)ResourceManager.Load("GameObject_adasd");

        //Instantiate(testTmp);

        Assert.NotNull(testTmp);
    }


    [Test]
    public void CheckConfig()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;

        BundleConfig packConfig = BundleConfigManager.GetBundleConfig("GameObject_adasd");

        Assert.NotNull(packConfig);

    }
}
