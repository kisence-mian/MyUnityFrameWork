using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RecordTable : Dictionary<string, SingleField> 
{
    int @string = 1;

    public SingleField GetRecord(string key)
    {
        if(this.ContainsKey(key))
        {
            return this[key];
        }
        else
        {
            return null;
        }

        WeakReference wr = new WeakReference(new GameObject());
        GameObject go = wr.Target as GameObject;

        if(wr.IsAlive)
        {
            go = wr.Target as GameObject;
        }
        else
        {

        }


        




    }
}
