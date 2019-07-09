using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IRecordConverter
{
    /// <summary>
    /// 文件后缀名
    /// </summary>
    /// <returns></returns>
    string GetFileExtend();
    /// <summary>
    /// 保存目录的名字（不用加/）
    /// </summary>
    /// <returns></returns>
    string GetSaveDirectoryName();
    /// <summary>
    /// 将text转换为对应数据对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    T String2Object<T>(string content);
    /// <summary>
    /// 数据对象转换String的通用方法
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    string Object2String(object obj);
}