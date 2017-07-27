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
    public class AutoDisableBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float m_DelayTime = -1;

        private TimeItem m_TimeItem;

        public float delayTime
        {
            get { return m_DelayTime; }
            set { m_DelayTime = value; }
        }

        private void OnEnable()
        {
            if (m_DelayTime > 0)
            {
                m_TimeItem = Timer.S.Post2Scale(OnTimeReach, m_DelayTime);
            }
        }

        private void OnDisable()
        {
            if (m_TimeItem != null)
            {
                m_TimeItem.Cancel();
                m_TimeItem = null;
            }
        }

        private void OnTimeReach(int count)
        {
            m_TimeItem = null;
            gameObject.SetActive(false);
        }
    }
}
