using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class HighlightMaskPanel : AbstractPanel
    {
        [SerializeField]
        private GuideHighlightMask m_HighlightMask;

		protected override void OnPanelOpen (params object[] args)
		{
			if (args.Length < 2)
			{
				return;
			}

			m_HighlightMask.target = args [0] as RectTransform;
			m_HighlightMask.shape = (GuideHighlightMask.Shape)args [1];
		}
    }
}
