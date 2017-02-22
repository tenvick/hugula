// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using Hugula.Utils;

namespace Hugula.Loader
{

    [SLua.CustomLuaClass]
    public class LRequestPool
    {
        static ObjectPool<LRequest> pool = new ObjectPool<LRequest>(m_ActionOnGet, m_ActionOnRelease);

        static public int countAll { get{return pool.countAll;} }
        static public int countActive { get { return countAll - countInactive; } }
        static public int countInactive { get { return pool.countInactive; } }

        private static void m_ActionOnGet(LRequest req)
        {
            req.pool = true;
            req.BindAction();
        }

        private static void m_ActionOnRelease(LRequest req)
        {
            req.Dispose();
        }

        public static LRequest Get()
        {
            return pool.Get();
        }

        public static void Release(CRequest toRelease)
        {
            if(toRelease is LRequest)
                pool.Release((LRequest)toRelease);
        }
    }


    public class GroupRequestRecordPool
    {
        static ObjectPool<GroupRequestRecord> pool = new ObjectPool<GroupRequestRecord>(null, m_ActionOnRelease);

        private static void m_ActionOnRelease(GroupRequestRecord re)
        {
            re.Count = 0;
            re.onGroupComplate = null;
			re.onGroupProgress = null;
        }

        public static GroupRequestRecord Get()
        {
            return pool.Get();
        }

        public static void Release(GroupRequestRecord toRelease)
        {
            pool.Release(toRelease);
        }
    }


	public class CacheDataPool
	{
		static ObjectPool<CacheData> pool = new ObjectPool<CacheData>(null, m_ActionOnRelease);

        static public int countAll { get{return pool.countAll;} }
        static public int countActive { get { return countAll - countInactive; } }
        static public int countInactive { get { return pool.countInactive; } }


        private static void m_ActionOnGet(CacheData cd)
        {
            // cd.Dispose();
        }
        private static void m_ActionOnRelease(CacheData cd)
        {
            cd.Dispose();
        }

        public static CacheData Get()
        {
            return pool.Get();
        }

        public static void Release(CacheData toRelease)
        {
            pool.Release(toRelease);
        }
	}


}