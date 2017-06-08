using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class ABFolderInfo
    {
        private string m_FolderFullPath;
        private bool m_isOpen = false;
        private int m_Level = 0;
        private ABFolderInfo[] m_ChildFolderInfos;
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

        public ABFolderInfo[] childFolderInfo
        {
            get { return m_ChildFolderInfos; }
        }

        public ABFolderInfo(string absPath, int level)
        {
            m_Level = level + 1;
            m_FolderFullPath = absPath;
            m_FolderFullPath = m_FolderFullPath.Replace("\\", "/");
            string[] dires = Directory.GetDirectories(absPath);

            if (dires != null && dires.Length > 0)
            {
                m_ChildFolderInfos = new ABFolderInfo[dires.Length];
                for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
                {
                    m_ChildFolderInfos[i] = new ABFolderInfo(dires[i], m_Level);
                }
            }
        }

        public ABFolderInfo()
        {

        }

        public void RemoveFolder(string absPath)
        {
            if (m_ChildFolderInfos == null)
            {
                return;
            }

            absPath = absPath.Replace("\\", "/");
            for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
            {
                if (m_ChildFolderInfos[i].folderFullPath == absPath)
                {
                    m_ChildFolderInfos[i] = null;
                    break;
                }
            }
        }

        public void AddFolder(string absPath)
        {
            if (!Directory.Exists(absPath))
            {
                return;
            }

            if (m_ChildFolderInfos == null)
            {
                m_ChildFolderInfos = new ABFolderInfo[1];
            }
            else
            {
                ABFolderInfo[] temp = new ABFolderInfo[m_ChildFolderInfos.Length + 1];
                Array.Copy(m_ChildFolderInfos, temp, m_ChildFolderInfos.Length);
                m_ChildFolderInfos = temp;
            }

            m_ChildFolderInfos[m_ChildFolderInfos.Length - 1] = new ABFolderInfo(absPath, m_Level);
        }
    }
}
