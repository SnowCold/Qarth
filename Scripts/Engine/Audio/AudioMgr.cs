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
    [TMonoSingletonAttribute("[Tools]/AudioMgr")]
    public class AudioMgr : TMonoSingleton<AudioMgr>
    {
        protected int m_MaxSoundCount = 5;
        protected AudioUnit m_MainUnit;
        protected Dictionary<string, AudioUnit> m_SingletonSoundMap = new Dictionary<string, AudioUnit>();

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

        public AudioUnit PlaySoundSingleton(string name, bool replace)
        {
            if (m_SingletonSoundMap.ContainsKey(name))
            {
                if (replace)
                {
                    m_SingletonSoundMap[name].Stop();
                    m_SingletonSoundMap.Remove(name);
                }
                else
                {
                    return null;
                }
            }

            AudioUnit unit = PlaySound(name, false, OnSingleAudioFinish);
            m_SingletonSoundMap.Add(name, unit);
            return unit;
        }
        
        private void OnSingleAudioFinish(AudioUnit unit)
        {
            if (m_SingletonSoundMap.ContainsKey(unit.audioName))
            {
                m_SingletonSoundMap.Remove(unit.audioName);
            }
        }

		public AudioUnit GetBGUnit()
		{
			return m_MainUnit;
		}
    }
}
