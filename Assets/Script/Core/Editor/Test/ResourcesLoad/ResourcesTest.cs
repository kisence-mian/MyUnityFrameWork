using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Reflection;
using System;

namespace Framework
{
    public class ResourcesTest
    {
        [Test(Description = "资源路径数据存在测试")]
        public void BundleConfigExistTest()
        {
            ResourcesConfigManager.Initialize();

            bool isExist = ConfigManager.GetIsExistConfig(ResourcesConfigManager.c_configFileName);

            Assert.AreEqual(isExist, true);
        }

        [Test(Description = "资源字段验证")]
        public void ResourcesFieldTest()
        {
            ResourcesConfigManager.Initialize();

            Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                //Attribute.GetCustomAttribute(types,typeof(ResourcesFieldAttribute));

                //for (int j = 0; j < types.; j++)
                //{
                    
                //}

                //if (types[i].IsSubclassOf(typeof(IApplicationStatus)))
                //{
                //    listTmp.Add(types[i].Name);
                //}
            }


            //Assert.AreEqual(isExist, true);
        }
    }
}
