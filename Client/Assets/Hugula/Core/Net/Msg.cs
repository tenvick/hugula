// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.IO;
using System;
using System.Text;

namespace Hugula.Net
{
    /// <summary>
    /// ÏûÏ¢·â×°
    /// </summary>
    [SLua.CustomLuaClass]
    public class Msg
    {

        public Msg()
        {
            buff = new MemoryStream();
            br = new BinaryReader(buff);
        }

        public Msg(byte[] bytes)
        {
            buff = new MemoryStream(bytes);
            br = new BinaryReader(buff);
            this.Type = ReadShort();
        }

        public long Length
        {
            get
            {
                return buff.Length;
            }
        }

        public long Position
        {
            get
            {
                return buff.Position;
            }
            set
            {
                buff.Position = value;
            }
        }

        public byte[] ToArray()
        {
            return buff.ToArray();
        }

        public string Debug()
        {
            byte[] bts = ToArray();

            string bstr = "";

            foreach (byte i in bts)
            {
                bstr += " " + i + " ";
            }
            return bstr;
        }

        public static string Debug(byte[] bts)
        {
            string bstr = "";

            foreach (byte i in bts)
            {
                bstr += " " + i + " ";
            }
            return bstr;
        }

        /// <summary>
        /// our message pro
        /// </summary>
        /// <returns>
        /// The C array.
        /// </returns>
        public byte[] ToCArray()
        {
            byte[] date = ToArray();

            short len = (short)(date.Length + 2);//date[].length+type(short)
            short type = (short)this.Type;

            byte[] lenBytes = BitConverter.GetBytes(len);// date.length
            System.Array.Reverse(lenBytes);

            byte[] typeBytes = BitConverter.GetBytes(type);//tyep bytes
            System.Array.Reverse(typeBytes);

            int allLen = lenBytes.Length + typeBytes.Length + date.Length;
            byte[] send = new byte[allLen];

            lenBytes.CopyTo(send, 0);//len
            typeBytes.CopyTo(send, lenBytes.Length);//type
            date.CopyTo(send, lenBytes.Length + typeBytes.Length);

            return send;
        }

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        #region write

        public void Write(byte[] value)
        {
            buff.Write(value, 0, value.Length);
        }

        public void WriteBoolean(bool value)
        {
            buff.WriteByte(value ? ((byte)1) : ((byte)0));
        }

        public void WriteByte(byte value)
        {
            buff.WriteByte(value);
        }

        public void WriteChar(char value)
        {
            byte b = Convert.ToByte(value);
            buff.WriteByte(b);
        }

        public void WriteUShort(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public void WriteUInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public void WriteULong(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public void WriteShort(int value)
        {
            byte[] bytes = BitConverter.GetBytes((short)value);
            WriteBigEndian(bytes);
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public void WriteInt(int value)
        {
            //		Debug.Log("writeInt                         ::::::::::::"+value);
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public void WriteString(string value)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            int byteCount = utf8Encoding.GetByteCount(value);
            byte[] buffer = utf8Encoding.GetBytes(value);
            this.WriteShort(byteCount);
            if (buffer.Length > 0)
                Write(buffer);
        }

        public void WriteUTFBytes(string value)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] buffer = utf8Encoding.GetBytes(value);
            if (buffer.Length > 0)
                Write(buffer);
        }

        #endregion

        #region read

        public bool ReadBoolean()
        {
            return br.ReadBoolean();
        }

        public byte ReadByte()
        {
            return br.ReadByte();
        }

        public char ReadChar()
        {
            byte byt = br.ReadByte();
            return Convert.ToChar(byt);
        }

        public ushort ReadUShort()
        {
            byte[] bytes = br.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public uint ReadUInt()
        {
            byte[] bytes = br.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public ulong ReadULong()
        {
            byte[] bytes = br.ReadBytes(8);
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public short ReadShort()
        {
            byte[] bytes = br.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public int ReadInt()
        {
            byte[] bytes = br.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public float ReadFloat()
        {
            byte[] bytes = br.ReadBytes(4);
            Array.Reverse(bytes);
            float value = BitConverter.ToSingle(bytes, 0);
            return value;
        }

        public string ReadString()
        {
            int length = ReadShort();
            return ReadUTF(length);
        }

        public string ReadUTF(int length)
        {
            if (length == 0)
                return string.Empty;

            byte[] encodedBytes = br.ReadBytes(length);
            string decodedString = Encoding.UTF8.GetString(encodedBytes, 0, encodedBytes.Length);
            return decodedString;
        }

        #endregion

        #region member

        protected void WriteBigEndian(byte[] bytes)
        {
            if (bytes == null)
                return;
            for (int i = bytes.Length - 1; i >= 0; i--) //		for(int i = 0; i < bytes.Length; i++)
            {
                buff.WriteByte(bytes[i]);
            }
        }

        protected MemoryStream buff;
        protected BinaryReader br;

        protected int _type;

        #endregion

    }
}