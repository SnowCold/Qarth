using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PTGame.Framework
{
    public class AssetDataGroup
    {
        [Serializable]
        public class SerializeData
        {
            private string m_Key;
            private string[] m_AssetBundleNameArray;
            private AssetData[] m_AssetDataArray;

            public string key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

            public string[] assetBundleNameArray
            {
                get { return m_AssetBundleNameArray; }
                set { m_AssetBundleNameArray = value; }
            }

            public AssetData[] assetDataArray
            {
                get { return m_AssetDataArray; }
                set { m_AssetDataArray = value; }
            }
        }

        private string m_Key;

        private List<string> m_AssetBundleNameArray;
        private Dictionary<string, AssetData> m_AssetDataMap;

        public string key
        {
            get { return m_Key; }
        }

        public AssetDataGroup(string key)
        {
            m_Key = key;
        }

        public AssetDataGroup(SerializeData data)
        {
            m_Key = data.key;
            SetSerizlizeData(data);
        }

        public void Reset()
        {
            if (m_AssetBundleNameArray != null)
            {
                m_AssetBundleNameArray.Clear();
            }

            if (m_AssetDataMap != null)
            {
                m_AssetDataMap.Clear();
            }
        }

        public int AddAssetBundleName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (m_AssetBundleNameArray == null)
            {
                m_AssetBundleNameArray = new List<string>();
            }

            AssetData config = GetAssetData(name);

            if (config != null)
            {
                return config.assetBundleIndex;
            }

            m_AssetBundleNameArray.Add(name);

            int index = m_AssetBundleNameArray.Count - 1;

            AddAssetData(new AssetData(name, eResType.kAssetBundle, index));

            return index;
        }

        public int GetAssetBundleIndex(string name)
        {
            if (m_AssetBundleNameArray == null)
            {
                return -1;
            }

            for (int i = 0; i < m_AssetBundleNameArray.Count; ++i)
            {
                if (m_AssetBundleNameArray[i].Equals(name))
                {
                    return i;
                }
            }
            Log.w("Failed Find AssetBundleIndex By Name:" + name);
            return -1;
        }

        public string GetAssetBundleName(string assetName, int index)
        {
            if (m_AssetBundleNameArray == null)
            {
                return null;
            }

            if (index >= m_AssetBundleNameArray.Count)
            {
                return null;
            }

            if (m_AssetDataMap.ContainsKey(assetName))
            {
                return m_AssetBundleNameArray[index];
            }

            return null;
        }

        public AssetData GetAssetData(string name)
        {
            if (m_AssetDataMap == null)
            {
                return null;
            }

            string key = name.ToLower();

            AssetData result = null;
            if (m_AssetDataMap.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        public bool AddAssetData(AssetData data)
        {
            if (m_AssetDataMap == null)
            {
                m_AssetDataMap = new Dictionary<string, AssetData>();
            }

            string key = data.assetName.ToLower();

            if (m_AssetDataMap.ContainsKey(key))
            {
                Log.e("Already Add AssetData:" + data.assetName);
                return false;
            }

            m_AssetDataMap.Add(key, data);
            return true;
        }

        public void LoadFromFile(string path)
        {
            object data = SerializeHelper.DeserializeBinary(path);

            if (data == null)
            {
                Log.e("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            SerializeData sd = data as SerializeData;

            if (sd == null)
            {
                Log.e("Failed Load AssetDataTable:" + path);
                return;
            }

            Reset();

            SetSerizlizeData(sd);
        }

        public SerializeData GetSerializeData()
        {
            SerializeData sd = new SerializeData();
            sd.key = m_Key;
            sd.assetBundleNameArray = m_AssetBundleNameArray.ToArray();
            if (m_AssetDataMap != null)
            {
                AssetData[] acArray = new AssetData[m_AssetDataMap.Count];

                int index = 0;
                foreach (var item in m_AssetDataMap)
                {
                    acArray[index++] = item.Value;
                }

                sd.assetDataArray = acArray;
            }

            return sd;
        }

        public void Save(string outPath)
        {
            SerializeData sd = new SerializeData();
            sd.assetBundleNameArray = m_AssetBundleNameArray.ToArray();
            if (m_AssetDataMap != null)
            {
                AssetData[] acArray = new AssetData[m_AssetDataMap.Count];

                int index = 0;
                foreach (var item in m_AssetDataMap)
                {
                    acArray[index++] = item.Value;
                }

                sd.assetDataArray = acArray;
            }

            if (SerializeHelper.SerializeBinary(outPath, sd))
            {
                Log.i("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Log.e("Failed Save AssetDataTable:" + outPath);
            }
        }

        public void Dump()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("#DUMP AssetDataGroup :" + m_Key);

            if (m_AssetBundleNameArray != null)
            {
                builder.AppendLine(" #DUMP AssetBundleNameArray BEGIN");
                for (int i = 0; i < m_AssetBundleNameArray.Count; ++i)
                {
                    builder.AppendLine(m_AssetBundleNameArray[i]);
                }
                builder.AppendLine(" #DUMP AssetBundleNameArray END");
            }

            if (m_AssetDataMap != null)
            {
                builder.AppendLine(" #DUMP AssetBundleNameArray BEGIN");
                foreach (var item in m_AssetDataMap)
                {
                    builder.AppendLine(item.Key);
                }
                builder.AppendLine(" #DUMP AssetBundleNameArray END");
            }

            builder.AppendLine("#DUMP AssetDataTable END");

            Log.i(builder.ToString());
        }

        private void SetSerizlizeData(SerializeData data)
        {
            if (data == null)
            {
                return;
            }

            m_AssetBundleNameArray = new List<string>(data.assetBundleNameArray);

            if (data.assetDataArray != null)
            {
                m_AssetDataMap = new Dictionary<string, AssetData>();

                for (int i = 0; i < data.assetDataArray.Length; ++i)
                {
                    AssetData config = data.assetDataArray[i];
                    AddAssetData(config);
                }
            }
        }
    }
}
