//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System.IO.Compression;

namespace Qarth
{
    public delegate void OnZipFinished(string zipFilePath, string m_OutDirPath);
    public delegate void OnZipError(string zipFilePath, string m_OutDirPath, string errorMsg);
    public delegate void OnZipProgress(string zipFilePath, string m_OutDirPath, float percent);

    [TMonoSingletonAttribute("[Tools]/ZipMgr")]
    class ZipMgr : TMonoSingleton<ZipMgr>
    {
        class ZipWorker
        {
            string m_ZipFilePath;
            string m_OutDirPath;
            event OnZipFinished m_OnZipFinished;
            event OnZipError m_OnZipError;
            event OnZipProgress m_OnZipProgress;
            Thread m_Thread;
            bool m_IsFinish;
            bool m_IsError;
            string m_ErrorMsg;
            long m_FileTotalCount;
            long m_FileCompletedCount;
            long m_CurFileProcessByteCount;
            long m_CurFileTotalByteCount;

            public bool isFinish
            {
                get { return m_IsFinish; }
            }

            public ZipWorker(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error, OnZipProgress progress)
            {
                m_ZipFilePath = zipFilePath;
                m_OutDirPath = outDirPath;
                m_OnZipFinished = finished;
                m_OnZipError = error;
                m_OnZipProgress = progress;
                m_Thread = new Thread(Work);
                m_IsFinish = false;
                m_IsError = false;
                m_ErrorMsg = "";
                m_FileTotalCount = 0;
                m_FileCompletedCount = 0;
                m_CurFileProcessByteCount = 0;
                m_CurFileTotalByteCount = 0;
            }

            public void Update()
            {
                if (m_IsError)
                {
                    if (m_OnZipError != null)
                    {
                        m_OnZipError(m_ZipFilePath, m_OutDirPath, m_ErrorMsg);
                    }
                    m_OnZipError = null;
                    m_OnZipFinished = null;
                    m_OnZipProgress = null;
                    return;
                }

                float percent = 0.0f;
                if (m_FileTotalCount == 1)
                {
                    percent = (float)m_CurFileProcessByteCount / (float)m_CurFileTotalByteCount;
                    if (m_CurFileProcessByteCount == 0)
                        percent = 0;
                    else if (m_CurFileTotalByteCount == 0)
                        percent = 1f;
                }
                else
                {
                    percent = (float)m_FileCompletedCount / (float)m_FileTotalCount;
                    if (m_FileCompletedCount == 0)
                        percent = 0;
                    else if (m_FileTotalCount == 0)
                        percent = 1f;
                }

                //Debug.LogError(percent);
                if (m_OnZipProgress != null)
                {
                    m_OnZipProgress(m_ZipFilePath, m_OutDirPath, percent);
                }

                if (m_IsFinish)
                {
                    if (m_OnZipFinished != null)
                    {
                        m_OnZipFinished(m_ZipFilePath, m_OutDirPath);
                    }
                    m_OnZipError = null;
                    m_OnZipFinished = null;
                    m_OnZipProgress = null;
                }
            }

            public void Start()
            {
                m_Thread.Start();
            }

            public void Stop()
            {
                //m_Thread.Interrupt();
            }

            void Work()
            {
                try
                {
                    ZipFile zipFile = new ZipFile(m_ZipFilePath);
                    m_FileTotalCount = zipFile.Count;
                    zipFile.Close();

                    FastZipEvents zipEvent = new FastZipEvents();
                    zipEvent.Progress = OnProcess;
                    zipEvent.CompletedFile = OnCompletedFile;

                    FastZip fastZip = new FastZip(zipEvent);
                    fastZip.CreateEmptyDirectories = true;
                    fastZip.ExtractZip(m_ZipFilePath, m_OutDirPath, null);
                    m_IsFinish = true;
                }
                catch (Exception exception)
                {
                    Log.e(exception.Message);
                    m_ErrorMsg = exception.Message;
                    m_IsError = true;
                    m_IsFinish = true;
                }
            }

            void OnProcess(object sender, ProgressEventArgs e)
            {
                m_CurFileProcessByteCount = e.Processed;
                m_CurFileTotalByteCount = e.Target > 0 ? e.Target : e.Processed;
            }

            void OnCompletedFile(object sender, ScanEventArgs e)
            {
                ++m_FileCompletedCount;
            }
        }

        List<ZipWorker> m_ZipWorkerList = new List<ZipWorker>();

        public override void OnSingletonInit()
        {
            
        }

        private void Update()
        {
            for (int i = m_ZipWorkerList.Count - 1; i >= 0; --i)
            {
                m_ZipWorkerList[i].Update();
                if (m_ZipWorkerList[i].isFinish)
                {
                    m_ZipWorkerList[i].Stop();
                    m_ZipWorkerList.RemoveAt(i);
                }
            }
        }

        public void UnZip(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error, OnZipProgress progress)
        {
            ZipWorker worker = new ZipWorker(zipFilePath, outDirPath, finished, error, progress);
            worker.Start();
            m_ZipWorkerList.Add(worker);
        }

        public bool UnZipData(byte[] inputData, string outDirPaht)
        {
            try
            {
                MemoryStream ms = new MemoryStream(inputData);
                FastZip fastZip = new FastZip();
                fastZip.CreateEmptyDirectories = true;
                fastZip.ExtractZip(ms, outDirPaht, FastZip.Overwrite.Always, null, null, null, false, true);
            }
            catch (System.Exception ex)
            {
                Log.e("Unzip Data error: " + ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        public byte[] UnZipData(byte[] inputData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(inputData))
                {
                    using (ZipFile zipFile = new ZipFile(ms))
                    {
                        ZipEntry zipEntry = zipFile[0];
                        byte[] outData = new byte[zipEntry.Size];
                        var stream = zipFile.GetInputStream(zipEntry);
                        stream.Read(outData, 0, outData.Length);
                        return outData;
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return null;
        }

        void Clean()
        {
            foreach (ZipWorker w in m_ZipWorkerList)
            {
                w.Stop();
            }
            m_ZipWorkerList.Clear();
        }

    }

}
