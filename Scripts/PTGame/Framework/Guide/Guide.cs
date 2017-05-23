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
		private int m_LastFinishStepID = 0;

		public int guideID
		{
			get { return m_GuideId; }
		}

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

		protected void ClearSelf()
		{
			if (!m_IsActive)
			{
				return;
			}

			m_IsActive = false;
			StopTrack ();

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
