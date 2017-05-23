using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Game]/GuideMgr")]
    public class GuideMgr : TMonoSingleton<GuideMgr>
    {
        private List<Guide> m_TrackingGuideList = new List<Guide>();
		private List<TDGuide> m_UnTrackingGuide = new List<TDGuide> ();
		private Guide m_CurrentGuide;

		public static string GetGuideKey(int guideID)
		{
			return string.Format("guide_{0}", guideID);
		}

		public static string GetLastKeyStepKey(int guideID)
		{
			return string.Format ("guidekey_{0}", guideID);
		}

		public bool IsGuideFinish(int guideID)
		{
			return DataRecord.S.GetBool(GetGuideKey(guideID));
		}

		public int GetGuideLastStep(int guideID)
		{
			int stepId = DataRecord.S.GetInt (GetLastKeyStepKey (guideID));
			if (stepId > 0)
			{
				return stepId;
			}

			var data = TDGuideStepTable.GetGuideFirstStep (guideID);

			if (data == null)
			{
				return -1;
			}

			return data.id - 1;
		}

        public void InitGuideMgr()
        {
			DataRecord.S.Reset ();
            Log.i("Init[Guide Mgr]");

			var dataList = TDGuideTable.dataList;

			for (int i = 0; i < dataList.Count; ++i)
			{
				TDGuide data = dataList [i];
				if (IsGuideFinish(data.id))
				{
					continue;
				}

				if (data.requireGuideId > 0)
				{
					if (IsGuideFinish(data.requireGuideId))
					{
						AddTrackingGuide (new Guide (data.id));
					}
					else
					{
						m_UnTrackingGuide.Add (data);
					}
				}
				else
				{
					AddTrackingGuide (new Guide (data.id));
				}
			}

			for (int i = m_TrackingGuideList.Count - 1; i >= 0; --i)
			{
				if (!m_TrackingGuideList[i].StartTrack())
				{
					m_TrackingGuideList.RemoveAt(i);
				}
			}
        }

        public override void OnSingletonInit()
        {
			InitGuideCommandFactory ();
			InitGuideTriggerFactory ();
        }

		public void TryActiveGuide(Guide guide)
		{
			if (m_CurrentGuide != null)
			{
				return;
			}

			if (guide.Active())
			{
				m_CurrentGuide = guide;
			}
		}

        public void FinishStep(GuideStep step)
        {
			int oldKeyStep = DataRecord.S.GetInt (GetLastKeyStepKey (step.guide.guideID));

			if (oldKeyStep >= step.stepID)
			{
				return;
			}

			var data = TDGuideStepTable.GetData (step.stepID);

			if (data != null)
			{
				if (data.keyFrame)
				{
					DataRecord.S.SetInt (GetLastKeyStepKey (step.guide.guideID), step.stepID);
					DataRecord.S.Save ();
				}
			}
        }

		public void FinishGuide(Guide guide)
		{
			if (m_CurrentGuide == guide)
			{
				m_CurrentGuide = null;
			}

			m_TrackingGuideList.Remove(guide);

			DataRecord.S.SetBool(GetGuideKey (guide.guideID), true);
			DataRecord.S.Save();

			int finishGuideId = guide.guideID;

			if (m_UnTrackingGuide.Count > 0)
			{
				for (int i = m_UnTrackingGuide.Count - 1; i >= 0; --i)
				{
					if (m_UnTrackingGuide[i].requireGuideId == finishGuideId)
					{
						Guide nextGuide = new Guide(m_UnTrackingGuide[i].id);
						m_UnTrackingGuide.RemoveAt (i);
						if (nextGuide.StartTrack())
						{
							m_TrackingGuideList.Add (nextGuide);
						}
					}
				}
			}
		}

		private void InitGuideCommandFactory()
		{
			RegisterGuideCommand(typeof(ButtonHackCommand));
			RegisterGuideCommand(typeof(HighlightUICommand));
			RegisterGuideCommand(typeof(GuideHandCommand));
			RegisterGuideCommand(typeof(PlayAudioCommand));
		}

		private void InitGuideTriggerFactory()
		{
			RegisterGuideTrigger(typeof(TopPanelTrigger));
			RegisterGuideTrigger(typeof(UINodeVisibleTrigger));
		}

		public void RegisterGuideCommand(Type type)
		{
			Type[] ty = new Type[0];
			var constructor = type.GetConstructor(ty);

			if (constructor == null)
			{
				return;
			}

			GuideCommandFactory.S.RegisterCreator (type.Name, () => {
				return constructor.Invoke(null) as GuideCommand;
			});
		}

		public void RegisterGuideTrigger(Type type)
		{
			Type[] ty = new Type[0];
			var constructor = type.GetConstructor(ty);
			if (constructor == null)
			{
				return;
			}

			GuideTriggerFactory.S.RegisterCreator (type.Name, () => {
				return constructor.Invoke(null) as ITrigger;
			});
		}

        private void AddTrackingGuide(Guide guide)
        {
            if (guide == null)
            {
                return;
            }

			m_TrackingGuideList.Add(guide);
        }
    }
}
