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
    public class WorldUIBindPos : WorldUI
    {
        [SerializeField]
        private Vector3 m_FollowPosition;

        public Vector3 followPosition
        {
            get { return m_FollowPosition; }
            set { m_FollowPosition = value; UpdateBinding(); }
        }

        protected override bool IsNeedUpdate()
        {
            return IsWorldPositionInView(m_FollowPosition);
        }

        protected override void OnWorldUIBinding()
        {
            if (m_Binding == null)
            {
                m_Binding = new WorldUIBinding();
            }

            m_Binding.Set(m_TargetUI, m_FollowPosition, m_UIOffset, m_WorldOffset);
        }
    }
}
