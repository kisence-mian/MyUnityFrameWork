using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class LanguageSettingConfig
{
    /// <summary>
    /// 默认语言
    /// </summary>
    public SystemLanguage defaultLanguage = SystemLanguage.Unknown;
    /// <summary>
    /// 游戏存在的语言
    /// </summary>
    public List<SystemLanguage> gameExistLanguages = new List<SystemLanguage>();
    ///// <summary>
    ///// 所有多语言文件名字
    ///// </summary>
    //public List<string> allLanguageFileNames = new List<string>();
}

