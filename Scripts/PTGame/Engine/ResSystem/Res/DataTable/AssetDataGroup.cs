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
        public class ABUnit
        {
            public string abName;
            public string[] abDepends;

            public ABUnit(string name, string[] depends)
            {
                this.abName = name;
                if (depends == null || depends.Length == 0)
                {

                }
                else
                {
                    this.abDepends = depends;
                }
            }

            public override string ToString()
            {
                string result = string.Format("ABName:" + abName);
                if (abDepends == null)
                {
                    return result;
                }

                for (int i = 0; i < abDepends.Length; ++i)
                {
                    result += string.Format(" #:{0}", abDepends[i]);
                }

                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            private string m_Key;
            private ABUnit[] m_ABUnitArray;
            private AssetData[] m_AssetDataArray;

            public string key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

            public ABUnit[] abUnitArray
            {
                get { return m_ABUnitArray; }
                set { m_ABUnitArray = value; }
            }

            public AssetData[] assetDataArray
            {
                get { return m_AssetDataArray; }
                set { m_AssetDataArray = value; }
            }
        }

        private string m_Key;

        private List<ABUnit> m_ABUnitArray;
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
            if (m_ABUnitArray != null)
            {
                m_ABUnitArray.Clear();
            }

            if (m_AssetDataMap != null)
            {
                m_AssetDataMap.Clear();
            }
        }

        public int AddAssetBundleName(string name, string[] depends)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (m_ABUnitArray == null)
            {
                m_ABUnitArray = new List<ABUnit>();
            }

            AssetData config = GetAssetData(name);

            if (config != null)
            {
                return config.assetBundleIndex;
            }

            m_ABUnitArray.Add(new ABUnit(name, depends));

            int index = m_ABUnitArray.Count - 1;

            AddAssetData(new AssetData(name, eResType.kAssetBundle, index));

            return index;
        }

        public int GetAssetBundleIndex(string name)
        {
            if (m_ABUnitArray == null)
            {
                return -1;
            }

            for (int i = 0; i < m_ABUnitArray.Count; ++i)
            {
                if (m_ABUnitArray[i].abName.Equals(name))
                {
                    return i;
                }
            }
            Log.w("Failed Find AssetBundleIndex By Name:" + name);
            return -1;
        }

        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            result = null;

            if (m_ABUnitArray == null)
            {
                return false;
            }

            if (index >= m_ABUnitArray.Count)
            {
                return false;
            }

            if (m_AssetDataMap.ContainsKey(assetName))
            {
                result = m_ABUnitArray[index].abName;
                return true;
            }

            return false;
        }

        public ABUnit GetABUnit(string assetName)
        {
            AssetData data = GetAssetData(assetName);

            if (data == null)
            {
                return null;
            }

            if (m_ABUnitArray == null)
            {
                return null;
            }

            return m_ABUnitArray[data.assetBundleIndex];
        }

        public bool GetAssetBundleDepends(string abName, out string[] result)
        {
            result = null;

            ABUnit unit = GetABUnit(abName);

            if (unit == null)
            {
                return false;
            }

            result = unit.abDepends;

            return true;
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
                AssetData old = GetAssetData(data.assetName);

                Log.e("Already Add AssetData Name:{0} \n OldAB:{1}      NewAB:{2}", data.assetName, m_ABUnitArray[old.assetBundleIndex].abName, m_ABUnitArray[data.assetBundleIndex].abName);
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
            sd.abUnitArray = m_ABUnitArray.ToArray();
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
            sd.abUnitArray = m_ABUnitArray.ToArray();
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

            if (m_ABUnitArray != null)
            {
                builder.AppendLine(" #DUMP AssetBundleNameArray BEGIN");
                for (int i = 0; i < m_ABUnitArray.Count; ++i)
                {
                    builder.AppendLine(m_ABUnitArray[i].ToString());
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

            builder.AppendLine("#DUMP AssetDataGroup END");

            Log.i(builder.ToString());
        }

        private void SetSerizlizeData(SerializeData data)
        {
            if (data == null)
            {
                return;
            }

            m_ABUnitArray = new List<ABUnit>(data.abUnitArray);

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
