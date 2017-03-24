using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace SCFramework.Editor
{
    public class SpritesDataBuilder
    {
        [MenuItem("Assets/SpritesBuilder/Build SpritesData")]
        public static void BuildSpritesData()
        {
            SpritesData data = null;
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            string spriteDataPath = folderPath + "SpritesData.asset";

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
        }
    }
}
