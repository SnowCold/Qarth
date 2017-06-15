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
    public class TopPanelTrigger : ITrigger
    {
        private static int[] m_BlackList;

        private int m_UIID = 0;
        private Action<bool, ITrigger> m_Listener;

		public void SetParam(object[] param)
		{
			string name = param[0] as string;
			m_UIID = UIDataTable.PanelName2UIID (name);
		}

        public void Start(Action<bool, ITrigger> l)
        {
            if (m_BlackList == null)
            {
                m_BlackList = new int[2];
                m_BlackList[0] = UIDataTable.PanelName2UIID("GuideHandPanel");
                m_BlackList[1] = UIDataTable.PanelName2UIID("HighlightMaskPanel");
            }

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
				//return UIMgr.S.FindPanel(m_UIID) != null;
				
				int topUI = UIMgr.S.FindTopPanel<int>(m_BlackList, false);
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
				m_Listener(false, this);
			}
        }
    }
}
