using UnityEngine;
using System.Collections.Generic;

namespace Hugula.Editor.Task
{
    public static class TaskManager<T>
    {

        static List<ITask<T>> tasks = new List<ITask<T>>();

        public static void AddTask(ITask<T> task)
        {
            tasks.Add(task);
        }

        public static void Run(T sharedata,System.Action<float,string> onProcess)
        {
			ITask<T> task;
			int count = tasks.Count;
            for(int i=0;i<count;i++)
			{
				task = tasks[i];
				if(onProcess!=null)onProcess( (float)i/(float)count,task.GetName());
				task.Run(sharedata);
			}
        }

        public static void Clear()
        {
            tasks.Clear();
        }

    }


    public interface ITask<T>
    {
        void Run(T sharedata);

		string GetName();
    }
}