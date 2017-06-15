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
using System.Threading;
using System.Text;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

namespace Qarth
{
    public class TableReadThreadWork
    {
        class TableInfo
        {
            public DTableOnParse    parseRun;
            public string           fileName;

            public TableInfo(Qarth.DTableOnParse parseRun, string fileName)
            {
                this.parseRun = parseRun;
                this.fileName = fileName;
            }
        }

        class ReadParams
        {
            public TableInfo    tableInfo;
            public byte[]       fileData;
            public string       filePath;
        }

        private Queue<ReadParams>   m_RequestFilePathQueue = null;
        private Thread              m_Thread = null;
        private bool                m_IsDone;
        private int                 m_FinishedCount;
        private int                 m_ReadMaxCount;
        //private string              m_SdcardPath = string.Empty;
        public bool                 IsDone = false;

        public int finishedCount
        {
            get { return m_FinishedCount; }
        }

        public int readMaxCount
        {
            get { return m_ReadMaxCount; }
        }

#if USE_TABLE_XC
        CryptoHelper m_CryptoHelper = null;
#endif

        public TableReadThreadWork()
        {
            m_RequestFilePathQueue = new Queue<ReadParams>();
#if USE_TABLE_XC
            m_CryptoHelper = new CryptoHelper(CryptoKey.rsaPublicKey, CryptoKey.desKeyStr, CryptoKey.desIV);
#endif
        }

        private static ReadParams CreateReadParams(Qarth.DTableOnParse onParse, string tableName)
        {
            ReadParams readParam = new ReadParams();
            
            readParam.filePath = TableHelper.GetTableFilePath(tableName);
            readParam.tableInfo = new TableInfo(onParse ,tableName);
            readParam.tableInfo.fileName = tableName;
            readParam.tableInfo.parseRun = onParse;
            return readParam;
        }

        public void AddJob(string tableName, Qarth.DTableOnParse onParse = null)
        {
            if (onParse == null)
            {
                return;
            }
            var readParam = CreateReadParams (onParse ,tableName);
            m_RequestFilePathQueue.Enqueue(readParam);
        }


        public void Start()
        {
            //初始化 避免在子线程调用mono
            FileMgr.S.InitStreamingAssetPath();
            IsDone = false;
            m_FinishedCount = 0;
            m_ReadMaxCount = m_RequestFilePathQueue.Count;
            #if UNITY_ANDROID && !UNITY_EDITOR
            //m_SdcardPath = AndroidSDKHelper.sdcardAbsPath;
            #endif
            //Log.i("SDCardPath:" + m_SdcardPath);
            m_Thread = new Thread(Work);
            m_Thread.Start();
        }

        private void Work()
        {
            ReadParams readparm;

            while (m_RequestFilePathQueue.Count > 0)
            {
                try
                {
                    readparm = m_RequestFilePathQueue.Dequeue();
                    byte[] fileData = FileMgr.S.ReadSync(readparm.filePath);
                    readparm.fileData = fileData;
                    bool isReadTxtSuccess = false;

                    //多国版优先读取 txt文件
                    if (isReadTxtSuccess == false)
                    {
                        ParseTable(readparm);
                    }
                    ++m_FinishedCount;
                }
                catch (Exception ex)
                {
                    Log.e(ex.ToString());
                    //DataCollection.S.PostError3rdOnlySafe(ex.Message, ex.StackTrace);
                }
            }
            IsDone = true;
        }

        public static void ReadSync( Qarth.DTableOnParse onParse, string fileName)
        {
            var readParam = CreateReadParams (onParse ,fileName);
            byte[] fileData = FileMgr.S.ReadSync(readParam.filePath);
            readParam.fileData = fileData;
            TableReadThreadWork work = new TableReadThreadWork();
            work.ParseTable(readParam);
        }

        private void ParseTable(ReadParams readParams)
        {
            byte[] plainText;
#if USE_TABLE_XC
            PTGame.FrameworkgFile PTGame.FrameworkgFile = new PTGame.FrameworkgFile();
            PTGame.FrameworkgFile.Read(readParams.fileData);

            if (!m_CryptoHelper.RsaVerify(PTGame.FrameworkgFile.FileData, PTGame.FrameworkgFile.RasText))
            {
                Log.iError("RsaVerify Fail");
            }
            byte[] plainZipText = m_CryptoHelper.DesDecrypt(PTGame.FrameworkgFile.FileData);
            //解压
            using (MemoryStream ms = new MemoryStream(plainZipText))
            {
                using (ZipFile zipFile = new ZipFile(ms))
                {
                    ZipEntry zipEntry = zipFile[0];
                    plainText = new byte[zipEntry.Size];
                    var stream = zipFile.GetInputStream(zipEntry);
                    stream.Read(plainText, 0, plainText.Length);
                    zipFile.Close();
                }
                ms.Close();
            } 
#else
            plainText = readParams.fileData;
            #endif
            ReadTable(readParams.tableInfo, plainText);
        }

        private void ReadTable(TableInfo tableInfo, byte[] data)
        {
            if (tableInfo != null)
            {
                try
                {
                    tableInfo.parseRun(data);
                }
                catch (System.Exception ex)
                {
                    Log.e("Parse table error TD" + tableInfo.fileName);
                    Log.e(ex.ToString() + ex.StackTrace);
                    //DataCollection.S.PostError3rdOnlySafe(ex.Message, ex.StackTrace);
                }
            }
        }
    }
}
