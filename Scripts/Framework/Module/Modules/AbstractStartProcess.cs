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
    public class AbstractStartProcess : AbstractMonoModule
    {
        private ExecuteNodeContainer    m_ExecuteContainer;
        private Action                  m_OnProcessFinish;

        public void SetFinishListener(Action listener)
        {
            m_OnProcessFinish = listener;
        }

        public ExecuteNodeContainer executeContainer
        {
            get
            {
                return m_ExecuteContainer;
            }
        }

        public float totalSchedule
        {
            get
            {
                if (m_ExecuteContainer == null)
                {
                    return 0;
                }

                return m_ExecuteContainer.totalSchedule;
            }
        }

        public void Append(ExecuteNode node)
        {
            if (node == null)
            {
                return;
            }

            if (m_ExecuteContainer == null)
            {
                m_ExecuteContainer = new ExecuteNodeContainer();
            }
            m_ExecuteContainer.Append(node);
        }

        protected override void OnComAwake()
        {
            InitExechuteContainer();
        }

        public override void OnComStart()
        {
            if (m_ExecuteContainer == null)
            {
                return;
            }

            m_ExecuteContainer.On_ExecuteContainerEndEvent += OnAllExecuteNodeEnd;
            m_ExecuteContainer.Start();
        }

        private void Update()
        {
            if (m_ExecuteContainer == null)
            {
                return;
            }

            m_ExecuteContainer.Update();
        }

        protected virtual void InitExechuteContainer()
        {

        }

        protected virtual void OnAllExecuteNodeEnd()
        {
            Log.i("#BaseStartProcess: OnAllExecuteNodeEnd");
            m_ExecuteContainer.On_ExecuteContainerEndEvent -= OnAllExecuteNodeEnd;

            if (m_OnProcessFinish != null)
            {
                m_OnProcessFinish();
            }

            m_ExecuteContainer = null;

            actor.RemoveCom(this);
        }
    }
}
