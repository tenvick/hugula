using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourcePlay : MonoBehaviour
    {
        public enum PlayType
        {
            ///<summary>
            /// 激活的时候播放
            ///</summary>
            OnEnable,

            ///<summary>
            /// 碰撞的时候播放
            ///</summary>
            OnTrigger,

            ///<summary>
            /// 手动
            ///</summary>
            OnManual

        }

        [SerializeField]
        private string TriggerTag;

        [SerializeField]
        private PlayType type = PlayType.OnEnable;

        [SerializeField]
        private float easeTime;

        [Tooltip("audio source")]
        [SerializeField]
        private AudioSource audioSource;
        // Start is called before the first frame update
        void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            if (type == PlayType.OnEnable) Play();
        }

        void OnDisable()
        {
            if (type == PlayType.OnEnable) Stop();
        }

        void OnTriggerEnter(Collider other)
        {
            if (type == PlayType.OnTrigger && string.Equals(TriggerTag, other.tag))
                Play();
        }

        void OnTriggerExit(Collider other)
        {
            if (type == PlayType.OnTrigger && string.Equals(TriggerTag, other.tag))
                Stop();
        }

        public void Play()
        {
            audioSource?.Play();
        }

        public void Stop()
        {
            audioSource?.Stop();
        }

        public void VolumeTo(float value, float easeTime = -1)
        {
            if (float.Equals(easeTime, -1f))
                easeTime = this.easeTime;

            LeanTween.value(this.gameObject, (v) => { audioSource.volume = v; }, 0, value, easeTime);

        }

        public void Pause()
        {
            audioSource?.Pause();
        }

        public void PlayDelay(float delay)
        {
            audioSource?.PlayDelayed(delay);
        }

    }
}