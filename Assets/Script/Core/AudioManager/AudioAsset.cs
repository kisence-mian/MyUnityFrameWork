using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    public enum AudioPlayState
    {
        Playing,
        Pause,
        Stop,
    }
    public class AudioAsset
    {
        public AudioSource audioSource;

        public string assetName = "";
        private float totleVolume = 1;
        /// <summary>
        /// 总音量
        /// </summary>
        public float TotleVolume
        {
            get
            {
                return totleVolume;
            }

            set
            {
                totleVolume = value;
                Volume = TotleVolume * volumeScale;
            }
        }
        /// <summary>
        /// 当前AudioSource 实际音量
        /// </summary>
        public float Volume
        {
            get { return audioSource.volume; }
            set { audioSource.volume = value; }
        }
        /// <summary>
        /// 相对于总音量当前当前AudioSource的音量缩放 Volume=TotleVolume * volumeScale
        /// </summary>
        private float volumeScale = 1f;
        public float VolumeScale
        {
            get { return volumeScale; }
            set
            {
                volumeScale = Mathf.Clamp01(value);
                Volume = TotleVolume * volumeScale;
            }
        }
        public bool IsPlay
        {
            get { return audioSource.isPlaying; }
        }

        private AudioPlayState playState = AudioPlayState.Stop;
        public AudioPlayState PlayState
        {
            get
            {
                if (audioSource==null ||( !audioSource.isPlaying && playState != AudioPlayState.Pause))
                    playState = AudioPlayState.Stop;

                return playState;
            }

        }

    

        public void SetPlaying()
        {
            playState = AudioPlayState.Playing;
        }

        public void Play(float delay = 0f)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayDelayed(delay);
                playState = AudioPlayState.Playing;

            }
        }
        public void Pause()
        {
            if (audioSource != null && audioSource.clip != null && audioSource.isPlaying)
            {
                audioSource.Pause();
                playState = AudioPlayState.Pause;
            }
        }
        public void Stop()
        {
            if (audioSource)
                audioSource.Stop();
            playState = AudioPlayState.Stop;
        }

    }

