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
    public class KeyCodeEventInfo
    {
        private bool m_IsProcess = false;

        public void Process()
        {
            m_IsProcess = true;
        }

        public bool IsProcess()
        {
            return m_IsProcess;
        }

        public void Reset()
        {
            m_IsProcess = false;
        }

    }
}
