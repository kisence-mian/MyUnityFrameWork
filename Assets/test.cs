using UnityEngine;
using System.Collections;

public class test : MonoBehaviour 
{
    public GameObject cube;
	// Use this for initialization
	void Start () 
    {
        AnimSystem.Move(gameObject, Vector3.zero, 0.5f, isLocal: true);
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
