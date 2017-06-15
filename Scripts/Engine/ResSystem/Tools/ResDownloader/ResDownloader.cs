//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Qarth
{
    public interface IDownloadTask
    {
        string localResPath
        {
            get;
        }

        bool needDownload
        {
            get;
        }

        string url
        {
            get;
        }

        int fileSize
        {
            get;
        }


        void SetDownloadProgress(int totalSize, int download);

        void DeleteOldResFile();

        void OnDownLoadResult(bool result);
    }

    //资源下载器
    public class ResDownloader : TSingleton<ResDownloader>
    {
        private Dictionary<string, IDownloadTask> m_AllDownloadTaskMap = new Dictionary<string, IDownloadTask>();
        private List<IDownloadTask> m_WaitDownloadList = new List<IDownloadTask>();
        private IDownloadTask m_DownloadingTask;//由于当前HttpDownloaderMgr只支持一个

        public bool HasDownloadTask(IDownloadTask res)
        {
            if (m_AllDownloadTaskMap.ContainsKey(res.localResPath))
            {
                return true;
            }
            return false;
        }

        public void AddDownloadTask(IDownloadTask res)
        {            
            if (res == null)
            {
                return;
            }

            //res.ResState = eResState.kWaitDownload;

            m_AllDownloadTaskMap.Add(res.localResPath, res);
            m_WaitDownloadList.Add(res);

            TryStartNextTask();
        }

        public bool RemoveDownloadTask(IDownloadTask res)
        {
            if (res == null)
            {
                return true;
            }

            //当前无法取消正在下载的资源
            /*
            if (res.ResState == eResState.kDownloading)
            {
                return false;
            }
            */

            if (!m_AllDownloadTaskMap.ContainsKey(res.localResPath))
            {
                return true;
            }

            m_AllDownloadTaskMap.Remove(res.localResPath);
            m_WaitDownloadList.Remove(res);
            return true;
        }

        #region
        protected IDownloadTask PopNextTask()
        {
            for (int i = m_WaitDownloadList.Count - 1; i >= 0; --i)
            {
                var res = m_WaitDownloadList[i];

                if (res.needDownload)
                {
                    m_WaitDownloadList.RemoveAt(i);
                    return res;
                }

                m_WaitDownloadList[i].OnDownLoadResult(false);
                m_AllDownloadTaskMap.Remove(res.localResPath);
                m_WaitDownloadList.RemoveAt(i);

                //res.ResState = eResState.kNull;
            }
            return null;
        }

        protected void TryStartNextTask()
        {
            if (m_DownloadingTask != null)
            {
                WWWDownloader.S.Clear();
                return;
            }

            if (m_WaitDownloadList.Count == 0)
            {
                WWWDownloader.S.Clear();
                return;
            }

            IDownloadTask next = PopNextTask();
            if (next == null)
            {
                WWWDownloader.S.Clear();
                return;
            }

            //next.ResState = eResState.kDownloading;

            m_DownloadingTask = next;

            //HttpDownloaderMgr.S.AddDownloadTask(next.url, next.localResPath, OnDownloadProgress, OnDownloadError, OnDownloadFinish, null);
            WWWDownloader.S.AddDownloadTask(next.url, next.localResPath, next.fileSize, OnDownloadProgress, OnDownloadError, OnDownloadFinish, null);

            next.DeleteOldResFile();
        }

        private void RemoveTask(IDownloadTask res)
        {
            if (res == null)
            {
                return;
            }
            m_AllDownloadTaskMap.Remove(res.localResPath);
        }

        private void OnDownloadProgress(int download, int totalFileLenght)
        {
            if (m_DownloadingTask == null)
            {
                return;
            }

            m_DownloadingTask.SetDownloadProgress(totalFileLenght, download);
        }

        private void OnDownloadError(string errorMsg)
        {
            if (m_DownloadingTask == null)
            {
                TryStartNextTask();
                return;
            }

            Log.i("ResDownloader: Downloading Error:" + errorMsg);
            RemoveTask(m_DownloadingTask);
            m_DownloadingTask.OnDownLoadResult(false);
            m_DownloadingTask = null;

            TryStartNextTask();
        }

        private void OnDownloadFinish(string fileName, int download, int totalFileLenght)
        {
            if (m_DownloadingTask == null)
            {
                Log.e("ResDownloader: Error, Current Res Begin Download Is Null...");
                TryStartNextTask();
                return;
            }

            if (fileName != m_DownloadingTask.localResPath)
            {
                Log.e("ResDownloader: Error, Not Current Res Begin Download...");
                m_DownloadingTask = null;
                TryStartNextTask();
                return;
            }

            //Log.i("ResDownloader: Downloading Success:" + fileName);
            RemoveTask(m_DownloadingTask);

            m_DownloadingTask.OnDownLoadResult(true);
            m_DownloadingTask = null;
            TryStartNextTask();
        }

        #endregion
    }
}

