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

namespace Qarth
{
    public class AudioUnit : ICacheAble, ICacheType
    {
        private ResLoader m_Loader;
        private AudioSource m_Source;
        private string m_Name;

        private bool m_IsLoop;
        private AudioClip m_AudioClip;
        private TimeItem m_TimeItem;
        private bool m_UsedCache = true;
        private bool m_IsCache = false;

        private Action<AudioUnit> m_OnFinishListener;
        private bool m_IsPause = false;
        private float m_LeftDelayTime = -1;
        private int m_PlayCount = 0;
        private int m_CustomEventID;

        public int customEventID
        {
            get { return m_CustomEventID; }
            set { m_CustomEventID = -1; }
        }

        public string audioName
        {
            get { return m_Name; }
        }

        public AudioUnit Allocate()
        {
            return ObjectPool<AudioUnit>.S.Allocate();
        }

        public void SetOnFinishListener(Action<AudioUnit> l)
        {
            m_OnFinishListener = l;
        }

        public bool usedCache
        {
            get { return m_UsedCache; }
            set { m_UsedCache = false; }
        }

        public int playCount
        {
            get { return m_PlayCount; }
        }

        public bool cacheFlag
        {
            get
            {
                return m_IsCache;
            }

            set
            {
                m_IsCache = value;
            }
        }

        public void SetAudio(GameObject root, string name, bool loop)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (m_Name == name)
            {
                return;
            }

            if (m_Source == null)
            {
                m_Source = root.AddComponent<AudioSource>();
            }

            //防止卸载后立马加载的情况
            ResLoader preLoader = m_Loader;
            m_Loader = null;
            CleanResources();

            m_Loader = ResLoader.Allocate("AudioUnit");

            m_IsLoop = loop;
            m_Name = name;

            m_Loader.Add2Load(name, OnResLoadFinish);

            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (m_Loader != null)
            {
                m_Loader.LoadAsync();
            }
        }

        public void Stop()
        {
            Release();
        }

        public void Pause()
        {
            if (m_IsPause)
            {
                return;
            }

            m_LeftDelayTime = -1;
            //暂停
            if(m_TimeItem != null)
            {
                m_LeftDelayTime = m_TimeItem.sortScore - Timer.S.currentScaleTime;
                m_TimeItem.Cancel();
                m_TimeItem = null;
            }

            m_IsPause = true;

            m_Source.Pause();
        }

        public void Resume()
        {
            if (!m_IsPause)
            {
                return;
            }

            if (m_LeftDelayTime >= 0)
            {
                m_TimeItem = Timer.S.Post2Scale(OnResumeTimeTick, m_LeftDelayTime);
            }

            m_IsPause = false;

            m_Source.Play();
        }

		public void SetVolume(float volume)
		{
			m_Source.volume = volume;
		}

        private void OnResLoadFinish(bool result, IRes res)
        {
            if (!result)
            {
                Release();
                return;
            }

            m_AudioClip = res.asset as AudioClip;

            if (m_AudioClip == null)
            {
                Log.e("Asset Is Invalid AudioClip:" + m_Name);
                Release();
                return;
            }

            PlayAudioClip();
        }

        private void PlayAudioClip()
        {
            if (m_Source == null || m_AudioClip == null)
            {
                Release();
                return;
            }

            m_Source.clip = m_AudioClip;
            m_Source.loop = m_IsLoop;
			m_Source.volume = 1.0f;

            int loopCount = 1;
            if (m_IsLoop)
            {
                loopCount = -1;
            }

            m_TimeItem = Timer.S.Post2Scale(OnSoundPlayFinish, m_AudioClip.length, loopCount);

            m_Source.Play();
        }

        private void OnResumeTimeTick(int repeatCount)
        {
            OnSoundPlayFinish(repeatCount);

            if (m_IsLoop)
            {
                m_TimeItem = Timer.S.Post2Scale(OnSoundPlayFinish, m_AudioClip.length, -1);
            }
        }

        private void OnSoundPlayFinish(int count)
        {
            ++m_PlayCount;

            if (m_OnFinishListener != null)
            {
                m_OnFinishListener(this);
            }

            if (m_CustomEventID > 0)
            {
                EventSystem.S.Send(m_CustomEventID, this);
            }

            if (!m_IsLoop)
            {
                Release();
            }
        }

        private void Release()
        {
            CleanResources();

            if (m_UsedCache)
            {
                Recycle2Cache();
            }
        }

        private void CleanResources()
        {
            m_Name = null;

            m_PlayCount = 0;
            m_IsPause = false;
            m_OnFinishListener = null;
            m_LeftDelayTime = -1;
            m_CustomEventID = -1;

            if (m_TimeItem != null)
            {
                m_TimeItem.Cancel();
                m_TimeItem = null;
            }

            if (m_Source != null)
            {
                if (m_Source.clip == m_AudioClip)
                {
                    m_Source.Stop();
                    m_Source.clip = null;
                }
            }

            m_AudioClip = null;

            if (m_Loader != null)
            {
                m_Loader.Recycle2Cache();
                m_Loader = null;
            }
        }

        public void OnCacheReset()
        {
            CleanResources();
        }

        public void Recycle2Cache()
        {
            if (!ObjectPool<AudioUnit>.S.Recycle(this))
            {
                if (m_Source != null)
                {
                    GameObject.Destroy(m_Source);
                    m_Source = null;
                }
            }
        }
    }

}
