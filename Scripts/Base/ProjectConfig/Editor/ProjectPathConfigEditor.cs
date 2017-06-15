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
    [CustomEditor(typeof(ProjectPathConfig), false)]
    public class ProjectPathConfigEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Qarth/Config/Build ProjectConfig")]
        public static void BuildProjectConfig()
        {

            ProjectPathConfig data = null;
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            string spriteDataPath = folderPath + "/ProjectConfig.asset";

            data = AssetDatabase.LoadAssetAtPath<ProjectPathConfig>(spriteDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<ProjectPathConfig>();
                AssetDatabase.CreateAsset(data, spriteDataPath);
            }
            Log.i("Create Project Config In Folder:" + spriteDataPath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Sync"))
            {
                ProjectPathConfig.Reset();
            }
        }
    }
}
