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
using System.Threading;

namespace Qarth
{
    public class TaskLoop
    {
        protected object m_Lock = new object();

        private List<IThreadTask> m_AddTaskList;
        private List<IThreadTask> m_TaskList;

        public void PostTask(IThreadTask task)
        {
            lock (m_Lock)
            {
                if (m_AddTaskList == null)
                {
                    m_AddTaskList = new List<IThreadTask>();
                }

                if (m_TaskList == null)
                {
                    m_TaskList = new List<IThreadTask>();
                }

                m_AddTaskList.Add(task);
            }
        }

        public bool OnceLoop()
        {
            if (m_TaskList == null)
            {
                return false;
            }

            if (m_AddTaskList.Count > 0)
            {
                lock (m_Lock)
                {
                    m_TaskList.AddRange(m_AddTaskList);
                    m_AddTaskList.Clear();
                }
            }

            if (m_TaskList.Count <= 0)
            {
                return false;
            }
            else
            {
                for (int i = m_TaskList.Count - 1; i >= 0; --i)
                {
                    if (m_TaskList[i].Execute())
                    {
                        m_TaskList.RemoveAt(i);
                    }
                }
            }

            return true;
        }

    }
}
