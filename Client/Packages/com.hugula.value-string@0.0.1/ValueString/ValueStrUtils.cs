using System.Collections;
using System.Collections.Generic;
using System.Buffers;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;


public static class ValueStrUtils 
{
    /// <summary>
    /// 按照长度缓存的字符串，应用程序不要缓存
    /// </summary>
    static Dictionary<int, string> cacheStr = new Dictionary<int, string>();

    /// <summary>
    /// 通过长度获取缓存字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static string GetCacheString(int length)
    {
        if(cacheStr.TryGetValue(length,out var str))
        {
            return str;
        }
        else
        {
            str = new string('\0', length);
            cacheStr.Add(length, str);
            return str;
        }
    }

    /// <summary>
    /// 替换字符，不支持多线程，不能缓存字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string ReplaceNoAlloc(string str,char source,char target)
    {
        var len = str.Length;
        var cache = GetCacheString(len);
        unsafe
        {
            fixed (char* s = str)
            {
                fixed (char* t = cache)
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (s[i] == source)
                            t[i] = target;
                        else
                            t[i] = s[i];
                    }
                }
            }
        }

        return cache;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf(string str,string value)
    {
        return IndexOf(str, value, 0, str.Length);
    }

    public static int IndexOf(string str, string value,int start)
    {
        return IndexOf(str, value, start, str.Length- start);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static unsafe int IndexOf(string str,string value, int start, int count)
    {
        int len = str.Length;

        if (start < 0 || start >= len)
            throw new ArgumentOutOfRangeException("start");

        if (count < 0 || start + count > len)
            throw new ArgumentOutOfRangeException("count=" + count + " start+count=" + (start + count));

        if (count == 0)
            return -1;

        fixed (char* ptr_input = str)
        {
            fixed (char* ptr_value = value)
            {
                int found = 0;
                int end = start + count;
                for (int i = start; i < end; i++)
                {
                    for (int j = 0; j < value.Length && i + j < len; j++)
                    {
                        if (ptr_input[i + j] == ptr_value[j])
                        {
                            found++;
                            if (found == value.Length)
                                return i;
                            continue;
                        }
                        if (found > 0)
                            break;
                    }
                }
                return -1;
            }
        }
    }


    private static List<int> m_FindsIndex = new List<int>();
    /// <summary>
    /// 替换字符串，不支持多线程，不能缓存字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static unsafe string ReplaceNoAlloc(string str, string source, string target)
    {
        if (str == null)
            throw new ArgumentNullException("str");

        if (source == null)
            throw new ArgumentNullException("source");

        if (target == null)
            throw new ArgumentNullException("target");

        int idx = IndexOf(str, source);
        //int idx = str.IndexOf(source);
        if (idx == -1)
            return str;

        m_FindsIndex.Clear();
        m_FindsIndex.Add(idx);

        // find all the indicies beforehand
        while (idx + source.Length < str.Length)
        {
            idx = IndexOf(str, source, idx + source.Length);
            //idx = str.IndexOf(source, idx + source.Length);
            if (idx == -1)
                break;
            m_FindsIndex.Add(idx);
        }

        // calc the right new total length
        int new_len;
        int dif = source.Length - target.Length;
        if (dif > 0)
            new_len = str.Length - (m_FindsIndex.Count * dif);
        else
            new_len = str.Length + (m_FindsIndex.Count * -dif);

        var cache = GetCacheString(new_len);
        fixed (char* ptr_this = str)
        {
            fixed (char* ptr_result = cache)
            {
                for (int i = 0, x = 0, j = 0; i < new_len;)
                {
                    if (x == m_FindsIndex.Count || m_FindsIndex[x] != j)
                    {
                        ptr_result[i++] = ptr_this[j++];
                    }
                    else
                    {
                        for (int n = 0; n < target.Length; n++)
                            ptr_result[i + n] = target[n];

                        x++;
                        i += target.Length;
                        j += source.Length;
                    }
                }
            }
        }
        return cache;
    }

    /// <summary>
    /// 连接字符串，最大长度为1024,不支持多线程，不能缓存字符串
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <param name="str3"></param>
    /// <returns></returns>
    public static string ConcatNoAlloc(string str1,string str2,string str3=null,string str4 = null)
    {
        var len = str1.Length + str2.Length;
        if (str3 != null) len += str3.Length;
        if (str4 != null) len += str4.Length;

        Span<char> sChar = stackalloc char[len];
        var vstr = new ValueStringBuilder(sChar);
        vstr.Append(str1); 
        vstr.Append(str2);
        if (str3 != null)
            vstr.Append(str3);
        if(str4 != null)
            vstr.Append(str4);

        var srcSpan = vstr.AsSpan();
        var cache = GetCacheString(len);
        unsafe
        {
            fixed (char* source = srcSpan)
            fixed (char* target = cache)
            {
                Unsafe.CopyBlock(target, source, (uint)srcSpan.Length * sizeof(char));
            }
        }
        vstr.Dispose();
        return cache;
    }

    /// <summary>
    /// 连接字符串，最大长度为1024
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <param name="str3"></param>
    /// <returns></returns>
    public static string Concat(string str1, string str2, string str3 = null,string str4 = null)
    {
        var len = str1.Length + str2.Length;
        if (str3 != null) len += str3.Length;
        if (str4 != null) len += str4.Length;

        Span<char> sChar = stackalloc char[len];
        var vstr = new ValueStringBuilder(sChar);
        vstr.Append(str1);
        vstr.Append(str2);
        if (str3 != null)
            vstr.Append(str3);
        if (str4 != null)
            vstr.Append(str4);

        return vstr.ToString();
    }

    /// <summary>
    /// 连接字符串，最大长度为1024，不支持多线程，不能缓存字符串
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string ConcatNoAlloc(params string[] args)
    {
        var len = 0;
        foreach(var s in args)
        {
            len += s.Length;
        }

        Span<char> sChar = stackalloc char[len];
        var vstr = new ValueStringBuilder(sChar);
        foreach (var s in args)
            vstr.Append(s);

        var srcSpan = vstr.AsSpan();
        var cache = GetCacheString(len);
        unsafe
        {
            fixed (char* source = srcSpan)
            fixed (char* target = cache)
            {
                Unsafe.CopyBlock(target, source, (uint)srcSpan.Length * sizeof(char));
            }
        }
        vstr.Dispose();
        return cache;

    }

    /// <summary>
    /// 连接字符串，最大长度为1024
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Concat(params string[] args)
    {
        var len = 0;
        foreach (var s in args)
        {
            len += s.Length;
        }

        Span<char> sChar = stackalloc char[len];
        var vstr = new ValueStringBuilder(sChar);
        foreach (var s in args)
            vstr.Append(s);
        return vstr.ToString();

    }

    /// <summary>
    /// 直接替换string char为小写，会改变原始string 谨慎使用。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static unsafe string ToLowerNoAlloc(string str)
    {

        fixed (char* s = str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                if (char.IsUpper(ch))
                    s[i] = char.ToLower(ch);
            }
        }
        return str;
    }
}
