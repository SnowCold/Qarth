using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class GuideTriggerHandler
	{
		protected List<ITrigger> m_Trigger;
		private bool m_IsTracking = false;

		public bool StartTrack()
		{
			if (m_IsTracking)
			{
				CheckAllTriggerState ();
				return true;
			}

			if (m_Trigger == null)
			{
				m_Trigger = GetTriggerArray ();
			}

			if (m_Trigger == null)
			{
				return false;
			}

			m_IsTracking = true;

			for (int i = 0; i < m_Trigger.Count; ++i)
			{
				m_Trigger[i].Start(OnTriggerEvent);
			}

			CheckAllTriggerState ();
			return true;
		}

		public void StopTrack()
		{
			if (!m_IsTracking)
			{
				return;
			}

			m_IsTracking = false;

			if (m_Trigger == null)
			{
				return;
			}

			for (int i = 0; i < m_Trigger.Count; ++i)
			{
				m_Trigger[i].Stop();
			}
		}

		private void OnTriggerEvent(bool result, ITrigger trigger)
		{
			CheckAllTriggerState ();
		}

		private void CheckAllTriggerState()
		{
			if (m_Trigger == null || m_Trigger.Count == 0)
			{
				return;
			}

			for (int i = 0; i < m_Trigger.Count; ++i)
			{
				if (!m_Trigger[i].isReady)
				{
					OnAllTriggerEvent (false);
					return;
				}
			}

			OnAllTriggerEvent (true);
		}

		//[name:p1,p2;name:p1,p2]
		protected List<ITrigger> LoadTrigger(string context, string common)
		{
			if (string.IsNullOrEmpty(context))
			{
				return null;
			}

			string[] msg = context.Split (';');
			if (msg == null || msg.Length == 0)
			{
				return null;
			}

			string[] commonParams = null;

			if (common != null)
			{
				commonParams = common.Split (',');
			}

			List<ITrigger> result = null;

			for (int i = 0; i < msg.Length; ++i)
			{
				if (string.IsNullOrEmpty(msg[i]))
				{
					continue;
				}
				string[] com = msg[i].Split (':');
				if (com == null || com.Length == 0)
				{
					continue;
				}

				ITrigger trigger = GuideTriggerFactory.S.Create(com[0]);
				if (trigger == null)
				{
					Log.e ("Create Trigger Failed:" + com [0]);
					continue;
				}

				if (com.Length > 1)
				{

					string[] paramsArray = com[1].Split (',');
					if (commonParams != null)
					{
						if (paramsArray.Length > 0)
						{
							for(int p = 0; p < paramsArray.Length; ++p)
							{
								if (paramsArray[p].StartsWith("#"))
								{
									int index = int.Parse (paramsArray[p].Substring(1));
									if (index < commonParams.Length)
									{
										paramsArray[p] = commonParams [index];
									}
									else
									{
										Log.w("Invalid Param For Trigger:" + com[0]);
									}
								}
							}
						}	
					}
					//处理参数
					trigger.SetParam(paramsArray);
				}

				if (result == null)
				{
					result = new List<ITrigger> ();
				}

				result.Add (trigger);
			}

			return result;
		}

		protected virtual void OnAllTriggerEvent(bool ready)
		{
			

		}

		protected virtual List<ITrigger> GetTriggerArray()
		{
			return null;
		}
	}
}

