using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class HighlightUICommand : AbstractGuideCommand
    {
		private IUINodeFinder m_Finder;
        private GuideHighlightMask.Shape m_Shape;
		private bool m_NeedClose = true;

		public override void SetParam(object[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("HighlightUICommand Init With Invalid Param.");
				return;
			}

			try
			{
				m_Finder = pv[0] as IUINodeFinder;

				if (pv.Length > 1)
				{
					this.m_Shape = (GuideHighlightMask.Shape)int.Parse((string)pv[1]);
					if (pv.Length > 2)
					{
						m_NeedClose = Helper.String2Bool((string)pv[2]);
					}
				}

			}
			catch(Exception e)
			{
				Log.e (e);
			}

		}

		protected override void OnStart()
        {
			RectTransform targetNode = m_Finder.FindNode(false) as RectTransform;

			if (targetNode == null)
            {
                return;
            }

			UIMgr.S.OpenTopPanel(EngineUI.HighlightMaskPanel, null, targetNode, m_Shape);
		}

		protected override void OnFinish(bool forceClean)
        {
			if (m_NeedClose || forceClean)
			{
				UIMgr.S.ClosePanelAsUIID(EngineUI.HighlightMaskPanel);	
			}
        }
    }
}
