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
    public class ThreadTaskTester : MonoBehaviour
    {
        private static int s_TaskCount = 500000;

        public class ProcessTask : CalculateResultTask
        {
            private string m_Context;

            public ProcessTask(string context, IThreadHandler threadHandler) : base(threadHandler)
            {
                m_Context = context;
            }

            public override bool Execute()
            {
                if (m_Context != null)
                {
                    m_Context = m_Context.Replace("abc", "中文");
                }

                SendResult();
                return true;
            }

            public override void ProcessResult()
            {
                --s_TaskCount;
                if ((s_TaskCount % 10000) == 0)
                {
                    Log.i("Result:" + s_TaskCount + ":" + Time.frameCount);
                }
                //Log.i("Result:" + m_Context + Time.frameCount);
            }
        }

        private void Awake()
        {
            ThreadMgr.S.Init();

            Log.i("### :" + Time.frameCount);
            for (int i = 0; i < s_TaskCount; ++i)
            {
                ThreadMgr.S.workThread.PostTask(new ProcessTask("askjhdajksdjkashd,abcasjkhdajksdhkjas-abc", ThreadMgr.S.mainThread));
            }
        }
    }
}
