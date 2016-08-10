using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour 
{
    public ResLoadType m_loadType = ResLoadType.Resource;
	
    public void Awake()
    {
        AppLaunch();
    }

    public void AppLaunch()
    {
        ResourceManager.gameLoadType = m_loadType;
    }

    public  void Init()
    {
        BundleConfigManager.Initialize();
        UIManager.Init();
    }

}
