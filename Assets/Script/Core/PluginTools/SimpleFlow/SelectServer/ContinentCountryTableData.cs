using UnityEngine;
using System.Collections;

/// <summary>
/// 国家大洲对应数据
/// </summary>
public class ContinentCountryTableData 
{
    /// <summary>
    /// 大洲中文名，如：亚洲
    /// </summary>
    public string continent_cname;
    /// <summary>
    /// 大洲英文缩写名，[AF]非洲, [EU]欧洲, [AS]亚洲, [OA]大洋洲, [NA]北美洲, [SA]南美洲, [AN]南极洲
    /// </summary>
    public string continent_name;
    /// <summary>
    /// 国家中文名
    /// </summary>
    public string country_cname;
    /// <summary>
    /// 2位字母国家code
    /// </summary>
    public string country_code;
    /// <summary>
    /// 国家英文名
    /// </summary>
    public string country_name;
}
