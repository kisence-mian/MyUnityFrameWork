using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class DataManagerTest {

	[Test]
	public void Test_1()
	{
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Resource;
        DataTable data = DataManager.GetData("testData");

        Assert.NotNull(data);
	}

    [Test]
    public void Test_2()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Resource;
        DataTable data = DataManager.GetData("testData");

        Assert.AreEqual( data.defaultValue["name"],"张三");
        //Assert.AreEqual(data["1"].GetString("name"), "李四");
    }

    [Test]
    public void Test_3()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Resource;
        DataTable data = DataManager.GetData("bugs");

        Assert.AreEqual(data.defaultValue["bug编号"], "37");
        Assert.AreEqual(data["1"].GetString("bug编号"), "1");
        Assert.AreEqual(data["1"].GetInt("bug编号"), 1);
    }
}
