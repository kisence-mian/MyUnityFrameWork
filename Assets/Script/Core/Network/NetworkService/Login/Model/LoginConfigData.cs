using System;
using UnityEngine;

//LoginConfigDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class LoginConfigData : DataGenerateBase 
{
	public string m_key;
	public LoginPlatform m_loginName; //登录类型名称
	public string m_UIIcon; //UI上显示的图标
	public bool m_UseItem; //是否所有平台都启用当前登录
	public string m_Description; //描述
	public string m_LoginClassName; //对接SDK的class Name
	public string[] m_SupportPlatform; //支持平台(使用UnityEngine.RuntimePlatform)
	public string m_CustomInfo; //传入的自定义文本

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("LoginConfigData");

		if (!table.ContainsKey(key))
		{
			throw new Exception("LoginConfigDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_key = key;
		m_loginName = data.GetEnum<LoginPlatform>("loginName");
		m_UIIcon = data.GetString("UIIcon");
		m_UseItem = data.GetBool("UseItem");
		m_Description = data.GetString("Description");
		m_LoginClassName = data.GetString("LoginClassName");
		m_SupportPlatform = data.GetStringArray("SupportPlatform");
		m_CustomInfo = data.GetString("CustomInfo");
	}
}
