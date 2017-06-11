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
using UnityEngine.UI;

namespace PTGame.Framework
{
	public class GuideHandPanel : AbstractPanel
	{
		[SerializeField]
		private Transform m_HandImage;
		[SerializeField]
		protected float m_FlySpeed = 500;
		[SerializeField]
		protected Vector3 m_OffsetPos;

		protected Vector3 m_OldPos = Vector3.zero;

		private Transform m_Target;

		protected override void OnPanelOpen (params object[] args)
		{
			if (args.Length == 0)
			{
				return;
			}

			m_Target = args [0] as Transform;

			if (args.Length > 1)
			{
				m_OffsetPos = (Vector3)args[1];
			}

			Update();
		}

		private void Update()
		{
			if (m_Target == null)
			{
				return;
			}

			Vector3 pos = GetHandPos();
			if (pos.x != m_OldPos.x || pos.y != m_OldPos.y)
			{
				m_HandImage.position = pos;

				var localpos = m_HandImage.localPosition;
				localpos = localpos + m_OffsetPos;
				m_HandImage.localPosition = localpos;

				m_OldPos = pos;
			}
		}

		private Vector3 GetHandPos()
		{
			return m_Target.position;
		}

	}
}

