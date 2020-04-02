using System;
using UnityEngine;

//HotUpdatePathDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class HotUpdatePathData : DataGenerateBase 
{
	public string m_key;
	public string m_HotupdatePath; //热跟新地址
	public string m_Description; //描述

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("HotUpdatePathData");

		if (!table.ContainsKey(key))
		{
			throw new Exception("HotUpdatePathDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_key = key;
		m_HotupdatePath = data.GetString("HotupdatePath");
		m_Description = data.GetString("Description");
	}

	 public override void LoadData(DataTable table,string key) 
	{
		SingleData data = table[key];

		m_key = key;
		m_HotupdatePath = data.GetString("HotupdatePath");
		m_Description = data.GetString("Description");
	}
}
