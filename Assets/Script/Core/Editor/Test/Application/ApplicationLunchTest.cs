using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace Framework
{
    public class ApplicationLunchTest
    {

        [Test(Description = "应用启动测试")]
        public void LunchTest()
        {
            //Arrange
            ApplicationManager app = GameObject.FindObjectOfType<ApplicationManager>();

            Assert.AreNotEqual(app.m_Status, "");
            Assert.AreNotEqual(app.m_Status, "None");


        }
    }
}
