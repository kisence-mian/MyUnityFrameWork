using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 不在默认Editor GUI里面显示
/// </summary>
[AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
public class NoShowInEditorAttribute : Attribute { }

/// <summary>
/// 在Editor GUI里面显示特别的名字
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ShowGUINameAttribute: Attribute
{
    private string name;
    public string Name
    {
        get
        {
            return name;
        }
    }
    public ShowGUINameAttribute(string newName)
    {
        name = newName;
    }
}
