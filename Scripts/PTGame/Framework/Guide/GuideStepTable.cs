using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GuideStepTable
    {
        public static GuideStep[] GetGuideStepByGuideID(int guideID)
        {
            if (guideID == 1)
            {
                return new GuideStep[]
                    {
                        new GuideStep(1),
                        new GuideStep(2),
                    };
            }

            if (guideID == 2)
            {
                return new GuideStep[]
                    {
                        new GuideStep(4),
                        new GuideStep(5),
                    };
            }

            return null;
        }
    }
}
