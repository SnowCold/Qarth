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
    public class CalculateResultTask : AbstractTask
    {
        protected IThreadHandler m_SourceThread;

        public CalculateResultTask(IThreadHandler sourceThread)
        {
            m_SourceThread = sourceThread;
        }

        protected void SendResult()
        {
            if (m_SourceThread == null)
            {
                return;
            }

            m_SourceThread.PostTask(new ResultTask(this));
        }

        public override void ProcessResult()
        {

        }
    }
}
