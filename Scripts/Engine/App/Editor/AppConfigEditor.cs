//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Qarth.Editor
{
    public class AppConfigEditor
    {
        [MenuItem("Assets/Qarth/Config/Build AppConfig")]
        public static void BuildAppConfig()
        {

            AppConfig data = null;
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            string spriteDataPath = folderPath + "/AppConfig.asset";

            data = AssetDatabase.LoadAssetAtPath<AppConfig>(spriteDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(data, spriteDataPath);
            }
            Log.i("Create App Config In Folder:" + spriteDataPath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
