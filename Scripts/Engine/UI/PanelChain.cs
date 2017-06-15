//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
    public class PanelChain
    {
        class OpenAction
        {
            private AbstractPanel m_Panel;
            private AbstractPanel m_MasterPanel;
            private int m_UIID;
            private bool m_VisibleFlag = true;
            private object[] m_Args;

            public OpenAction(int uiID, AbstractPanel masterPanel)
            {
                m_UIID = uiID;
                m_MasterPanel = masterPanel;
            }

            public void SetParam(object[] args)
            {
                m_Args = args;
            }

            public void Hide()
            {
                m_VisibleFlag = false;
                if (m_Panel != null)
                {
                    m_Panel.customVisibleFlag = m_VisibleFlag;
                }
            }

            public void Close()
            {
                if (m_Panel == null)
                {
                    return;
                }

                if (m_MasterPanel != null)
                {
                    UIMgr.S.CloseDependPanel(m_MasterPanel.panelID, m_UIID);
                }
                else
                {
                    UIMgr.S.ClosePanel(m_Panel);
                }
            }

            public void Show()
            {
                if (m_Panel == null)
                {
                    if (m_MasterPanel == null)
                    {
                        UIMgr.S.OpenPanel(m_UIID, OnPanelOpen, m_Args);
                    }
                    else
                    {
                        UIMgr.S.OpenDependPanel(m_MasterPanel.panelID, m_UIID, OnPanelOpen, m_Args);
                    }
                }

                m_VisibleFlag = true;
                if (m_Panel != null)
                {
                    m_Panel.customVisibleFlag = m_VisibleFlag;
                }
            }

            private void OnPanelOpen(AbstractPanel panel)
            {
                m_Panel = panel;
            }
        }

        private List<OpenAction> m_Stack;

        private AbstractPanel m_RootPanel;

        public void CloseRootPanel()
        {
            if (m_RootPanel == null)
            {
                return;
            }

            UIMgr.S.ClosePanel(m_RootPanel);
        }

        public AbstractPanel rootPanel
        {
            get { return m_RootPanel; }
            set { m_RootPanel = value; }
        }

        public void Open<T>(T uiID, AbstractPanel masterPanel, params object[] args) where T : IConvertible
        {
            if (m_Stack == null)
            {
                m_Stack = new List<OpenAction>();
            }

            int id = uiID.ToInt32(null);

            OpenAction action = new OpenAction(id, masterPanel);
            action.SetParam(args);

            for (int i = 0; i < m_Stack.Count; ++i)
            {
                m_Stack[i].Hide();
            }

            m_Stack.Add(action);

            action.Show();
        }

        public void CloseTopPanel()
        {
            if (m_Stack == null || m_Stack.Count == 0)
            {
                return;
            }

            var action = m_Stack[m_Stack.Count - 1];
            m_Stack.RemoveAt(m_Stack.Count - 1);
            action.Close();

            if (m_Stack.Count > 0)
            {
                var preAction = m_Stack[m_Stack.Count - 1];
                preAction.Show();
            }
        }

        public int panelCount
        {
            get
            {
                if (m_Stack == null)
                {
                    return 0;
                }

                return m_Stack.Count;
            }
        }
    }
}
