
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Hugula.Audio
{
    /// <summary>
    /// Plays the specified sound.
    /// </summary>
    public class PlaySound : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        public enum Trigger
        {
            OnClick,
            OnEnable,
            OnDisable,
            OnDown,
        }

        private const string Key = "playerPrefsSound";
        public string audioClip;
        [Tooltip("when Button interactable is false trigger this audio")] public string audioClipDisabled;
        public Trigger trigger = Trigger.OnClick;

        private Button btn;

        void Awake()
        {
            btn = GetComponent<Button>();
        }

        void PlayEffect(string clipname)
        {
            if (!string.IsNullOrEmpty(clipname))
            {
                AudioManager.instance.PlaySound(clipname);
            }
        }

        void OnEnable()
        {
            if (trigger == Trigger.OnEnable && AudioManager.instance)
                PlayEffect(audioClip);
        }

        void OnDisable()
        {
            if (trigger == Trigger.OnDisable && AudioManager.instance)
                PlayEffect(audioClip);
        }

        void OnDestroy()
        {
            
            AudioManager.instance.Substract(audioClip);
        }

        public void Play()
        {
            if (AudioManager.instance)
                PlayEffect(audioClip);
        }

        public void Stop()
        {
            // StopE
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (trigger == Trigger.OnClick && AudioManager.instance)
            {
                if (null != btn && !string.IsNullOrEmpty(audioClipDisabled) && !btn.interactable)
                    PlayEffect(audioClipDisabled);
                else
                    PlayEffect(audioClip);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (trigger == Trigger.OnDown && AudioManager.instance)
            {
                PlayEffect(audioClip);
            }
        }
    }
}