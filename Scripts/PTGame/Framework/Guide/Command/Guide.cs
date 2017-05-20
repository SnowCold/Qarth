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

        //激活检测
        public bool StartTrack()
        {
            m_Trigger = GuideTriggerTable.GetTriggerByGuideID(m_GuideId);
            if (m_Trigger == null)
            {
                return false;
            }

            InnerStartTrack();
            return true;
        }

        private void InnerStartTrack()
        {
            for (int i = 0; i < m_Trigger.Length; ++i)
            {
                if (m_Trigger[i].isReady)
                {
                    OnTriggerEvent(m_Trigger[i]);
                    return;
                }
            }

            for (int i = 0; i < m_Trigger.Length; ++i)
            {
                m_Trigger[i].Start(OnTriggerEvent);
            }
        }

        public void OnStepFinish(GuideStep step)
        {
            GuideMgr.S.FinishStep(step);
            //如果当前Step是该引导的最后一个step，那么记录关闭
        }

        private void OnTriggerEvent(ITrigger trigger)
        {
            trigger.Stop();
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
