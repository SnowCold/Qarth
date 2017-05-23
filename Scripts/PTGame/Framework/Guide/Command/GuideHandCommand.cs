using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
	public class GuideHandCommand : GuideCommand
	{
		private UINodeFinder m_Finder;
		private bool m_NeedClose = true;

		public override void SetParam (string param)
		{	
			string[] pv = param.Split (',');
			if (pv.Length == 0)
			{
				Log.w ("GuideHandCommand Init With Invalid Param.");
				return;
			}

			try
			{
				m_Finder = new UINodeFinder();
				m_Finder.SetParam(pv[0], pv[1]);

				if (pv.Length > 2)
				{
					m_NeedClose = Helper.String2Bool(pv[2]);
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

			UIMgr.S.OpenTopPanel(EngineUI.GuideHandPanel, null, targetNode);
		}

		public override void OnFinish ()
		{
			if (m_NeedClose)
			{
				UIMgr.S.ClosePanelAsUIID (EngineUI.GuideHandPanel);
			}
		}
	}
}

