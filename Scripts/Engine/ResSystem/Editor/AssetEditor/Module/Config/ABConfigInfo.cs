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

    public class ABConfigInfo
    {
        protected List<ABConfigFolderHandler> m_RootFolderArray;

        public List<ABConfigFolderHandler> GetRootFolderList()
        {
            return m_RootFolderArray;
        }

        public ABConfigUnit GetConfigUnit(string folderFullPath)
        {
            if (m_RootFolderArray == null)
            {
                return null;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                var result = m_RootFolderArray[i].GetConfigUnit(folderFullPath);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public void RemoveFolder(string folderAssetsPath)
        {
            if (m_RootFolderArray == null)
            {
                return;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                if (m_RootFolderArray[i].folderAssetsPath == folderAssetsPath)
                {
                    m_RootFolderArray.RemoveAt(i);
                    break;
                }
            }
        }

        public void RefreshConfig()
        {
            if (m_RootFolderArray == null)
            {
                return;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                m_RootFolderArray[i].RefreshConfig();
            }
        }

        public void AddFolder(string folderAssetsPath, ABConfigFolderHandler.SerializeData data)
        {
            var rootFolder = FindResRootFolder(folderAssetsPath);
            if (rootFolder != null)
            {
                Log.w("Already Add Root Folder.");
                return;
            }

            if (!Directory.Exists(EditorUtils.AssetsPath2ABSPath(folderAssetsPath)))
            {
                Log.w("Folder not Exit.");
                return;
            }

            ABConfigFolderHandler folder = new ABConfigFolderHandler();
            
            if (data == null)
            {
                folder.BuildAsFileSystem(folderAssetsPath);
            }
            else
            {
                folder.BuildAsConfigFile(data);
            }

            if (m_RootFolderArray == null)
            {
                m_RootFolderArray = new List<ABConfigFolderHandler>();
            }

            m_RootFolderArray.Add(folder);
        }

        public void ExportEditorConfig(string assetPath)
        {
            if (m_RootFolderArray == null)
            {
                Log.w("Config Is Empty, Nothing to Export.");
                return;
            }

            List<ABConfigFolderHandler.SerializeData> exportData = new List<ABConfigFolderHandler.SerializeData>();
            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                exportData.Add(m_RootFolderArray[i].GetSerizlizeData());
            }

            SerializeHelper.SerializeXML(EditorUtils.AssetsPath2ABSPath(assetPath), exportData);
        }

        public void LoadFromEditorConfig(string path)
        {
            if (!File.Exists(path))
            {
                Log.w("Not Find Config File:" + path);
                return;
            }
            path = EditorUtils.AssetsPath2ABSPath(path);
            List<ABConfigFolderHandler.SerializeData> loadData = SerializeHelper.DeserializeXML<List<ABConfigFolderHandler.SerializeData>>(path) as List<ABConfigFolderHandler.SerializeData>;
            if (loadData != null && loadData.Count > 0)
            {
                m_RootFolderArray = new List<ABConfigFolderHandler>();
                for (int i = 0; i < loadData.Count; ++i)
                {
                    AddFolder(loadData[i].folderPath, loadData[i]); 
                }
            }
        }

        public void Dump()
        {
            if (m_RootFolderArray == null)
            {
                return;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                m_RootFolderArray[i].Dump();
            }
        }

        protected ABConfigFolderHandler FindResRootFolder(string folderPath)
        {
            if (m_RootFolderArray == null)
            {
                return null;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                if (m_RootFolderArray[i].folderAssetsPath == folderPath)
                {
                    return m_RootFolderArray[i];
                }
            }

            return null;
        }
        
    }
}
