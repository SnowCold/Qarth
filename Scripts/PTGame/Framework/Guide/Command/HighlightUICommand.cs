using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class HighlightUICommand : AbstractGuideCommand
    {
		private UINodeFinder m_Finder;
        private GuideHighlightMask.Shape m_Shape;
		private bool m_NeedClose = true;

		public override void SetParam(string[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("HighlightUICommand Init With Invalid Param.");
				return;
			}

			try
			{
				m_Finder = new UINodeFinder();
				m_Finder.SetParam(pv);

				if (pv.Length > 2)
				{
					this.m_Shape = (GuideHighlightMask.Shape)int.Parse(pv[2]);
					if (pv.Length > 3)
					{
						m_NeedClose = Helper.String2Bool(pv[3]);
					}
				}

			}
			catch(Exception e)
			{
				Log.e (e);
			}

		}

        public override void Start()
        {
			RectTransform targetNode = m_Finder.FindNode() as RectTransform;

			if (targetNode == null)
            {
                return;
            }

			UIMgr.S.OpenTopPanel(EngineUI.HighlightMaskPanel, null, targetNode, m_Shape);
		}

        public override void OnFinish()
        {
			if (m_NeedClose)
			{
				UIMgr.S.ClosePanelAsUIID(EngineUI.HighlightMaskPanel);	
			}
        }
    }
}
