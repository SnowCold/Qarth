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
    public class AbstractGuideCommand
    {
        private GuideStep m_GuideStep;
		private bool m_IsRunning = false;

        public GuideStep guideStep
        {
            get { return m_GuideStep; }
            set { m_GuideStep = value; }
        }

		public virtual void SetParam(object[] pv)
		{
			
		}

        protected void FinishStep()
        {
            if (m_GuideStep == null)
            {
                return;
            }

			m_GuideStep.OnCommandFinish();
        }

        public void Start()
        {
			if (m_IsRunning)
			{
				return;
			}

			m_IsRunning = true;
			OnStart ();
        }

		public void Finish(bool forceClean)
        {
			if (!m_IsRunning)
			{
				return;
			}

			m_IsRunning = false;

			OnFinish (forceClean);
        }

		protected virtual void OnStart()
		{
			
		}

		protected virtual void OnFinish(bool forceClean)
		{
			
		}

    }
}
