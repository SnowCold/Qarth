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
    public class TransitionPanel : AbstractPanel, ITransitionHandler, ITransitionAction
    {
        private TransitionHelper.OpenParam m_OpenParam;
        
        private ResLoader m_NextPanelResLoader;

        private ITransitionAction m_Action;
        private AbstractPanel m_NextPanel;

        public AbstractPanel transitionPanel
        {
            get
            {
                return this;
            }
        }

        public ITransitionHandler transitionHandler
        {
            get
            {
                return this;
            }

            set
            {
                
            }
        }

        public virtual bool transitionSameTime
        {
            get
            {
                return false;
            }
        }

        protected override void OnPanelOpen(params object[] args)
        {
            m_OpenParam = null;

            if (args.Length > 0)
            {
                m_OpenParam = args[0] as TransitionHelper.OpenParam;
            }

            if (m_OpenParam == null)
            {
                CloseSelfPanel();
                return;
            }

            if (m_OpenParam.action == null)
            {
                m_Action = this;
            }
            else
            {
                m_Action = m_OpenParam.action;
            }
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

            if (m_OpenParam.nextPanelUIID < 0)
            {
                OnNextPanelResLoadFinish();
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
            /*
            if (panel == null)
            {
                CloseSelfPanel();
                return;
            }
            */
            m_NextPanel = panel;

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
            if (m_OpenParam.nextPanelUIID < 0)
            {
                OnNextPanelOpen(null);
            }
            else
            {
                UIMgr.S.OpenPanel(m_OpenParam.nextPanelUIID, OnNextPanelOpen, m_OpenParam.args);
            }

            m_Action.OnNextPanelReady();
        }

        protected override void OnClose()
        {
            if (m_Action != null)
            {
                m_Action.OnTransitionDestroy();
            }

            m_OpenParam = null;

            m_Action = null;

            m_NextPanel = null;

            if (m_NextPanelResLoader != null)
            {
                m_NextPanelResLoader.Recycle2Cache();
                m_NextPanelResLoader = null;
            }
        }

        public void OnTransitionPrepareFinish()
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
            if (m_NextPanel != null)
            {
                m_NextPanel.OnTransitionFinish(true);
            }
            CloseSelfPanel();
        }

        public virtual void PrepareTransition()
        {
            transitionHandler.OnTransitionPrepareFinish();
        }

        public virtual void TransitionIn(AbstractPanel panel)
        {
            transitionHandler.OnTransitionInFinish();
        }

        public virtual void TransitionOut(AbstractPanel panel)
        {
            transitionHandler.OnTransitionOutFinish();
        }

        public void OnTransitionDestroy()
        {
            
        }

        public void OnNextPanelReady()
        {
            
        }
    }
}
