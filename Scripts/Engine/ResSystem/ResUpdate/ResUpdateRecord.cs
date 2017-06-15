//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Qarth
{
    public class ResUpdateRecord
    {
        //ABName:md5:buildTime
        public class RecordCell
        {
            public string name
            {
                get;
                set;
            }

            public string md5
            {
                get;
                set;
            }

            public long buildTime
            {
                get;
                set;
            }

            public int fileSize
            {
                get;
                set;
            }

            public void SetData(string[] param)
            {
                name = param[0];
                md5 = param[1];
                fileSize = int.Parse(param[2]);
                buildTime = long.Parse(param[3]);
            }

            public void SetData(string name, string md5, int fileSize, long buildTime)
            {
                this.name = name;
                this.md5 = md5;
                this.fileSize = fileSize;
                this.buildTime = buildTime;
            }
        }

        protected List<RecordCell> m_UpdateRecordList = new List<RecordCell>();
        protected FileStream m_RecordFileStream;
        protected ResPackage m_Package;

        public ResUpdateRecord(ResPackage package)
        {
            m_Package = package;
        }

        public void ModifyAssetDataTable(AssetDataTable table)
        {
            RecordCell cell = null;
            AssetDataPackage package = null;
            for (int i = 0; i < m_UpdateRecordList.Count; ++i)
            {
                cell = m_UpdateRecordList[i];
                ABUnit unit = table.GetABUnit(cell.name);
                if (unit == null)
                {
                    table.AddAssetBundleName(cell.name, null, cell.md5, 1, cell.buildTime, out package);
                    continue;
                }
                else
                {
                    unit.buildTime = cell.buildTime;
                    unit.md5 = cell.md5;
                    unit.fileSize = cell.fileSize;
                }
            }
        }

        public void Load()
        {

            OpenRecordFileStream(false);

            if (m_RecordFileStream == null)
            {
                return;
            }

            try
            {
                m_UpdateRecordList.Clear();

                if (m_RecordFileStream.Length <= 0)
                {
                    return;
                }

                byte[] byteData = new byte[m_RecordFileStream.Length];

                m_RecordFileStream.Read(byteData, 0, byteData.Length);

                string context = UTF8Encoding.UTF8.GetString(byteData);
                if (string.IsNullOrEmpty(context))
                {
                    return;
                }

                string[] msgs = context.Split('\n');

                for (int i = 0; i < msgs.Length; ++i)
                {
                    if (string.IsNullOrEmpty(msgs[i]))
                    {
                        continue;
                    }

                    string[] param = msgs[i].Split(':');

                    if (param == null || param.Length < 4)
                    {
                        continue;
                    }

                    RecordCell cell = new RecordCell();
                    cell.SetData(param);
                    m_UpdateRecordList.Add(cell);
                }
            }
            catch(Exception e)
            {
                Log.e(e);
            }

        }

        public void AddRecord(string name, string md5, int fileSize, long buildTime)
        {
            OpenRecordFileStream(true);

            if (m_RecordFileStream == null)
            {
                return;
            }

            try
            {
                string writeData = string.Format("{0}:{1}:{2}:{3}\n", name, md5, fileSize, buildTime);
                byte[] writeDataArray = UTF8Encoding.UTF8.GetBytes(writeData);
                m_RecordFileStream.Write(writeDataArray, 0, writeDataArray.Length);

                m_RecordFileStream.Flush();

                RecordCell cell = new RecordCell();
                cell.SetData(name, md5, fileSize, buildTime);
                m_UpdateRecordList.Add(cell);
            }
            catch (Exception e)
            {
                Log.e(e);
            }
        }

        public void Close()
        {
            if (m_RecordFileStream == null)
            {
                return;
            }

            m_RecordFileStream.Close();
            m_RecordFileStream = null;
        }

        public void Delete()
        {
            Close();
            FileInfo fileInfo = new FileInfo(recordFilePath);

            try
            {
                if (!fileInfo.Exists)
                {
                    return;
                }

                fileInfo.Delete();
            }
            catch (Exception e)
            {
                Log.e(e);
            }
        }

        private string recordFilePath
        {
            get
            {
                return FilePath.persistentDataPath4Res + m_Package.relativeLocalPackageFolder + "/update_config.bin";
            }
        }

        private void OpenRecordFileStream(bool create = false)
        {
            if (m_RecordFileStream != null)
            {
                return;
            }

            FileInfo fileInfo = new FileInfo(recordFilePath);

            try
            {
                if (!fileInfo.Exists)
                {
                    if (create)
                    {
                        m_RecordFileStream = fileInfo.Create();
                    }

                    return;
                }

                m_RecordFileStream = fileInfo.Open(FileMode.Open);
            }
            catch(Exception e)
            {
                Log.e(e);
            }
        }
    }
}
