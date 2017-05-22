using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class Guide : GuideTriggerHandler
    {
        protected int m_GuideId;

		protected List<GuideStep> m_GuideSteps;
		private bool m_IsActive = false;
        public Guide(int guideID)
        {
            m_GuideId = guideID;
        }
			
		public bool Active()
		{
			if (m_IsActive)
			{
				return true;
			}

			m_IsActive = true;

			if (m_GuideSteps == null)
			{
				int lastKeyStepId = GuideMgr.S.GetGuideLastStep (m_GuideId);

				var dataList = TDGuideStepTable.GetDataAsGuideID(m_GuideId);
				for (int i = 0; i < dataList.Count; ++i)
				{
					if (dataList[i].id <= lastKeyStepId)
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

			for (int i = 0; i < m_GuideSteps.Count; ++i)
			{
				m_GuideSteps[i].StartTrack();
			}

			return true;
		}

        public void OnStepFinish(GuideStep step)
        {
            GuideMgr.S.FinishStep(step);

            //如果当前Step是该引导的最后一个step，那么记录关闭
        }

		protected override void OnAllTriggerEvent(bool ready)
		{
			if (ready)
			{
				if (!m_IsActive)
				{
					GuideMgr.S.TryActiveGuide (this);
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

			return LoadTrigger (data.trigger1);
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
