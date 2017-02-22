// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using System.Collections.Generic;

namespace Hugula.Loader
{

	[SLua.CustomLuaClass]
    public class GroupRequestRecord
    {
        public System.Action<object> onGroupComplate;

		public System.Action<LoadingEventArg> onGroupProgress;

        HashSet<CRequest> groupRes = new HashSet<CRequest>();

		public void Progress()
		{
			arg.total = Total;
			arg.current = Total - Count;
			if (onGroupProgress != null)
				onGroupProgress (arg);
		}

		/// <summary>
		/// The length.
		/// </summary>
		public int Total = 0;

        public void Add(CRequest req)
        {
            groupRes.Add(req);
			Total++;
        }

        public void Complete(CRequest req)
        {
            groupRes.Remove(req);
        }

		/// <summary>
		/// 当前数量
		/// </summary>
		/// <value>The count.</value>
        public int Count
        {
            get
            {
                return groupRes.Count;
            }
            set
            {
                groupRes.Clear();
				Total = 0;
				arg.current = 0;
				arg.progress = 0;
				arg.total = 0;
            }
        }

		private LoadingEventArg arg = new LoadingEventArg();
    }
}