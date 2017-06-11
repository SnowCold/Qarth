//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public enum MsgBoxUIStyle
    {
        NONE = 0,

        ShowTitle = 1,
        ShowContext = 2,
        ShowOneButton = 4,
        ShowTwoButton = 8,

        S_Title_Ok = (ShowTitle | ShowOneButton),
        S_Title_YesNo = (ShowTitle | ShowTwoButton),
        S_Context_OK = (ShowContext | ShowOneButton),
        S_Context_YesNo = (ShowContext | ShowTwoButton),

        S_Title_Context_OK = (ShowTitle | ShowContext | ShowOneButton),
        Default = (ShowTitle | ShowContext | ShowTwoButton),
    }

    public class MsgBoxPanel : AbstractPanel
    {


        [SerializeField]
        private Text m_Title;
        [SerializeField]
        private Text m_Context;
        [SerializeField]
        private Button m_OkButton;
        [SerializeField]
        private Button m_CancelButton;

        [SerializeField]
        private Button m_CenterButton;
        [SerializeField]
        private Text m_OkButtonText;
        [SerializeField]
        private Text m_CancelButtonText;
        [SerializeField]
        private Text m_CenterButtonText;

        private Action m_OkListener;
        private Action m_CancelListener;

        private MsgBoxUIStyle m_UIStyle = MsgBoxUIStyle.Default;

        public MsgBoxUIStyle uiStyle
        {
            set
            {
                if (m_UIStyle == value)
                {
                    return;
                }

                m_UIStyle = value;
                UpdateStyle();
            }
        }

        public string title
        {
            set { m_Title.text = value; }
        }

        public string context
        {
            set
            {
                m_Context.text = value;
            }
        }

        public string okButtonTitle
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    string title = "OK";
                    m_OkButtonText.text = title;
                    m_CenterButtonText.text = title;
                }
                else
                {
                    m_OkButtonText.text = value;
                    m_CenterButtonText.text = value;
                }
            }
        }

        public string cancelButtonTitle
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    string title = "Cancel";
                    m_CancelButtonText.text = title;
                }
                else
                {
                    m_CancelButtonText.text = value;
                }
            }
        }

        public Action okListener
        {
            set { m_OkListener = value; }
        }

        public Action cancelListener
        {
            set { m_CancelListener = value; }
        }

        private void UpdateStyle()
        {

            if (CheckEnable(MsgBoxUIStyle.ShowTitle))
            {
                m_Title.gameObject.SetActive(true);
            }
            else
            {
                m_Title.gameObject.SetActive(false);
            }

            if (CheckEnable(MsgBoxUIStyle.ShowContext))
            {
                m_Context.gameObject.SetActive(true);
            }
            else
            {
                m_Context.gameObject.SetActive(false);
            }

            if (CheckEnable(MsgBoxUIStyle.ShowTwoButton))
            {
                m_OkButton.gameObject.SetActive(true);
                m_CancelButton.gameObject.SetActive(true);
                m_CenterButton.gameObject.SetActive(false);
            }
            else
            {
                if (CheckEnable(MsgBoxUIStyle.ShowOneButton))
                {
                    m_CenterButton.gameObject.SetActive(true);
                }
                else
                {
                    m_CenterButton.gameObject.SetActive(false);
                }

                m_OkButton.gameObject.SetActive(false);
                m_CancelButton.gameObject.SetActive(false);
            }
        }

        private bool CheckEnable(MsgBoxUIStyle mask)
        {
            return CheckBit(m_UIStyle, mask);
        }

        private bool CheckBit(MsgBoxUIStyle target, MsgBoxUIStyle mask)
        {
            return CheckBit((int)target, (int)mask);
        }

        private bool CheckBit(int value, int mask)
        {
            if(((value ^ mask) & mask) == 0)
            {
                return true;
            }

            return false;
        }

        protected override void OnUIInit()
        {
            m_CancelButton.onClick.AddListener(OnCloseButtonClick);
            m_OkButton.onClick.AddListener(OnOkButtonClick);
            m_CenterButton.onClick.AddListener(OnOkButtonClick);
        }

        protected override void OnOpen()
        {
            uiStyle = m_UIStyle;
        }

        protected override void OnClose()
        {
            m_OkListener = null;
            m_CancelListener = null;
        }

        private void OnOkButtonClick()
        {
            if (m_OkListener != null)
            {
                m_OkListener();
            }

            CloseSelfPanel();
        }

        private void OnCloseButtonClick()
        {
            if (m_CancelListener != null)
            {
                m_CancelListener();
            }
            CloseSelfPanel();
        }
    }
}
