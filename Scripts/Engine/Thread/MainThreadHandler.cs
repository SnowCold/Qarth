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
    [TMonoSingletonAttribute("[App]/AppLoopMgr")]
    public class MainThreadHandler : TMonoSingleton<MainThreadHandler>, IThreadHandler
    {
        protected TaskLoop m_TaskLoop = new TaskLoop();

        public void PostTask(IThreadTask task)
        {
            m_TaskLoop.PostTask(task);
        }

        public void Dump()
        {

        }

        private void Update()
        {
            m_TaskLoop.OnceLoop();
        }

       
    }
}
