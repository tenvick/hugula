using UnityEngine;

namespace Hugula.Utils
{
    public static class HugulaDebug
    {

        public static string filterNames = string.Empty;

        public static bool CheckInFilterMd5Name(string md5Name)
        {

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(filterNames)) return true;
            string[] sp = md5Name.Split(',');
            foreach (var s in sp)
            {
                if (filterNames.Contains(s))
                {
                    return true;
                }
            }
            return false;
#else
                return true;
#endif

        }

        public static void FilterLogFormat(string md5, string source, params object[] args)
        {
            if (CheckInFilterMd5Name(md5))
            {
                Debug.LogFormat(source, args);
            }
        }

        public static void FilterLogWarningFormat(string md5, string source, params object[] args)
        {
            if (CheckInFilterMd5Name(md5))
            {
                Debug.LogWarningFormat(source, args);
            }
        }

        public static void FilterLog(string md5, string msg)
        {
            if (CheckInFilterMd5Name(md5))
            {
                Debug.Log(msg);
            }
        }

        public static void FilterLogWarning(string md5, string msg)
        {
            if (CheckInFilterMd5Name(md5))
            {
                Debug.LogWarning(msg);
            }
        }
    }
}