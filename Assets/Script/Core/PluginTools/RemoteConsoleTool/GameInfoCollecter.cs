using UnityEngine;
using System.Collections;
using GameConsoleController;

public class GameInfoCollecter
{
    public const string Label_Player = "Player";
    public static void AddPlayerInfoValue( string key, object value, string description = null)
    {
        AddCustomInfoValue(Label_Player, key, value, description);
    }
    public const string Label_NetworkState = "NetworkState";
    public static void AddNetworkStateInfoValue(string key, object value, string description = null)
    {
        AddCustomInfoValue(Label_NetworkState, key, value, description);
    }
    public const string Label_App = "App";
    public static void AddAppInfoValue(string key, object value, string description = null)
    {
        AddCustomInfoValue(Label_App, key, value, description);
    }
    public static void AddCustomInfoValue(string label, string key, object value, string description = null)
    {
        AppInfoCollecter.AddCustomInfoValue(label, key, value, description);
    }
}
