
using HDJ.Framework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HDJ.Framework.Core
{
    public class GameBootConfigEditorWindow : EditorWindow
    {

        [MenuItem("Tool/游戏框架设置管理(0)", priority = 0)]
        private static void OpenWindow()
        {
            GameBootConfigEditorWindow win = GetWindow<GameBootConfigEditorWindow>();

            win.Init();
        }
        private static GameBootConfig config;
        private static Dictionary<Type, AppModuleBase> allModules = new Dictionary<Type, AppModuleBase>();
        private void Init()
        {
            config = GameBootConfig.LoadConfig();
            if (config == null)
                config = new GameBootConfig();

            allModules.Clear();
            Type[] types = ReflectionUtils.GetChildTypes(typeof(AppModuleBase));
            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                AppModuleBase appModule = null; 
               
                if (config.allAppModuleSetting.ContainsKey(type.Name))
                {
                    appModule = (AppModuleBase)ReflectionUtils.CreateDefultInstance(type);
                    appModule = (AppModuleBase)config.allAppModuleSetting[type.Name].GetValue(appModule);
                }
                else
                {
                    appModule =  (AppModuleBase)ReflectionUtils.CreateDefultInstance(type);
                }
                allModules.Add(type, appModule);
            }
        }
  
        private void OnEnable()
        {
            Init();
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            EditorDrawGUIUtil.DrawScrollView(this, () =>
             {
                 EditorDrawGUIUtil.DrawClassData("", config);

                 foreach (var item in allModules)
                 {
                     EditorDrawGUIUtil.DrawClassData(item.Key.Name, item.Value);
                 }
                
             });

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("保存"))
            {
                Save();
                AssetDatabase.Refresh();
                ShowNotification(new GUIContent("保存成功！"));
            }
        }

       
        private void Save()
        {
            config.allAppModuleSetting.Clear();
            foreach (var item in allModules)
            {
                ClassValue value = new ClassValue(item.Value,false);
                config.allAppModuleSetting.Add(item.Key.Name, value);
            }

            GameBootConfig.Save(config);
        }
    }
}
