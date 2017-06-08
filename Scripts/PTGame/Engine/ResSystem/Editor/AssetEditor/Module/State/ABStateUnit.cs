using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class ABStateUnit
    {
        private string m_FolderAssetPath;
        private ABState m_State;

        public ABState state
        {
            get { return m_State; }
            set { m_State = value; }
        }

        public string folderAssetPath
        {
            get { return m_FolderAssetPath; }
            set { m_FolderAssetPath = value; }
        }

        public override string ToString()
        {
            return string.Format("Folder:{0}, State:{1}", m_FolderAssetPath, m_State);
        }
    }
}
