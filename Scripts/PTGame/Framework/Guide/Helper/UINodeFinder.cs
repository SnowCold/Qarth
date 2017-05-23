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

		public void SetParam(string targetPanel, string targetNode)
		{
			m_TargetPanel = targetPanel;
			m_TargetUINode = targetNode;
		}

		public void SetParam(string param)
		{

			string[] pv = param.Split (',');
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

