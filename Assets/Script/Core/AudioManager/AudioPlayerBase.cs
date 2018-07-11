
using System;
using System.Collections;
using UnityEngine;

    public abstract class AudioPlayerBase
    {
        protected MonoBehaviour mono;
        protected int maxSFXAudioAssetNum = 10;
    protected float musicVolume = 1f;

     protected float sfxVolume = 1f;
    public AudioPlayerBase(MonoBehaviour mono)
        {
            this.mono = mono;
        }

        public void SetMaxSFXAudioAssetNum(int max)
        {
            maxSFXAudioAssetNum = Mathf.Clamp(max, 5, 100);
        }
    public abstract void SetMusicVolume(float volume);
    public abstract void SetSFXVolume(float volume);


    public  AudioClip GetAudioClip(string name)
        {
            AudioClip red = ResourceManager.Load<AudioClip>(name);
            if (red != null)
            {
                return red;
            }
            Debug.LogError("Can not find AudioClip:" + name);
            return null;
        }

    public AudioAsset CreateAudioAsset(GameObject gameObject, bool is3D, bool isMusic)
    {
        AudioAsset au = new AudioAsset();
        au.audioSource = gameObject.AddComponent<AudioSource>();
        au.audioSource.spatialBlend = is3D ? 1 : 0;
        if (isMusic)
            au.TotleVolume = musicVolume;
        else
            au.TotleVolume = sfxVolume;
        return au;
    }

        public  IEnumerator EaseToChangeVolume(AudioAsset au, string name, bool isLoop, float volumeScale, float delay, float fadeTime)
        {
            AudioClip ac = GetAudioClip(name);
            float oldVolume = au.Volume;
            float target = au.Volume;
            if (au.audioSource && au.IsPlay)
            {
                while (target > 0f)
                {
                    float speed = oldVolume / fadeTime;
                    target = target - speed * Time.fixedDeltaTime;
                    au.Volume = target;
                    yield return new WaitForFixedUpdate();
                }
                au.Stop();
            }
            au.assetName = name;
            au.audioSource.clip = ac;
            au.audioSource.loop = isLoop;
            au.Play(delay);
            target = 0;
            yield return new WaitForSeconds(delay);

            while (target < oldVolume)
            {
                float speed = oldVolume / fadeTime * 1.2f;
                target = target + speed * Time.fixedDeltaTime;
                au.Volume = target;
                yield return new WaitForFixedUpdate();
            }
            au.VolumeScale = volumeScale;
        }
    }
