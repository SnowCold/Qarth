using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class GuideTriggerHandler
	{
		protected ITrigger[] m_Trigger;
		private bool m_IsTracking = false;

		public void StartTrack()
		{
			if (m_IsTracking)
			{
				return;
			}

			if (m_Trigger == null)
			{
				m_Trigger = GetTriggerArray ();
			}

			if (m_Trigger == null)
			{
				return;
			}

			m_IsTracking = true;

			for (int i = 0; i < m_Trigger.Length; ++i)
			{
				m_Trigger[i].Start(OnTriggerEvent);
			}
		}

		private void OnTriggerEvent(bool result, ITrigger trigger)
		{
			CheckAllTriggerstate ();
		}

		protected virtual ITrigger[] GetTriggerArray()
		{
			return null;
		}
	}
}

