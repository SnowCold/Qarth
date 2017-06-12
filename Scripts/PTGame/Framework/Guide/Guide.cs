//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
	public class Guide : GuideTriggerHandler
    {
        protected int m_GuideId;

		protected List<GuideStep> m_GuideSteps;
		private bool m_IsActive = false;
		private int m_LastFinishStepID = 0;

		public int guideID
		{
			get { return m_GuideId; }
		}
			
        public Guide(int guideID)
        {
            m_GuideId = guideID;
        }
			
		public bool ActiveSelf()
		{
			if (m_IsActive)
			{
				return true;
			}

			m_IsActive = true;

			if (m_GuideSteps == null)
			{
				m_LastFinishStepID = GuideMgr.S.GetGuideLastStep (m_GuideId);

				var dataList = TDGuideStepTable.GetDataAsGuideID(m_GuideId);
				for (int i = 0; i < dataList.Count; ++i)
				{
					if (dataList[i].id <= m_LastFinishStepID)
					{
						continue;
					}
					GuideStep step = new GuideStep (dataList [i].id);
					AddStep (step);
				}
			}

			if (m_GuideSteps == null)
			{
				return false;
			}

            Log.i("#Guide Start:" + m_GuideId);

			TrackNextStep ();
            
			return true;
		}
			
        public void OnStepFinish(GuideStep step)
        {
			if (step.stepID > m_LastFinishStepID)
			{
				m_LastFinishStepID = step.stepID;
				GuideMgr.S.FinishStep(step);
			}
            
			//GuideMgr.S.FinishStep(step);

			TDGuideStep lastStep = TDGuideStepTable.GetGuideLastStep (m_GuideId);

			if (lastStep == null || lastStep.id == m_LastFinishStepID)
			{
                ClearSelf ();
				
				GuideMgr.S.FinishGuide(this);

                Log.i("#Guide Finish:" + m_GuideId);
            }
            else
			{
				TrackNextStep();
			}
        }

		private void TrackNextStep()
		{
			for (int i = 0; i < m_GuideSteps.Count; ++i)
			{
				if (m_GuideSteps[i].stepID <= m_LastFinishStepID)
				{
					m_GuideSteps[i].Finish();
					continue;
				}
				m_GuideSteps[i].StartTrack();
			}
		}

		public bool IsStepFinish(int stepID)
		{
			return stepID <= m_LastFinishStepID;
		}

        protected override bool CheckNeedJump()
        {
            TDGuide data = TDGuideTable.GetData(m_GuideId);

            if (data == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(data.jumpTrigger))
            {
                return false;
            }

            List<ITrigger> triggerList = LoadTrigger(data.jumpTrigger, data.commonParam);
            if (triggerList == null || triggerList.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < triggerList.Count; ++i)
            {
                if (!triggerList[i].isReady)
                {
                    return false;
                }
            }

            return true;
        }

        protected void ClearSelf()
		{
            StopTrack();

            if (!m_IsActive)
			{
				return;
			}

			m_IsActive = false;

			if (m_GuideSteps != null)
			{
				for (int i = 0; i < m_GuideSteps.Count; ++i)
				{
					m_GuideSteps[i].Finish();
				}

				m_GuideSteps = null;
			}
		}

		protected override void OnAllTriggerEvent(bool ready)
		{
			if (ready)
			{
				if (!m_IsActive)
				{
                    ActiveSelf();
				}
			}
			else
			{
				
			}

		}

		protected override List<ITrigger> GetTriggerArray ()
		{
			TDGuide data = TDGuideTable.GetData (m_GuideId);

			if (data == null)
			{
				return null;
			}

			return LoadTrigger(data.trigger, data.commonParam);
		}

        private void AddStep(GuideStep step)
        {
			if (m_GuideSteps == null)
			{
				m_GuideSteps = new List<GuideStep>();
			}

			step.guide = this;

			m_GuideSteps.Add (step);
        }
    }
}
