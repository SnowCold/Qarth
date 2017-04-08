using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Framework;
using System.Text;

namespace PTGame.Framework
{

    public class AssetDataTable : TSingleton<AssetDataTable>
    {
        [Serializable]
        public class SerializeData
        {
            private AssetDataGroup.SerializeData[] m_AssetDataGroup;

            public AssetDataGroup.SerializeData[] assetDataGroup
            {
                get { return m_AssetDataGroup; }
                set { m_AssetDataGroup = value; }
            }
        }

        public const string DEFAULT_GROUP_KEY = "default";

        private List<AssetDataGroup> m_ActiveAssetDataGroup = new List<AssetDataGroup>();
        private List<AssetDataGroup> m_AllAssetDataGroup = new List<AssetDataGroup>();

        public void SwitchLanguage(string key)
        {
            m_ActiveAssetDataGroup.Clear();
            for (int i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                AssetDataGroup group = m_AllAssetDataGroup[i];
                if (group.key.Equals(key) || group.key.Equals(DEFAULT_GROUP_KEY))
                {
                    m_ActiveAssetDataGroup.Add(group);
                }
            }
            Log.i("AssetDataTable Switch 2 Language:" + key);
        }

        public void Reset()
        {
            for (int i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                m_AllAssetDataGroup[i].Reset();
            }

            m_AllAssetDataGroup.Clear();
            m_ActiveAssetDataGroup.Clear();
        }

        public int AddAssetBundleName(string name, out AssetDataGroup group)
        {
            group = null;

            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            string key = null;
            if (name.StartsWith("i18res"))
            {
                key = GetKeyFromABName(name);
            }
            else
            {
                key = DEFAULT_GROUP_KEY;
            }

            group = GetAssetDataGroup(key);

            if (group == null)
            {
                group = new AssetDataGroup(key);
                m_AllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name);
        }

        public string GetAssetBundleName(string assetName, int index)
        {
            for (int i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string result = m_ActiveAssetDataGroup[i].GetAssetBundleName(assetName, index);
                if (string.IsNullOrEmpty(result))
                {
                    continue;
                }
                return result;
            }
            Log.w(string.Format("Failed GetAssetBundleName : {0} - Index:{1}", assetName, index));
            return null;
        }

        public AssetData GetAssetData(string assetName)
        {
            for (int i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                AssetData result = m_ActiveAssetDataGroup[i].GetAssetData(assetName);
                if (result == null)
                {
                    continue;
                }
                return result;
            }
            //Log.w(string.Format("Not Find Asset : {0}", assetName));
            return null;
        }

        public bool AddAssetData(string key, AssetData data)
        {
            var group = GetAssetDataGroup(key);
            if (group == null)
            {
                Log.e("Not Find Group:" + key);
                return false;
            }
            return group.AddAssetData(data);
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

        public void Save(string outPath)
        {
            SerializeData sd = new SerializeData();

            sd.assetDataGroup = new AssetDataGroup.SerializeData[m_AllAssetDataGroup.Count];

            for (int i = 0; i < m_AllAssetDataGroup.Count; ++i)
            {
                sd.assetDataGroup[i] = m_AllAssetDataGroup[i].GetSerializeData();
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

            Log.i("#DUMP AssetDataTable BEGIN");

            for (int i = 0; i < m_AllAssetDataGroup.Count; ++i)
            {
                m_AllAssetDataGroup[i].Dump();
            }

            Log.i("#DUMP AssetDataTable END");
        }

        private void SetSerizlizeData(SerializeData data)
        {
            if (data == null || data.assetDataGroup == null)
            {
                return;
            }

            m_AllAssetDataGroup = new List<AssetDataGroup>(data.assetDataGroup.Length);

            for (int i = data.assetDataGroup.Length - 1; i >= 0; --i)
            {
                m_AllAssetDataGroup.Add(BuildAssetDataGroup(data.assetDataGroup[i]));
            }
        }

        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            return new AssetDataGroup(data);
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                if (m_AllAssetDataGroup[i].key.Equals(key))
                {
                    return m_AllAssetDataGroup[i];
                }
            }

            return null;
        }

        private string GetKeyFromABName(string name)
        {
            name = name.Substring(7);
            string key = name.Substring(0, name.IndexOf('/'));
            return key;
        }

    }
}
