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

namespace SCEngine
{
    [TMonoSingletonAttribute("[Game]/GuideMgr")]
    public class GuideMgr : TMonoSingleton<GuideMgr>
    {
        private List<Guide> m_TrackingGuideList = new List<Guide>();
		private List<TDGuide> m_UnTrackingGuide = new List<TDGuide>();

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
			int stepId = DataRecord.S.GetInt(GetLastKeyStepKey (guideID));
			if (stepId > 0)
			{
				return stepId;
			}

			var data = TDGuideStepTable.GetGuideFirstStep(guideID);

			if (data == null)
			{
				return -1;
			}

			return data.id - 1;
		}

        public void StartGuideTrack()
        {
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
						AddTrackingGuide(new Guide(data.id));
					}
					else
					{
						m_UnTrackingGuide.Add(data);
					}
				}
				else
				{
					AddTrackingGuide(new Guide(data.id));
				}
			}

            bool needRecheck = false;

			for (int i = m_TrackingGuideList.Count - 1; i >= 0; --i)
			{
				if (!m_TrackingGuideList[i].StartTrack())
				{
                    SaveFinishGuideID(m_TrackingGuideList[i].guideID);
                    m_TrackingGuideList.RemoveAt(i);
                    needRecheck = true;
				}
			}

            if (needRecheck)
            {
                ReCheckUnTrackGuide();
            }
        }

        public override void OnSingletonInit()
        {
			InitGuideCommandFactory();
			InitGuideTriggerFactory();
			InitRuntimeParamFactory();
        }

        public void FinishStep(GuideStep step)
        {
			int oldKeyStep = DataRecord.S.GetInt(GetLastKeyStepKey (step.guide.guideID));

			if (oldKeyStep >= step.stepID)
			{
				return;
			}

			//TODO:需要找到最近的关键帧并保存
			var data = TDGuideStepTable.GetData(step.stepID);

			if (data != null)
			{
				if (data.keyFrame)
				{
					DataRecord.S.SetInt(GetLastKeyStepKey(step.guide.guideID), step.stepID);
					DataRecord.S.Save();
				}
				else
				{
					//纪录最近的keyframe
					var allStep = TDGuideStepTable.GetDataAsGuideID(step.guide.guideID);
					for (int i = allStep.Count - 1; i >= 0; --i)
					{
						if (!allStep[i].keyFrame)
						{
							continue;
						}

						if (allStep[i].id <= oldKeyStep)
						{
							break;
						}

						if (allStep[i].id <= data.id)
						{
							DataRecord.S.SetInt(GetLastKeyStepKey(step.guide.guideID), allStep[i].id);
							DataRecord.S.Save();
							break;
						}

					}
				}
			}
        }

		public void FinishGuide(Guide guide)
		{
			m_TrackingGuideList.Remove(guide);

            SaveFinishGuideID(guide.guideID);

			int finishGuideId = guide.guideID;

            bool needRecheck = false;
			if (m_UnTrackingGuide.Count > 0)
			{
				for (int i = m_UnTrackingGuide.Count - 1; i >= 0; --i)
				{
					if (m_UnTrackingGuide[i].requireGuideId == finishGuideId)
					{
						Guide nextGuide = new Guide(m_UnTrackingGuide[i].id);
						m_UnTrackingGuide.RemoveAt(i);
						if (nextGuide.StartTrack())
						{
							m_TrackingGuideList.Add(nextGuide);
						}
                        else
                        {
                            SaveFinishGuideID(nextGuide.guideID);
                            needRecheck = true;
                        }
					}
				}
			}

            if (needRecheck)
            {
                ReCheckUnTrackGuide();
            }
		}

        private void ReCheckUnTrackGuide()
        {
            if (m_UnTrackingGuide.Count > 0)
            {
                bool needCheck = false;
                for (int i = m_UnTrackingGuide.Count - 1; i >= 0; --i)
                {
                    if (m_UnTrackingGuide[i].requireGuideId > 0)
                    {
                        if (IsGuideFinish(m_UnTrackingGuide[i].requireGuideId))
                        {
                            Guide guide = new Guide(m_UnTrackingGuide[i].id);
                            m_UnTrackingGuide.RemoveAt(i);
                            AddTrackingGuide(guide);

                            if (!guide.StartTrack())
                            {
                                SaveFinishGuideID(guide.guideID);
                                m_TrackingGuideList.Remove(guide);
                                needCheck = true;
                            }
                        }
                    }
                }

                if (needCheck)
                {
                    ReCheckUnTrackGuide();
                }
            }
        }

        private void SaveFinishGuideID(int guideID)
        {
            DataRecord.S.SetBool(GetGuideKey(guideID), true);
            DataRecord.S.Save();
        }

		private void InitGuideCommandFactory()
		{
			RegisterGuideCommand(typeof(ButtonHackCommand));
			RegisterGuideCommand(typeof(HighlightUICommand));
			RegisterGuideCommand(typeof(GuideHandCommand));
			RegisterGuideCommand(typeof(PlayAudioCommand));
            RegisterGuideCommand(typeof(EventPauseCommand));
            RegisterGuideCommand(typeof(MonoFuncCall));
		}

		private void InitGuideTriggerFactory()
		{
			RegisterGuideTrigger(typeof(TopPanelTrigger));
			RegisterGuideTrigger(typeof(UINodeVisibleTrigger));
		}

		private void InitRuntimeParamFactory()
		{
			RegisterRuntimeParam(typeof(UINodeFinder));
			RegisterRuntimeParam(typeof(MonoFuncCall));
		}

		public void RegisterRuntimeParam(Type type)
		{
			Type[] ty = new Type[0];
			var constructor = type.GetConstructor(ty);

			if (constructor == null)
			{
				return;
			}

			RuntimeParamFactory.S.RegisterCreator(type.Name, () => {
				return constructor.Invoke(null) as IRuntimeParam;
			});
		}

		public void RegisterGuideCommand(Type type)
		{
			Type[] ty = new Type[0];
			var constructor = type.GetConstructor(ty);

			if (constructor == null)
			{
				return;
			}

			GuideCommandFactory.S.RegisterCreator(type.Name, () => {
				return constructor.Invoke(null) as AbstractGuideCommand;
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

			GuideTriggerFactory.S.RegisterCreator(type.Name, () => {
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
