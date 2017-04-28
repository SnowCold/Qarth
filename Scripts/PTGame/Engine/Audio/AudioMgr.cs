using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/AudioMgr")]
    public class AudioMgr : TMonoSingleton<AudioMgr>
    {
        protected int m_MaxSoundCount = 5;
        protected AudioUnit m_MainUnit;

        public override void OnSingletonInit()
        {
            ObjectPool<AudioUnit>.S.Init(m_MaxSoundCount, 1);
            m_MainUnit = new AudioUnit();
            m_MainUnit.usedCache = false;
        }

        public AudioUnit PlayBg(string name, bool loop = true, Action<AudioUnit> callBack = null, int customEventID = -1)
        {
            m_MainUnit.SetAudio(gameObject, name, loop);
            m_MainUnit.SetOnFinishListener(callBack);
            m_MainUnit.customEventID = customEventID;
            return m_MainUnit;
        }

        public AudioUnit PlaySound(string name, bool loop = false, Action<AudioUnit> callBack = null, int customEventID = -1)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            AudioUnit unit = ObjectPool<AudioUnit>.S.Allocate();

            unit.SetAudio(gameObject, name, loop);
            unit.SetOnFinishListener(callBack);
            unit.customEventID = customEventID;
            return unit;
        }
    }
}
