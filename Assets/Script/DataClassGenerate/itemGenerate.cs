using System;
using UnityEngine;

//itemGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class itemGenerate : DataGenerateBase 
{
	public Color m_asdasd; //颜色
	public Vector2 m_tt; //枚举测试

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("item");

		if (!table.ContainsKey(key))
		{
			throw new Exception("itemGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_asdasd = data.GetColor("asdasd");
		m_tt = data.GetVector2("tt");
	}
}
