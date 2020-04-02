using System;
using UnityEngine;

public class CameraTool : MonoBehaviour
{
    private static float decay = .01f;//every time do
    private static float time;//do time
    private static float dctime = 0f;//time count
    private static float intensity;
    private static float nwintensity;
    private static Vector3 oldpos;
    private static Transform ts;
    private static TimerEvent te;
    private static bool _lock = true;

    /// <summary>
    /// 对像,强度,时间
    /// </summary>
    /// <param name="_ts">对像</param>
    /// <param name="_intensity">强度</param>
    /// <param name="_decay">时间</param>
    public static void shake(Transform _ts, float _intensity = .2f, float _time = 1f)
    {
        _lock = false;
        ts = _ts;
        time = _time * _intensity;
        dctime = 0f;
        nwintensity = (decay / _intensity / _time)*.2f;
        intensity = _intensity;
        //Debug.Log(">>>>"+intensity+">>>"+ decay / intensity);
        oldpos = ts.position;
        te = Timer.CallBackOfIntervalTimer(decay, doUpdate);
    }

    private static void doUpdate(object[] l_objs)
    {
        if (ts == null || _lock) return;
        if (time >= dctime)
        {
            ts.position = oldpos + UnityEngine.Random.insideUnitSphere * intensity;
            intensity -= nwintensity;
            dctime += decay;
            //Debug.Log(">>>>" + dctime + ">>>" + intensity);
        }
        else
        {
            _lock = true;
            ts.position = oldpos;
            Timer.DestroyTimer(te);
        }
    }
}
