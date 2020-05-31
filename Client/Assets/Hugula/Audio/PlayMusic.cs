using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Audio
{

    public class PlayMusic : MonoBehaviour
    {
        public enum Trigger
        {
            OnStart,
            OnEnable,
            OnDisable,
            OnDesotry,
        }

        public Trigger trigger = Trigger.OnEnable;
        public string audioClip;

        // Start is called before the first frame update
        void Start()
        {
            if (trigger == Trigger.OnStart) Play();

        }

        void OnEnable()
        {
            if (trigger == Trigger.OnEnable) Play();

        }

        void OnDisable()
        {
            if (trigger == Trigger.OnDisable) Play();

        }

        void OnDesotry()
        {
            if (trigger == Trigger.OnDesotry) Play();
        }

        public void Play()
        {
            AudioManager.instance.PlayMusic(audioClip);
        }

    }
}
