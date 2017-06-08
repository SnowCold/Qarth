using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class AssetEditorWindow : EditorWindow
    {
        [MenuItem("Assets/SCEngine/Res/导出配置")]
        public static void SavaConfig()
        {
            ABEditorConfig config = new ABEditorConfig();
            config.AddRootFolder("Assets/Res", null);
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
            AssetEditorWindow window = EditorWindow.GetWindow<AssetEditorWindow>();
            window.Show();
        }

        private ABEditorConfig m_Config;
        private FolderInfo m_RootFolder;
        private Vector2 scrollPos;
        private GUIStyle m_Style = "Label";
        private Texture m_FolderIcon;
        private Texture m_TrangleDownIcon;
        private Texture m_TrangleRightIcon;

        private void Awake()
        {
            m_Config = new ABEditorConfig();
            m_Config.LoadFromEditorConfig("abConfig.xml");

            m_RootFolder = new FolderInfo();

            var list = m_Config.GetRootFolderList();
            
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    m_RootFolder.AddFolder(EditorUtils.AssetsPath2ABSPath(list[i].folderAssetsPath));
                }
            }

            m_Style.normal.textColor = Color.white;

            List<string> outResult = new List<string>();
            FilePath.GetFolderInFolder(Application.dataPath, "PTFramework", outResult);

            if (outResult.Count > 0)
            {
                string frameworkPath = EditorUtils.ABSPath2AssetsPath(outResult[0]);
                m_FolderIcon = AssetDatabase.LoadAssetAtPath<Texture>(frameworkPath + "/Editor/Res/folder_icon.png");
                m_TrangleDownIcon = AssetDatabase.LoadAssetAtPath<Texture>(frameworkPath + "/Editor/Res/triangle_down.png");
                m_TrangleRightIcon = AssetDatabase.LoadAssetAtPath<Texture>(frameworkPath + "/Editor/Res/triangle_right.png");
            }
        }

        private void OnGUI()
        {
            using (var verView = new EditorGUILayout.VerticalScope())
            {
                ShowAddRootFolderUI();

                ShowControlUI();

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, false, true))
                {
                    scrollPos = scrollView.scrollPosition;
                    DrawFolder(m_RootFolder);
                }
            }
        }

        private string m_AddRootFolderName = "";

        private void ShowAddRootFolderUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_AddRootFolderName = GUILayout.TextField(m_AddRootFolderName);
            if (GUILayout.Button("Add"))
            {
                m_Config.AddRootFolder("Assets/" + m_AddRootFolderName, null);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowControlUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export"))
            {
                m_Config.ExportEditorConfig("abConfig.xml");
            }
            if (GUILayout.Button("Import"))
            {
                m_Config.LoadFromEditorConfig("abConfig.xml");
            }
            if (GUILayout.Button("Refresh"))
            {

            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFolder(FolderInfo info)
        {
            if (info == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(info.folderFullPath))
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
                if (info.childFolderInfo != null)
                {
                    rt.width = 20;
                    //EditorGUI.DrawRect(rt, Color.white);
                    if (info.isOpen)
                    {
                        if (GUI.Button(rt, m_TrangleDownIcon, m_Style))
                        {
                            info.isOpen = !info.isOpen;
                        }
                    }
                    else
                    {
                        if (GUI.Button(rt, m_TrangleRightIcon, m_Style))
                        {
                            info.isOpen = !info.isOpen;
                        }
                    }

                }

                rt.x += 20;

                GUI.Label(rt, m_FolderIcon, m_Style);
                rt.x += 20;
                rt.width = 120;
                m_Style.normal.textColor = Color.white;
                GUI.Label(rt, info.folderName);

                var config = m_Config.GetConfigUnit(info.folderFullPath);
                if (config != null)
                {
                    rt.x += 120;
                    config.isFolderTagMode = GUI.Toggle(rt, config.isFolderTagMode, "文件夹模式");

                    rt.x += 120;
                    rt.width = 160;

                    string stateMsg = null;
                    if (FlagMode2Msg(config.flagMode, out stateMsg))
                    {
                        m_Style.normal.textColor = Color.gray;
                    }
                    else
                    {
                        m_Style.normal.textColor = Color.red;
                    }
                    stateMsg = string.Format("当前状态:{0}", stateMsg);
                    GUI.Label(rt, stateMsg, m_Style);
                    
                }
            }
        }

        private bool FlagMode2Msg(int flagMode, out string msg)
        {
            msg = "";
            if (((flagMode ^ ABFlagMode.MIXED) & ABFlagMode.MIXED) == 0)
            {
                msg += "-混合标记";
            }

            if (((flagMode ^ ABFlagMode.LOST) & ABFlagMode.LOST) == 0)
            {
                msg += "-丢失";
            }

            if (string.IsNullOrEmpty(msg))
            {
                msg = "正常";
                return true;
            }

            return false;
        }

        void OnInspectorUpdate()
        {
            //Debug.Log("窗口面板的更新");
            //这里开启窗口的重绘，不然窗口信息不会刷新
            this.Repaint();
        }
    }
}
