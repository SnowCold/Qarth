using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GuideCommandTable
    {
        public static GuideCommand[] GetGuideCommandByStepID(int stepID)
        {
            if (stepID == 1)
            {
                return new GuideCommand[]
                    {
                        new ButtonHackCommand("HomePanel", "Root/Button"),
                        new HighlightUICommand("HomePanel", "Root/Button", GuideHighlightMask.Shape.Square),
                    };
            }

            if (stepID == 2)
            {
                return new GuideCommand[]
                    {
                        new ButtonHackCommand("HomePanel", "Root/Button"),
                        new HighlightUICommand("HomePanel", "Root/Button", GuideHighlightMask.Shape.Square),
                    };
            }

            return null;
        }
    }
}
