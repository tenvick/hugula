using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using Hugula.Audio;


namespace HugulaEditor.Audio
{
    [CustomEditor(typeof(AudioSourcePrefab), true)]
    public class AudioSourcePrefabEditor : Editor
    {
        // [MenuItem("GameObject/Audio/AudioSourcePrefab")]
        // public static void AddSoundMenu(MenuCommand menuCommand)
        // {
        //     GameObject parent = menuCommand.context as GameObject; // Selection.activeGameObject;
        //     var audioPrefab = new GameObject("AudioPrefab", typeof(AudioSource));
        //     audioPrefab.transform.SetParent(parent.transform);
        //     var audio = audioPrefab.CheckAddComponent<AudioSourcePrefab>();
        // }


        private const string EMPTY_TIPS = "null";
        public static List<string> allsound;
        public static Dictionary<string, string> allsoundPath;

        SerializedProperty m_Serialize_audioSource;
        SerializedProperty m_Serialize_type;

        SerializedProperty m_Serialize_easeTime;

        void OnEnable()
        {
            m_Serialize_audioSource = serializedObject.FindProperty("m_AudioSource");
            m_Serialize_type = serializedObject.FindProperty("type");
            m_Serialize_easeTime = serializedObject.FindProperty("easeTime");

            if (m_Serialize_audioSource.objectReferenceValue == null)
            {
                m_Serialize_audioSource.objectReferenceValue = ((AudioSourcePrefab)target).GetComponent<AudioSource>();
                serializedObject.ApplyModifiedProperties();

            }

        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) return;
            EditorGUILayout.Space();
            var temp = target as AudioSourcePrefab;

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Serialize_audioSource);
            EditorGUILayout.PropertyField(m_Serialize_type);
            EditorGUILayout.PropertyField(m_Serialize_easeTime);

            serializedObject.ApplyModifiedProperties();
        }
    }
}