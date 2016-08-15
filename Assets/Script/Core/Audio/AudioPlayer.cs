using UnityEngine;
using System.Collections;
using System;

public class AudioPlayer : MonoBehaviour 
{
    public string m_AudioName = "";

    public AudioCallBack m_completeCallBack;

    public object[] l_objs;
    public AudioSource m_player;
    public SoundType m_soundType;

    public bool m_isPlaying = false;
    public bool m_isLoop = false;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(m_isPlaying == true)
        {
            if(m_player != null && !m_player.isPlaying)
            {
                m_isPlaying = false;

                try
                {
                    m_completeCallBack(m_AudioName, l_objs);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
	}

    public void Init()
    {
        m_player = gameObject.AddComponent<AudioSource>();
    }

    public void SetAudio(string l_AudioName,AudioClip l_audio,AudioCallBack l_callBack)
    {
        m_AudioName = l_AudioName;
        m_player.clip = l_audio;
    }
}
