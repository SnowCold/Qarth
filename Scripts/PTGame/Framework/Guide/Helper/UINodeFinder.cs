using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class UINodeFinder
	{

		private string m_TargetPanel;
		private string m_TargetUINode;

		public override string ToString ()
		{
			return string.Format("Panel:{0},UI:{1}", m_TargetPanel, m_TargetUINode);
		}

		public void SetParam(string[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("UINodeFinder Init With Invalid Param.");
				return;
			}

			try
			{
				m_TargetPanel = pv[0];
				m_TargetUINode = pv[1];
			}
			catch(Exception e)
			{
				Log.e (e);
			}

		}

		public Transform FindNode()
		{
			return GuideHelper.FindTransformInPanel(m_TargetPanel, m_TargetUINode);
		}


	}
}

