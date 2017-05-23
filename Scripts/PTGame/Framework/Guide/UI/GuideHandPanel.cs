using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
	public class GuideHandPanel : AbstractPanel
	{
		[SerializeField]
		private Transform m_HandImage;
		[SerializeField]
		protected float m_FlySpeed = 500;

		private Transform m_Target;

		protected override void OnPanelOpen (params object[] args)
		{
			if (args.Length == 0)
			{
				return;
			}

			m_Target = args [0] as Transform;
		}

		private void Update()
		{
			if (m_Target == null)
			{
				return;
			}

			Vector3 pos = GetHandPos();

			m_HandImage.position = pos;
		}

		private Vector3 GetHandPos()
		{
			return m_Target.position;
		}

	}
}

