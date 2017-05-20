using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GuideTriggerTable
    {
        public static ITrigger[] GetTriggerByGuideID(int guideID)
        {
            if (guideID == 1)
            {
                return new ITrigger[]
                    {
                        new TopPanelTrigger(0),
                    };
            }

            if (guideID == 2)
            {
                return new ITrigger[]
                    {
                        new TopPanelTrigger(1),
                    };
            }

            return null;
        }

        public static ITrigger[] GetTriggerByGuideStepID(int stepID)
        {
            if (stepID == 1)
            {
                return new ITrigger[]
                    {
                        new TopPanelTrigger(0),
                    };
            }

            if (stepID == 2)
            {
                return new ITrigger[]
                    {
                        new TopPanelTrigger(1),
                    };
            }

            return null;
        }
    }
}
