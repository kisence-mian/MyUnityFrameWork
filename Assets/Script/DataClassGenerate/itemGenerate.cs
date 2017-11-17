using System;
using UnityEngine;

//itemGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class itemGenerate : DataGenerateBase 
{
	public string m_key;
	public string m_ItemName;
	public int m_Cost;

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("item");

		if (!table.ContainsKey(key))
		{
			throw new Exception("itemGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_key = key;
		m_ItemName = data.GetString("ItemName");
		m_Cost = data.GetInt("Cost");
	}
}
