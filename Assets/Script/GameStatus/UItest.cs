using UnityEngine;
using System.Collections;

public class UItest : IApplicationStatus 
{
    public override void OnEnterStatus()
    {
        UIManager.OpenUIWindow<test01Window>();

        UIManager.CloseUIWindow<test01Window>();

        UIManager.OpenUIWindow<test01Window>();

        UIManager.CloseUIWindow<test01Window>();

        UIManager.OpenUIWindow<test01Window>();

        //UIManager.CloseUIWindow<testWindow>();
    }
}
