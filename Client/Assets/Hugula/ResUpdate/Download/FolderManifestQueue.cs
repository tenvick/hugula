using System.Collections.Generic;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;
using System.Net;

namespace Hugula.ResUpdate
{

    public class FolderManifestQueue : IReleaseToPool
    {

        public System.Action<LoadingEventArg> onProgress;
        public System.Action<FolderManifestQueue, bool> onComplete;

        public int priority;

        public FolderQueueGroup group
        {
            get;
            internal set;
        }
        public bool isError
        {
            get; private set;
        }

        public FolderManifest currFolder
        {
            get;
            private set;
        }
        protected Queue<FileResInfo> groupRes = new Queue<FileResInfo>();
        protected List<FileResInfo> loadingRes = new List<FileResInfo>();

        //下载失败的文件
        public List<FileResInfo> errorFiles = new List<FileResInfo>();

        public int LoadingCount
        {
            get
            {
                return this.loadingRes.Count;
            }
        }

        public bool isEmpty
        {
            get
            {
                return groupRes.Count == 0;
            }
        }

        public bool isDown
        {
            get
            {
                return (groupRes.Count == 0 && LoadingCount == 0);
            }
        }

        public FolderQueueGroup SetFolder(FolderManifest folder, FolderQueueGroup group, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onComplete)
        {
            this.currFolder = folder;
            this.onComplete = onComplete;
            this.onProgress = onProgress;
            this.priority = folder.priority;
            this.isError = false;

            if (group == null)
            {
                group = new FolderQueueGroup();
            }

            group.AddChild(this);

#if !HUGULA_NO_LOG 
            // Debug.Log($"进度信息:{folder}  totalBytesToReceive:{this.group.totalBytesToReceive} ");
#endif
            var allFileInfos = folder.allFileInfos;
            groupRes.Clear();
            loadingRes.Clear();
            for (int i = 0; i < allFileInfos.Count; i++)
            {
                groupRes.Enqueue(allFileInfos[i]);
            }

            return group;
        }

        //重新下载错误文件
        public void ReLoadError()
        {
            if (errorFiles.Count == 0) return;

            this.isError = false;
            var allFileInfos = currFolder.allFileInfos;
            FileResInfo item;
            for (int i = 0; i < errorFiles.Count; i++)
            {
                item = errorFiles[i];
                groupRes.Enqueue(item);
            }
            errorFiles.Clear();
        }

        ///<summary>
        /// 取出加载文件信息，并放入loading列表
        ///</summary>
        public FileResInfo Dequeue()
        {
            if (this.groupRes.Count > 0)
            {
                var req = groupRes.Dequeue();
                loadingRes.Add(req);
                return req;
            }
            return null;
        }

        public bool Complete(FileResInfo req, bool isError)
        {
            var idx = loadingRes.IndexOf(req);
            if (idx >= 0)
            {
                loadingRes.RemoveAt(idx);
            }

            var down = isDown;

            if (isError)
            {
                errorFiles.Add(req); //下载失败
                this.isError = true;
            }
            // Debug.LogFormat("i={0},totalCount={1},loadedCount={2},IsDown={3},req={4},isErr={5}",i,totalCount,loadedCount,IsDown,req,isError);
            if (down)
                DispatchOnComplete();

            return down;
        }

        public void OnDownloadProgressChanged(FileResInfo fileResInfo, DownloadingProgressChangedEventArgs e)
        {
            group.OnDownloadProgressChanged(fileResInfo, e);
        }

        public void DispatchProgressChanged()
        {
            if (onProgress != null)
            {
                onProgress(group.loadingEventArg);
            }
        }

        protected virtual void DispatchOnComplete()
        {
            if (onProgress != null)
            {
                onProgress(group.loadingEventArg);
            }
            if (onComplete != null)
                onComplete(this, isError);
        }

        public void ReleaseToPool()
        {

        }

        // public static ObjectPool<FolderManifestQueue> 
    }


    public class FolderQueueGroup
    {
        public FolderQueueGroup()
        {
            totalBytesToReceive = 0;
            bytesReceived = 0;
        }

        public List<FolderManifestQueue> children = new List<FolderManifestQueue>();
        public LoadingEventArg loadingEventArg = new LoadingEventArg();

        public void OnDownloadProgressChanged(FileResInfo finfo, DownloadingProgressChangedEventArgs e)
        {
            bytesReceived += e.BytesRead;
            loadingEventArg.current = bytesReceived;
            // Debug.Log($"FolderGroup({this.GetHashCode()}) OnDownloadProgressChanged：{finfo.name}:e.BytesRead{e.BytesRead},currentReceived={loadingEventArg.current},fileszie：{finfo.size},total={loadingEventArg.total};{System.DateTime.Now}");
        }

        //重新设置大小
        public void ResetSize(long total)
        {
            loadingEventArg.total = total;
            bytesReceived = 0;
        }

        public long totalBytesToReceive
        {
            get
            {
               return loadingEventArg.total;
            }
            internal set{
                loadingEventArg.total = value;
            }
        }
        public long bytesReceived
        {
            get;
            internal set;
        }

        internal void AddChild(FolderManifestQueue child)
        {
            children.Add(child);
            child.group = this;
            totalBytesToReceive += child.currFolder.totalSize;
        }

        public bool anyError
        {
            get
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].isError) return true;
                }
                return false;
            }
        }


        public bool isAllDown
        {
            get
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (!children[i].isDown) return false;
                }
                return true;
            }
        }

    }
}