//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;

namespace Qarth
{
	
	#region 枚举
	public enum APP_MODE
	{
		DebugMode,
		TestMode,
		ReleaseMode,
	}

	public enum eServerMode
	{
		kLocal,
		kRemote
	}
	#endregion

	#region DebugSetting
	[System.Serializable]
	class DebugSetting
	{
		public string m_DumpPath = null;
		public bool m_DumpToScreen = false;
		public bool m_DumpToFile = true;
	}
	#endregion

	[System.Serializable]
	public class AppConfig : ScriptableObject
	{

	#region 初始化过程
		private static AppConfig s_Instance;

		private static AppConfig LoadInstance()
        {
            ResLoader loader = ResLoader.Allocate("AppConfig", null);

            UnityEngine.Object obj = loader.LoadSync(ProjectPathConfig.appConfigPath);
            if (obj == null)
            {
                Log.w("Not Find App Config, Will Use Default App Config.");
                loader.ReleaseAllRes();
                obj = loader.LoadSync(ProjectPathConfig.DEFAULT_APP_CONFIG_PATH);
                if (obj == null)
                {
                    Log.e("Not Find Default App Config File!");
                    loader.Recycle2Cache();
                    loader = null;
                    return null;
                }
            }

            //Log.i("Success Load App Config.");
            s_Instance = obj as AppConfig;

            AppConfig newAB = GameObject.Instantiate(s_Instance);

            s_Instance = newAB;

            loader.Recycle2Cache();
            
			return s_Instance;
		}

	#endregion

		public static AppConfig S
		{
			get
			{
				if(s_Instance == null)
				{
					s_Instance = LoadInstance();
				}
				
				return s_Instance;
			}
		}

        public void InitAppConfig()
        {
            Log.i("Init[AppConfig]");
            Log.Level = m_LogLevel;
        }

        #region 数据区

        #region 字段
        [SerializeField] private string 		m_ServerIp;
		[SerializeField] private uint			m_ServerPort;
		[SerializeField] private eServerMode	m_ServerMode = eServerMode.kLocal;
		[SerializeField] private APP_MODE 		m_AppMode;
		[SerializeField] private DebugSetting 	m_DebugSetting;
        [SerializeField] private LogLevel       m_LogLevel = LogLevel.Max;
		[SerializeField] private bool			m_IsGuideActive = false;
        [SerializeField] private bool           m_IsResUpdateActive = false;
        [SerializeField] private string         m_ReleaseBundleId = "com.putao.logic.cn";
		#endregion

		#region 属性

		public bool isGuideActive
		{
			get { return m_IsGuideActive; }
		}

        public bool isResUpdateActive
        {
            get { return m_IsResUpdateActive; }
        }

		public string serverIp
		{
			get { return m_ServerIp; }
		}

		public uint serverPort
		{
			get { return m_ServerPort; }
		}

		public APP_MODE appMode
		{
			get { return m_AppMode; }
			set
			{
				if(m_AppMode != value)
				{
					m_AppMode = value;
				}
			}
		}

		public string dumpPath
		{
			get { return m_DebugSetting.m_DumpPath; }
		}

		public bool dumpToScreen
		{
			get { return m_DebugSetting.m_DumpToScreen; }
		}

		public bool dumpToFile
		{
			get { return m_DebugSetting.m_DumpToFile; }
		}

		public eServerMode serverMode
		{
			get { return m_ServerMode; }
		}

        public string ReleaseBundleId
        {
            get {return m_ReleaseBundleId;}
        }
		#endregion
		
	#endregion

	}

}

