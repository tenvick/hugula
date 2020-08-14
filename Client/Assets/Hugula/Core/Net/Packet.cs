// Galaxy Network

using System;
using System.Net;

namespace Hugula.Net
{
    public class Packet
    {
        public const int BufferSize = 4096;
        protected int size = 0;
        protected int type = 0;
        protected bool storing = false;
        protected byte[] data = null;
        protected int offset = 0;

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Length; i++)
            {
                s += data[i].ToString("x") + ",";
            }
            return s;
        }

        public Packet()
        {
            data = new byte[0];
        }

        public Packet(int packetType)
        {
            type = packetType;
            data = new byte[BufferSize];
            storing = true;
            offset = sizeof(int) * 2;   // skip size and type
            size = sizeof(int);
        }

        public void ReUse(int packetType)
        {
            type = packetType;
            // data = new byte[BufferSize];
            storing = true;
            offset = sizeof(int) * 2;   // skip size and type
            size = sizeof(int);
        }
      
        public int Size
        {           // type+data
            get { return size; }
            internal set { size = value; }
        }

        public int Type
        {
            get { return type; }
            internal set { type = value; }
        }

        public bool Storing
        {
            get { return storing; }
            internal set { storing = value; }
        }

        public void SetType(int newType)
        {
            type = newType;
        }

        public byte[] GetBytes()
        {
            return data;
        }

        public void Set(int packetSize, byte[] buffer, int start)
        {
            size = packetSize;
            type = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, start));

            data = new byte[size];
            Buffer.BlockCopy(buffer, start, data, 0, size);
            offset = sizeof(int);   //skip packet type
        }

        public char ReadChar()
        {
            if (offset >= data.Length) return '0';
            char value = (char)data[offset];
            offset++;
            return value;
        }

        public byte ReadByte()
        {
            if (offset >= data.Length) return 0;
            byte value = data[offset];
            offset++;
            return value;
        }

        public short ReadShort()
        {
            if (offset >= data.Length) return 0;
            short value = BitConverter.ToInt16(data, offset);
            offset += sizeof(short);
            return IPAddress.NetworkToHostOrder(value);
        }

        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }

        public int ReadInt()
        {
            if (offset >= data.Length) return 0;
            int value = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            return IPAddress.NetworkToHostOrder(value);
        }

        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }


        public string ReadString()
        {
            ushort len = ReadUShort();
            if (offset + len > data.Length) return "";
            string str = System.Text.Encoding.UTF8.GetString(data, offset, len);
            offset += len;
            return str;
        }

        public byte[] ReadBytes(int len)
        {
            if (offset + len > data.Length) return null;
            byte[] buffer = new byte[len];
            Buffer.BlockCopy(data, offset, buffer, 0, len);
            offset += len;
            return buffer;
        }

        public byte[] ReadBlock()
        {
            if (offset >= data.Length) return null;
            int len = ReadInt();
            return ReadBytes(len);
        }



        public void WriteChar(char value)
        {
            if (!storing) return;

            data[offset] = (byte)value;
            offset++;
            size++;
        }

        public void WriteByte(byte value)
        {
            if (!storing) return;

            data[offset] = value;
            offset++;
            size++;
        }


        public void WriteShort(short value)
        {
            if (!storing) return;

            value = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
            offset += bytes.Length;
            size += bytes.Length;
        }

        public void WriteUShort(ushort value)
        {
            WriteShort((short)value);
        }


        public void WriteInt(int value)
        {
            if (!storing) return;

            value = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
            offset += bytes.Length;
            size += bytes.Length;
        }

        public void WriteUInt(uint value)
        {
            WriteInt((int)value);
        }

        public void WriteString(string value)
        {
            if (!storing) return;
            if (value.Length > UInt16.MaxValue) return;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            ushort len = (ushort)bytes.Length;
            WriteUShort(len);
            Buffer.BlockCopy(bytes, 0, data, offset, len);
            offset += len;
            size += len;
        }


        public void WriteBytes(byte[] buffer)
        {
            if (!storing) return;

            int len = buffer.Length;
            Buffer.BlockCopy(buffer, 0, data, offset, len);
            offset += len;
            size += len;
        }

        public void WriteBlock(byte[] buffer)
        {
            WriteInt(buffer.Length);
            WriteBytes(buffer);
        }

        public void Finish()
        {
            if (!storing) return;

            // write packet size
            int value = IPAddress.HostToNetworkOrder(size);
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, data, 0, sizeof(int));

            // write packet type
            value = IPAddress.HostToNetworkOrder(type);
            bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, data, sizeof(int), sizeof(int));
        }

        public bool Equals(Packet other)
        {
            if (size == other.Size)
            {
                byte[] otherData = other.GetBytes();
                for (int i = 0; i < size; i++)
                {
                    if (data[i] != otherData[i])
                        return false;
                }
                return true;
            }

            return false;
        }
    }
}

