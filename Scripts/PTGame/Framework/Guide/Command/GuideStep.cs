using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class GuideStep
    {
        private int m_GuideStepID;

        private Guide m_Guide;
        protected ITrigger[] m_Trigger;
        protected GuideCommand[] m_Commands;

        public Guide guide
        {
            get { return m_Guide; }
            set { m_Guide = value; }
        }

        public GuideStep(int stepID)
        {
            m_GuideStepID = stepID;
        }

        public void StartTrack()
        {
            m_Trigger = GuideTriggerTable.GetTriggerByGuideStepID(m_GuideStepID);
            if (m_Trigger == null)
            {
                return;
            }

            InnerStartTrack();
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

        public void Active()
        {
            if (m_Trigger != null)
            {
                for (int i = 0; i < m_Trigger.Length; ++i)
                {
                    m_Trigger[i].Stop();
                }

                m_Trigger = null;
            }

            m_Commands = GuideCommandTable.GetGuideCommandByStepID(m_GuideStepID);

            if (m_Commands == null)
            {
                return;
            }

            for (int i = 0; i < m_Commands.Length; ++i)
            {
                m_Commands[i].guideStep = this;
                m_Commands[i].Start();
            }
        }

        private void OnTriggerEvent(ITrigger trigger)
        {
            GuideMgr.S.TryActiveStep(this);
        }

        public void Finish()
        {
            if (m_Commands == null || m_Commands.Length == 0)
            {
                m_Guide.OnStepFinish(this);
                return;
            }

            for (int i = 0; i < m_Commands.Length; ++i)
            {
                m_Commands[i].OnFinish();
            }

            m_Guide.OnStepFinish(this);
        }
    }
}
