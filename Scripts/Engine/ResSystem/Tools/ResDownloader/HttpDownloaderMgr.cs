//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Net;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace Qarth
{
    //http下载管理器
    [TMonoSingletonAttribute("[Tools]/HttpDownloader")]
    public class HttpDownloaderMgr : TMonoSingleton<HttpDownloaderMgr>
    {
        public static string TimeOutError = "Time-Out";
        enum DownloadEvent
        {
            Begin = 0,      //开始
            Error = 1,      // 出错
            Progress = 2,   // 进度
            Finish = 3,     // 下载完毕
            DownloadOrExit = 4 // 如果不是wifi状态，要下载的目标文件大于指定值，提示用户继续或者退出
        }

        private const int BUFFER_SIZE = 1024 * 200;
        private const int TIME_OUT = 10;

        // 用于和网络子线程交互
        private List<DownloadEvent> m_Event = new List<DownloadEvent>();
        string m_ErrorMsg = string.Empty;

        private string m_Uri, m_SaveFile;

        private event OnDownloadFinished m_OnFinished;
        private event OnDownloadError m_OnError;
        private event OnDownloadProgress m_OnProgress;
        private event OnDownloadBegin m_OnDownloadBegin;

        byte[] m_Buffer = new byte[BUFFER_SIZE];
        const string m_RequestLock = "WebRequestLock";
        HttpWebRequest m_Request;
        // 将返回数据写入本地文件流
        FileStream m_FileStream;
        // http请求响应
        HttpWebResponse m_WriteResponse;
        // 要下载文件的长度
        int m_FileLength = 0;
        // 当前已经下了多少字节
        int m_CurrentDownloadByte = 0;
        int m_StartPosition = 0;
        // 临时文件名
        string m_TmpFile;

        // 是否正在下载
        bool m_IsDownloading = false;
        // 当前是否正在使用wifi
        bool m_UseWifi = true;
        // 当前正在下载的任务数
        int m_TaskCount = 0;
        // 最后一次网络返回，用于超时
        long m_LastResponseTime = 0;

        // 在等待是否下载和退出
        bool m_IsWaitDownloadOrExit;

        public int alreadyDownloadByte
        {
            get { return m_CurrentDownloadByte; }
        }

        public string targetFile
        {
            get { return m_SaveFile; }
        }

        // 添加下载任务，目前只支持一个任务同时进行
        public bool AddDownloadTask(string uri, string localPath, OnDownloadProgress onProgress, OnDownloadError onError, OnDownloadFinished onFinshed, OnDownloadBegin onBegin = null)
        {
            if (m_IsDownloading)
            {
                Log.e("HttpDownloaderMgr is busy!");
                return false;
            }

            if (string.IsNullOrEmpty(uri) == true)
            {
                Log.e("uri is empty");
                return false;
            }

            if (string.IsNullOrEmpty(localPath) == true)
            {
                Log.e("LocalPath is empty");
                return false;
            }

            if (onError == null || onFinshed == null)
            {
                Log.e("onError & onFinshed should not be null!");
                return false;
            }

            m_OnProgress = onProgress;
            m_OnError = onError;
            m_OnFinished = onFinshed;
            m_OnDownloadBegin = onBegin;

            m_Uri = uri;
            m_SaveFile = localPath;

            m_TaskCount++;

            //Log.i("[HttpDownload]about to download new data:" + m_Uri);

            return true;
        }

        public void WorkForground()
        {
            if (m_IsWaitDownloadOrExit)
            {
                ShowDownloadOrExitPanel();
            }
        }

        void Update()
        {
            ProcessEvent();
            if (m_IsDownloading)
            {
                long diffTime = DateTime.Now.Ticks / 10000000 - m_LastResponseTime;
                if (diffTime > TIME_OUT)
                {
                    HandleError(TimeOutError);
                    return;
                }
            }

            if (m_IsDownloading || m_TaskCount == 0)
            {
                return;
            }

            m_TaskCount--;
            m_IsDownloading = true;
            InitDownloadInfo();
            AsyncDownfile();
        }

        private void ProcessEvent()
        {
            DownloadEvent downloadEvent;
            string errorMsg;
            lock (m_Event)
            {
                if (m_Event.Count == 0)
                {
                    return;
                }

                downloadEvent = m_Event[0];
                m_Event.RemoveAt(0);
                errorMsg = string.Format("{0},{1}", m_ErrorMsg, m_Uri);
            }

            if (downloadEvent == DownloadEvent.Error)
            {
                Log.e(errorMsg);

                m_IsDownloading = false;

                if (m_Request != null)
                {
                    m_Request.Abort();
                    m_Request = null;
                }

                if (m_FileStream != null)
                {
                    m_FileStream.Close();
                }

                if (m_OnError != null)
                {
                    m_OnError(errorMsg);
                }
            }
            else if (downloadEvent == DownloadEvent.Begin)
            {
                if (m_OnDownloadBegin != null)
                {
                    m_OnDownloadBegin(m_FileLength);
                    m_OnDownloadBegin = null;
                }
            }
            else if (downloadEvent == DownloadEvent.Progress)
            {
                if (m_OnProgress != null)
                {
                    m_OnProgress(m_CurrentDownloadByte, m_FileLength);
                }
            }
            else if (downloadEvent == DownloadEvent.Finish)
            {
                if (m_OnFinished != null)
                {
                    m_IsDownloading = false;
                    m_OnFinished(m_SaveFile, m_CurrentDownloadByte, m_FileLength);
                }
            }
            else if (downloadEvent == DownloadEvent.DownloadOrExit)
            {
                m_IsWaitDownloadOrExit = true;
            }
        }

        private void InitDownloadInfo()
        {
            m_Request = null;
            // 将返回数据写入本地文件流
            m_FileStream = null;
            // 要下载文件的长度
            m_FileLength = 0;
            // 当前已经下了多少字节
            m_CurrentDownloadByte = 0;
            m_StartPosition = 0;
            // 临时文件名
            m_TmpFile = null;
            m_LastResponseTime = 0;
            m_ErrorMsg = string.Empty;
            m_Event.Clear();
            m_IsWaitDownloadOrExit = false;
        }

        private void UpdateTimeOut()
        {
            long tick = DateTime.Now.Ticks;
            m_LastResponseTime = tick / 10000000;
        }

        // 同步下载（非断点续传）
        private void AsyncDownfile()
        {
            m_UseWifi = !(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
            // 创建本地文件
            m_TmpFile = m_SaveFile + ".temp";

            CheckLocalFile();

            if (m_StartPosition < 0)
            {
                HandleError("CheckLocalFile fail");
                return;
            }

            UpdateTimeOut();

            Thread startRequest = new Thread(StartHttpWebRequest);
            startRequest.Start();
        }

        private void HandleError(string errorMsg)
        {
            lock (m_Event)
            {
                m_Event.Clear();
                m_Event.Add(DownloadEvent.Error);
                m_ErrorMsg = errorMsg;
            }
        }

        // 检查本地是否已有文件。有则断点续传。
        private void CheckLocalFile()
        {
            m_StartPosition = -1;
            try
            {
                if (File.Exists(m_TmpFile))
                {
                    m_FileStream = File.OpenWrite(m_TmpFile);
                    m_StartPosition = (int)m_FileStream.Length;
                    if (m_StartPosition > 0)
                    {
                        m_StartPosition -= 1;
                    }
                    //Log.i("exist tmp file:" + m_TmpFile + ", m_StartPosition:" + m_StartPosition + ", IsAsync:" + m_FileStream.IsAsync);
                    m_FileStream.Seek(m_StartPosition, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
                }
                else
                {

                    string folder = m_TmpFile.Substring(0, m_TmpFile.LastIndexOf('/'));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    m_FileStream = new FileStream(m_TmpFile, FileMode.Create);
                    m_StartPosition = 0;
                    //Log.i("NOT exist tmp file:" + m_TmpFile + ", IsAsync:" + m_FileStream.IsAsync);
                }
            }
            catch (Exception exception)
            {
                if (m_FileStream != null)
                {
                    m_FileStream.Close();
                }
                Log.e("CheckLocalFile error:" + exception.Message);
            }
        }

        private void OnResponeCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // 请求已经失效
            if (null == m_Request || request != m_Request)
            {
                return;
            }

            //Log.i("[HttpDownload]connect responed.");

            try
            {
                m_WriteResponse = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                if ((int)m_WriteResponse.StatusCode >= 300)
                {
                    Log.e("StatusCode=" + m_WriteResponse.StatusCode + ", desc=" + m_WriteResponse.StatusDescription);

                    HandleError(m_WriteResponse.StatusDescription);

                    return;
                }
            }
            catch (Exception exception)
            {
                HandleError("#OnResponeCallback" + exception.Message);
                return;
            }

            UpdateTimeOut();

            // 要下载文件长度
            m_FileLength = (int)m_WriteResponse.ContentLength + m_StartPosition;
            m_CurrentDownloadByte = m_StartPosition;

            // 启动回调
            lock (m_Event)
            {
                m_Event.Add(DownloadEvent.Begin);
            }

            //当用户使用移动网络时
            int limit = 1024 * 1024;//ServerConfigMgr.S.nonWifiLimit; // >指定值，提示玩家是否下载
            if (!m_UseWifi && m_FileLength > limit)
            {
                lock (m_Event)
                {
                    m_Event.Add(DownloadEvent.DownloadOrExit);
                    m_IsDownloading = false;
                }
            }
            else
            {
                ReadData(m_WriteResponse);
            }
        }

        private void ReadData(HttpWebResponse writeResponse)
        {
            // 开始读数据
            try
            {
                // 开始读数据
                Stream responseStream = writeResponse.GetResponseStream();
                responseStream.BeginRead(m_Buffer, 0, BUFFER_SIZE, new AsyncCallback(OnReadCallback), responseStream);
            }
            catch (Exception exception)
            {
                HandleError("#ReadData:" + exception.Message);
            }
        }

        // 读取http返回数据流（回调）
        private void OnReadCallback(IAsyncResult asyncResult)
        {
            Stream responseStream = (Stream)asyncResult.AsyncState;
            try
            {
                int readCount = responseStream.EndRead(asyncResult);
                if (readCount > 0)
                {
                    m_CurrentDownloadByte += readCount;

                    // write to file
                    if (m_FileStream == null)
                    {
                        m_FileStream = new FileStream(m_TmpFile, FileMode.Create);
                    }
                    m_FileStream.Write(m_Buffer, 0, readCount);

                    UpdateTimeOut();

                    // 进度回调
                    lock (m_Event)
                    {
                        for (int i = m_Event.Count - 1; i >= 0; i--)
                        {
                            if (m_Event[i] == DownloadEvent.Progress)
                            {
                                m_Event.RemoveAt(i);
                            }
                        }

                        m_Event.Add(DownloadEvent.Progress);
                    }

                    // 继续读取
                    responseStream.BeginRead(m_Buffer, 0, BUFFER_SIZE, new AsyncCallback(OnReadCallback), responseStream);
                }
                else // 已经读完
                {
                    responseStream.Close();
                    m_FileStream.Close();

                    if (File.Exists(m_SaveFile))
                    {
                        File.Delete(m_SaveFile);
                    }

                    File.Move(m_TmpFile, m_SaveFile);
                    //Log.i("Finished!! fileLength:" + m_FileLength + ",Download byte:" + m_CurrentDownloadByte);

                    // 进度回调
                    lock (m_Event)
                    {
                        m_Event.Clear();
                        m_Event.Add(DownloadEvent.Finish);
                    }
                }
            }
            catch (Exception exception)
            {
                HandleError("#OnReadCallback:" + exception.Message);
            }
        }

        // 退出游戏回调，只在游戏结束时调用一次
        protected void OnDestroy()
        {
            lock (m_RequestLock)
            {
                m_IsDownloading = false;

                if (m_Request != null)
                {
                    m_Request.Abort();
                    m_Request = null;

                    if (m_FileStream != null)
                    {
                        m_FileStream.Close();
                        m_FileStream = null;
                    }
                }
            }

            HandleError("App Quit");
        }

        void ShowDownloadOrExitPanel()
        {
            m_IsWaitDownloadOrExit = false;

            //string lengthInMB = string.Format("{0:f1}M", (float)m_FileLength / (1024 * 1024));
            /*
            MsgMgr.S.ShowBox(MsgBoxStyle.S_YesNo | MsgBoxStyle.IsModal, null,
                             TDLanguageTable.GetFormat("MSG_NotWifi_Download", lengthInMB),
                             TDLanguageTable.Get("UI_Download"),
                             TDLanguageTable.Get("UI_Leave_Game"),
                             () =>
                             {
                                 m_IsDownloading = true;
                                 UpdateTimeOut();
                                 ReadData(m_WriteResponse);
                             },
                            () =>
                            {
                                Application.Quit();
                            }
            );
            */
        }

        void StartHttpWebRequest()
        {
            try
            {
                lock (m_RequestLock)
                {
                    if (m_IsDownloading)
                    {
                        m_Request = (HttpWebRequest)WebRequest.Create(m_Uri);

                        if (m_StartPosition > 0)
                        {
                            m_Request.AddRange(m_StartPosition);
                        }

                        m_Request.KeepAlive = false;
                        m_Request.BeginGetResponse(new AsyncCallback(OnResponeCallback), m_Request);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("#StartHttpWebRequest:" + ex.Message);
                return;
            }
        }
    }
}
