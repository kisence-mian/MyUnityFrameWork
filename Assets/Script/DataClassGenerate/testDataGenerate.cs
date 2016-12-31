using System;
using UnityEngine;

//testDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class testDataGenerate : DataGenerateBase 
{
	public Vector2 m_name;
	public Vector3 m_pos;
	public int m_age;

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("testData");

		if (!table.ContainsKey(key))
		{
			throw new Exception("testDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_name = data.GetVector2("name");
		m_pos = data.GetVector3("pos");
		m_age = data.GetInt("age");
	}
}
