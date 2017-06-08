using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class FolderInfo
    {
        private string m_FolderFullPath;
        private bool m_isOpen = false;
        private int m_Level = 0;
        private FolderInfo[] m_ChildFolderInfos;
        private string m_DisplayString;

        public string folderFullPath
        {
            get { return m_FolderFullPath; }
        }

        public bool isOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }

        public int level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public string folderName
        {
            get
            {
                if (string.IsNullOrEmpty(m_DisplayString))
                {
                    m_DisplayString = EditorFileUtils.GetFileName(m_FolderFullPath);
                }

                return m_DisplayString;
            }
        }

        public FolderInfo[] childFolderInfo
        {
            get { return m_ChildFolderInfos; }
        }

        public FolderInfo(string folderPath, int level)
        {
            m_Level = level + 1;
            m_FolderFullPath = folderPath;
            m_FolderFullPath = m_FolderFullPath.Replace("\\", "/");
            string[] dires = Directory.GetDirectories(folderPath);

            if (dires != null && dires.Length > 0)
            {
                m_ChildFolderInfos = new FolderInfo[dires.Length];
                for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
                {
                    m_ChildFolderInfos[i] = new FolderInfo(dires[i], m_Level);
                }
            }
        }

        public FolderInfo()
        {

        }

        public void AddFolder(string folderPath)
        {
            if (m_ChildFolderInfos == null)
            {
                m_ChildFolderInfos = new FolderInfo[1];
            }
            else
            {
                FolderInfo[] temp = new FolderInfo[m_ChildFolderInfos.Length + 1];
                Array.Copy(m_ChildFolderInfos, temp, m_ChildFolderInfos.Length);
                m_ChildFolderInfos = temp;
            }

            m_ChildFolderInfos[m_ChildFolderInfos.Length - 1] = new FolderInfo(folderPath, m_Level);
        }
    }
}
