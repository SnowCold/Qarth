using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public enum ABFlagMode
    {
        NONE,
        FILE,
        FOLDER,
    }

    public class ABConfigUnit
    {
        public string folderPath;
        public bool isResFolder;
        public ABFlagMode flagMode;

        public override string ToString()
        {
            return string.Format("Folder:{0},IsResFolder:{1},FlagMode:{2}", folderPath, isResFolder, flagMode);
        }
    }

    public class AssetBundleEditorConfig
    {
        private Dictionary<string, ABConfigUnit> m_ConfigMap = new Dictionary<string, ABConfigUnit>();

        public void SavaConfig()
        {
            string abPath = EditorUtils.AssetsPath2ABSPath("abConfig.xml");

            List<ABConfigUnit> array = new List<ABConfigUnit>();
            foreach(var item in m_ConfigMap)
            {
                array.Add(item.Value);
            }

            SerializeHelper.SerializeXML(abPath, array);
        }

        public void LoadConfig()
        {
            string abPath = EditorUtils.AssetsPath2ABSPath("abConfig.xml");
            List<ABConfigUnit> array = SerializeHelper.DeserializeXML<List<ABConfigUnit>>(abPath) as List<ABConfigUnit>;

            for (int i = 0; i < array.Count; ++i)
            {
                Log.i(array[i].ToString());
            }
        }

        public void CheckResIntegrity(bool fix)
        {
            m_ConfigMap.Clear();
            string rootPath = Application.dataPath;
            GenerateConfigInFolder(rootPath);

            foreach(var item in m_ConfigMap)
            {
                string folder = item.Value.folderPath;
                var files = Directory.GetFiles(folder);

                for (int i = 0; i < files.Length; ++i)
                {
                    if (AssetFileFilter.IsAsset(files[i]))
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(EditorUtils.ABSPath2AssetsPath(files[i]));
                        if (ai != null)
                        {
                            if (string.IsNullOrEmpty(ai.assetBundleName))
                            {
                                if (fix)
                                {
                                    ABConfigUnit unit = item.Value;
                                    if (unit.flagMode == ABFlagMode.FILE)
                                    {
                                        ai.assetBundleName = EditorFileUtils.RemoveFileExtend(EditorUtils.AssetPath2ReltivePath(EditorUtils.ABSPath2AssetsPath(ai.assetPath)).ToLower());
                                    }
                                    else if (unit.flagMode == ABFlagMode.FOLDER)
                                    {
                                        ai.assetBundleName = EditorUtils.AssetPath2ReltivePath(EditorUtils.ABSPath2AssetsPath(unit.folderPath)).ToLower();
                                    }
                                    Log.e(string.Format("Fixed:{0}-->{1}", ai.assetPath, ai.assetBundleName));
                                }
                                else
                                {
                                    Log.e("EmptyResConfig:" + ai.assetPath);
                                }
                            }
                        }
                    }
                }
            }
            Log.i("Success CheckResIntegrity.");
        }

        public void AutoGenerateConfig()
        {
            m_ConfigMap.Clear();
            string rootPath = Application.dataPath;
            GenerateConfigInFolder(rootPath);
            SavaConfig();
            Log.i("Success Sava Config.");
        }

        public ABFlagMode GenerateConfigInFolder(string absPath)
        {
            var dirs = Directory.GetDirectories(absPath);
            if (dirs.Length > 0)
            {
                bool isResFolder = false;
                for (int i = 0; i < dirs.Length; ++i)
                {
                    ABFlagMode flagMode = GenerateConfigInFolder(dirs[i]);
                    if (flagMode != ABFlagMode.NONE)
                    {
                        isResFolder = true;
                        AddConfig(dirs[i], true, flagMode);
                    }
                }

                if (isResFolder)
                {
                    return ABFlagMode.FOLDER;
                }
                return ABFlagMode.NONE;
            }
            else
            {
                var files = Directory.GetFiles(absPath);
                if (files.Length == 0)
                {
                    return ABFlagMode.NONE;
                }

                ABFlagMode flagMode = ABFlagMode.NONE;

                if (absPath.Contains("Res\\Textures"))
                {
                    Log.i("!");
                }

                for (int i = 0; i < files.Length; ++i)
                {
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
                                flagMode = ABFlagMode.FILE;
                            }
                            else
                            {
                                //Log.i("##Folder Flag:" + assetPath);
                                flagMode = ABFlagMode.FOLDER;
                            }
                            
                            break;
                        }
                    }
                }

                return flagMode;
            }
        }

        public void AddConfig(ABConfigUnit unit)
        {
            if (unit == null)
            {
                return;
            }

            ABConfigUnit oldUnit = null;
            if (m_ConfigMap.TryGetValue(unit.folderPath, out oldUnit))
            {
                m_ConfigMap.Remove(unit.folderPath);
            }

            m_ConfigMap.Add(unit.folderPath, unit);
        }

        public void AddConfig(string folderPath, bool isResFolder, ABFlagMode mode)
        {
            ABConfigUnit unit = null;
            if (!m_ConfigMap.TryGetValue(folderPath, out unit))
            {
                unit = new ABConfigUnit();
                m_ConfigMap.Add(folderPath, unit);
            }
            unit.folderPath = folderPath;
            unit.isResFolder = isResFolder;
            unit.flagMode = mode;
        }

        [MenuItem("Assets/SCEngine/Res/导出生成配置")]
        public static void BuildDataTable()
        {
            AssetBundleEditorConfig readConfig = new AssetBundleEditorConfig();
            readConfig.LoadConfig();
        }

        [MenuItem("Assets/SCEngine/Res/自动生成配置")]
        public static void AutoBuildDataTable()
        {
            AssetBundleEditorConfig readConfig = new AssetBundleEditorConfig();
            readConfig.AutoGenerateConfig();
        }

        [MenuItem("Assets/SCEngine/Res/资源完整性检测")]
        public static void CheckResIntegrityStatic()
        {
            AssetBundleEditorConfig readConfig = new AssetBundleEditorConfig();
            readConfig.CheckResIntegrity(false);
        }

        [MenuItem("Assets/SCEngine/Res/资源完整性修复")]
        public static void FixedResIntegrityStatic()
        {
            AssetBundleEditorConfig readConfig = new AssetBundleEditorConfig();
            readConfig.CheckResIntegrity(true);
        }
    }
}
