//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using System.Text;

namespace Qarth
{
    public class AssetDataTable : TSingleton<AssetDataTable>
    {

        private List<AssetDataPackage> m_ActiveAssetDataPackages = new List<AssetDataPackage>();
        private List<AssetDataPackage> m_AllAssetDataPackages = new List<AssetDataPackage>();

        public List<AssetDataPackage> allAssetDataPackages
        {
            get { return m_AllAssetDataPackages; }
        }

        public void SwitchLanguage(string key)
        {
            m_ActiveAssetDataPackages.Clear();

            string languageKey = string.Format("[{0}]", key);

            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                AssetDataPackage group = m_AllAssetDataPackages[i];

                if (!group.key.Contains("i18res"))
                {
                    m_ActiveAssetDataPackages.Add(group);
                }
                else if (group.key.Contains(languageKey))
                {
                    m_ActiveAssetDataPackages.Add(group);
                }

            }
            Log.i("AssetDataTable Switch 2 Language:" + key);
        }

        public void Reset()
        {
            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                m_AllAssetDataPackages[i].Reset();
            }

            m_AllAssetDataPackages.Clear();
            m_ActiveAssetDataPackages.Clear();
        }

        public int AddAssetBundleName(string name, string[] depends, string md5, int fileSize, long buildTime, out AssetDataPackage package)
        {
            package = null;

            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }
            
            string key = null;
            string path = null;

            GetPackageKeyFromABName(name, out key, out path);

            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            package = GetAssetDataPackage(key);

            if (package == null)
            {
                package = new AssetDataPackage(key, path, System.DateTime.Now.Ticks);
                m_AllAssetDataPackages.Add(package);
                Log.i("#Create Config Group:" + key);
            }

            return package.AddAssetBundleName(name, depends, md5, fileSize, buildTime);
        }

        public string GetAssetBundleName(string assetName, int index)
        {
            string result = null;
            for (int i = m_ActiveAssetDataPackages.Count - 1; i >= 0; --i)
            {
                if (!m_ActiveAssetDataPackages[i].GetAssetBundleName(assetName, index, out result))
                {
                    continue;
                }

                return result;
            }
            Log.w(string.Format("Failed GetAssetBundleName : {0} - Index:{1}", assetName, index));
            return null;
        }

        public List<ABUnit> GetAllABUnit()
        {
            List<ABUnit> result = new List<ABUnit>();
            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                result.AddRange(m_AllAssetDataPackages[i].GetAllABUnit());
            }
            return result;
        }

        public ABUnit GetABUnit(string name)
        {
            ABUnit result = null;

            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                result = m_AllAssetDataPackages[i].GetABUnit(name);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        //该函数的使用对打包规划要求太高，暂不提供
        public string GetAssetBundlePath(string assetName)
        {
            string result = null;
            for (int i = m_ActiveAssetDataPackages.Count - 1; i >= 0; --i)
            {
                if (!m_ActiveAssetDataPackages[i].GetAssetBundlePath(assetName, out result))
                {
                    continue;
                }

                return result;
            }
            Log.w(string.Format("Failed GetAssetBundlePath : {0}", assetName));
            return null;
        }

        public string[] GetAllDependenciesByUrl(string url)
        {
            string abName = ProjectPathConfig.AssetBundleUrl2Name(url);
            string[] depends = null;

            for (int i = m_ActiveAssetDataPackages.Count - 1; i >= 0; --i)
            {
                if (!m_ActiveAssetDataPackages[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }

        public AssetData GetAssetData(string assetName)
        {
            for (int i = m_ActiveAssetDataPackages.Count - 1; i >= 0; --i)
            {
                AssetData result = m_ActiveAssetDataPackages[i].GetAssetData(assetName);
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
            var group = GetAssetDataPackage(key);
            if (group == null)
            {
                Log.e("Not Find Group:" + key);
                return false;
            }
            return group.AddAssetData(data);
        }

        public void LoadPackageFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            object data = SerializeHelper.DeserializeBinary(FileMgr.S.OpenReadStream(path));

            if (data == null)
            {
                Log.e("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            AssetDataPackage.SerializeData sd = data as AssetDataPackage.SerializeData;

            if (sd == null)
            {
                Log.e("Failed Load AssetDataTable:" + path);
                return;
            }

            //string parentFolder = PathHelper.GetFolderPath(path);

            AssetDataPackage package = BuildAssetDataPackage(sd, path);

            string key = package.key;


            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                if (m_AllAssetDataPackages[i].key.Equals(key))
                {
                    var oldConfig = m_AllAssetDataPackages[i];
                    
                    if (oldConfig.buildTime > package.buildTime)
                    {
                        return;
                    }
                    else
                    {
                        m_AllAssetDataPackages.RemoveAt(i);
                        break;
                    }
                    
                }
            }

            m_AllAssetDataPackages.Add(package);
        }

        public AssetDataPackage GetAssetDataPackage(string key)
        {
            for (int i = m_AllAssetDataPackages.Count - 1; i >= 0; --i)
            {
                if (m_AllAssetDataPackages[i].key.Equals(key))
                {
                    return m_AllAssetDataPackages[i];
                }
            }

            return null;
        }

        public void Save(string outFolder)
        {
            for (int i = 0; i < m_AllAssetDataPackages.Count; ++i)
            {
                m_AllAssetDataPackages[i].Save(outFolder);
            }
        }

        public void Dump()
        {
            //StringBuilder builder = new StringBuilder();

            Log.i("#DUMP AssetDataTable BEGIN");

            for (int i = 0; i < m_AllAssetDataPackages.Count; ++i)
            {
                m_AllAssetDataPackages[i].Dump();
            }

            Log.i("#DUMP AssetDataTable END");
        }

        private AssetDataPackage BuildAssetDataPackage(AssetDataPackage.SerializeData data, string path)
        {
            return new AssetDataPackage(data, path);
        }

        private void GetPackageKeyFromABName(string name, out string key, out string path)
        {
            int pIndex = name.IndexOf('/');

            if (pIndex < 0)
            {
                key = name;
                path = name;
                return;
            }

            key = name.Substring(0, pIndex);
            path = key;
            string keyResult = null;
            string pathResult = key;

            if (SpecialFolderProcess(name, "i18res", out keyResult, out pathResult))
            {
                if (!string.IsNullOrEmpty(keyResult))
                {
                    key = keyResult;
                    path = pathResult;
                }
            }
            else if (SpecialFolderProcess(name, "subres", out keyResult, out pathResult))
            {
                if (!string.IsNullOrEmpty(keyResult))
                {
                    key = keyResult;
                    path = pathResult;
                }
            }

            return;
        }

        private bool SpecialFolderProcess(string name, string parren, out string keyResult, out string pathResult)
        {
            keyResult = null;
            pathResult = null;

            if (name.Contains(parren))
            {
                int parrentStart = name.IndexOf(parren) + parren.Length + 1;
                string parrenPath = name.Substring(0, parrentStart);
                string childPath = name.Substring(parrentStart);
                int pIndex = childPath.IndexOf('/');

                string folder = null;
                if (pIndex < 0)
                {
                    folder = childPath;
                }
                else
                {
                    folder = childPath.Substring(0, pIndex);
                }

                if (string.IsNullOrEmpty(folder))
                {
                    return true;
                }

                keyResult = string.Format("{0}[{1}]", parrenPath, folder);
                pathResult = string.Format("{0}{1}", parrenPath, folder);

                return true;
            }

            return false;
        }

    }
}
