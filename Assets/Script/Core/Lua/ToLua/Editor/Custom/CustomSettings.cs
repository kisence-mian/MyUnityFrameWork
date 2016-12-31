using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;

using BindType = ToLuaMenu.BindType;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class CustomSettings
{
    public static string saveDir = Application.dataPath + "/Script/LuaGenerate";
    public static string toluaBaseType = Application.dataPath + "/Script/LuaGenerate/BaseType/";    

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {    
        //typeof(UnityEngine.Application),
        //typeof(UnityEngine.Time),
        //typeof(UnityEngine.Screen),
        //typeof(UnityEngine.SleepTimeout),
        //typeof(UnityEngine.Input),
        //typeof(UnityEngine.Resources),
        //typeof(UnityEngine.Physics),
        //typeof(UnityEngine.RenderSettings),
        //typeof(UnityEngine.QualitySettings),
        //typeof(UnityEngine.GL),
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        //_DT(typeof(Action)),                
        //_DT(typeof(UnityEngine.Events.UnityAction)),
        //_DT(typeof(System.Predicate<int>)),
        //_DT(typeof(System.Action<int>)),
        //_DT(typeof(System.Comparison<int>)),
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {                
     
        _GT(typeof(Debugger)).SetNameSpace(null),        

    #region Base

        _GT(typeof(GameObject)),
        _GT(typeof(Transform)),
        _GT(typeof(Component)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),
        
    #endregion

    #region UGUI
        _GT(typeof(Selectable)),
        _GT(typeof(UIBehaviour)),
        _GT(typeof(Graphic)),
        _GT(typeof(MaskableGraphic)),
        _GT(typeof(ScrollRect)),
        _GT(typeof(Button)),
        _GT(typeof(Image)),
        _GT(typeof(Text)),

        //自定义的UGUI拓展类
        _GT(typeof(ScrollRectInput)),
        _GT(typeof(ReusingScrollRect)),

    #endregion

    #region Framework
         _GT(typeof(AnimSystem)),
         _GT(typeof(ResourceManager)),
         _GT(typeof(UIManager)),
         _GT(typeof(ApplicationManager)),
         _GT(typeof(ApplicationStatusManager)),

    #endregion

    #region Custom

    #endregion

    #region Note
        
        //_GT(typeof(Component)),

        //_GT(typeof(Material)),
        //_GT(typeof(Light)),
        //_GT(typeof(Rigidbody)),
        //_GT(typeof(Camera)),
        //_GT(typeof(AudioSource)),

        //_GT(typeof(Behaviour)),
        //_GT(typeof(MonoBehaviour)),        

        //_GT(typeof(TrackedReference)),
        //_GT(typeof(Application)),
        //_GT(typeof(Physics)),
        //_GT(typeof(Collider)),
        //_GT(typeof(Time)),        
        //_GT(typeof(Texture)),
        //_GT(typeof(Texture2D)),
        //_GT(typeof(Shader)),        
        //_GT(typeof(Renderer)),
        //_GT(typeof(WWW)),
        //_GT(typeof(Screen)),        
        //_GT(typeof(CameraClearFlags)),
        //_GT(typeof(AudioClip)),        
        //_GT(typeof(AssetBundle)),
        //_GT(typeof(ParticleSystem)),
        //_GT(typeof(AsyncOperation)).SetBaseType(typeof(System.Object)),        
        //_GT(typeof(LightType)),
        //_GT(typeof(SleepTimeout)),
        //_GT(typeof(Animator)),
        //_GT(typeof(Input)),
        //_GT(typeof(KeyCode)),
        //_GT(typeof(SkinnedMeshRenderer)),
        //_GT(typeof(Space)),      
       
        //_GT(typeof(MeshRenderer)),            
        //_GT(typeof(ParticleEmitter)),
        //_GT(typeof(ParticleRenderer)),
        //_GT(typeof(ParticleAnimator)), 
                              
        //_GT(typeof(BoxCollider)),
        //_GT(typeof(MeshCollider)),
        //_GT(typeof(SphereCollider)),        
        //_GT(typeof(CharacterController)),
        //_GT(typeof(CapsuleCollider)),
        
        //_GT(typeof(Animation)),        
        //_GT(typeof(AnimationClip)).SetBaseType(typeof(UnityEngine.Object)),        
        //_GT(typeof(AnimationState)),
        //_GT(typeof(AnimationBlendMode)),
        //_GT(typeof(QueueMode)),  
        //_GT(typeof(PlayMode)),
        //_GT(typeof(WrapMode)),

        //_GT(typeof(QualitySettings)),
        //_GT(typeof(RenderSettings)),                                                   
        //_GT(typeof(BlendWeights)),           
        //_GT(typeof(RenderTexture)),

#endregion

    };

    public static List<Type> dynamicList = new List<Type>()
    {
        //typeof(MeshRenderer),
        //typeof(BoxCollider),
        //typeof(MeshCollider),
        //typeof(SphereCollider),
        //typeof(CharacterController),
        //typeof(CapsuleCollider),

        //typeof(Animation),
        //typeof(AnimationClip),
        //typeof(AnimationState),

        //typeof(BlendWeights),
        //typeof(RenderTexture),
        //typeof(Rigidbody),
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {
        
    };

    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }    
}
