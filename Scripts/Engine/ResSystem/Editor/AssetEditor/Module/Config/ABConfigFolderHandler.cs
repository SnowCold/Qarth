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
using UnityEditor;
using System.IO;

namespace Qarth.Editor
{
    public class ABConfigFolderHandler
    {
        public class SerializeData
        {
            public string folderPath;
            public List<ABConfigUnit> dataArray;
        }

        interface IFolderVisitor
        {
            void OnFolderVisitorResult(string folderFullPath);
            void OnAssetVisitorResult(AssetImporter ai, int flagMode);
        }

        class GenerateConfigVisitor : IFolderVisitor
        {
            private ABConfigFolderHandler m_Handler;

            public GenerateConfigVisitor(ABConfigFolderHandler handler)
            {
                m_Handler = handler;
            }

            public void OnFolderVisitorResult(string folderFullPath)
            {
                m_Handler.AddConfig(folderFullPath);
            }

            public void OnAssetVisitorResult(AssetImporter ai, int flagMode)
            {
                if (flagMode == ABStateDefine.LOST)
                {
                    //Log.w("Asset Lost AB Name:" + ai.assetPath);
                }
            }
        }

        protected string m_FolderAssetsPath;
        protected Dictionary<string, ABConfigUnit> m_ConfigMap;

        public string folderAssetsPath
        {
            get { return m_FolderAssetsPath; }
        }

        public void BuildAsFileSystem(string folderPath, bool rebuild = true)
        {
            m_FolderAssetsPath = folderPath;

            if (m_ConfigMap == null)
            {
                m_ConfigMap = new Dictionary<string, ABConfigUnit>();
            }
            else
            {
                if (rebuild)
                {
                    m_ConfigMap.Clear();
                }
            }

            string fullPath = EditorUtils.AssetsPath2ABSPath(folderPath);
            var gen = new GenerateConfigVisitor(this);
            VisitorFolder(fullPath, gen);
        }

        public void RefreshConfig()
        {
            BuildAsFileSystem(m_FolderAssetsPath, false);
        }

        public void BuildAsConfigFile(SerializeData config)
        {
            if (config == null)
            {
                return;
            }

            m_FolderAssetsPath = config.folderPath;
            if (config.dataArray != null)
            {
                m_ConfigMap = new Dictionary<string, ABConfigUnit>();

                for (int i = 0; i < config.dataArray.Count; ++i)
                {
                    AddConfigUnit(EditorUtils.AssetsPath2ABSPath(config.dataArray[i].folderAssetPath), config.dataArray[i]);
                }
            }
        }

        private void AddConfigUnit(string fullPath, ABConfigUnit unit)
        {
            fullPath = fullPath.Replace("\\", "/");
            if (m_ConfigMap.ContainsKey(fullPath))
            {
                return;
            }
            m_ConfigMap.Add(fullPath, unit);
        }

        public SerializeData GetSerizlizeData()
        {
            SerializeData data = new SerializeData();
            data.folderPath = m_FolderAssetsPath;

            if (m_ConfigMap != null)
            {
                List<ABConfigUnit> list = new List<ABConfigUnit>();
                foreach (var unit in m_ConfigMap)
                {
                    list.Add(unit.Value);
                }
                data.dataArray = list;
            }

            return data;
        }

        private void VisitorFolder(string absPath, IFolderVisitor visitor)
        {
            if (visitor == null)
            {
                return;
            }

            var dirs = Directory.GetDirectories(absPath);
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; ++i)
                {
                    VisitorFolder(dirs[i], visitor);
                }
            }
            visitor.OnFolderVisitorResult(absPath);

            return;
        }

        public void AddConfig(string folderFullPath)
        {
            folderFullPath = folderFullPath.Replace("\\", "/");
            ABConfigUnit unit = null;
            if (!m_ConfigMap.TryGetValue(folderFullPath, out unit))
            {
                unit = new ABConfigUnit();
                AddConfigUnit(folderFullPath, unit);
            }

            unit.folderAssetPath = EditorUtils.ABSPath2AssetsPath(folderFullPath);
        }

        public ABConfigUnit GetConfigUnit(string folderFullPath)
        {
            if (m_ConfigMap == null)
            {
                return null;
            }

            if (m_ConfigMap.ContainsKey(folderFullPath))
            {
                return m_ConfigMap[folderFullPath];
            }
            return null;
        }

        public void Dump()
        {
            if (m_ConfigMap == null)
            {
                return;
            }

            foreach (var item in m_ConfigMap)
            {
                Log.i(item.Value.ToString());
            }
        }
    }

}
