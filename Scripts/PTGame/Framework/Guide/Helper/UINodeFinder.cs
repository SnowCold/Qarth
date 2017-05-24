using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class UINodeFinder : IUINodeFinder
	{

		private string m_TargetPanel;
		private string m_TargetUINode;

		private Transform m_Result;

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

		public Transform FindNode(bool search)
		{
			//if (search)
			{
				m_Result = GuideHelper.FindTransformInPanel(m_TargetPanel, m_TargetUINode);
			}

			return m_Result;
		}


	}
}

