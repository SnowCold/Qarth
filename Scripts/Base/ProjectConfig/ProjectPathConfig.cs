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

namespace Qarth
{
    public class ProjectPathConfig : ScriptableObject
    {
        public const string DEFAULT_TABLE_EXPORT_PATH = "StreamingAssets/config";
        public const string DEFAULT_APP_CONFIG_PATH = "Resources/Config/DefaultAppConfig";
        private const string PROJECT_CONFIG_PATH = "Resources/Config/ProjectConfig";
        private const string DEFAULT_PROJECT_CONFIG_PATH = "Resources/Config/DefaultProjectConfig";
        private static ProjectPathConfig s_Instance;

        #region 序列化区域

        [SerializeField]
        private string m_AppConfigPath = "Resources/Config/AppConfig";

        [SerializeField]
        private string m_UIRootPath = "Resources/UI/UIRoot";
        [SerializeField]
        private string m_ABRelativePath = "Assets/";
        [SerializeField]
        private string m_ExportABConfigFile = "asset_bindle_config.bin";

        [SerializeField]
        private string m_TableFolder = "config/";

        #region 配置工具相关
        [SerializeField]
        private string m_BuildCSharpWinShell = "table/output_code_csharp.bat";
        [SerializeField]
        public string m_BuildTxtDataWinShell = "table/output_txt.bat";
        [SerializeField]
        public string m_BuildLrgDataWinShell = "table/output_xc.bat";
        [SerializeField]
        public string m_BuildCSharpLinuxShell = "table/output_code_csharp.sh";
        [SerializeField]
        public string m_BuildTxtDataLinuxShell = "table/output_txt.sh";
        [SerializeField]
        public string m_BuildLrgDataLinuxShell = "table/output_xc.sh";

        [SerializeField]
        private string m_ProjectToolsFolder = "/../../../Tools/";
        #endregion

        #endregion

        private static string ResourcesPath2Path(string path)
        {
            return path.Substring(10);
        }

        private static ProjectPathConfig LoadInstance()
        {
            UnityEngine.Object obj = Resources.Load(ResourcesPath2Path(PROJECT_CONFIG_PATH));

            if (obj == null)
            {
                Log.w("Not Find Project Config File, Try Load Default Config.");

                obj = Resources.Load(ResourcesPath2Path(DEFAULT_PROJECT_CONFIG_PATH));

                if (obj == null)
                {
                    Log.e("Not Find Default Project Config File!");
                    return null;
                }
            }

            //Log.i("Success Load Project Config.");

            s_Instance = obj as ProjectPathConfig;

            return s_Instance;
        }

        public static ProjectPathConfig S
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = LoadInstance();
                }

                return s_Instance;
            }
        }

        public static void Reset()
        {
            Resources.UnloadAsset(s_Instance);
            s_Instance = null;
        }

        public static string appConfigPath
        {
            get
            {
                return S.m_AppConfigPath;
            }
        }

        public static string uiRootPath
        {
            get
            {
                return S.m_UIRootPath;
            }
        }

        public static string tableFolder
        {
            get
            {
                return S.m_TableFolder;
            }
        }

        #region AssetBundle 相关

        public static string abRelativePath
        {
            get { return S.m_ABRelativePath; }
        }

        public static string AssetBundleUrl2Name(string url)
        {
            string parren = FilePath.streamingAssetsPath + S.m_ABRelativePath;
            return url.Replace(parren, "");
        }

        //1.默认先去外存找，然后在内部查找
        public static string AssetBundleName2Url(string name)
        {
            name = S.m_ABRelativePath + name;
            string dependURL = FilePath.GetResPathInPersistentOrStream(name);
            return dependURL;
        }

        public static string AssetBundleName2ExterUrl(string name)
        {
            name = S.m_ABRelativePath + name;
            string dependURL = FilePath.persistentDataPath4Res + name;
            return dependURL;
        }

        //导出目录
        public static string exportRootFolder
        {
            get { return "StreamingAssets/" + S.m_ABRelativePath; }
        }

        public static string absExportRootFolder
        {
            get { return FilePath.streamingAssetsPath + S.m_ABRelativePath; }
        }

        public static string abConfigfileName
        {
            get { return S.m_ExportABConfigFile; }
        }
        #endregion

        #region 配置工具相关
        public static string buildCSharpWinShell
        {
            get { return S.m_BuildCSharpWinShell; }
        }

        public static string buildTxtDataWinShell
        {
            get { return S.m_BuildTxtDataWinShell; }
        }
        public static string buildLrgDataWinShell
        {
            get { return S.m_BuildLrgDataWinShell; }
        }

        public static string buildCSharpLinuxShell
        {
            get { return S.m_BuildCSharpLinuxShell; }
        }
        public static string buildTxtDataLinuxShell
        {
            get { return S.m_BuildTxtDataLinuxShell; }
        }
        
        public static string buildLrgDataLinuxShell
        {
            get { return S.m_BuildLrgDataLinuxShell; }
        }

        public static string projectToolsFolder
        {
            get { return Application.dataPath + S.m_ProjectToolsFolder; }
        }
        #endregion
    }
}
