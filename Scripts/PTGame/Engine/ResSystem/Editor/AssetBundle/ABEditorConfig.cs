using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{

    public class ABEditorConfigUnit
    {
        public string folderPath;
        public int flagMode;

        public override string ToString()
        {
            return string.Format("Folder:{0},FlagMode:{1}", folderPath, flagMode);
        }
    }

    public class ABEditorConfig
    {
        protected List<ResRootFolderHandler> m_RootFolderArray;

        public void AddRootFolder(string folderAssetsPath, ResRootFolderHandler.SerializeData data)
        {
            var rootFolder = FindResRootFolder(folderAssetsPath);
            if (rootFolder != null)
            {
                Log.w("Already Add Root Folder.");
                return;
            }

            ResRootFolderHandler folder = new ResRootFolderHandler();
            
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
                m_RootFolderArray = new List<ResRootFolderHandler>();
            }

            m_RootFolderArray.Add(folder);
        }

        public void ExportEditorConfig(string path)
        {
            if (m_RootFolderArray == null)
            {
                Log.w("Config Is Empty, Nothing to Export.");
                return;
            }

            List<ResRootFolderHandler.SerializeData> exportData = new List<ResRootFolderHandler.SerializeData>();
            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                exportData.Add(m_RootFolderArray[i].GetSerizlizeData());
            }

            SerializeHelper.SerializeXML(EditorUtils.AssetsPath2ABSPath(path), exportData);
        }

        public void LoadFromEditorConfig(string path)
        {
            if (!File.Exists(path))
            {
                Log.w("Not Find Config File:" + path);
                return;
            }
            path = EditorUtils.AssetsPath2ABSPath(path);
            List<ResRootFolderHandler.SerializeData> loadData = SerializeHelper.DeserializeXML<List<ResRootFolderHandler.SerializeData>>(path) as List<ResRootFolderHandler.SerializeData>;
            if (loadData != null && loadData.Count > 0)
            {
                m_RootFolderArray = new List<ResRootFolderHandler>();
                for (int i = 0; i < loadData.Count; ++i)
                {
                    AddRootFolder(loadData[i].folderPath, loadData[i]); 
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

        protected ResRootFolderHandler FindResRootFolder(string folderPath)
        {
            if (m_RootFolderArray == null)
            {
                return null;
            }

            for (int i = 0; i < m_RootFolderArray.Count; ++i)
            {
                if (m_RootFolderArray[i].folderPath == folderPath)
                {
                    return m_RootFolderArray[i];
                }
            }

            return null;
        }
        
    }
}
