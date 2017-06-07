using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class ResRootFolderHandler
    {
        public class SerializeData
        {
            public string folderPath;
            public List<ABEditorConfigUnit> dataArray;
        }

        interface IFolderVisitor
        {
            void OnFolderVisitorResult(string folderFullPath, int flagMode);
            void OnAssetVisitorResult(AssetImporter ai, int flagMode);
        }

        class GenerateConfigVisitor : IFolderVisitor
        {
            private ResRootFolderHandler m_Handler;

            public GenerateConfigVisitor(ResRootFolderHandler handler)
            {
                m_Handler = handler;
            }

            public void OnFolderVisitorResult(string folderFullPath, int flagMode)
            {
                m_Handler.AddConfig(folderFullPath, flagMode);
                if (((flagMode ^ ABFlagMode.MIXED) & ABFlagMode.MIXED) == 0)
                {
                    Log.w("Folder Has Mixed Flag:" + folderFullPath + " :" + flagMode);
                }
            }

            public void OnAssetVisitorResult(AssetImporter ai, int flagMode)
            {
                if (flagMode == ABFlagMode.LOST)
                {
                    Log.w("Asset Lost AB Name:" + ai.assetPath);
                }
            }
        }

        protected string m_FolderPath;
        protected Dictionary<string, ABEditorConfigUnit> m_ConfigMap;

        public string folderPath
        {
            get { return m_FolderPath; }
        }

        public void BuildAsFileSystem(string folderPath)
        {
            m_FolderPath = folderPath;

            if (m_ConfigMap == null)
            {
                m_ConfigMap = new Dictionary<string, ABEditorConfigUnit>();
            }
            else
            {
                m_ConfigMap.Clear();
            }

            VisitorFolder(EditorUtils.AssetsPath2ABSPath(folderPath), new GenerateConfigVisitor(this));
        }

        public void BuildAsConfigFile(SerializeData config)
        {
            if (config == null)
            {
                return;
            }

            m_FolderPath = config.folderPath;
            if (config.dataArray != null)
            {
                m_ConfigMap = new Dictionary<string, ABEditorConfigUnit>();

                for (int i = 0; i < config.dataArray.Count; ++i)
                {
                    m_ConfigMap.Add(config.dataArray[i].folderPath, config.dataArray[i]);
                }
            }
        }

        public SerializeData GetSerizlizeData()
        {
            SerializeData data = new SerializeData();
            data.folderPath = m_FolderPath;

            if (m_ConfigMap != null)
            {
                List<ABEditorConfigUnit> list = new List<ABEditorConfigUnit>();
                foreach (var unit in m_ConfigMap)
                {
                    list.Add(unit.Value);
                }
                data.dataArray = list;
            }

            return data;
        }

        private int VisitorFolder(string absPath, IFolderVisitor visitor)
        {
            if (visitor == null)
            {
                return ABFlagMode.NONE;
            }

            int folderMode = ABFlagMode.NONE;

            var dirs = Directory.GetDirectories(absPath);
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; ++i)
                {
                    int flagMode = VisitorFolder(dirs[i], visitor);
                    visitor.OnFolderVisitorResult(dirs[i], flagMode);
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
                        if (!String.IsNullOrEmpty(ai.assetBundleName))
                        {
                            //Log.i("Files:" + files[i] + " ---> " + ai.assetBundleName);
                            string assetPath = EditorFileUtils.RemoveFileExtend(ai.assetPath).ToLower();
                            if (assetPath.EndsWith(ai.assetBundleName))
                            {
                                //Log.i("File Flag:" + assetPath);
                                folderMode |= ABFlagMode.FILE;
                                visitor.OnAssetVisitorResult(ai, ABFlagMode.FILE);
                            }
                            else
                            {
                                //Log.i("##Folder Flag:" + assetPath);
                                folderMode |= ABFlagMode.FOLDER;
                                visitor.OnAssetVisitorResult(ai, ABFlagMode.FOLDER);
                            }
                        }
                        else
                        {
                            folderMode |= ABFlagMode.LOST;
                            visitor.OnAssetVisitorResult(ai, ABFlagMode.LOST);
                        }
                    }
                }
                
            }

            return folderMode;
        }

        public void AddConfig(ABEditorConfigUnit unit)
        {
            if (unit == null)
            {
                return;
            }

            ABEditorConfigUnit oldUnit = null;
            if (m_ConfigMap.TryGetValue(unit.folderPath, out oldUnit))
            {
                m_ConfigMap.Remove(unit.folderPath);
            }

            m_ConfigMap.Add(unit.folderPath, unit);
        }

        public void AddConfig(string folderPath, int mode)
        {
            ABEditorConfigUnit unit = null;
            if (!m_ConfigMap.TryGetValue(folderPath, out unit))
            {
                unit = new ABEditorConfigUnit();
                m_ConfigMap.Add(folderPath, unit);
            }

            unit.folderPath = EditorUtils.ABSPath2AssetsPath(folderPath);
            unit.flagMode = mode;
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
