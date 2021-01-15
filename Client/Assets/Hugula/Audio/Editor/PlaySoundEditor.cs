using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hugula.Utils;
using Hugula.Audio;


namespace HugulaEditor.Audio
{

    [CustomEditor(typeof(PlaySound), true)]
    public class PlaySoundEditor : Editor
    {

        private const string EMPTY_TIPS = "null";
        public static List<string> allsound;
        public static Dictionary<string, string> allsoundPath;

        SerializedProperty m_Serialize_trigger;
        // SerializedProperty m_Serialize_volume;
        // SerializedProperty m_Serialize_pitch;
        SerializedProperty m_Serialize_audioClip;

        [MenuItem("GameObject/Audio/PlaySound")]
        public static void AddSoundMenu(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject; // Selection.activeGameObject;
            var playSound = parent.CheckAddComponent<PlaySound>();
            playSound.audioClip = "ui_button";
        }

        void OnEnable()
        {
            GetSoundEffectRes();
            m_Serialize_audioClip = serializedObject.FindProperty("audioClip");
            m_Serialize_trigger = serializedObject.FindProperty("trigger");
            // m_Serialize_volume = serializedObject.FindProperty("volume");
            // m_Serialize_pitch = serializedObject.FindProperty("pitch");

        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) return;
            EditorGUILayout.Space();
            PlaySound temp = target as PlaySound;
            Undo.RecordObject(target, "F");
            string clipName = temp.audioClip;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sound", GUILayout.MinWidth(50));
            string clipPath = GetSoundPath(clipName);
            AudioClip clip = GetAudioClip(clipPath);
            if (clip != null)
            {
                temp.audioClip = clip.name;
                clipName = clip.name;
            }
            int index = allsound.IndexOf(clipName);
            //xiala
            index = EditorGUILayout.Popup(index, allsound.ToArray(), GUILayout.MinWidth(50));
            if (index >= 1)
            {
                temp.audioClip = allsound[index];
            }
            else
                temp.audioClip = string.Empty;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("UnInteractable Sound", GUILayout.MinWidth(50));
            clipName = temp.audioClipDisabled;
            clipPath = GetSoundPath(clipName);
            clip = GetAudioClip(clipPath);
            if (clip != null)
            {
                temp.audioClipDisabled = clip.name;
                clipName = clip.name;
            }
            index = allsound.IndexOf(clipName);
            //xiala
            index = EditorGUILayout.Popup(index, allsound.ToArray(), GUILayout.MinWidth(50));
            if (index >= 1)
            {
                temp.audioClipDisabled = allsound[index];
            }
            else
                temp.audioClipDisabled = string.Empty;

            EditorGUILayout.EndHorizontal();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Serialize_trigger);
            // EditorGUILayout.PropertyField(m_Serialize_volume);
            // EditorGUILayout.PropertyField(m_Serialize_pitch);
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("refresh sound res"))
            {
                GetSoundEffectRes(true);
            }
        }

        static string GetSoundPath(string clipname)
        {
            string path = string.Empty;
            if(clipname!=null)
                allsoundPath?.TryGetValue(clipname, out path);
            return path;
        }

        static AudioClip GetAudioClip(string clipPath)
        {
            AudioClip clip = null;
            if (!string.IsNullOrEmpty(clipPath))
                clip = (AudioClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AudioClip));

            clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(AudioClip), false, GUILayout.MinWidth(50));
            // Debug.LogFormat("clip={0},path={1} ",clip,clipPath);
            return clip;
        }

        static void GetSoundEffectRes(bool need = false)
        {
            if (allsound == null || need)
            {
                List<string> needName = new List<string>();
                needName.Add(EMPTY_TIPS);
                List<string> path = new List<string>();
                var guids = AssetDatabase.FindAssets("t:AudioClipAsset");
                allsoundPath = new Dictionary<string, string>();
                foreach (string guid in guids)
                {
                    string p = AssetDatabase.GUIDToAssetPath(guid);
                    var audioClipAsset = AssetDatabase.LoadAssetAtPath<AudioClipAsset>(p);
                    if (audioClipAsset != null)
                    {
                        foreach (var cp in audioClipAsset.audioClips)
                        {
                            string k = cp.name;
                            string ap1 = AssetDatabase.GetAssetPath(cp);
                            needName.Add(k);
                            if (allsoundPath.ContainsKey(k))
                            {
                                Debug.LogWarningFormat("声音文件{0}重名 路径：{1} ", k, ap1);
                            }
                            else
                            {
                                allsoundPath.Add(k, ap1);
                            }
                        }
                    }
                }
                allsound = needName;
            }
        }
    }

}