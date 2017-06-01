using System;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class KeyCodeTracker
	{
		public class PressKeyRecord
		{
			public float PressTime;

			public bool CanActionTime {
				get { return DateTime.Now.Second - PressTime <= 2;}
			}

			public PressKeyRecord()
			{
				PressTime = DateTime.Now.Second;
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
                    if (m_CurPressKeyRecord.CanActionTime || m_SimalutionKeyBack)
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