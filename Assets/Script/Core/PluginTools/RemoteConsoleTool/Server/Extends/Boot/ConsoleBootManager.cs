using UnityEngine;
using System.Collections;
using LiteNetLibManager;
using System;
using System.Collections.Generic;

namespace GameConsoleController
{
    public class ConsoleBootManager
    {

        private static List<BootFunctionBase> bootFunctions = new List<BootFunctionBase>();
        // Use this for initialization
      public static  void Init(GameConsolePanelSettingConfig config,Action OnTriggerBoot)
        {
           Type[] types=  ReflectionTool.GetChildTypes(typeof(BootFunctionBase));
            Debug.Log("types.cout:" + types.Length);
            foreach (var item in types)
            {
                object obj = ReflectionTool.CreateDefultInstance(item);
                if (obj != null)
                {
                    BootFunctionBase function = (BootFunctionBase)obj;
                    bootFunctions.Add(function);
                }
            }

            foreach (var item in bootFunctions)
            {
                try
                {
                    item.OnInit(config,OnTriggerBoot);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
              
            }
        }

        // Update is called once per frame
       public static void OnUpdate()
        {
            foreach (var item in bootFunctions)
            {
               
                try
                {
                    item.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public static void OnGUI()
        {
            foreach (var item in bootFunctions)
            {
               
                try
                {
                    item.OnGUI();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}
