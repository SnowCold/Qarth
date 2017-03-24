using System;
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCFramework.Editor
{
    public class AssetBundleExporter
    {

#region 处理AssetBundle Name
        //自动设置选中目录下的AssetBundle Name
        [MenuItem("Assets/SCEngine/Asset/GenAssetNameAsFolderName(按文件夹名字设置Asset名字)")]
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
        [MenuItem("Assets/SCEngine/Asset/GenAssetNameAsFileName(按文件名字设置Asset名字)")]
        public static void GenAssetNameAsFileName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            AutoGenAssetNameInFolder(selectPath, false);
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
                    return;
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
        [MenuItem("Assets/SCEngine/Asset/BuildAllAB(构建所有AB)")]
        public static void BuildAllAssetBundles()
        {
            Log.i("Start Build All AssetBundles.");

            string exportPath = Application.dataPath + "/" + ProjectPathConfig.EXPORT_ROOT_FOLDER;

            if (Directory.Exists(exportPath) == false)
            {
                Directory.CreateDirectory(exportPath);
            }

            BuildPipeline.BuildAssetBundles("Assets/" + ProjectPathConfig.EXPORT_ROOT_FOLDER, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        }
#endregion

#region 指定具体文件构建

        [MenuItem("Assets/SCEngine/Asset/BuildABInFolder(指定文件夹构建AB)")]
        public static void BuildAssetBundlesInSelectFolder()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();//.CurrentSelectFolder;
            if (selectPath == null)
            {
                Log.w("Not Select Any Folder!");
                return;
            }

            BuildAssetBundlesInFolder(selectPath);
        }

        private static void BuildAssetBundlesInFolder(string folderPath)
        {
            if (folderPath == null)
            {
                Log.w("Folder Path Is Null.");
                return;
            }

            Log.i("Start Build AssetBundle:" + folderPath);
            string fullFolderPath = EditorUtils.AssetsPath2ABSPath(folderPath);//EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath);// EditUtils.GetReltivePath4AssetPath(folderPath);
            var filePaths = Directory.GetFiles(fullFolderPath);

            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = assetBundleName;

            List<string> fileNameList = new List<string>();

            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!AssetFileFilter.IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);
                fileName = string.Format("{0}/{1}", folderPath, fileName);
                fileNameList.Add(fileName);
            }

            if (fileNameList.Count <= 0)
            {
                Log.w("Not Find Asset In Folder:" + folderPath);
                return;
            }

            abb.assetNames = fileNameList.ToArray();
            BuildPipeline.BuildAssetBundles(ProjectPathConfig.EXPORT_ROOT_FOLDER,
                new AssetBundleBuild[1] { abb },
                BuildAssetBundleOptions.ChunkBasedCompression,
                BuildTarget.StandaloneWindows);
        }

        #endregion

#endregion

#region 构建 AssetDataTable

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

        [MenuItem("Assets/SCEngine/Asset/BuildDataTable(生成Asset配置表)")]
        public static void BuildDataTable()
        {
            Log.i("Start BuildAssetDataTable!");
            AssetDataTable table = new AssetDataTable();

            ProcessAssetBundleRes(table);

            string filePath = FilePath.streamingAssetsPath + ProjectPathConfig.EXPORT_ASSETBUNDLE_CONFIG_PATH;
            table.Save(filePath);
        }

        private static void ProcessResourcesRes(AssetDataTable table)
        {
            
        }

        private static void ProcessAssetBundleRes(AssetDataTable table)
        {
            int abIndex = table.AddAssetBundleName(ProjectPathConfig.ABMANIFEST_AB_NAME);
            table.AddAssetData(new AssetData(ProjectPathConfig.ABMANIFEST_ASSET_NAME, eResType.kABAsset, abIndex));

            AssetDatabase.RemoveUnusedAssetBundleNames();

            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNames != null && abNames.Length > 0)
            {
                for (int i = 0; i < abNames.Length; ++i)
                {
                    Log.i("AB Name:" + abNames[i]);
                    abIndex = table.AddAssetBundleName(abNames[i]);
                    string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);
                    foreach (var cell in assets)
                    {
                        if (cell.EndsWith(".unity"))
                        {
                            table.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABScene, abIndex));
                        }
                        else
                        {
                            table.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABAsset, abIndex));
                        }
                    }
                }
            }

            table.Dump();
        }
#endregion

    }
}
