using UnityEngine;
using System.Collections;

namespace PTGame.Framework
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
        private static ResLoader m_ResLoader;

		private static AppConfig LoadInstance()
        {
            if (m_ResLoader != null)
            {
                m_ResLoader.ReleaseAllRes();
            }

            if (m_ResLoader == null)
            {
                m_ResLoader = ResLoader.Allocate(null);
            }

            m_ResLoader.Add2Load(ProjectPathConfig.APP_CONFIG_PATH);

            m_ResLoader.LoadSync();

            IRes res = ResMgr.S.GetRes(ProjectPathConfig.APP_CONFIG_PATH, false);
            if (res != null)
            {
                s_Instance = res.asset as AppConfig;
            }

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
        }

        #region 数据区

        #region 字段
        [SerializeField] private string 		m_ServerIp;
		[SerializeField] private uint			m_ServerPort;
		[SerializeField] private eServerMode	m_ServerMode = eServerMode.kLocal;
		[SerializeField] private APP_MODE 		m_AppMode;
		[SerializeField] private DebugSetting 	m_DebugSetting;
		[SerializeField] private bool			m_IsGuideActive = false;
		#endregion

		#region 属性

		public bool isGuideActive
		{
			get { return m_IsGuideActive; }
		}

		public string serverIp
		{
			get { return m_ServerIp; }
		}

		public uint serverPort
		{
			get { return m_ServerPort; }
		}

		public APP_MODE AppMode
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
		#endregion
		
	#endregion

	}

}

