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
    public partial class AudioMgr : TMonoSingleton<AudioMgr>
    {
        protected int m_MaxSoundCount = 5;
        protected AudioUnit m_MainUnit;
        protected Dictionary<string, AudioUnit> m_SingletonSoundMap = new Dictionary<string, AudioUnit>();

        public override void OnSingletonInit()
        {
            ObjectPool<AudioUnit>.S.Init(m_MaxSoundCount, 1);
            m_MainUnit = AudioUnit.Allocate();
            m_MainUnit.usedCache = false;
        }

        public int PlayBg(string name, bool loop = true, Action<int> callBack = null, int customEventID = -1)
        {
            m_MainUnit.SetAudio(gameObject, name, loop);
            m_MainUnit.SetOnFinishListener(callBack);
            m_MainUnit.customEventID = customEventID;
            return m_MainUnit.id;
        }

        public int PlaySound(string name, bool loop = false, Action<int> callBack = null, int customEventID = -1)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            AudioUnit unit = AudioUnit.Allocate();

            unit.SetAudio(gameObject, name, loop);
            unit.SetOnFinishListener(callBack);
            unit.customEventID = customEventID;
            return unit.id;
        }

        public int PlaySoundSingleton(string name, bool replace)
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
                    return -1;
                }
            }

            int id = PlaySound(name, false);

            if (id < 0)
            {
                return id;
            }

            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            unit.SetOnStopListener(OnSingleAudioFinish);
            m_SingletonSoundMap.Add(name, unit);
            return unit.id;
        }

        public bool Resume(int id)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                return false;
            }

            unit.Resume();
            return true;
        }

        public bool Pause(int id)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                return false;
            }

            unit.Pause();
            return true;
        }

        public bool Stop(int id)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                return false;
            }

            unit.Stop();
            return true;
        }

        public bool SetVolume(int id, float volume)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                return false;
            }

            unit.SetVolume(volume);
            return true;
        }

        public bool SetOnFinishListener(int id, Action<int> l)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                return false;
            }

            unit.SetOnFinishListener(l);
            return true;
        }

        private void OnSingleAudioFinish(int id)
        {
            AudioUnit unit = AudioUnit.GetAudioUnitByID(id);
            if (unit == null)
            {
                Log.e("WTF! Not Impossible.");
                return;
            }

            if (m_SingletonSoundMap.ContainsKey(unit.audioName))
            {
                m_SingletonSoundMap.Remove(unit.audioName);
            }
        }

        public int GetBGID()
        {
            return m_MainUnit.id;
        }
    }
}
