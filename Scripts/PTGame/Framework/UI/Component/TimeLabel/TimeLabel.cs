﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTGame.Framework;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class TimeLabel : MonoBehaviour
    {
        public delegate string TimeFormater(long time);

        [SerializeField]
        private Text m_TimeLabel;
        [SerializeField]
        private bool m_ZeroPretect = false;

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

        public bool isRunning
        {
            get { return m_IsRunning; }
            set { m_IsRunning = value; }
        }

        public Text textLabel
        {
            get { return m_TimeLabel; }
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
                int leftTime = --m_Time;

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
