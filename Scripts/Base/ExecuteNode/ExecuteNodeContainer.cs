//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{

    public class ExecuteNodeContainer
    {
        #region Event
        public Run<float>           On_ExecuteScheduleEvent;
        public Run<string>          On_ExecuteTipsEvent;
        public Run                  On_ExecuteContainerBeginEvent;
        public Run                  On_ExecuteContainerEndEvent;
        #endregion

        #region 属性&字段
        private List<ExecuteNode>   m_NodeList;
        private int                 m_CurrentIndex;
        private ExecuteNode         m_CurrentNode;

        private float               m_TotalSchedule = 0;
        #endregion

        public float totalSchedule
        {
            get { return m_TotalSchedule; }
        }

        public ExecuteNode currentNode
        {
            get
            {
                return m_CurrentNode;
            }
        }

        public void Append(ExecuteNode item)
        {
            if (m_NodeList == null)
            {
                m_NodeList = new List<ExecuteNode>();
                m_CurrentIndex = -1;
            }

            m_NodeList.Add(item);
        }

        public void Start()
        {
            m_CurrentIndex = -1;
            MoveToNextUpdateFunc();
        }

        public void Update()
        {
            if (m_CurrentNode != null)
            {
                m_CurrentNode.OnExecute();

                float schedule = m_CurrentNode.progress;

                m_TotalSchedule = m_CurrentIndex * (1.0f / m_NodeList.Count) + schedule / m_NodeList.Count;

                if (On_ExecuteScheduleEvent != null)
                {
                    On_ExecuteScheduleEvent(m_TotalSchedule);
                }

                if (m_CurrentNode.isFinish)
                {
                    MoveToNextUpdateFunc();
                }
            }
        }

        private void MoveToNextUpdateFunc()
        {
            if (m_CurrentNode != null)
            {
                m_CurrentNode.OnEnd();
            }

            ++m_CurrentIndex;
            if (m_CurrentIndex >= m_NodeList.Count)
            {
                m_TotalSchedule = 1.0f;
                m_CurrentNode = null;

                if (On_ExecuteContainerEndEvent != null)
                {
                    On_ExecuteContainerEndEvent();

                    On_ExecuteContainerEndEvent = null;
                }
            }
            else
            {
                m_CurrentNode = m_NodeList[m_CurrentIndex];
                m_CurrentNode.OnBegin();

                if (m_CurrentIndex == 0)
                {
                    if (On_ExecuteContainerBeginEvent != null)
                    {
                        On_ExecuteContainerBeginEvent();
                    }
                }

                if (On_ExecuteTipsEvent != null)
                {
                    On_ExecuteTipsEvent(m_CurrentNode.tips);
                }
            }
        }

    }

}
