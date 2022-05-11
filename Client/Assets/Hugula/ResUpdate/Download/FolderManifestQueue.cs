using System.Collections.Generic;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;
using System.Net;

namespace Hugula.ResUpdate
{

    public class FolderManifestQueue : IReleaseToPool
    {

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

        public long totalBytesToReceive
        {
            get;
            internal set;
        }

        private long m_BytesReceived;

        public long bytesReceived
        {
            get
            {
                if (m_Dirty)
                {
                    m_BytesReceived = 0;
                    List<int> calc = Hugula.Utils.ListPool<int>.Get();
                    calc.Clear();
                    calc.InsertRange(0,fileReceiveBytes.Values);
                    foreach (var i in calc)
                    {
                        m_BytesReceived += i;
                    }
                    Hugula.Utils.ListPool<int>.Release(calc);
                }
                return m_BytesReceived;
            }
            // internal set;
        }
        //是否需要计算BytesReceived
        private bool m_Dirty = false;
        // public 
        protected Queue<FileResInfo> groupRes = new Queue<FileResInfo>();
        protected List<FileResInfo> loadingRes = new List<FileResInfo>();

        //下载失败的文件
        public List<FileResInfo> errorFiles = new List<FileResInfo>();

        //记录每个文件下载bytes
        protected SafeDictionary<string, int> fileReceiveBytes = new SafeDictionary<string, int>();

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

        public FolderQueueGroup SetFolder(FolderManifest folder, FolderQueueGroup group)
        {
            this.currFolder = folder;
            this.priority = folder.priority;
            this.isError = false;
            group.AddChild(this);
            totalBytesToReceive = folder.totalSize;
            m_Dirty = true;
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
        public bool ReEnqueueLoadError()
        {
            if (errorFiles.Count == 0) return false;

            this.isError = false;
            var allFileInfos = currFolder.allFileInfos;
            m_Dirty = true;
            FileResInfo item;
            for (int i = 0; i < errorFiles.Count; i++)
            {
                item = errorFiles[i];
                groupRes.Enqueue(item);
                fileReceiveBytes[item.name] = 0;//需要重新下载
            }
            errorFiles.Clear();
            return true;
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
            else
            {

            }
            // Debug.LogFormat("i={0},totalCount={1},loadedCount={2},IsDown={3},req={4},isErr={5}",i,totalCount,loadedCount,IsDown,req,isError);
            if (down)
                DispatchOnComplete();

            return down;
        }

        public void OnDownloadProgressChanged(FileResInfo fileResInfo, DownloadingProgressChangedEventArgs e)
        {
            // bytesReceived += e.BytesRead;
            AddReceiveBytes(fileResInfo.name, (int)e.BytesReceived);
            m_Dirty = true;
            group.OnDownloadProgressChanged(fileResInfo, this, e);
        }


        protected virtual void DispatchOnComplete()
        {
            group?.DispatchChildComplete(this);
            // if (onComplete != null)
            //     onComplete(this, isError);
        }

        protected void AddReceiveBytes(string fName, int bytesRead)
        {
            lock (this)
            {
                fileReceiveBytes[fName] = bytesRead;
            }
        }
        public void ReleaseToPool()
        {

        }

        // public static ObjectPool<FolderManifestQueue> 
    }


    public class FolderQueueGroup
    {
        int frameCount = 0; // 
        public FolderQueueGroup(System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onItemComplete, System.Action<FolderQueueGroup, bool> onAllComplete)
        {
            totalBytesToReceive = 0;
            this.onProgress = onProgress;
            this.onItemComplete = onItemComplete;
            this.onAllComplete = onAllComplete;
        }

        public System.Action<LoadingEventArg> onProgress;
        public System.Action<FolderManifestQueue, bool> onItemComplete;
        public System.Action<FolderQueueGroup, bool> onAllComplete;

        public List<FolderManifestQueue> children = new List<FolderManifestQueue>();
        public LoadingEventArg loadingEventArg = new LoadingEventArg();

        public void OnDownloadProgressChanged(FileResInfo finfo, FolderManifestQueue queue, DownloadingProgressChangedEventArgs e)
        {
            // queue.lo
            // bytesReceived += e.BytesRead;
            // loadingEventArg.current = queue.bytesReceived;
            // Debug.Log($"FolderGroup({this.GetHashCode()}) OnDownloadProgressChanged：{finfo.name}:e.BytesRead{e.BytesRead},currentReceived={loadingEventArg.current},fileszie：{finfo.size},total={loadingEventArg.total};{System.DateTime.Now}");
        }

        public void DispatchProgressChanged()
        {
            if (onProgress != null)
            {
                if (frameCount != Time.frameCount)
                {
                    frameCount = Time.frameCount;
                    FolderManifestQueue folderManifestQueue;
                    long bytesReceived = 0;
                    for (int i = 0; i < children.Count; i++)
                    {
                        folderManifestQueue = children[i];
                        bytesReceived += folderManifestQueue.bytesReceived;
                    }
                    loadingEventArg.current = bytesReceived;
                    onProgress(loadingEventArg);
                }
            }
        }

        public void DispatchChildComplete(FolderManifestQueue child)
        {
            if (onItemComplete != null)
            {
                onItemComplete(child, child.isError);
            }
            if (!child.isError && child.currFolder.transformZipFolder && child.currFolder.isZipDone)
            {
                var fastManifest = FileManifestManager.FindStreamingFolderManifest(child.currFolder.folderName);
                if (fastManifest != null)
                    FileManifestManager.GenZipPackageTransform(fastManifest);

                FileManifestManager.ClearOverrideAddressTransformFunc(child.currFolder.folderName);
            }
            if (isAllDown && onAllComplete != null)
            {
                onAllComplete(this, anyError);
            }
        }

        public void ReEnqueueLoadError()
        {
            FolderManifestQueue folderManifestQueue;
            for (int i = 0; i < children.Count; i++)
            {
                folderManifestQueue = children[i];
                folderManifestQueue.ReEnqueueLoadError();
            }
        }

        public long totalBytesToReceive
        {
            get
            {
                return loadingEventArg.total;
            }
            internal set
            {
                loadingEventArg.total = value;
            }
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