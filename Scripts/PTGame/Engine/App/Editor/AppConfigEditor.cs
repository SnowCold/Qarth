using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace SCFramework.Editor
{
    public class AppConfigEditor
    {
        [MenuItem("Assets/AppConfig/Build AppConfig")]
        public static void BuildAppConfig()
        {

            AppConfig data = null;
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            string spriteDataPath = folderPath + "AppConfig.asset";

            data = AssetDatabase.LoadAssetAtPath<AppConfig>(spriteDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(data, spriteDataPath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
