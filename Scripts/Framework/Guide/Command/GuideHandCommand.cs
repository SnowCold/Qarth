//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
	public class GuideHandCommand : AbstractGuideCommand
	{
		private IUINodeFinder m_Finder;
		private bool m_NeedClose = true;
		private Vector3 m_Offset;

		public override void SetParam (object[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("GuideHandCommand Init With Invalid Param.");
				return;
			}

			try
			{
				m_Finder = pv[0] as IUINodeFinder;

				if (pv.Length > 1)
				{
					m_NeedClose = Helper.String2Bool((string)pv[1]);
				}

				if (pv.Length > 2)
				{
					m_Offset = Helper.String2Vector3((string)pv[2], '|');
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

			UIMgr.S.OpenTopPanel(EngineUI.GuideHandPanel, null, targetNode, m_Offset);
		}

		protected override void OnFinish (bool forceClean)
		{
			if (m_NeedClose || forceClean)
			{
				UIMgr.S.ClosePanelAsUIID (EngineUI.GuideHandPanel);
			}
		}
	}
}

