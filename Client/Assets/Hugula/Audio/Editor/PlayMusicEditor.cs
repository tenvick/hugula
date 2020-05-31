using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using Hugula.Audio;


namespace HugulaEditor.Audio
{

    [CustomEditor(typeof(PlayMusic), true)]
    public class PlayMusicEditor : Editor
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
            playSound.audioClip = "btn_click";
        }

        void OnEnable()
        {
            GetMusicRes();
            m_Serialize_audioClip = serializedObject.FindProperty("audioClip");
            m_Serialize_trigger = serializedObject.FindProperty("trigger");
            // m_Serialize_volume = serializedObject.FindProperty("volume");
            // m_Serialize_pitch = serializedObject.FindProperty("pitch");

        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) return;
            EditorGUILayout.Space();
            PlayMusic temp = target as PlayMusic;

            Undo.RecordObject(target, "F");
            string clipName = temp.audioClip;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Music", GUILayout.MinWidth(+50));
            string clipPath = GetMusicPath(clipName);
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

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Serialize_trigger);
            // EditorGUILayout.PropertyField(m_Serialize_volume);
            // EditorGUILayout.PropertyField(m_Serialize_pitch);
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("refresh music res"))
            {
                GetMusicRes(true);
            }
        }

        static string GetMusicPath(string clipname)
        {
            string path = string.Empty;
            if (clipname != null)
                allsoundPath?.TryGetValue(clipname, out path);
            return path;
        }

        static AudioClip GetAudioClip(string clipPath)
        {
            AudioClip clip = null;
            if (!string.IsNullOrEmpty(clipPath))
                clip = (AudioClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AudioClip));

            clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(AudioClip), false, GUILayout.MinWidth(50));
            return clip;
        }

        static void GetMusicRes(bool need = false)
        {
            if (allsound == null || need)
            {
                List<string> needName = new List<string>();
                needName.Add(EMPTY_TIPS);
                List<string> path = new List<string>();
                var guids = AssetDatabase.FindAssets("t:AudioClip");
                allsoundPath = new Dictionary<string, string>();
                foreach (string guid in guids)
                {
                    string p = AssetDatabase.GUIDToAssetPath(guid);
                    var im = AssetImporter.GetAtPath(p);
                    if (im != null && !string.IsNullOrEmpty(im.assetBundleName))
                    {
                        var abFileName = CUtils.GetAssetName(im.assetBundleName);
                        string k = CUtils.GetAssetName(p);
                        if (k.Equals(abFileName)) //assetbundle name equals asset name is music
                        {
                            needName.Add(k);
                            allsoundPath.Add(k, p);
                        }
                    }
                }
                allsound = needName;
            }
        }
    }
}