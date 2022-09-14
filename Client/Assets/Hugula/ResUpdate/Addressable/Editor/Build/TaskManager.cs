using System.Collections;
using System.Collections.Generic;

namespace HugulaEditor.ResUpdate
{   
    //构建流水线
    public static class TaskManager<T>
    {
        static List<ITask<T>> tasks = new List<ITask<T>>();

        public static void AddTask(ITask<T> task)
        {
            tasks.Add(task);
            // tasks.Sort((a,b)=>
            // {
            //     return a.priority - b.priority;
            // });
        }

        public static void Run(T sharedata,System.Action<float,string> onProcess=null)
        {
			ITask<T> task;
			int count = tasks.Count;
            for(int i=0;i<count;i++)
			{
				task = tasks[i]; 
                try{
                    UnityEngine.Debug.Log($"Run Task ({task.name}) time:{System.DateTime.Now}");
                    task.Run(sharedata);
                    Hugula.Utils.CUtils.DebugCastTime($"Run Task End ({task.name})");
                    if(onProcess!=null)onProcess( (float)i/(float)count,task.name);
                }catch(System.Exception ex)
                {
                     UnityEngine.Debug.LogException(ex);
                    UnityEngine.Debug.LogError(ex.Message+ex.ToString());
                }
                finally
                {

                }
			}
            UnityEditor.EditorUtility.ClearProgressBar();

        }

        public static void Clear()
        {
            tasks.Clear();
        }
    }


    public interface ITask<T>
    {
        string name{get;}
        int priority {get;}
        void Run(T data);
    }
}
