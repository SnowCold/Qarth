//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
	public class KeyCodeTracker
	{
		public class PressKeyRecord
		{
			public float m_PressTime;

			public bool canActionTime
            {
				get { return DateTime.Now.Second - m_PressTime <= 2;}
			}

			public PressKeyRecord()
			{
				m_PressTime = DateTime.Now.Second;
			}
		}

		private PressKeyRecord m_CurPressKeyRecord;
		private bool m_SimalutionKeyBack;
        private KeyCodeEventInfo m_BackKeyDownInfo = new KeyCodeEventInfo();
        private Action m_DefaultProcessListener;

        public void SetDefaultProcessListener(Action l)
        {
            m_DefaultProcessListener = l;
        }

        public void ProcessKeyDown()
		{
            m_BackKeyDownInfo.Reset();

            EventSystem.S.Send(EngineEventID.BackKeyDown, m_BackKeyDownInfo);

            if (!m_BackKeyDownInfo.IsProcess())
            {
                if (null != m_CurPressKeyRecord)
                {
                    if (m_CurPressKeyRecord.canActionTime || m_SimalutionKeyBack)
                    {
                        Application.Quit();
                    }

                    m_CurPressKeyRecord = null;
                }
                else
                {
                    m_CurPressKeyRecord = new PressKeyRecord();

                    if (m_DefaultProcessListener != null)
                    {
                        m_DefaultProcessListener();
                    }
                }

                m_SimalutionKeyBack = false;
            }
		}

		public void LateUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || m_SimalutionKeyBack)
			{
				ProcessKeyDown();
			}
		}

#if UNITY_EDITOR && UNITY_ANDROID
		private void OnGUI()
		{
			if (GUI.Button(new Rect(200, 200, 100, 100), "Back"))
			{
				m_SimalutionKeyBack = true;
			}
		}
#endif
	}
}
