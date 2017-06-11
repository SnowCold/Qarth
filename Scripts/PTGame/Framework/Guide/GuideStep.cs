//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class GuideStep : GuideTriggerHandler
    {
        private int m_GuideStepID;

        private Guide m_Guide;
		protected List<AbstractGuideCommand> m_Commands;
		protected bool m_IsActive;

        public Guide guide
        {
            get { return m_Guide; }
            set { m_Guide = value; }
        }

		public int stepID
		{
			get { return m_GuideStepID; }
		}

        public GuideStep(int stepID)
        {
            m_GuideStepID = stepID;
        }
			
		public void OnCommandFinish()
		{
			m_Guide.OnStepFinish(this);
		}
			
        public void Finish()
        {
            StopTrack();

            if (!m_IsActive)
			{
				return;
			}

			m_IsActive = false;

			for (int i = 0; i < m_Commands.Count; ++i)
			{
				m_Commands[i].Finish(false);
			}

            Log.i("#GuideStep Finish:" + m_GuideStepID);
        }

		protected override void OnAllTriggerEvent (bool ready)
		{
			if (ready)
			{
				if (!m_IsActive)
				{
					var data = TDGuideStepTable.GetData (m_GuideStepID);

					if (data.requireStepId > 0)
					{
						if (m_Guide.IsStepFinish(data.requireStepId))
						{
							ActiveSelf();	
						}
					}
					else
					{
						ActiveSelf();
					}
				}
			}
			else
			{
				if (!m_IsActive)
				{
					return;
				}

				m_IsActive = false;
				//Log.e ("Force Finish Step:" + m_GuideStepID);
				if (m_Commands != null)
				{
					for (int i = 0; i < m_Commands.Count; ++i)
					{
						m_Commands[i].Finish(true);
					}	
				}
			}
		}

		private bool ActiveSelf()
		{
			if (m_IsActive)
			{
				return true;
			}

			if (m_Commands == null)
			{
				m_Commands = LoadCommands ();
			}

			if (m_Commands == null)
			{
				return false;
			}

            Log.i("#GuideStep Start:" + m_GuideStepID);

			m_IsActive = true;

			for (int i = 0; i < m_Commands.Count; ++i)
			{
				m_Commands[i].guideStep = this;
				m_Commands[i].Start();
			}

			return true;
		}

		//command format:[c1:p1,p2,p3;c2:p1,p2,p3]
		private List<AbstractGuideCommand> LoadCommands()
		{
			TDGuideStep data = TDGuideStepTable.GetData (m_GuideStepID);
			if (data == null)
			{
				return null;
			}

			string[] msg = data.command.Split (';');
			if (msg == null || msg.Length == 0)
			{
				return null;
			}
				
			object[] commonParams = null;

			if (data.commonParam != null)
			{
				string[] comParamString = data.commonParam.Split(';');
				if (comParamString.Length > 0)
				{
					commonParams = new object[comParamString.Length];
					for (int i = 0; i < comParamString.Length; ++i)
					{
						if (comParamString[i].Contains(":"))
						{
							string[] dynaParams = comParamString [i].Split (':');
							IRuntimeParam runtimeParam = RuntimeParamFactory.S.Create(dynaParams[0]);
							if (runtimeParam == null)
							{
								Log.e ("Create RuntimeParam Failed:" + dynaParams[0]);
							}
							else
							{
								if (dynaParams.Length > 1)
								{
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

			List<AbstractGuideCommand> result = null;

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

				AbstractGuideCommand command = GuideCommandFactory.S.Create(com[0]);
				if (command == null)
				{
					Log.e ("Create Command Failed:" + com [0]);
					continue;
				}

				if (com.Length > 1)
				{
                    object[] resultArray = GuideConfigParamProcess.ProcessParam(com[1], commonParams);
					//处理参数
					command.SetParam(resultArray);
				}

				if (result == null)
				{
					result = new List<AbstractGuideCommand> ();
				}

				result.Add(command);
			}

			return result;
		}

        protected override List<ITrigger> GetTriggerArray ()
		{
			TDGuideStep data = TDGuideStepTable.GetData(m_GuideStepID);
			if (data == null)
			{
				return null;
			}

			return LoadTrigger(data.trigger, data.commonParam);
		}
    }
}
