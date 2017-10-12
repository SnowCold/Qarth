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
    public class TransitionHelper
    {
        public class OpenParam
        {
            private AbstractPanel m_PrePanel;
            private int m_NextPanelUIID = -1;
            private bool m_NeedClosePrePanel;
            private object[] m_Args;
            private ITransitionAction m_Action;

            public int nextPanelUIID
            {
                get { return m_NextPanelUIID; }
            }

            public OpenParam SetNextPanelUIID<T>(T uiid) where T : IConvertible
            {
                m_NextPanelUIID = uiid.ToInt32(null);
                return this;
            }

            public ITransitionAction action
            {
                get { return m_Action; }
            }

            public OpenParam SetTransitionAction(ITransitionAction action)
            {
                m_Action = action;
                return this;
            }

            public bool needClosePrePanel
            {
                get { return m_NeedClosePrePanel; }
            }

            public OpenParam SetNeedClosePrePanel(bool needClose)
            {
                m_NeedClosePrePanel = needClose;
                return this;
            }

            public AbstractPanel prePanel
            {
                get { return m_PrePanel; }
            }

            public OpenParam SetPrePanel(AbstractPanel panel)
            {
                m_PrePanel = panel;
                return this;
            }

            public object[] args
            {
                get { return m_Args; }
            }

            public OpenParam SetOpenParam(params object[] args)
            {
                m_Args = args;
                return this;
            }

            public void Open()
            {
                UIMgr.S.OpenTopPanel(EngineUI.TransitionPanel, null, this);
            }
        }

        public static OpenParam CreateBuilder()
        {
            OpenParam param = new OpenParam();
            return param;
        }

    }
}
