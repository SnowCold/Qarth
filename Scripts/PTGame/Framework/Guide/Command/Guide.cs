using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class Guide
    {
        protected int m_GuideId;

        protected ITrigger[] m_Trigger;
        protected GuideStep[] m_GuideSteps;
		private bool m_IsActive = false;
        public Guide(int guideID)
        {
            m_GuideId = guideID;
        }

        //检测是否需要激活:
        public bool CheckNeedTrack()
        {
            if (m_GuideId <= 0)
            {
                return false;
            }

            bool isComplate = DataRecord.S.GetBool(GetGuideKey(m_GuideId));

            if (isComplate)
            {
                return false;
            }

            return true;
        }

        //激活
        public bool StartTrack()
        {
			//
            m_Trigger = GuideTriggerTable.GetTriggerByGuideID(m_GuideId);
            if (m_Trigger == null)
            {
				return false;
            }

			for (int i = 0; i < m_Trigger.Length; ++i)
			{
				m_Trigger[i].Start(OnTriggerEvent);
			}

			CheckAllTriggerstate();
            return true;
        }

		public void Active()
		{
			if (m_IsActive)
			{
				return;
			}

			m_IsActive = true;

			if (m_GuideSteps == null)
			{
				m_GuideSteps = GuideStepTable.GetGuideStepByGuideID(m_GuideId);
			}

			if (m_GuideSteps == null)
			{
				return;
			}

			for (int i = 0; i < m_GuideSteps.Length; ++i)
			{
				m_GuideSteps[i].guide = this;
				m_GuideSteps[i].StartTrack();
			}
		}

        public void OnStepFinish(GuideStep step)
        {
            GuideMgr.S.FinishStep(step);
            //如果当前Step是该引导的最后一个step，那么记录关闭
        }

		private void CheckAllTriggerstate()
		{
			if (m_Trigger == null || m_Trigger.Length == 0)
			{
				return;
			}

			for (int i = 0; i < m_Trigger.Length; ++i)
			{
				if (!m_Trigger[i].isReady)
				{
					OnTriggerEvent (false);
					return;
				}
			}

			OnAllTriggerEvent (true);
		}

		private void OnAllTriggerEvent(bool ready)
		{
			if (ready)
			{
				if (!m_IsActive)
				{
					GuideMgr.S.TryActiveGuide (this);
				}

				m_GuideSteps = GuideStepTable.GetGuideStepByGuideID(m_GuideId);

				if (m_GuideSteps == null)
				{
					return;
				}

				for (int i = 0; i < m_GuideSteps.Length; ++i)
				{
					m_GuideSteps[i].guide = this;
					m_GuideSteps[i].StartTrack();
				}
			}
			else
			{
				
			}

		}

        private void OnTriggerEvent(bool result, ITrigger trigger)
        {
			CheckAllTriggerstate ();
        }

        private void AddStep(GuideStep[] steps)
        {
            if (steps == null || steps.Length == 0)
            {
                return;
            }

            m_GuideSteps = steps;

            for (int i = 0; i < steps.Length; ++i)
            {
                steps[i].guide = this;
            }
        }

        private string GetGuideKey(int guideID)
        {
            return string.Format("guide_{0}", guideID);
        }
    }
}
