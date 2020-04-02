
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDataEditorTest : MonoBehaviour {

    [Test(Description = "Array")]
  public  void PaseArray1 () {
        {
            string arrayStringContent = "mm#123,haha|papa,haha|ko#888";
            char[] arraySplitFormat = new char[] { ',', '#' };

            Array array = ParseTool.String2Array(FieldType.StringArray, arrayStringContent, arraySplitFormat);
            Debug.Assert(array != null);
            Debug.Log(JsonUtils.ToJson(array));

            string res = ParseTool.ArrayObject2String(array, arraySplitFormat);
            Debug.Log(res);
            Debug.Assert(res == arrayStringContent);
        }
        {
            string arrayStringContent = "NULl";
            char[] arraySplitFormat = new char[] { ',', '#' };

            Array array = ParseTool.String2Array(FieldType.StringArray, arrayStringContent, arraySplitFormat);
            Debug.Assert(array == null);
            Debug.Log(JsonUtils.ToJson(array));

            string res = ParseTool.ArrayObject2String(array, arraySplitFormat);
            Debug.Log(res);
            Debug.Assert(res == arrayStringContent.ToLower());
        }
    }

    [Test(Description = "Array")]
    public void PaseArray2()
    {
        string arrayStringContent = "haha|papa|ko#888";


        Array array = ParseTool.String2Array(FieldType.StringArray, arrayStringContent, new char[] { });
        Debug.Assert(array != null);
        Debug.Log(JsonUtils.ToJson(array));
    }

    //[Test(Description = "Array")]
    //public void PaseArray3()
    //{
    //    TestTableGenerate testTable = DataGenerateManager<TestTableGenerate>.GetData("xx");
    //    Debug.Log(JsonUtils.ToJson(testTable));
    //}
}
