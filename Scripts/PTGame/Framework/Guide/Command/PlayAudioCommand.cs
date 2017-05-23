using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class PlayAudioCommand : AbstractGuideCommand
	{
		private string m_AudioName;
		private bool m_FinishStep = false;
		private AudioUnit m_AudioUnit;

		public override void SetParam (string[] pv)
		{
			if (pv.Length == 0)
			{
				Log.w ("PlayAudioCommand Init With Invalid Param.");
				return;
			}

			m_AudioName = pv[0];
			if (pv.Length > 1)
			{
				m_FinishStep = Helper.String2Bool(pv[1]);
			}
		}

		public override void Start()
		{
			m_AudioUnit = AudioMgr.S.PlaySound(m_AudioName, false, OnAoundPlayFinish);
		}

		private void OnAoundPlayFinish(AudioUnit unit)
		{
			m_AudioUnit = null;
			if (m_FinishStep)
			{
				FinishStep ();
			}
		}

		public override void OnFinish()
		{
			if (m_AudioUnit != null)
			{
				m_AudioUnit.Stop ();
				m_AudioUnit = null;
			}
		}

	}
}

