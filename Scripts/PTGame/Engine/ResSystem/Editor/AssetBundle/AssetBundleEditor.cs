using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("Assets/SCEngine/Res/导出配置")]
        public static void SavaConfig()
        {
            ABEditorConfig config = new ABEditorConfig();
            config.AddRootFolder("Assets/Res", null);
            config.AddRootFolder("Assets/Books", null);
            config.ExportEditorConfig("abConfig.xml");
            Log.i("## Success Export Config");
        }

        [MenuItem("Assets/SCEngine/Res/读取配置")]
        public static void LoadConfig()
        {
            ABEditorConfig config = new ABEditorConfig();
            config.LoadFromEditorConfig("abConfig.xml");
            config.Dump();
        }

        [MenuItem("Assets/SCEngine/Res/AB编辑器")]
        public static void OpenABWindow()
        {
            AssetBundleEditor window = EditorWindow.GetWindow<AssetBundleEditor>();
            window.Show();
        }

        class FolderInfo
        {
            private string m_FolderPath;
            private bool m_isOpen = false;
            private int m_Level = 0;
            private FolderInfo[] m_ChildFolderInfos;
            private string m_DisplayString;

            public string folderPath
            {
                get { return m_FolderPath; }
            }

            public bool isOpen
            {
                get { return m_isOpen; }
                set { m_isOpen = value; }
            }

            public int level
            {
                get { return m_Level; }
                set { m_Level = value; }
            }

            public string displayString
            {
                get
                {
                    if (string.IsNullOrEmpty(m_DisplayString))
                    {
                        string state = "*";
                        if (m_ChildFolderInfos == null)
                        {
                            state = "";
                        }
                        m_DisplayString = string.Format("{0}{1}", state, EditorFileUtils.GetFileName(m_FolderPath));
                    }

                    return m_DisplayString;
                }
            }

            public FolderInfo[] childFolderInfo
            {
                get { return m_ChildFolderInfos; }
            }

            public FolderInfo(string folderPath, int level)
            {
                m_Level = level + 1;
                m_FolderPath = EditorUtils.ABSPath2AssetsPath(folderPath);
                string[] dires = Directory.GetDirectories(folderPath);

                if (dires != null && dires.Length > 0)
                {
                    m_ChildFolderInfos = new FolderInfo[dires.Length];
                    for (int i = 0; i < m_ChildFolderInfos.Length; ++i)
                    {
                        m_ChildFolderInfos[i] = new FolderInfo(dires[i], m_Level);
                    }
                }
            }

            public FolderInfo()
            {

            }

            public void AddFolder(string folderPath)
            {
                if (m_ChildFolderInfos == null)
                {
                    m_ChildFolderInfos = new FolderInfo[1];
                }
                else
                {
                    FolderInfo[] temp = new FolderInfo[m_ChildFolderInfos.Length + 1];
                    Array.Copy(m_ChildFolderInfos, temp, m_ChildFolderInfos.Length);
                    m_ChildFolderInfos = temp;
                }

                m_ChildFolderInfos[m_ChildFolderInfos.Length - 1] = new FolderInfo(folderPath, m_Level);
            }
        }

        private ABEditorConfig m_Config;
        private FolderInfo m_RootFolder;
        Vector2 scrollPos;
        GUIStyle m_Style = "Label";

        private void Awake()
        {
            m_Config = new ABEditorConfig();
            m_Config.LoadFromEditorConfig("abConfig.xml");

            m_RootFolder = new FolderInfo();
            m_RootFolder.AddFolder(EditorUtils.AssetsPath2ABSPath("Assets/Res"));
            m_RootFolder.AddFolder(EditorUtils.AssetsPath2ABSPath("Assets/Books"));

            m_Style.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, false, true))
            {
                scrollPos = scrollView.scrollPosition;
                DrawFolder(m_RootFolder);
            }
        }

        private void DrawFolder(FolderInfo info)
        {
            if (info == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(info.folderPath))
            {
                EditorGUI.indentLevel = info.level;
                DrawGUIData(info);
            }

            if (info.isOpen || info.level <= 0)
            {
                if (info.childFolderInfo != null)
                {
                    for (int i = 0; i < info.childFolderInfo.Length; ++i)
                    {
                        DrawFolder(info.childFolderInfo[i]);
                    }
                }
            }
        }

        private void DrawGUIData(FolderInfo info)
        {

            Rect rt = GUILayoutUtility.GetRect(20, 20, m_Style);
            rt.x += (16 * EditorGUI.indentLevel);

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                rt.width = 100;
                //EditorGUI.DrawRect(rt, Color.white);

                if (GUI.Button(rt, info.displayString, m_Style))
                {
                    info.isOpen = !info.isOpen;
                }

                rt.x += 120;
                //EditorGUI.DrawRect(rt, Color.white);
                GUI.Label(rt, "asd", m_Style);
            }
        }

        void OnInspectorUpdate()
        {
            //Debug.Log("窗口面板的更新");
            //这里开启窗口的重绘，不然窗口信息不会刷新
            this.Repaint();
        }
    }
}
