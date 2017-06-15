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
    public class ABStateInfo
    {
        protected List<ABStateFolderHandler> m_RootFolderArray;

        public List<ABStateFolderHandler> GetRootFolderList()
        {
            return m_RootFolderArray;
        }

        public ABStateUnit GetStateUnit(string folderFullPath)
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

        public void RefreshState()
        {
            if (m_RootFolderArray == null)
            {
                return;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                m_RootFolderArray[i].RefreshState();
            }
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

        public void AddFolder(string folderAssetsPath, ABStateFolderHandler.SerializeData data)
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

            ABStateFolderHandler folder = new ABStateFolderHandler();

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
                m_RootFolderArray = new List<ABStateFolderHandler>();
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

            List<ABStateFolderHandler.SerializeData> exportData = new List<ABStateFolderHandler.SerializeData>();
            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                exportData.Add(m_RootFolderArray[i].GetSerizlizeData());
            }

            SerializeHelper.SerializeXML(EditorUtils.AssetsPath2ABSPath(assetPath), exportData);
        }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Log.w("Not Find State Cache File:" + path);
                return;
            }
            path = EditorUtils.AssetsPath2ABSPath(path);
            List<ABStateFolderHandler.SerializeData> loadData = SerializeHelper.DeserializeXML<List<ABStateFolderHandler.SerializeData>>(path) as List<ABStateFolderHandler.SerializeData>;
            if (loadData != null && loadData.Count > 0)
            {
                m_RootFolderArray = new List<ABStateFolderHandler>();
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

        protected ABStateFolderHandler FindResRootFolder(string folderPath)
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
