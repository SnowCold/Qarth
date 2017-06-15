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
    public class StepProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image[] m_TargetImages;
        [SerializeField, Range(0, 1)]
        private float m_Progress;

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

        private void UpdateUI()
        {
            if (m_TargetImages == null || m_TargetImages.Length == 0)
            {
                return;
            }

            float step = 1.0f / m_TargetImages.Length;
            for (int i = 0; i < m_TargetImages.Length; ++i)
            {
                if (m_TargetImages[i] != null)
                {
                    float v = (m_Progress - (i * step)) / step;
                    m_TargetImages[i].fillAmount = v;
                }
            }
        }

        private void OnValidate()
        {
            UpdateUI();
            if (m_TargetImages == null || m_TargetImages.Length == 0)
            {
                int childCount = transform.childCount;
                m_TargetImages = new Image[childCount];
                for (int i = 0; i < childCount; ++i)
                {
                    m_TargetImages[i] = transform.GetChild(i).GetComponent<Image>();
                }
            }
        }
    }
}
