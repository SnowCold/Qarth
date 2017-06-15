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
    public class ProgressPositionSetter : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float m_Progress;
        [SerializeField]
        private Transform m_Target;
        [SerializeField]
        private Vector3 m_StartLocalPosition;
        [SerializeField]
        private Vector3 m_EndLocalPosition;

        public float progress
        {
            get { return m_Progress; }
            set
            {
                m_Progress = value;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (m_Target == null)
            {
                return;
            }

            Vector3 dis = m_EndLocalPosition - m_StartLocalPosition;
            dis = dis * m_Progress;
            Vector3 pos = m_StartLocalPosition + dis;
            m_Target.localPosition = pos;
        }
    }
}
