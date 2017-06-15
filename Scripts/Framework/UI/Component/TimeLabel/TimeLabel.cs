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
using Qarth;
using UnityEngine.UI;

namespace Qarth
{
    public class TimeLabel : MonoBehaviour
    {
        public enum TimeMode
        {
            Normal,
            CoundDown,
        }

        public delegate string TimeFormater(long time);

        [SerializeField]
        private Text m_TimeLabel;
        [SerializeField]
        private bool m_ZeroPretect = false;
        [SerializeField]
        private TimeMode m_TimeMode = TimeMode.CoundDown;

        public Run OnTimeReach;
        public TimeFormater CustomFormater;

        private int m_Time = -1;
        private bool m_HasTimeReach = true;

        private bool m_IsRunning = true;

        public bool zeroPretect
        {
            get { return m_ZeroPretect; }
            set { m_ZeroPretect = value; }
        }

        public TimeMode timeMode
        {
            get { return m_TimeMode; }
            set { m_TimeMode = value; }
        }

        public bool isRunning
        {
            get { return m_IsRunning; }
            set { m_IsRunning = value; }
        }

        public Text textLabel
        {
            get { return m_TimeLabel; }
        }

        public int time
        {
            get { return m_Time; }
        }

        void Awake()
        {
            if (m_TimeLabel == null)
            {
                m_TimeLabel = GetComponent<Text>();
            }

            InvokeRepeating("ShowTime", 1.0f, 1.0f);

            ShowTime();
        }

        void OnDestroy()
        {
            CancelInvoke("ShowTime");
        }

        void ShowTime()
        {
            if (!m_IsRunning)
            {
                return;
            }

            if (m_TimeLabel != null)
            {
                int leftTime = 0;

                if (m_TimeMode == TimeMode.CoundDown)
                {
                    leftTime = --m_Time;
                }
                else
                {
                    leftTime = ++m_Time;
                }

                int displayLeftTime = leftTime;
                if (leftTime < 0 && m_ZeroPretect)
                {
                    displayLeftTime = 0;
                }

                if (CustomFormater == null)
                {
                    m_TimeLabel.text = DateFormatHelper.FormatMaxUnitTimeOutTwo(displayLeftTime);
                }
                else
                {
                    m_TimeLabel.text = CustomFormater(displayLeftTime);
                }

                if (!m_HasTimeReach && OnTimeReach != null && leftTime < 0)
                {
                    m_HasTimeReach = true;
                    OnTimeReach();
                }
            }
        }

        public void SetTime(int time)
        {
            if (m_Time != time)
            {
                m_HasTimeReach = false;
            }
            m_Time = time;
            ShowTime();
        }
    }
}
