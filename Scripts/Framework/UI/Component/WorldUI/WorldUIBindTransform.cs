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
    public class WorldUIBindTransform : WorldUI
    {
        [SerializeField]
        private Transform m_FollowObject;

        public Transform followTransform
        {
            get { return m_FollowObject; }
            set { m_FollowObject = value; UpdateBinding(); }
        }

        protected override bool IsNeedUpdate()
        {
            if (m_FollowObject == null)
            {
                return false;
            }

            return IsWorldPositionInView(m_FollowObject.position);
        }

        protected override void OnWorldUIBinding()
        {
            if (m_FollowObject == null)
            {
                return;
            }

            if (m_Binding == null)
            {
                m_Binding = new WorldUIBinding();
            }

            m_Binding.Set(m_TargetUI, m_FollowObject, m_UIOffset, m_WorldOffset);
        }
    }
}
