using UnityEngine;
using System.Collections.Generic;
using Kud.Common;
using Kud.Editor;
using UnityEngine.Audio;

namespace Kud.Sound
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        public enum SE
        {
            Enter,
            Get,
            BadHit,
            Critical,
            Num
        }

        [SerializeField, NamedArray(typeof(SE))] AudioClip[] audioClips = new AudioClip[(int)SE.Num];
        [SerializeField] AudioMixerGroup seGroup; 

        protected override void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        /// <summary>
        /// 再生処理
        /// </summary>
        /// <param name="_se"></param>
        public void PlaySound(SE _se)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = audioClips[(int)_se];
            source.outputAudioMixerGroup = seGroup;
            source.Play();
            Debug.Log($"[Sound] SE {_se.ToString()}: {audioClips[(int)_se].length}");
            Destroy(source, audioClips[(int)_se].length + 1);
        }
    }
}