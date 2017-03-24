using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PTGame.Framework
{
    public class TipsPanel : TipsBehaviour
    {
        [SerializeField]
        private AbstractPanel m_Panel;

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
            if (m_Panel != null)
            {
                if(m_Panel.hasOpen)
                {
                    if (Time.frameCount > m_Panel.lastOpenFrame)
                    {
                        UIMgr.S.ClosePanel(m_Panel);
                    }
                }
            }
        }
    }
}
