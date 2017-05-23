using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class TopPanelTrigger : ITrigger
    {
        private int m_UIID = 0;
        private Action<bool, ITrigger> m_Listener;

		public void SetParam(string[] param)
		{
			m_UIID = UIDataTable.PanelName2UIID (param[0]);
		}

        public void Start(Action<bool, ITrigger> l)
        {
            m_Listener = l;
            EventSystem.S.Register(EngineEventID.OnPanelUpdate, OnPanelUpdte);
        }

        public void Stop()
        {
            m_Listener = null;
            EventSystem.S.UnRegister(EngineEventID.OnPanelUpdate, OnPanelUpdte);
        }

        public bool isReady
        {
            get
            {
                int topUI = UIMgr.S.FindTopPanel<int>(null);
				if (topUI == m_UIID && topUI >= 0)
                {
                    return true;
                }
                return false;
            }
        }

        private void OnPanelUpdte(int key, params object[] args)
        {
            if (m_Listener == null)
            {
                return;
            }

			if (isReady)
            {
                m_Listener(true, this);
            }
			else 
			{
				m_Listener (false, this);
			}
        }
    }
}
