using System;

namespace Protocol
{
    /// <summary>
    /// 在Protocol以Int16传输的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class Int16Attribute : System.Attribute { }

    /// <summary>
    /// 在Protocol以Int8传输的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class Int8Attribute : System.Attribute { }

    /// <summary>
    /// 在Protocol以Int32传输的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class Int32Attribute : System.Attribute { }

    /// <summary>
    /// 模块名与模块消息编码
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ModuleAttribute : System.Attribute
    {
        public int MessageCode;
        public string ModuleName;

        public ModuleAttribute(int messageCode, string moduleName)
        {
            MessageCode = messageCode;
            ModuleName = moduleName;
        }
    }

    /// <summary>
    /// 消息发送模式，如果不加默认为Both
    /// 生成 protocol 时 ToClient 和 ToServer 类型不会自动加后缀，要保证类名后面有_s 或者 _c后缀
    /// Both自动添加 _s 和 _c 后缀
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageModeAttribute : System.Attribute
    {
        public SendMode Mode;

        public MessageModeAttribute(SendMode mode)
        {
            Mode = mode;
        }
    }

    public enum SendMode
    {
        ToClient,
        ToServer,
        Both,
    }

    /// <summary>
    /// 自动被Protocol解析的基类
    /// </summary>
    public interface IProtocolMessageInterface { }

    /// <summary>
    /// 自动被Protocol解析的结构
    /// </summary>
    public interface IProtocolStructInterface { }

    /// <summary>
    /// 
    /// </summary>
    public interface CsharpProtocolInterface : IProtocolMessageInterface { }
}