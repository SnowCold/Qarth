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
    public class SizeAdjuster : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_Target;
        [SerializeField]
        private Vector2 m_Offset;

        private RectTransform m_SelfTransform;

        private void Awake()
        {
            m_SelfTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (m_Target == null)
            {
                return;
            }

            Vector2 size = m_Target.rect.size + m_Offset;
            m_SelfTransform.sizeDelta = size;
        }
    }
}
