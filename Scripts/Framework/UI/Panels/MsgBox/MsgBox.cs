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
    public class MsgBox : TSingleton<MsgBox>
    {
        public class MsgBoxBuilder
        {
            private string m_Title;
            private string m_Context;
            private bool m_IsModal;
            private Action m_OkListener;
            private Action m_CancelListener;
            private MsgBoxUIStyle m_UIStyle;
            private string m_OKButtonTitle;
            private string m_CancelButtonTitle;
            private bool m_AutoReset;

            public MsgBoxBuilder(bool autoReset = false)
            {
                m_AutoReset = autoReset;
            }

            public void Reset()
            {
                m_Title = null;
                m_Context = null;
                m_IsModal = false;
                m_OkListener = null;
                m_CancelListener = null;
                m_OKButtonTitle = null;
                m_CancelButtonTitle = null;
            }

            public MsgBoxBuilder SetOKButtonTitle(string title)
            {
                m_OKButtonTitle = title;
                return this;
            }

            public MsgBoxBuilder SetCancelButtonTitle(string title)
            {
                m_CancelButtonTitle = title;
                return this;
            }

            public MsgBoxBuilder SetUIStyle(MsgBoxUIStyle style)
            {
                m_UIStyle = style;
                return this;
            }

            public MsgBoxBuilder SetTitle(string title)
            {
                m_Title = title;
                return this;
            }

            public MsgBoxBuilder SetContext(string context)
            {
                m_Context = context;
                return this;
            }

            public MsgBoxBuilder SetModal(bool isModal)
            {
                m_IsModal = isModal;
                return this;
            }

            public MsgBoxBuilder SetOkListener(Action listener)
            {
                m_OkListener = listener;
                return this;
            }

            public MsgBoxBuilder SetCancelListener(Action listener)
            {
                m_CancelListener = listener;
                return this;
            }

            public void Show()
            {
                UIMgr.S.OpenTopPanel(EngineUI.MsgBoxPanel, (panel) =>
                {
                    if (panel == null)
                    {
                        return;
                    }

                    MsgBoxPanel mbP = panel as MsgBoxPanel;

                    if (mbP == null)
                    {
                        return;
                    }

                    mbP.uiStyle = m_UIStyle;
                    mbP.title = m_Title;
                    mbP.context = m_Context;
                    mbP.okListener = m_OkListener;
                    mbP.cancelListener = m_CancelListener;
                    mbP.okButtonTitle = m_OKButtonTitle;
                    mbP.cancelButtonTitle = m_CancelButtonTitle;

                    if (m_IsModal)
                    {
                        mbP.hideMask = PanelHideMask.UnInteractive;
                    }
                    else
                    {
                        mbP.hideMask = PanelHideMask.None;
                    }

                    if (m_AutoReset)
                    {
                        Reset();
                    }
                });
            }
        }


        private MsgBoxBuilder m_TempBuilder = new MsgBoxBuilder(true);

        public void Show(string title, string context, bool modal, MsgBoxUIStyle style = MsgBoxUIStyle.Default, Action okCallback = null, Action cancelCallback = null, string okTitle = null, string cancelTitle = null)
        {
            m_TempBuilder.SetTitle(title)
                .SetContext(context)
                .SetModal(modal)
                .SetOkListener(okCallback)
                .SetCancelListener(cancelCallback)
                .SetUIStyle(style)
                .SetOKButtonTitle(okTitle)
                .SetCancelButtonTitle(cancelTitle)
                .Show();
        }

        public MsgBoxBuilder CreateBuilder()
        {
            return new MsgBoxBuilder();
        }
    }
}
