using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PTGame.Framework
{
    public class HotUpdateRes : AbstractRes, IDownloadTask
    {
        private string  m_Url;
        private int     m_TotalSize = 1;
        private int     m_DownloadSize;
        private bool    m_IsCacheMode = false;
        private string  m_LocalPath;

        public static HotUpdateRes Allocate(string name)
        {
            HotUpdateRes res = ObjectPool<HotUpdateRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
            }
            return res;
        }

        public void SetUpdateInfo(string localPath, string url, bool cacheMode)
        {
            m_LocalPath = localPath;
            m_IsCacheMode = cacheMode;
            m_Url = url;
        }

        public string localResPath
        {
            get
            {
                if (m_IsCacheMode)
                {
                    return FilePath.persistentDownloadCachePath + m_LocalPath;
                }
                return destionResPath;
            }
        }

        public string destionResPath
        {
            get { return FilePath.persistentDataPath4Res + m_LocalPath; }
        }

        public void MoveCacheFile2Destion()
        {
            if (!m_IsCacheMode)
            {
                return;
            }

            if (resState != eResState.kReady)
            {
                return;
            }

            if (!File.Exists(localResPath))
            {
                return;
            }

            File.Move(localResPath, destionResPath);
        }

        public void SetDownloadProgress(int totalSize, int download)
        {
            m_TotalSize = totalSize + 1;
            m_DownloadSize = download;
            Log.i(string.Format("#>> {0}:{1}", m_Name, CalculateProgress()));
        }

        public bool needDownload
        {
            get
            {
                return refCount > 0;
            }
        }

        public string url
        {
            get
            {
                return m_Url;
            }
        }

        public override bool UnloadImage(bool flag)
        {
            return false;
        }

        public override bool LoadSync()
        {
            return false;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(m_Url))
            {
                return;
            }

            if (string.IsNullOrEmpty(m_LocalPath))
            {
                return;
            }

            DoLoadWork();
        }

        private void DoLoadWork()
        {
            resState = eResState.kLoading;

            m_DownloadSize = 0;
            m_TotalSize = 1;
            //OnDownLoadResult(true);

            if (m_IsCacheMode)
            {
                if (File.Exists(localResPath))
                {
                    //如果cache中文件存在，则删除,避免断点续传
                    File.Delete(localResPath);
                }
            }

            ResDownloader.S.AddDownloadTask(this);
        }

        protected override void OnReleaseRes()
        {
            if (File.Exists(localResPath))
            {
                //如果cache中文件存在，则删除
                File.Delete(localResPath);
            }
        }

        public override void Recycle2Cache()
        {
            ObjectPool<HotUpdateRes>.S.Recycle(this);
        }

        public override void OnCacheReset()
        {
            m_LocalPath = null;
            m_IsCacheMode = false;
            m_Url = null;
        }

        public void DeleteOldResFile()
        {
            //throw new NotImplementedException();
        }

        public void OnDownLoadResult(bool result)
        {
            if (!result)
            {
                OnResLoadFaild();
                return;
            }

            if (refCount <= 0)
            {
                resState = eResState.kWaiting;
                return;
            }

            resState = eResState.kReady;
        }

        protected override float CalculateProgress()
        {
            return m_DownloadSize / (float)m_TotalSize;
        }
    }
}
