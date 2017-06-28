//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Qarth
{
    /*[TMonoSingletonAttribute("[Tools]/DataRecord")]*/
    public class DataRecord_Default : IDataRecord
    {
        [Serializable]
        public class RecordCell
        {
            private string m_Key;
            private string m_Value;

            public string stringValue
            {
                get { return m_Value; }
                set { m_Value = value; }
            }

            public bool boolValue
            {
                get { return "1".Equals(m_Value); }
                set
                {
                    if (value)
                    {
                        m_Value = "1";
                    }
                    else
                    {
                        m_Value = "0";
                    }
                }
            }

            public int intValue
            {
                get
                {
                    if (string.IsNullOrEmpty(m_Value))
                    {
                        return 0;
                    }

                    return int.Parse(m_Value);
                }
                set { m_Value = value.ToString(); }
            }

            public float floatValue
            {
                get
                {
                    if (string.IsNullOrEmpty(m_Value))
                    {
                        return 0;
                    }
                    return float.Parse(m_Value);
                }
                set { m_Value = value.ToString(); }
            }

            public string key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

            public void SetData(string data)
            {
                m_Key = data.Substring(0, data.IndexOf(':'));
                m_Value = data.Substring(data.IndexOf(':') + 1);
            }

            public void WriteData(StringBuilder builder)
            {
                builder.Append("|");
                builder.Append(m_Key);
                builder.Append(":");
                builder.Append(m_Value);
            }
        }

        private Dictionary<string, RecordCell> m_DataMap = new Dictionary<string, RecordCell>();
        private bool m_IsMapDirty = false;

        public void Init()
        {
            LoadFromFile();
        }

        public void Reset()
        {
            m_DataMap.Clear();
            m_IsMapDirty = true;
        }

        public void Save()
        {
            if (!m_IsMapDirty)
            {
                return;
            }

            //SerializeData sd = GetSerializeData();
            //string path = GetLocalFilePath();

            string sd = GetSerializeData();
            PlayerPrefs.SetString("DataRecord", sd);

            PlayerPrefs.Save();
            m_IsMapDirty = false;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                return defaultValue;
            }
            return cell.boolValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                return defaultValue; 
            }
            return cell.stringValue;
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                return defaultValue;
            }
            return cell.floatValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                return defaultValue;
            }
            return cell.intValue;
        }

        public void SetString(string key, string value)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                cell = new RecordCell();
                cell.key = key;
                m_DataMap.Add(key, cell);
            }

            cell.stringValue = value;
            m_IsMapDirty = true;
        }

        public void SetBool(string key, bool value)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                cell = new RecordCell();
                cell.key = key;
                m_DataMap.Add(key, cell);
            }

            cell.boolValue = value;
            m_IsMapDirty = true;
        }

        public void SetFloat(string key, float value)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                cell = new RecordCell();
                cell.key = key;
                m_DataMap.Add(key, cell);
            }

            cell.floatValue = value;
            m_IsMapDirty = true;
        }

        public void SetInt(string key, int value)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                cell = new RecordCell();
                cell.key = key;
                m_DataMap.Add(key, cell);
            }

            cell.intValue = value;
            m_IsMapDirty = true;
        }
        
        public void AddInt(string key, int offset)
        {
            RecordCell cell = null;
            if (!m_DataMap.TryGetValue(key, out cell))
            {
                cell = new RecordCell();
                cell.key = key;
                m_DataMap.Add(key, cell);
            }

            cell.intValue += offset;
            m_IsMapDirty = true;
        }

        protected void OnApplicationQuit()
        {
            Save();
        }

        private void SetSerizlizeData(string data)
        {
            string[] values = data.Split('|');

            for (int i = 0; i < values.Length; ++i)
            {
                if (string.IsNullOrEmpty(values[i]))
                {
                    continue;
                }

                RecordCell cell = new RecordCell();
                cell.SetData(values[i]);

                m_DataMap.Add(cell.key, cell);
            }
        }

        private string GetLocalFilePath()
        {
            string filePath = FilePath.GetPersistentPath("Record/");
            filePath = filePath + "dataRecord.bin";
            return filePath;
        }

        private void LoadFromFile()
        {
            /*
            string path = GetLocalFilePath();
            object data = SerializeHelper.DeserializeBinary(FileMgr.S.OpenReadStream(path));

            if (data == null)
            {
                Log.w("Failed Deserialize DataRecord:" + path);
                return;
            }
            */

            m_DataMap.Clear();

            string sd = PlayerPrefs.GetString("DataRecord", "");

            if (sd == null)
            {
                //Log.w("Failed Load AssetDataTable:" + path);
                return;
            }

            SetSerizlizeData(sd);
        }

        private string GetSerializeData()
        {
            if (m_DataMap != null)
            {
                StringBuilder builder = new StringBuilder();

                foreach (var item in m_DataMap)
                {
                    item.Value.WriteData(builder);
                }

                return builder.ToString();
            }

            return "";
        }
    }
}
