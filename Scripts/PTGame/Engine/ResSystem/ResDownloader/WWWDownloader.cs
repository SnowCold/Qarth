using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/HttpDownloader")]
    public class WWWDownloader : TMonoSingleton<WWWDownloader>, IHttpDownloader
    {
        private const int TIME_OUT = 10;
        enum DownloadEvent
        {
            Begin = 0,      //开始
            Error = 1,      // 出错
            Progress = 2,   // 进度
            Downloaded = 3, //下载完毕
            Finish = 4,     //完成完毕

            DownloadOrExit = 5 // 如果不是wifi状态，要下载的目标文件大于指定值，提示用户继续或者退出
        }

        private string m_Uri;
        private string m_SaveFile;

        private bool m_IsDownloading = false;
        private string m_EventMsg;

        private Stack<DownloadEvent> m_Event = new Stack<DownloadEvent>();
        private event OnDownloadFinished m_OnFinished;
        private event OnDownloadError m_OnError;
        private event OnDownloadProgress m_OnProgress;
        private event OnDownloadBegin m_OnDownloadBegin;

        private WWW m_WWW;

        protected float m_PreSize;
        protected long m_LastChangeTime;
        protected int m_FileSize;

        public float alreadyDownloadProgress
        {
            get
            {
                if (m_WWW == null)
                {
                    return 0;
                }
                return m_WWW.progress;
            }
        }

        public int alreadyDownloadSize
        {
            get
            {
                return (int)(alreadyDownloadProgress * m_FileSize);
            }
        }

        public string targetFile
        {
            get { return m_SaveFile; }
        }

        private long currentTimeTick
        {
            get { return DateTime.Now.Ticks / 10000000; }
        }

        public bool AddDownloadTask(string uri, string localPath, int fileSize, OnDownloadProgress onProgress, OnDownloadError onError, OnDownloadFinished onFinshed, OnDownloadBegin onBegin = null)
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

            Clear();

            m_FileSize = fileSize;
            m_OnProgress = onProgress;
            m_OnError = onError;
            m_OnFinished = onFinshed;
            m_OnDownloadBegin = onBegin;

            m_Uri = uri;
            m_SaveFile = localPath;

            m_IsDownloading = true;

            m_PreSize = 0;
            m_LastChangeTime = currentTimeTick;

            StartCoroutine(StartIEnumeratorTask());
            return true;
        }

        public void Clear()
        {
            if (m_IsDownloading)
            {
                return;
            }

            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }

            m_PreSize = 0;
            m_LastChangeTime = currentTimeTick;
            m_Event.Clear();

            m_OnProgress = null;
            m_OnError = null;
            m_OnFinished = null;
            m_OnDownloadBegin = null;

            //StopAllCoroutines();
        }
        //完全的WWW方式,Unity 帮助管理纹理缓存，并且效率貌似更高
        private IEnumerator StartIEnumeratorTask()
        {
            m_WWW = new WWW(m_Uri);

            yield return m_WWW;

            if (m_WWW.error != null)
            {
                string errorMsg = string.Format("WWWDownload :{0}, WWW Errors:{1}", m_Uri, m_WWW.error);
                Log.e(errorMsg);
                m_IsDownloading = false;

                HandleEvent(DownloadEvent.Error, errorMsg);

                yield break;
            }

            if (!m_WWW.isDone)
            {
                string errorMsg = string.Format("WWWDownloader WWW Not Done! Url:{0}", m_Uri);
                Log.e(errorMsg);

                m_IsDownloading = false;

                HandleEvent(DownloadEvent.Error, errorMsg);

                yield break;
            }

            HandleEvent(DownloadEvent.Downloaded, "");
        }

        private void SavaFileFromMemory()
        {
            if (m_WWW == null)
            {
                HandleEvent(DownloadEvent.Error, "SavaFileFromMemory Error");
                return;
            }

            m_IsDownloading = false;
            //dt.End();

            try
            {

                byte[] msg = m_WWW.bytes;

                FileInfo info = new FileInfo(m_SaveFile);

                if (info.Exists)
                {
                    info.Delete();
                }

                string folder = m_SaveFile.Substring(0, m_SaveFile.LastIndexOf('/'));
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                using (FileStream writeStream = info.Open(FileMode.CreateNew))
                {
                    writeStream.Write(msg, 0, msg.Length);
                }

                msg = null;

                HandleEvent(DownloadEvent.Finish, "");
            }
            catch (Exception e)
            {
                string errorMsg = "## Write WWW Download Data in File Error:" + m_SaveFile;
                Log.e(e);
                HandleEvent(DownloadEvent.Error, errorMsg);
            }
        }

        private void Update()
        {

            ProcessEvent();

            if (m_IsDownloading)
            {
                float alreadySize = alreadyDownloadProgress;
                if (m_PreSize != alreadyDownloadProgress)
                {
                    m_PreSize = alreadyDownloadProgress;
                    m_LastChangeTime = currentTimeTick;
                }
                else
                {
                    long diffTime = currentTimeTick - m_LastChangeTime;
                    if (diffTime > TIME_OUT)
                    {
                        HandleEvent(DownloadEvent.Error, "TimeOut");
                    }
                }
            }
        }

        private void ProcessEvent()
        {
            if (m_Event.Count > 0)
            {
                DownloadEvent de = m_Event.Pop();
                m_Event.Clear();
                switch (de)
                {
                    case DownloadEvent.Downloaded:
                        {
                            SavaFileFromMemory();
                        }
                        break;
                    case DownloadEvent.Begin:
                        break;
                    case DownloadEvent.DownloadOrExit:
                        break;
				case DownloadEvent.Error:
						OnDownloadError eL = m_OnError;
						m_IsDownloading = false;
                        Clear();
                        if (eL != null)
                        {
                            eL(m_EventMsg);
                            eL = null;
                        }
                        break;
                    case DownloadEvent.Finish:
                        OnDownloadFinished fL = m_OnFinished;
                        Clear();
                        if (fL != null)
                        {
                            fL(m_SaveFile, 0, 0);
                        }
                        break;
                    case DownloadEvent.Progress:
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleEvent(DownloadEvent eventID, string msg)
        {
            m_EventMsg = msg;
            m_Event.Push(eventID);
        }

    }
}
