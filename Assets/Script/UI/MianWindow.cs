using UnityEngine;
using System.Collections;

public class MianWindow : UIWindowBase 
{
    public override void OnStartEnterAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        AnimSystem.uguiAlpha(gameObject, 0, 1, 1, InteType.Linear, true,(object[] obj)=>
        {
            base.OnStartEnterAnim(l_animComplete, l_callBack, objs);
        });
    }

    public override void OnStartExitAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        AnimSystem.uguiAlpha(gameObject, 1, 0, 1, InteType.Linear, true, (object[] obj) =>
        {
            base.OnStartExitAnim(l_animComplete, l_callBack, objs);
        });
    }
}
