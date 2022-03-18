using System.Text;
using UnityEngine;

namespace Hugula.ResUpdate
{    
    /// <summary>
    /// 单文件信息
    /// </summary>
    [System.SerializableAttribute]
    public class FileResInfo 
    {
        public uint crc32 = 0;

        public uint size = 0;
        public string name = string.Empty;

        //运行时记录crc校验结果
        public FileInfoState state = FileInfoState.None;

        //Addressables.BuildPath的相对目录路径，目前用于文件build 文件的copy
        // public string relativeDir = string.Empty;

        public FileResInfo(string name, uint crc32, uint size)
        {
            this.name = name;
            this.crc32 = crc32;
            this.size = size;
        }

        public FileResInfo Clone()
        {
            var finfo = new FileResInfo(this.name, this.crc32, this.size);
            return finfo;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("(path={0},", name);
            stringBuilder.AppendFormat("crc32={0},", this.crc32);
            stringBuilder.AppendFormat("size={0},", this.size);
            stringBuilder.Append(")");            
            return stringBuilder.ToString();
        }

    }

    public enum FileInfoState
    {
        //未判断
        None = 0,
        //存在版本号正确
        Success = 1 << 0,
        //存在判断失败
        Fail = 1 << 1,
        //不存在或者加载失败
        NotExist = 1 << 2
    }
}