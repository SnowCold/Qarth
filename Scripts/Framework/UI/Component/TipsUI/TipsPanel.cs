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
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Qarth
{
    public class TipsPanel : TipsBehaviour
    {
        [SerializeField]
        private AbstractPanel m_Panel;
        [SerializeField]
        private bool m_CheckOnlyTop = true;

        private Action m_Listener;
        private int m_PanelEventFrame = 0;

        public void SetOnClickEmptyListener(Action l)
        {
            m_Listener = l;
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Panel == null)
            {
                m_Panel = gameObject.GetComponent<AbstractPanel>();
            }

            if (m_Panel != null)
            {
                UIMgr.S.uiEventSystem.Register(m_Panel.GetParentPanelID(), OnParentPanelEvent);
            }

            EventSystem.S.Register(EngineEventID.OnPanelUpdate, OnPanelUpdate);
        }

        protected void OnDestroy()
        {
            if (UIMgr.isApplicationQuit)
            {
                return;
            }

            if (m_Panel != null)
            {
                UIMgr.S.uiEventSystem.UnRegister(m_Panel.GetParentPanelID(), OnParentPanelEvent);
            }

            EventSystem.S.UnRegister(EngineEventID.OnPanelUpdate, OnPanelUpdate);
        }

        private void OnPanelUpdate(int key, params object[] args)
        {
            m_PanelEventFrame = Time.frameCount;
        }

        protected void OnParentPanelEvent(int key, params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return;
            }

            int e = (int)args[0];

            switch (e)
            {
                case (int)ViewEvent.OnPanelClose:
                    enabled = false;
                    break;
                case (int)ViewEvent.OnPanelOpen:
                    enabled = true;
                    break;
                default:
                    break;
            }
        }

        protected override void OnClickEmptyArea()
        {
            if (m_PanelEventFrame == Time.frameCount)
            {
                return;
            }

            if (m_Panel != null)
            {
                if(m_Panel.hasOpen)
                {
                    if (m_CheckOnlyTop)
                    {
                        if (m_Panel.uiID != UIMgr.S.FindTopPanel<int>())
                        {
                            return;
                        }
                    }

                    if (Time.frameCount > m_Panel.lastOpenFrame)
                    {
                        if (m_Listener != null)
                        {
                            m_Listener();
                        }
                        else
                        {
                            UIMgr.S.ClosePanel(m_Panel);
                        }
                    }
                }
            }
        }
    }
}
