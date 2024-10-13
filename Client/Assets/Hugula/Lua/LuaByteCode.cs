using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using Hugula.Utils;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Bindings;
using System.Runtime.InteropServices;
using System;
using System.Buffers;

namespace Hugula
{

    [XLua.DoNotGen]
    public class LuaByteCode
    {
        internal const int HeadLength = 4;

        /// <summary>
        /// Merge the 64-bit bytecode with 32-bit bytecode .
        /// </summary>
        /// <param name="bytecode64">The 64-bit bytecode.</param>
        /// <param name="bytecode32">The 32-bit bytecode.</param>
        /// <returns>The Merge bytecode.</returns>
        public static byte[] MergeByteCode(byte[] bytecode64, byte[] bytecode32,string luaPath="")
        {

            int bytecode64Length = bytecode64.Length;
            int bytecode32Length = bytecode32.Length;

            byte[] delta = ConstructBytecodeDelta(bytecode64, bytecode32);

            int deltaLength = delta.Length;

            // 计算总长度
            int totalLength = HeadLength + bytecode64Length + HeadLength + deltaLength + HeadLength;

            if(deltaLength>=(bytecode64Length*0.25f)){

                Debug.Log($"The difference is now greater than one-fourth of the original .lua:{luaPath} bytecode64Length={bytecode64Length} bytecode32Length={bytecode32Length} deltaLength={deltaLength} totalLength={totalLength}");
            }

            // 预分配内存
            byte[] result = new byte[totalLength];
            int offset = 0;

            // 写入 bytecode64 的内容
            Buffer.BlockCopy(bytecode64, 0, result, offset, bytecode64Length);
            offset += bytecode64Length;
            
            // 写入 delta 的内容
            Buffer.BlockCopy(delta, 0, result, offset, deltaLength);
            offset += deltaLength;

            // 写入 bytecode32 的长度
            if (bytecode32Length == 0)
            {
                bytecode32Length = bytecode64Length;
            }

            var bytecode32LengthBytes = BitConverter.GetBytes(bytecode32Length);
            Buffer.BlockCopy(bytecode32LengthBytes, 0, result, offset, HeadLength);
            offset += HeadLength;

            // 写入 delta 的长度
            var deltaLenBytes = BitConverter.GetBytes(deltaLength);
            Buffer.BlockCopy(deltaLenBytes, 0, result, offset, HeadLength);
            offset += HeadLength;

            // 写入 bytecode64 的长度
            var bytecode64LenBytes = BitConverter.GetBytes(bytecode64Length);
            Buffer.BlockCopy(bytecode64LenBytes, 0, result, offset, HeadLength);
          
            return result;      
        }

        /// <summary>
        /// Patch the 64-bit bytecode with the delta.
        /// </summary>
        /// <param name="MergeBytecode">The Merge bytecode.</param>
        /// <returns>The 64-bit bytecode.</returns>        
        public static byte[] Patch64Bytecode(byte[] mergedByteCode,out int length,int mergeLength = 0)
        {
            // 读取 bytecode64 的长度
            if (mergeLength == 0)
            {
                mergeLength = mergedByteCode.Length;
            }

            if(mergeLength < HeadLength)
            {
                throw new Exception("Byte code length mismatch");
            }

            int bytecode64Length = BitConverter.ToInt32(mergedByteCode, mergeLength - HeadLength);
            length = bytecode64Length;
            return mergedByteCode;
        }

        /// <summary>
        /// Patch the 32-bit bytecode with the delta.
        /// </summary>
        /// <param name="MergeBytecode">The Merge bytecode.</param>
        /// <returns>The 32-bit bytecode.</returns>
        public static byte[] Patch32Bytecode(byte[] mergedByteCode,out int length,int mergeLength = 0)
        {
            if (mergeLength == 0)
            {
                mergeLength = mergedByteCode.Length;
            }

            if(mergeLength < 3 * HeadLength)
            {
                throw new Exception("Byte code length mismatch");
            }

            // 读取 bytecode64 的长度
            var bc_size = BitConverter.ToInt32(mergedByteCode, mergeLength - HeadLength);
            // 读取 delta 的长度
            var delta_size = BitConverter.ToInt32(mergedByteCode,mergeLength - 2 * HeadLength);
            // 读取 bytecode32 的长度
            var bc32_size = BitConverter.ToInt32(mergedByteCode, mergeLength - 3 *HeadLength);
            length = bc32_size;

            // 计算 delta 的起始位置
            var deltaStart =  bc_size;
            // Debug.Log($"bc_size={bc_size} delta_size={delta_size} deltaStart={deltaStart}");           
            var i = deltaStart;
            delta_size = deltaStart+delta_size;
            var delta = mergedByteCode;
            while (i < delta_size)
            {
                uint index = delta[i++];
                if (bc_size >= (1 << 24))
                {
                    index += (uint)((delta[i++]) << 8);
                    index += (uint)((delta[i++]) << 16);
                    index += (uint)((delta[i++]) << 24);
                }
                else if (bc_size >= (1 << 16))
                {
                    index += (uint)((delta[i++]) << 8);
                    index += (uint)((delta[i++]) << 16);
                }
                else if (bc_size >= (1 << 8))
                {
                    index += (uint)((delta[i++]) << 8);
                }

                // get the number of consecutive bytes of changes
                uint count = delta[i++];
                while (count-- > 0)
                {
                    // apply the delta to the bytes
                    mergedByteCode[index++] = delta[i++];
                }               
            }         

            return mergedByteCode;
        }

        /// <summary>
        /// Construct the 32-bit bytecode from the 64-bit bytecode and the delta.
        /// </summary>
        /// <param name="bytecode64">The 64-bit bytecode.</param>
        /// <param name="bytecode32">The 32-bit bytecode.</param>
        /// <returns>The 32-bit bytecode.</returns>

        static byte[] ConstructBytecodeDelta(byte[] bytecode64, byte[] bytecode32)
        {
            // expect same length on 32 and 64 bit bytecode if storing a delta
            // if (bytecode32.Length > bytecode64.Length)
            // {
            //     throw new Exception($"Byte code length mismatch  bytecode32.Length{bytecode32.Length} > bytecode64.Length{bytecode64.Length}");
            // }

            /**
            * Calculate the difference/delta between the 64-bit and 32-bit
            * bytecode.
            * The delta is stored together with the 64-bit bytecode and when
            * the 32-bit bytecode is needed the delta is applied to the 64-bit
            * bytecode to transform it to the equivalent 32-bit version.
            *
            * The delta is stored in the following format:
            *
            * * index - The index where to apply the next change. 1-4 bytes.
            *           The size depends on the size of the entire bytecode:
            *           1 byte - Size less than 2^8
            *           2 bytes - Size less than 2^16
            *           3 bytes - Size less than 2^24
            *           4 bytes - Size more than or equal to 2^24
            * * count - The number of consecutive bytes to alter. 1 byte (ie max 255 changes)
            * * bytes - The 32-bit bytecode values to apply to the 64-bit bytecode starting
            *           at the index.
            */
            using (MemoryStream ms = new MemoryStream())
            {
                int i = 0;
                var len = bytecode32.Length;
                var bytecode64Length = bytecode64.Length;
                while (i < len)
                {
                    // find sequences of consecutive bytes that differ
                    // max 255 at a time
                    int count = 0;
                    while (count < 255 && (i + count) < len)
                    {
                        if (bytecode64Length > i+count &&  bytecode32[i + count] == bytecode64[i + count]) //检测范围
                        {
                            break;
                        }
                        count++;
                    }

                    // found a sequence of bytes
                    // write index, count and bytes
                    if (count > 0)
                    {
                        if (count == 256)
                        {
                            // Console.WriteLine("\n\nLuaBuilder count {0}\n\n", count);
                        }

                        // write index of diff
                        ms.WriteByte((byte)i); // 4
                        if (len >= 256) // (1 << 8))
                        {
                            ms.WriteByte((byte)((i & 0xFF00) >> 8));
                        }
                        if (len >= 65536)// (1 << 16))
                        {
                            ms.WriteByte((byte)((i & 0xFF0000) >> 16));
                        }
                        if (len >= 16777216) //(1 << 24))
                        {
                            ms.WriteByte((byte)((i & 0xFF000000) >> 24)); // 8
                        }

                        ms.WriteByte((byte)count); //1

                        // write consecutive bytes that differ
                        ms.Write(bytecode32, i, count);
                        i += count;
                    }
                    else
                    {
                        i += 1;
                    }
                }
               

                return ms.ToArray();
            }
        }
    }
   
}