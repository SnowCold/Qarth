using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ProjectPathConfig : ScriptableObject
    {
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
        private string m_ABManifestABName = "Assets";
        [SerializeField]
        private string m_ABRelativePath = "Assets/";
        [SerializeField]
        private string m_ABManifestAssetName = "assetbundlemanifest";
        [SerializeField]
        private string m_ExportABConfigFile = "asset_bindle_config.bin";

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

            Log.i("Success Load Project Config.");

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

        #region AssetBundle 相关

        public static string abManifestABName
        {
            get
            {
                return S.m_ABManifestABName;
            }
        }

        public static string abManifestAssetName
        {
            get { return S.m_ABManifestAssetName; }
        }

        public static string AssetBundleUrl2Name(string url)
        {
            string parren = FilePath.streamingAssetsPath + S.m_ABRelativePath;
            return url.Replace(parren, "");
        }

        public static string AssetBundleName2Url(string name)
        {
            string parren = FilePath.streamingAssetsPath + S.m_ABRelativePath;
            return parren + name;
        }

        //导出目录
        public static string exportRootFolder
        {
            get { return "StreamingAssets/" + S.m_ABRelativePath; }
        }


        public static string exportABConfigFile
        {
            get { return FilePath.streamingAssetsPath + S.m_ExportABConfigFile; }
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
