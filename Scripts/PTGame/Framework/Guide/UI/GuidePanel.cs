using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class GuidePanel : AbstractPanel
    {
        [SerializeField]
        private GuideHighlightMask m_HighlightMask;

        public GuideHighlightMask highlightMask
        {
            get { return m_HighlightMask; }
        }
    }
}
