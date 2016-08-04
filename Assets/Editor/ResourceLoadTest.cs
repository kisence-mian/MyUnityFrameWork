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

        GameObject testTmp = (GameObject)ResourceManager.Load("UItest 1");

        Assert.NotNull(testTmp);
	}

    [Test]
    public void LoadByResourceAsync()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Resource;

        ResourceManager.LoadAsync("UItest 2", (LoadState state, object obj) =>
        {
            if (state.isDone)
            {
                GameObject go = (GameObject)obj;
                Assert.NotNull(go);
            }
            else
            {
                Debug.Log(state.progress);
            }

        });
    }

    [Test]
    public void LoadByBundle()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;

        GameObject testTmp = (GameObject)ResourceManager.Load("UItest 3");

        //Instantiate(testTmp);

        Assert.NotNull(testTmp);
    }

    [Test]
    public void LoadByBundleAsync()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;

        ResourceManager.LoadAsync("UItest 4", (LoadState state, object obj) => 
        {
            GameObject go = (GameObject)obj;

            Assert.NotNull(go);
        });
    }


    [Test]
    public void CheckConfig()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;

        BundleConfig packConfig = BundleConfigManager.GetBundleConfig("UItest 5");

        Assert.NotNull(packConfig);

    }
}
