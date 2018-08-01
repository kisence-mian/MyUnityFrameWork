using System;
using System.Collections.Generic;

/// <summary>
/// 字段的用途区分
/// </summary>
public enum DataFieldAssetType
{
    /// <summary>
    /// 单纯的数据
    /// </summary>
    Data,
    /// <summary>
    /// 多语言字段
    /// </summary>
    LocalizedLanguage,
    /// <summary>
    /// 预制
    /// </summary>
    Prefab,
    /// <summary>
    /// 关联其他表格的key
    /// </summary>
    TableKey,
    /// <summary>
    /// 图片资源
    /// </summary>
    Texture,
}

