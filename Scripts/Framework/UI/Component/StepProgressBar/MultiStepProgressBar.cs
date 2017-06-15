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

namespace Qarth
{
    public class MultiStepProgressBar : MonoBehaviour
    {
        [SerializeField]
        private bool m_AutoBiild = false;
        [SerializeField]
        private Image[] m_TargetImages;
        [SerializeField, Range(0, 1)]
        private float m_Progress;
        [SerializeField]
        private float[] m_Precents;

        public float progress
        {
            get { return m_Progress; }
            set
            {
                m_Progress = value;
                m_Progress = Mathf.Min(1, Mathf.Max(0, m_Progress));
                UpdateUI();
            }
        }

        public Image[] targetImages
        {
            get { return m_TargetImages; }
            set { m_TargetImages = value; }
        }

        public float[] precents
        {
            get { return m_Precents; }
            set { m_Precents = value; }
        }


        private void Awake()
        {
            InitPrecentData();
        }

        private void InitPrecentData()
        {
            if (!m_AutoBiild)
            {
                return;
            }

            RectTransform selfTransform = GetComponent<RectTransform>();
            float totalWidth = selfTransform.rect.width;

            m_Precents = new float[m_TargetImages.Length];
            for (int i = 0; i < m_TargetImages.Length; ++i)
            {
                RectTransform rectTr = m_TargetImages[i].GetComponent<RectTransform>();
                m_Precents[i] = rectTr.rect.width / totalWidth;
            }
        }

        private void UpdateUI()
        {
            if (m_TargetImages == null || m_Precents == null)
            {
                return;
            }

            float preValue = 0;
            for (int i = 0; i < m_TargetImages.Length; ++i)
            {
                if (i >= m_Precents.Length)
                {
                    continue;
                }

                if (m_TargetImages[i] != null)
                {
                    float v = (m_Progress - preValue) / m_Precents[i];
                    m_TargetImages[i].fillAmount = v;
                }

                preValue += m_Precents[i];
            }
        }

        private void OnValidate()
        {
            UpdateUI();

            if (m_AutoBiild)
            {
                if (m_TargetImages == null || m_TargetImages.Length == 0)
                {
                    int childCount = transform.childCount;
                    m_TargetImages = new Image[childCount];
                    for (int i = 0; i < childCount; ++i)
                    {
                        m_TargetImages[i] = transform.GetChild(i).GetComponent<Image>();
                    }

                    InitPrecentData();
                }
            }
        }
    }
}
