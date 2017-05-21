using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Game]/GuideMgr")]
    public class GuideMgr : TMonoSingleton<GuideMgr>
    {
        private List<Guide> m_TrackingGuideList = new List<Guide>();
        private GuideStep m_CurretStep;
		private Guide m_CurrentGuide;

        public void InitGuideMgr()
        {
            Log.i("Init[Guide Mgr]");
        }

        public override void OnSingletonInit()
        {
            AddTrackingGuide(new Guide(1));
            AddTrackingGuide(new Guide(2));

            for (int i = m_TrackingGuideList.Count - 1; i >= 0; --i)
            {
                if (!m_TrackingGuideList[i].StartTrack())
                {
                    m_TrackingGuideList.RemoveAt(i);
                }
            }
        }

        public void TryActiveStep(GuideStep step)
        {
            if (m_CurretStep != null)
            {
                return;
            }

            m_CurretStep = step;

            m_CurretStep.Active();
        }

		public void TryActiveGuide(Guide guide)
		{
			if (m_CurrentGuide != null)
			{
				return;
			}

			m_CurrentGuide = guide;

			m_CurrentGuide.Active();
		}

        public void FinishStep(GuideStep step)
        {
            if (m_CurretStep == step)
            {
                m_CurretStep = null;
            }
        }

        private void AddTrackingGuide(Guide guide)
        {
            if (guide == null)
            {
                return;
            }

            if (guide.CheckNeedTrack())
            {
                m_TrackingGuideList.Add(guide);
            }
        }
    }
}
