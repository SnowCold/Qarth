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
    public class ThreadMgr : TSingleton<ThreadMgr>
    {
        private ThreadHandler m_WorkThread;
        private MainThreadHandler m_MainThread;

        public void Init()
        {
            m_MainThread = MainThreadHandler.S;
            m_WorkThread = new ThreadHandler("WorkThread");
        }

        public IThreadHandler workThread
        {
            get { return m_WorkThread; }
        }

        public IThreadHandler mainThread
        {
            get { return m_MainThread; }
        }

    }
}
