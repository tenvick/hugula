// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using Hugula.Utils;

namespace Hugula.Loader
{
	 /// <summary>
    /// 计数器管理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CountMananger
    {
        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static bool Subtract(int hashcode)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count--;// = cached.count - 1;
                if (cached.count <= 0) //所有引用被清理。
                {
                    CacheManager.ClearCache(hashcode);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Subtract(string key)
        {
            int hashcode =  LuaHelper.StringToHash(key);
            return Subtract(hashcode);
        }

        /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static int Add(int hashcode)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count++;//= cached.count + 1;
                return cached.count;
            }
            return -1;
        }

         /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Add(string key)
        {
            int hashcode =  LuaHelper.StringToHash(key);
            return Add(hashcode);
        }

        /// <summary>
        /// 目标引用加n
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        internal static int Add(int hashcode, int add)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count += add;//= cached.count + 1;
                return cached.count;
            }
            return -1;
        }

		/// <summary>
		/// Adds the dependencies.
		/// </summary>
		/// <param name="hashcode">Hashcode.</param>
		internal static void AddDependencies(int hashcode)
		{
			CacheData cached = CacheManager.GetCache(hashcode);
			if (cached != null && cached.allDependencies!=null)
			{
				foreach (int hash in cached.allDependencies)
					Add (hash);
			}
		}
    }
}