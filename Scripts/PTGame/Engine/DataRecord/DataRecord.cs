using System;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class DataRecord : TMonoSingleton<DataRecord>
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

            public string key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

        }
        
        /*
        [StructLayout(LayoutKind.Explicit)]
        public struct RecordField
        {
            [FieldOffset(0)]
            private int intVal;
            [FieldOffset(0)]
            private float floatVal;
            [FieldOffset(0)]
            private string stringVal;
        }
        */

        [Serializable]
        public class SerializeData
        {
            private List<RecordCell> m_RecordCellList;

            public List<RecordCell> recordCellList
            {
                get { return m_RecordCellList; }
                set { m_RecordCellList = value; }
            }
        }

        private Dictionary<string, RecordCell> m_DataMap = new Dictionary<string, RecordCell>();
        private bool m_IsMapDirty = false;

        public override void OnSingletonInit()
        {
            LoadFromFile();
        }

        public string GetLocalFilePath()
        {
            string filePath = FilePath.GetPersistentPath("Record/");
            filePath = filePath + "dataRecord.bin";
            return filePath;
        }

        public void LoadFromFile()
        {
            string path = GetLocalFilePath();
            object data = SerializeHelper.DeserializeBinary(FileMgr.S.OpenReadStream(path));

            if (data == null)
            {
                Log.w("Failed Deserialize DataRecord:" + path);
                return;
            }

            SerializeData sd = data as SerializeData;

            if (sd == null)
            {
                Log.w("Failed Load AssetDataTable:" + path);
                return;
            }

            m_DataMap.Clear();

            SetSerizlizeData(sd);
        }

        public void Save()
        {
            if (!m_IsMapDirty)
            {
                return;
            }

            SerializeData sd = GetSerializeData();
            string path = GetLocalFilePath();

            if (SerializeHelper.SerializeBinary(path, sd))
            {
                Log.i("Success Save AssetDataTable:" + path);
            }
            else
            {
                Log.e("Failed Save AssetDataTable:" + path);
            }
        }

        public SerializeData GetSerializeData()
        {
            SerializeData sd = new SerializeData();
            if (m_DataMap != null)
            {
                List<RecordCell> list = new List<RecordCell>();

                foreach (var item in m_DataMap)
                {
                    list.Add(item.Value);
                }

                sd.recordCellList = list;
            }

            return sd;
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

        public void SetFloat(string key, float v)
        {

        }

        public void SetInt(int v)
        {

        }

        protected new void OnApplicationQuit()
        {
            Save();
        }

        private void SetSerizlizeData(SerializeData sd)
        {
            var list = sd.recordCellList;
            if (list == null)
            {
                return;
            }

            for (int i = 0; i < list.Count; ++i)
            {
                m_DataMap.Add(list[i].key, list[i]);
            }
        }
    }
}
