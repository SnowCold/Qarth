using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class ABEditorMgr
    {
        private ABFolderInfo m_RootFolder;
        private ABConfigInfo m_ABConfigInfo;
        private ABStateInfo m_ABStateInfo;

        public ABFolderInfo rootFolder
        {
            get { return m_RootFolder; }
        }

        public ABEditorMgr()
        {
            Init();
        }

        private void Init()
        {
            m_ABConfigInfo = new ABConfigInfo();
            m_ABConfigInfo.LoadFromEditorConfig("abConfig.xml");
            m_ABConfigInfo.RefreshConfig();

            m_ABStateInfo = new ABStateInfo();
            //m_ABStateInfo.LoadFromFile("abState.xml");

            m_RootFolder = new ABFolderInfo();

            var list = m_ABConfigInfo.GetRootFolderList();

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    m_ABStateInfo.AddFolder(list[i].folderAssetsPath, null);
                    m_RootFolder.AddFolder(EditorUtils.AssetsPath2ABSPath(list[i].folderAssetsPath));
                }
            }
        }

        public int GetABFlag(string absPath)
        {
            var unit = m_ABConfigInfo.GetConfigUnit(absPath);
            if (unit == null)
            {
                return ABFlagDefine.FOLDER;
            }

            return unit.abFlag;
        }

        public ABConfigUnit GetConfigUnit(string absPath)
        {
            return m_ABConfigInfo.GetConfigUnit(absPath);
        }

        public ABStateUnit GetStateUnit(string absPath)
        {
            return m_ABStateInfo.GetStateUnit(absPath);
        }

        public void FixedFolder(string assetPath)
        {
            string absPath = EditorUtils.AssetsPath2ABSPath(assetPath);
            if (!Directory.Exists(absPath))
            {
                return;
            }

            AutoGenAssetNameInFolder(assetPath);

            RefreshState();
        }

        public void AddFolder(string assetsPath)
        {
            string absPath = EditorUtils.AssetsPath2ABSPath(assetsPath);

            //这里会卡住
            m_ABConfigInfo.AddFolder(assetsPath, null);
            m_ABStateInfo.AddFolder(assetsPath, null);

            m_RootFolder.AddFolder(absPath);
        }

        public void RemoveFolder(string assetsPath)
        {
            string absPath = EditorUtils.AssetsPath2ABSPath(assetsPath);

            //这里会卡住
            m_ABConfigInfo.RemoveFolder(assetsPath);
            m_ABStateInfo.RemoveFolder(assetsPath);

            m_RootFolder.RemoveFolder(absPath);
        }

        public void ExportConfig()
        {
            m_ABConfigInfo.ExportEditorConfig("abConfig.xml");
        }

        public void RefreshState()
        {
            m_ABConfigInfo.RefreshConfig();
            m_ABStateInfo.RefreshState();
        }

        private void AutoGenAssetNameInFolder(string folderPath)
        {
            if (folderPath == null)
            {
                Log.w("Folder Path Is Null!");
                return;
            }

            Log.i("Start Set Asset Name. Folder:" + folderPath);
            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath).Replace("\\", "/"); //EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath).ToLower(); //EditUtils.GetReltivePath4AssetPath(folderPath).ToLower();
            assetBundleName = assetBundleName.Replace("resources/", "");
            //处理文件
            var filePaths = Directory.GetFiles(workPath);

            ABConfigUnit configUnit = GetConfigUnit(workPath);
            bool isFolderFlag = true;
            if (configUnit != null)
            {
                isFolderFlag = configUnit.isFolderFlag;
            }

            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!AssetFileFilter.IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);

                string fullFileName = string.Format("{0}/{1}", folderPath, fileName);

                AssetImporter ai = AssetImporter.GetAtPath(fullFileName);
                if (ai == null)
                {
                    Log.e("Not Find Asset:" + fullFileName);
                    continue;
                }
                else
                {
                    if (isFolderFlag)
                    {
                        ai.assetBundleName = assetBundleName;
                    }
                    else
                    {
                        ai.assetBundleName = string.Format("{0}/{1}", assetBundleName, PathHelper.FileNameWithoutSuffix(fileName));
                    }
                }

                //ai.SaveAndReimport();
                //Log.i("Success Process Asset:" + fileName);
            }

            //递归处理文件夹
            var dirs = Directory.GetDirectories(workPath);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string fileName = Path.GetFileName(dirs[i]);

                fileName = string.Format("{0}/{1}", folderPath, fileName);
                AutoGenAssetNameInFolder(fileName);
            }
        }
    }
}
