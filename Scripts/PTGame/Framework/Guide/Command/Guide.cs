using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class Guide
    {
        private int m_Id;
        private CommandSequence m_StepList;

        public GuideStep CreateStep()
        {
            GuideStep step = new GuideStep();
            if (m_StepList == null)
            {
                m_StepList = new CommandSequence();
            }

            m_StepList.Append(step);
            return step;
        }

        public void Start()
        {
            if (m_StepList == null)
            {
                return;
            }

            m_StepList.SetCommandNodeEventListener(OnStepFinish);
            m_StepList.Start();
        }

        private void OnStepFinish(CommandNode node)
        {

        }
    }
}
