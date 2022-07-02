using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using Hugula.Utils;

namespace Hugula.Audio
{

    public class AudioManager : BehaviourSingleton<AudioManager>
    {

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Hugula/Audio/Create Audiomanager")]
        static void CreateAudioManager()
        {
            GameObject audio = GameObject.Find(typeof(AudioManager).Name);
            if (audio == null)
                audio = new GameObject(typeof(AudioManager).Name, typeof(AudioManager));

            int count = 8;

            for (int i = 0; i < count; i++)
            {
                var adprefab = new GameObject("AudioSourcePrefab" + i, typeof(AudioSource), typeof(AudioSourcePrefab));
                var audioSourcePrefab = adprefab.GetComponent<AudioSourcePrefab>();
                var audioSource = adprefab.GetComponent<AudioSource>();
                audioSource.playOnAwake = false;
                if (i < 2)
                {
                    audioSourcePrefab.type = AudioSourcePrefab.AudioSourceType.Music;
                    audioSource.loop = true;
                }
                else
                    audioSourcePrefab.type = AudioSourcePrefab.AudioSourceType.Effect;

                adprefab.transform.parent = audio.transform;
            }

        }

#endif
        private const string MUSIC_VOLUME = "audioManager_Music_volume";
        private const string SOUND_VOLUME = "audioManager_Sound_volume";

        float m_Musicvolume = 1f;
        float m_SoundVolume = 1f;
        float m_MusicRateVolume = 1f;
        float m_SoundRateVolume = 1f;

        int m_SoundIndex = -1;
        int m_MusicIndex = -1;
        bool m_soundLoaded;
        bool m_effectLoaded;


        /// <summary>
        /// 音乐音量
        /// </summary>
        public float musicVolume
        {
            get
            {
                if (!m_soundLoaded)
                {
                    m_soundLoaded = true;
                    m_Musicvolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
                }
                return m_Musicvolume;
            }
            set
            {
                if (!float.Equals(m_Musicvolume, value))
                {
                    m_Musicvolume = value;
                    PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
                    MusicVolumeTo();
                }

            }
        }

        /// <summary>
        /// 音效音量
        /// </summary>
        public float soundVolume
        {
            get
            {
                if (!m_effectLoaded)
                {
                    m_effectLoaded = true;
                    m_SoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME, 1);
                }
                return m_SoundVolume;
            }
            set
            {
                if (!float.Equals(m_SoundVolume, value))
                {
                    m_SoundVolume = value;
                    PlayerPrefs.SetFloat(SOUND_VOLUME, value);
                    SoundVolumeTo();
                }
            }
        }

        /// <summary>
        ///  音乐音量调整系数
        /// </summary>
        public float musicRateVolume
        {
            get
            {
                return m_MusicRateVolume;
            }
            set
            {
                if (!float.Equals(m_MusicRateVolume, value))
                {
                    m_MusicRateVolume = value;
                    MusicVolumeTo();
                }
            }
        }

        /// <summary>
        ///  音效音量调整系数
        /// </summary>
        public float soundRateVolume
        {
            get
            {
                return m_SoundRateVolume;
            }
            set
            {
                if (!float.Equals(m_SoundRateVolume, value))
                {
                    m_SoundRateVolume = value;
                    SoundVolumeTo();
                }
            }
        }

        string m_PlayMusicName = string.Empty;
        public string playMusicName
        {
            get
            {
                return m_PlayMusicName;
            }
        }

        bool m_IsReady = false;
        public bool isReady
        {
            get
            {
                return m_IsReady;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        IEnumerator Start()
        {
            while (!ResLoader.Ready)
                yield return null;

            InitAudioClipAsset();
        }

        #region dnot public
        /// <summary>
        /// system music audioclip 
        /// </summary>
        private static List<AudioSourcePrefab> musicSroucePrefabs = new List<AudioSourcePrefab>();

        /// <summary>
        /// system effect audioclip 
        /// </summary>
        private static List<AudioSourcePrefab> soundSourcePrefabs = new List<AudioSourcePrefab>();

        static internal void AddMusicAudioSource(AudioSourcePrefab acp)
        {
            if (!musicSroucePrefabs.Contains(acp)) musicSroucePrefabs.Add(acp);
        }

        static internal void RemoveMusicAudioSource(AudioSourcePrefab acp)
        {
            musicSroucePrefabs.Remove(acp);
        }

        static internal void AddSoundAudioSource(AudioSourcePrefab acp)
        {
            if (!soundSourcePrefabs.Contains(acp)) soundSourcePrefabs.Add(acp);
        }

        static internal void RemoveSoundAudioSource(AudioSourcePrefab acp)
        {
            soundSourcePrefabs.Remove(acp);
        }

        private static AudioClipAsset m_AudioClipAsset;

        private void OnAudioClipAssetComp(AudioClipAsset aclipasset, object arg)
        {
            m_AudioClipAsset = aclipasset;
            m_IsReady = true;
        }

        internal void InitAudioClipAsset()
        {
            ResLoader.LoadAssetAsync<AudioClipAsset>(AudioClipAsset.DEFALUT_SOUND_ASSET_NAME, OnAudioClipAssetComp, null);
        }

        /// <summary>
        /// 获取AudioClip
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal AudioClip GetAudioClip(string key)
        {
            AudioClip ac = m_AudioClipAsset?.GetAsset(key); //ResLoader.LoadAsset<AudioClip>(key); // AudioClipMappingManager.instance.GetAsset(key);
            return ac;
        }

        private void SoundVolumeTo()
        {
            AudioSourcePrefab audioSourcePrefab;
            for (int i = 0; i < soundSourcePrefabs.Count; i++)
            {
                audioSourcePrefab = soundSourcePrefabs[i];
                audioSourcePrefab.VolumeTo(soundVolume * m_SoundRateVolume);
            }
        }

        private void MusicVolumeTo()
        {
            AudioSourcePrefab audioSourcePrefab;
            for (int i = 0; i < musicSroucePrefabs.Count; i++)
            {
                audioSourcePrefab = musicSroucePrefabs[i];
                audioSourcePrefab.VolumeTo(musicVolume * m_MusicRateVolume);
            }
        }

        #endregion

        /// <summary>
        /// 暂停所有
        /// </summary>
        /// <param name="ispause"></param>
        public void Pause(bool ispause)
        {
            AudioListener.pause = ispause;
        }

        /// <summary>
        /// 停止音乐
        /// </summary>
        public void StopMusic(float easyTime = -1)
        {
            int len = musicSroucePrefabs.Count;
            for (int i = 0; i < len; i++)
            {
                musicSroucePrefabs[i].Stop(easyTime);
            }
            m_PlayMusicName = null;
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        public void StopEffect(float easyTime = -1)
        {
            int len = soundSourcePrefabs.Count;
            for (int i = 0; i < len; i++)
            {
                soundSourcePrefabs[i].Stop(easyTime);
            }
        }


        /// <summary>
        /// 停止所有
        /// </summary>
        public void StopAll()
        {
            StopMusic();
            StopEffect();
        }

        /// <summary>
        /// play music
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AudioSource PlayMusic(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return PlayMusic(name, musicVolume, 1f);
        }

        public AudioSource PlayMusic(string name, float volume, float pitch)
        {
            AudioSourcePrefab clipPrefab;
            if (string.Equals(m_PlayMusicName, name))
            {
                clipPrefab = musicSroucePrefabs[m_MusicIndex % musicSroucePrefabs.Count];
                return clipPrefab.audioSource;
            }

            EaseOutMusic(m_PlayMusicName);
            m_PlayMusicName = name;
            AudioClip clip = null;//GetAudioClip(name);
            // if (clip == null)
            // {
            //     // //load assetbundle
            //     // var abName = name.ToLower() + Common.CHECK_ASSETBUNDLE_SUFFIX;
            //     // clip = ResourcesLoader.LoadAsset<AudioClip>(abName, name);
            // }
            int count = musicSroucePrefabs.Count;
            m_MusicIndex++;
            clipPrefab = musicSroucePrefabs[m_MusicIndex % count];
            var audioSource = clipPrefab.audioSource;
            audioSource.clip = clip;
            audioSource.pitch = pitch;
            // if (clip)
            //     clipPrefab.Play(volume * m_RateVolume);
            // else
            clipPrefab.PlayAsync(name, volume * m_MusicRateVolume);

            return clipPrefab.audioSource;

        }

        private void EaseOutMusic(string lastMusicName)
        {
            if (m_MusicIndex >= 0)
            {
                int count = musicSroucePrefabs.Count;
                AudioSourcePrefab clipPrefab = musicSroucePrefabs[m_MusicIndex % count];
                clipPrefab.Stop();
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void PlaySound(string name)
        {
            PlaySound(name, soundVolume, 1f);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void PlaySoundScheduled(string name, double time)
        {
            AudioSourcePrefab clipPrefab;
            //find clip
            AudioClip clip = GetAudioClip(name);
            if (clip != null)
            {
                int count = soundSourcePrefabs.Count;
                m_SoundIndex++;
                clipPrefab = soundSourcePrefabs[m_SoundIndex % count];
                var audioSource = clipPrefab.audioSource;
                audioSource.clip = clip;
                audioSource.pitch = 1;
                audioSource.loop = false;
                audioSource.volume = soundVolume * soundRateVolume;
                clipPrefab.PlayScheduled(time);
            }
        }


        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void PlaySound(string name, float volume, float pitch)
        {
            //find clip
            AudioClip clip = GetAudioClip(name);
            if (clip != null)
            {
                int count = soundSourcePrefabs.Count;
                m_SoundIndex++;
                var clipPrefab = soundSourcePrefabs[m_SoundIndex % count];
                var audioSource = clipPrefab.audioSource;
                audioSource.pitch = pitch;
                audioSource.loop = false;
                clipPrefab.PlayOnShot(clip, volume * soundRateVolume);
            }
        }

        public void Substract(string clipName)
        {
            // AudioClipMappingManager.instance.Subtract(clipName);
        }
    }
}

