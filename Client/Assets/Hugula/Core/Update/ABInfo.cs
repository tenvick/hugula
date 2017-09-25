using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace Hugula.Update
{
    [System.SerializableAttribute]
    public class ABInfo
    {
        public ABInfo(string abName, uint crc32, uint size, int priority)
        {
            this.abName = abName;
            this.crc32 = crc32;
            this.size = size;
            this.priority = priority;
        }
        public uint crc32 = 0;

        public uint size = 0;

        public int priority = 0;

        public string abName = string.Empty;

        public string[] dependencies;

        //运行时记录crc校验结果
        internal ABInfoState state = ABInfoState.None;

        public ABInfo Clone()
        {
            var abinfo = new ABInfo(this.abName, this.crc32, this.size, this.priority);
            abinfo.dependencies = this.dependencies;
            return abinfo;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("(abName={0},", this.abName);
            stringBuilder.AppendFormat("crc32={0},", this.crc32);
            stringBuilder.AppendFormat("size={0},", this.size);
            stringBuilder.AppendFormat("priority={0},", this.priority);
            if (dependencies == null)
                stringBuilder.Append("dependencies:null");
            else
            {
                stringBuilder.Append("dependencies:");
                foreach (var s in dependencies)
                    stringBuilder.AppendFormat("{0},", s);
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

#if UNITY_EDITOR
        //editor用于 记录资源路径
        [System.NonSerializedAttribute]
        public string assetPath = string.Empty;

        public bool EqualsDependencies(ABInfo abinfo)
        {
            if(abinfo==null)
            {
                 return false;   
            }
            else if (dependencies == null && abinfo.dependencies == null) //相同
            {
                return true;
            }
            else if (dependencies == null && (abinfo.dependencies != null && abinfo.dependencies.Length==0)) //不相同
            {
                return true;
            }
            else if (dependencies.Length == 0 && abinfo.dependencies == null ) //相同
            {
                return true;
            }
            else if (dependencies.Length != abinfo.dependencies.Length) //不相同
            {
                return false;
            }
            else
            {
                foreach (var s in dependencies)
                {
                    int i = System.Array.IndexOf(abinfo.dependencies, s);
                    if (i == -1)
                        return false;
                }
            }

            return true;
        }

#endif

    }

    public enum ABInfoState
    {
        None = 0,
        Success = 1 << 0,
        Fail = 1 << 1,
    }
}
