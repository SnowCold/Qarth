//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Qarth.Editor
{
    [CustomEditor(typeof(LoopScrollRect), true)]
    public class LoopScrollRectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            LoopScrollRect scroll = (LoopScrollRect)target;

            //scroll.m_PrefabGameObject = EditorGUILayout.ObjectField("Prefab", scroll.m_PrefabGameObject, typeof(GameObject), true, null) as GameObject;

            scroll.totalCount = EditorGUILayout.IntField("Total Count", scroll.totalCount);
            scroll.threshold = Mathf.Max(10, EditorGUILayout.FloatField("Threshold", scroll.threshold));
            scroll.reverseDirection = EditorGUILayout.Toggle("Reverse Direction", scroll.reverseDirection);
            scroll.rubberScale = Mathf.Max(0, EditorGUILayout.FloatField("Rubber Scale", scroll.rubberScale));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                scroll.ClearCells();
            }
            if (GUILayout.Button("Refill"))
            {
                scroll.RefillCells();
            }
            if (GUILayout.Button("Refresh"))
            {
                scroll.RefreshCells();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
