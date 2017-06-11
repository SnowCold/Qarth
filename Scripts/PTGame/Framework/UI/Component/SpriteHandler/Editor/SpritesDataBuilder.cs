//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class SpritesDataBuilder
    {
        [MenuItem("Assets/SpritesBuilder/Build SpritesData")]
        private static void BuildSpritesDataInFolder()
        {
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            DirectoryInfo dInfo = new DirectoryInfo(EditorUtils.AssetsPath2ABSPath(folderPath));
            DirectoryInfo[] subFolders = dInfo.GetDirectories();
            if (subFolders == null || subFolders.Length == 0)
            {
                BuildSpritesData(folderPath);
            }
            else
            {
                for (int i = 0; i < subFolders.Length; ++i)
                {
                    BuildSpritesData(EditorUtils.ABSPath2AssetsPath(subFolders[i].FullName));
                }
            }
        }

        public static void BuildSpritesData(string folderPath)
        {
            SpritesData data = null;

            string folderName = PathHelper.FullAssetPath2Name(folderPath);
            string spriteDataPath = folderPath + "/" + folderName + "SpritesData.asset";

            data  = AssetDatabase.LoadAssetAtPath<SpritesData>(spriteDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SpritesData>();
                data.atlasName = folderPath;
                AssetDatabase.CreateAsset(data, spriteDataPath);
            }

            data.ClearAllSprites();

            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath);
            var filePaths = Directory.GetFiles(workPath);
            for (int i = 0; i < filePaths.Length; ++i)
            {
                string relPath = EditorUtils.ABSPath2AssetsPath(filePaths[i]);
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(relPath);

                if (objs != null)
                {
                    for (int j = 0; j < objs.Length; ++j)
                    {
                        if (objs[j] is Sprite)
                        {
                            data.AddSprite(objs[j] as Sprite);
                        }
                        else if (objs[j] is Texture2D)
                        {
                            ProcessSpriteTextureImport(relPath, folderPath);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            Log.i("Success Process SpriteImport:" + folderPath);
        }

        protected static void ProcessSpriteTextureImport(string texPath, string folderPath)
        {
            TextureImporter import = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (import == null)
            {
                return;
            }
            import.spritePackingTag = folderPath.ToLower();
            import.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(texPath);
            //EditorUtility.SetDirty(import);
        }

        [MenuItem("Assets/SpritesBuilder/Readable Setting")]
        private static void ReadableSetting()
        {
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            DirectoryInfo dInfo = new DirectoryInfo(EditorUtils.AssetsPath2ABSPath(folderPath));
            DirectoryInfo[] subFolders = dInfo.GetDirectories();
            if (subFolders == null || subFolders.Length == 0)
            {
                ProcessTexture(folderPath);
            }
            else
            {
                for (int i = 0; i < subFolders.Length; ++i)
                {
                    ProcessTexture(EditorUtils.ABSPath2AssetsPath(subFolders[i].FullName));
                }
            }
        }

        private static void ProcessTexture(string folderPath)
        {
            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath);
            var filePaths = Directory.GetFiles(workPath);
            for (int i = 0; i < filePaths.Length; ++i)
            {
                string relPath = EditorUtils.ABSPath2AssetsPath(filePaths[i]);
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(relPath);

                if (objs != null)
                {
                    for (int j = 0; j < objs.Length; ++j)
                    {
                        if (objs[j] is Texture2D)
                        {
                            ProcessTextureImport(relPath);
                        }
                    }
                }
            }
        }

        private static void ProcessTextureImport(string texPath)
        {
            TextureImporter import = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (import == null)
            {
                return;
            }

            import.isReadable = false;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
        }
    }
}
