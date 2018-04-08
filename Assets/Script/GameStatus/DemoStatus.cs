using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStatus : IApplicationStatus
{
    public override void OnEnterStatus()
    {
        GuideSyetem.Instance.Start();
        OpenUI<MainWindow>();
    }
}
