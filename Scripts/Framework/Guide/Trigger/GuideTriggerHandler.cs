//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
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

            if (CheckNeedJump())
            {
                return false;
            }

			if (m_Trigger == null)
			{
				m_Trigger = GetTriggerArray();
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

        protected virtual bool CheckNeedJump()
        {
            return false;
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

			object[] commonParams = null;

			if (common != null)
			{
				string[] comParamString = common.Split(';');
				if (comParamString.Length > 0)
				{
					commonParams = new object[comParamString.Length];
					for (int i = 0; i < comParamString.Length; ++i)
					{
						if (comParamString[i].Contains(":"))
						{
							string[] dynaParams = comParamString [i].Split(':');
							IRuntimeParam runtimeParam = RuntimeParamFactory.S.Create(dynaParams[0]);
							if (runtimeParam == null)
							{
								Log.e ("Create Runtime Param Failed:" + dynaParams[0]);
							}
							else
							{
								if (dynaParams.Length > 1)
								{
									//string[] findParams = dynaParams[1].Split(',');
                                    object[] resultArray = GuideConfigParamProcess.ProcessParam(dynaParams[1], commonParams);
									runtimeParam.SetParam(resultArray);
								}	
							}

							commonParams[i] = runtimeParam;
						}
						else
						{
							commonParams[i] = comParamString[i];
						}
					}
				}
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
					Log.e ("Create Trigger Failed:" + com[0]);
					continue;
				}

				if (com.Length > 1)
				{
					object[] resultArray = GuideConfigParamProcess.ProcessParam(com[1], commonParams);
					
					//处理参数
					trigger.SetParam(resultArray);
				}

				if (result == null)
				{
					result = new List<ITrigger> ();
				}

				result.Add(trigger);
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

