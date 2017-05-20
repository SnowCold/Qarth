using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class TopPanelTrigger : ITrigger
    {
        private int m_UIID;
        private Action<ITrigger> m_Listener;

        public TopPanelTrigger(int uiID)
        {
            m_UIID = uiID;
        }

        public void Start(Action<ITrigger> l)
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
                if (topUI == m_UIID)
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

            int topUI = UIMgr.S.FindTopPanel<int>(null);
            if (topUI == m_UIID)
            {
                m_Listener(this);
            }
        }
    }
}
