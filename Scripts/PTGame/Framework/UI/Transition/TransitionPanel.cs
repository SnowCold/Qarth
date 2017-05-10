using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class TransitionPanel : AbstractPanel, ITransitionHandler
    {
        private TransitionHelper.OpenParam m_OpenParam;
        
        private ResLoader m_NextPanelResLoader;

        private ITransitionAction m_Action;

        public AbstractPanel transitionPanel
        {
            get
            {
                return this;
            }
        }

        protected override void OnPanelOpen(params object[] args)
        {
            m_OpenParam = null;

            if (args.Length > 0)
            {
                m_OpenParam = args[0] as TransitionHelper.OpenParam;
            }

            if (m_OpenParam == null || m_OpenParam.action == null)
            {
                CloseSelfPanel();
                return;
            }

            m_Action = m_OpenParam.action;
            m_Action.transitionHandler = this;
            m_Action.PrepareTransition();
        }

        private void LoadNextPanelRes()
        {
            if (m_NextPanelResLoader != null)
            {
                m_NextPanelResLoader.ReleaseAllRes();
            }
            else
            {
                m_NextPanelResLoader = ResLoader.Allocate("SwitchAnimPanel");
            }

            UIData data = UIDataTable.Get(m_OpenParam.nextPanelUIID);

            if (data == null)
            {
                return;
            }

            UIMgr.CollectDynamicResource(data, m_NextPanelResLoader, m_OpenParam.args);

            m_NextPanelResLoader.Add2Load(data.fullPath);

            m_NextPanelResLoader.LoadAsync(OnNextPanelResLoadFinish);
        }

        private void OnNextPanelOpen(AbstractPanel panel)
        {
            if (m_NextPanelResLoader != null)
            {
                m_NextPanelResLoader.Recycle2Cache();
                m_NextPanelResLoader = null;
            }

            if (panel == null)
            {
                CloseSelfPanel();
                return;
            }

            if (m_Action.transitionSameTime)
            {
                m_Action.TransitionIn(m_OpenParam.prePanel);
                m_Action.TransitionOut(panel);
            }
            else
            {
                m_Action.TransitionOut(panel);
            }
        }

        private void OnNextPanelResLoadFinish()
        {

            UIMgr.S.OpenPanel(m_OpenParam.nextPanelUIID, OnNextPanelOpen, m_OpenParam.args);
        }

        protected override void OnClose()
        {
            m_Action.OnTransitionDestroy();

            m_OpenParam = null;

            m_Action = null;

            if (m_NextPanelResLoader != null)
            {
                m_NextPanelResLoader.Recycle2Cache();
                m_NextPanelResLoader = null;
            }
        }

        public void OnTransitionPrepareFiish()
        {
            if (m_Action.transitionSameTime)
            {
                LoadNextPanelRes();
            }
            else
            {
                m_Action.TransitionIn(m_OpenParam.prePanel);
            }
        }

        public void OnTransitionInFinish()
        {
            if (m_Action.transitionSameTime)
            {

            }
            else
            {
                LoadNextPanelRes();
            }

            if (m_OpenParam.needClosePrePanel)
            {
                UIMgr.S.ClosePanel(m_OpenParam.prePanel);
            }
        }

        public void OnTransitionOutFinish()
        {
            CloseSelfPanel();
        }
    }
}
