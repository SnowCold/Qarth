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
    public class WorldRoot : MonoBehaviour
    {
        [SerializeField]
        private Camera m_WorldCamera;
        [SerializeField]
        private Transform m_EnvironmentRoot;

        public Camera worldCamera
        {
            get { return m_WorldCamera; }
        }

        public Transform environmentRoot
        {
            get { return m_EnvironmentRoot; }
        }
    }
}
