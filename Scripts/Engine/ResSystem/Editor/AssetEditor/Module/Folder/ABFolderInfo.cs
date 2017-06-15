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
            
			Init (null);
        }

		private void Init(ABFolderInfo[] history)
		{
			string[] dires = Directory.GetDirectories(m_FolderFullPath);

			if (dires != null && dires.Length > 0)
			{
				m_ChildFolderInfos = new ABFolderInfo[dires.Length];
				for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
				{
					m_ChildFolderInfos[i] = new ABFolderInfo(dires[i], m_Level);
					ABFolderInfo old = FindInArray (history, m_ChildFolderInfos [i].folderFullPath);
					if (old != null)
					{
						m_ChildFolderInfos [i] = old;
					}
				}
			}
			else
			{
				m_ChildFolderInfos = null;
			}
		}

		private ABFolderInfo FindInArray(ABFolderInfo[] array, string name)
		{
			if (array == null)
			{
				return null;
			}

			for (int i = 0; i < array.Length; ++i)
			{
				if (name == array[i].folderFullPath)
				{
					return array [i];
				}
			}
			return null;
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
				if (m_ChildFolderInfos[i] == null)
				{
					continue;
				}
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

        public void RefreshFolder()
        {
            if (m_ChildFolderInfos == null)
            {
                return;
            }

            
            for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
            {
				m_ChildFolderInfos[i].Rebuild();
            }

            /*
            var temp = m_ChildFolderInfos;
            m_ChildFolderInfos = null;

            for (int i = 0; i < temp.Length; ++i)
            {
                if (temp[i] == null)
                {
                    continue;
                }

                AddFolder(temp[i].folderFullPath);
            }
            */
        }

		private void Rebuild()
		{
			ABFolderInfo[] history = m_ChildFolderInfos;
			Init (history);

			if (m_ChildFolderInfos == null)
			{
				return;
			}
				
			for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
			{
				m_ChildFolderInfos[i].Rebuild();
			}
		}
    }
}
