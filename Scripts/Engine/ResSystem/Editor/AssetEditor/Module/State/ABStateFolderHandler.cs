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
    public class ABStateFolderHandler
    {
        public class SerializeData
        {
            public string folderPath;
            public List<ABStateUnit> dataArray;
        }

        interface IFolderVisitor
        {
            void OnFolderVisitorResult(string folderFullPath, ABState state);
            void OnAssetVisitorResult(AssetImporter ai, int state);
        }

        class GenerateConfigVisitor : IFolderVisitor
        {
            private ABStateFolderHandler m_Handler;

            public GenerateConfigVisitor(ABStateFolderHandler handler)
            {
                m_Handler = handler;
            }

            public void OnFolderVisitorResult(string folderFullPath, ABState state)
            {
                m_Handler.AddConfig(folderFullPath, state);
                if (((state.flag ^ ABStateDefine.MIXED) & ABStateDefine.MIXED) == 0)
                {
                    //Log.w("Folder Has Mixed Flag:" + folderFullPath);
                }
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
        protected Dictionary<string, ABStateUnit> m_StateMap;

        public string folderAssetsPath
        {
            get { return m_FolderAssetsPath; }
        }

        public void RefreshState()
        {
            BuildAsFileSystem(m_FolderAssetsPath);
        }

        public void BuildAsFileSystem(string folderPath)
        {
            m_FolderAssetsPath = folderPath;

            if (m_StateMap == null)
            {
                m_StateMap = new Dictionary<string, ABStateUnit>();
            }
            else
            {
                m_StateMap.Clear();
            }

            var gen = new GenerateConfigVisitor(this);

            string fullPath = EditorUtils.AssetsPath2ABSPath(folderPath);
            ABState subState = VisitorFolder(fullPath, gen);

            gen.OnFolderVisitorResult(fullPath, subState);
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
                m_StateMap = new Dictionary<string, ABStateUnit>();

                for (int i = 0; i < config.dataArray.Count; ++i)
                {
                    AddConfigUnit(EditorUtils.AssetsPath2ABSPath(config.dataArray[i].folderAssetPath), config.dataArray[i]);
                }
            }
        }

        private void AddConfigUnit(string fullPath, ABStateUnit unit)
        {
            fullPath = fullPath.Replace("\\", "/");
            if (m_StateMap.ContainsKey(fullPath))
            {
                return;
            }
            m_StateMap.Add(fullPath, unit);
        }

        public SerializeData GetSerizlizeData()
        {
            SerializeData data = new SerializeData();
            data.folderPath = m_FolderAssetsPath;

            if (m_StateMap != null)
            {
                List<ABStateUnit> list = new List<ABStateUnit>();
                foreach (var unit in m_StateMap)
                {
                    list.Add(unit.Value);
                }
                data.dataArray = list;
            }

            return data;
        }

        private ABState VisitorFolder(string absPath, IFolderVisitor visitor)
        {
            if (visitor == null)
            {
                return ABState.NONE;
            }
            
            ABState state = new ABState();

            var dirs = Directory.GetDirectories(absPath);
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; ++i)
                {
                    ABState subState = VisitorFolder(dirs[i], visitor);

                    if (subState.hasMixed)
                    {
                        state.hasMixed = true;
                    }

                    if (subState.isLost)
                    {
                        state.isLost = true;
                    }
                }
            }

            var files = Directory.GetFiles(absPath);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    if (!AssetFileFilter.IsAsset(files[i]))
                    {
                        continue;
                    }

                    AssetImporter ai = AssetImporter.GetAtPath(EditorUtils.ABSPath2AssetsPath(files[i]));
                    if (ai != null)
                    {
                        if (!string.IsNullOrEmpty(ai.assetBundleName))
                        {
                            //Log.i("Files:" + files[i] + " ---> " + ai.assetBundleName);
                            string assetPath = EditorFileUtils.RemoveFileExtend(ai.assetPath).ToLower();
                            string bundleName = EditorFileUtils.RemoveFileExtend(ai.assetBundleName);
                            if (assetPath.EndsWith(bundleName))
                            {
                                //Log.i("File Flag:" + assetPath);
                                state.flag |= ABStateDefine.FILE;
                                visitor.OnAssetVisitorResult(ai, ABStateDefine.FILE);
                            }
                            else
                            {
                                //Log.i("##Folder Flag:" + assetPath);
                                state.flag |= ABStateDefine.FOLDER;
                                visitor.OnAssetVisitorResult(ai, ABStateDefine.FOLDER);
                            }
                        }
                        else
                        {
                            state.isLost = true;
                            visitor.OnAssetVisitorResult(ai, ABStateDefine.LOST);
                        }
                    }
                }
            }

            if (state.isMixedFlag)
            {
                state.hasMixed = true;
            }
            visitor.OnFolderVisitorResult(absPath, state);
            return state;
        }

        public void AddConfig(string folderFullPath, ABState state)
        {
            ABStateUnit unit = null;
            if (!m_StateMap.TryGetValue(folderFullPath, out unit))
            {
                unit = new ABStateUnit();
                AddConfigUnit(folderFullPath, unit);
            }

            unit.folderAssetPath = EditorUtils.ABSPath2AssetsPath(folderFullPath);
            unit.state = state;
        }

        public ABStateUnit GetConfigUnit(string folderFullPath)
        {
            if (m_StateMap == null)
            {
                return null;
            }

            if (m_StateMap.ContainsKey(folderFullPath))
            {
                return m_StateMap[folderFullPath];
            }
            return null;
        }

        public void Dump()
        {
            if (m_StateMap == null)
            {
                return;
            }

            foreach (var item in m_StateMap)
            {
                Log.i(item.Value.ToString());
            }
        }
    }

}
