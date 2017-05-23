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
		protected List<GuideCommand> m_Commands;
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
			if (!m_IsActive)
			{
				return;
			}

			m_IsActive = false;

			StopTrack ();

			for (int i = 0; i < m_Commands.Count; ++i)
			{
				m_Commands[i].OnFinish();
			}
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

			m_IsActive = true;

			for (int i = 0; i < m_Commands.Count; ++i)
			{
				m_Commands[i].guideStep = this;
				m_Commands[i].Start();
			}

			return true;
		}

		//command format:[c1:p1,p2,p3;c2:p1,p2,p3]
		private List<GuideCommand> LoadCommands()
		{
			TDGuideStep data = TDGuideStepTable.GetData (m_GuideStepID);
			if (data == null)
			{
				return null;
			}

			string[] msg = data.command1.Split (';');
			if (msg == null || msg.Length == 0)
			{
				return null;
			}

			List<GuideCommand> result = null;

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

				GuideCommand command = GuideCommandFactory.S.Create(com[0]);
				if (command == null)
				{
					Log.e ("Create Command Failed:" + com [0]);
					continue;
				}

				if (com.Length > 1)
				{
					command.SetParam(com[1]);
				}

				if (result == null)
				{
					result = new List<GuideCommand> ();
				}

				result.Add (command);
			}

			return result;
		}

		protected override List<ITrigger> GetTriggerArray ()
		{
			TDGuideStep data = TDGuideStepTable.GetData (m_GuideStepID);
			if (data == null)
			{
				return null;
			}

			return LoadTrigger (data.trigger1);
		}
    }
}
