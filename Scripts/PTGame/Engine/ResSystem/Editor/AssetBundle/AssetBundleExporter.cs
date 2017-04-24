using System;
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PTGame.Framework.Editor
{
    public class AssetBundleExporter
    {

#region 处理AssetBundle Name
        //自动设置选中目录下的AssetBundle Name
        [MenuItem("Assets/SCEngine/Asset/设置Asset名字[文件夹])")]
        public static void GenAssetNameAsFolderName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            AutoGenAssetNameInFolder(selectPath, true);
            Log.i("Finish GenAssetNameAsFolderName.");
        }

        //自动设置选中目录下的AssetBundle Name
        [MenuItem("Assets/SCEngine/Asset/设置Asset名字[文件])")]
        public static void GenAssetNameAsFileName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            AutoGenAssetNameInFolder(selectPath, false);

            AssetDatabase.SaveAssets();
            Log.i("Finish GenAssetNameAsFileName.");
        }

        /// <summary>
        // 递归处理文件夹下所有Asset 文件
        /// </summary>
        /// <param name="folderPath">Asset目录下文件夹</param>
        private static void AutoGenAssetNameInFolder(string folderPath, bool useFolderName)
        {
            if (folderPath == null)
            {
                Log.w("Folder Path Is Null!");
                return;
            }

            Log.i("Start Set Asset Name. Folder:" + folderPath);
            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath); //EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath).ToLower(); //EditUtils.GetReltivePath4AssetPath(folderPath).ToLower();
            assetBundleName = assetBundleName.Replace("resources/", "");
            //处理文件
            var filePaths = Directory.GetFiles(workPath);
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
                    if (useFolderName)
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
                AutoGenAssetNameInFolder(fileName, useFolderName);
            }
        }
#endregion

#region 构建AssetBundle

#region 构建所有AssetBundle
        [MenuItem("Assets/SCEngine/Asset/构建AB[全局])")]
        public static void BuildAllAssetBundles()
        {
            Log.i("Start Build All AssetBundles.");

            string exportPath = Application.dataPath + "/" + ProjectPathConfig.exportRootFolder;

            if (Directory.Exists(exportPath) == false)
            {
                Directory.CreateDirectory(exportPath);
            }

            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#if UNITY_IPHONE
            buildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#elif UNITY_STANDALONE_OSX
            buildTarget = BuildTarget.StandaloneOSXUniversal;
#elif UNITY_STANDALONE_WIN
            buildTarget = BuildTarget.StandaloneWindows;
#endif

            BuildPipeline.BuildAssetBundles("Assets/" + ProjectPathConfig.exportRootFolder, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
        }
#endregion

#region 指定具体文件构建

        [MenuItem("Assets/SCEngine/Asset/构建AB[当前文件夹]")]
        public static void BuildAssetBundlesInSelectFolder()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();//.CurrentSelectFolder;
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            string exportPath = Application.dataPath + "/" + ProjectPathConfig.exportRootFolder;

            if (Directory.Exists(exportPath) == false)
            {
                Directory.CreateDirectory(exportPath);
            }

            Dictionary<string, int> builderData = new Dictionary<string, int>();
            CollectABInFolder(selectPath, builderData);

            List<AssetBundleBuild> builderList = new List<AssetBundleBuild>();
            foreach(var cell in builderData)
            {
                string abName = cell.Key;
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = abName;
                build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                builderList.Add(build);
            }

            if (builderList.Count == 0)
            {
                Log.i("No AssetBundles Found InSelectFolder:" + selectPath);
                return;
            }

            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#if UNITY_IPHONE
            buildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#elif UNITY_STANDALONE_OSX
            buildTarget = BuildTarget.StandaloneOSXUniversal;
#elif UNITY_STANDALONE_WIN
            buildTarget = BuildTarget.StandaloneWindows;
#endif

            BuildPipeline.BuildAssetBundles("Assets/" + ProjectPathConfig.exportRootFolder,
                builderList.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression,
                buildTarget);

            Log.i("Finish Build AssetBundles InSelectFolder:" + selectPath);
        }

        private static void CollectABInFolder(string folderPath, Dictionary<string, int> builderData)
        {
            if (folderPath == null)
            {
                Log.w("Folder Path Is Null.");
                return;
            }

            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath);

            var filePaths = Directory.GetFiles(workPath);

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
                    Log.w("Not Find Asset:" + fullFileName);
                    continue;
                }
                else if (!string.IsNullOrEmpty(ai.assetBundleName))
                {
                    RecordNewAB(ai.assetBundleName, builderData);
                }
            }

            //递归处理文件夹
            var dirs = Directory.GetDirectories(workPath);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string fileName = Path.GetFileName(dirs[i]);

                fileName = string.Format("{0}/{1}", folderPath, fileName);
                CollectABInFolder(fileName, builderData);
            }
        }

        private static void RecordNewAB(string abName, Dictionary<string, int> builderData)
        {
            if (builderData.ContainsKey(abName))
            {
                return;
            }

            builderData.Add(abName, 0);

            string[] depends = AssetDatabase.GetAssetBundleDependencies(abName, true);

            if (depends != null)
            {
                for (int i = 0; i < depends.Length; ++i)
                {
                    if (builderData.ContainsKey(depends[i]))
                    {
                        continue;
                    }

                    builderData.Add(depends[i], 0);
                }
            }
        }

#endregion

#endregion

#region 构建 AssetDataTable

        [MenuItem("Assets/SCEngine/Asset/生成Asset清单[全局]")]
        public static void BuildDataTable()
        {
            Log.i("Start BuildAssetDataTable!");
            AssetDataTable table = new AssetDataTable();

            ProcessAssetBundleRes(table, null);

            table.Save(ProjectPathConfig.absExportRootFolder);
        }

        [MenuItem("Assets/SCEngine/Asset/生成Asset清单[当前文件夹]")]
        public static void BuildDataTableInFolder()
        {
            Log.i("Start BuildAssetDataTable!");

            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            Dictionary<string, int> builderData = new Dictionary<string, int>();
            CollectABInFolder(selectPath, builderData);

            if (builderData.Count <= 0)
            {
                return;
            }

            AssetDataTable table = new AssetDataTable();

            string[] abNames = new string[builderData.Keys.Count];

            int index = 0;
            foreach (var cell in builderData)
            {
                abNames[index++] = cell.Key;
            }

            ProcessAssetBundleRes(table, abNames);

            table.Save(ProjectPathConfig.absExportRootFolder);
        }

        [MenuItem("Assets/SCEngine/Asset/生成Table清单")]
        public static void BuildTableConfigTable()
        {
            Log.i("Start BuildTableConfigTable!");
            AssetDataTable table = new AssetDataTable();
            string folder = Application.dataPath + "/" + ProjectPathConfig.DEFAULT_TABLE_EXPORT_PATH;
            ProcessTableConfig(table, folder);

            table.Save(FilePath.streamingAssetsPath);
        }

        private static string AssetPath2Name(string assetPath)
        {
            int startIndex = assetPath.LastIndexOf("/") + 1;
            int endIndex = assetPath.LastIndexOf(".");

            if (endIndex > 0)
            {
                int length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }

        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.e(ex);
            }

            return null;
        }

        private static void ProcessTableConfig(AssetDataTable table, string folder)
        {

            AssetDataPackage group = null;

            DirectoryInfo direInfo = new DirectoryInfo(folder);

            FileInfo[] fileInfos = direInfo.GetFiles();

            if (fileInfos == null || fileInfos.Length == 0)
            {
                return;
            }

            for (int i = 0; i < fileInfos.Length; ++i)
            {
                FileInfo info = fileInfos[i];

                if (AssetFileFilter.IsConfigTable(info.FullName))
                {
                    string md5 = GetMD5HashFromFile(info.FullName);
                    table.AddAssetBundleName(ProjectPathConfig.tableFolder + info.Name, null, md5, (int)info.Length, out group);
                }
            }

            table.Dump();
        }

        private static void ProcessAssetBundleRes(AssetDataTable table, string[] abNames)
        {
            AssetDataPackage group = null;

            int abIndex = -1;

            AssetDatabase.RemoveUnusedAssetBundleNames();

            if (abNames == null)
            {
                abNames = AssetDatabase.GetAllAssetBundleNames();
            }

            if (abNames != null && abNames.Length > 0)
            {
                for (int i = 0; i < abNames.Length; ++i)
                {
                    string abPath = Application.dataPath + "/" + ProjectPathConfig.exportRootFolder + abNames[i];

                    string md5 = GetMD5HashFromFile(abPath);

                    string[] depends = AssetDatabase.GetAssetBundleDependencies(abNames[i], false);

                    FileInfo info = new FileInfo(abPath);
                    abIndex = table.AddAssetBundleName(abNames[i], depends, md5, (int)info.Length, out group);
                    if (abIndex < 0)
                    {
                        continue;
                    }
                    //Log.i("MD5:" + GetMD5HashFromFile(abPath));

                    string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);
                    foreach (var cell in assets)
                    {
                        if (cell.EndsWith(".unity"))
                        {
                            group.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABScene, abIndex));
                        }
                        else
                        {
                            group.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABAsset, abIndex));
                        }
                    }
                }
            }

            table.Dump();
        }
#endregion

    }
}
