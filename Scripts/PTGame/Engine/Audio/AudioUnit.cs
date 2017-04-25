using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
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

        private Action m_OnFinishListener;
        private bool m_IsPause = false;
        private float m_LeftDelayTime = -1;

        public AudioUnit Allocate()
        {
            return ObjectPool<AudioUnit>.S.Allocate();
        }

        public void SetOnFinishListener(Action l)
        {
            m_OnFinishListener = l;
        }

        public bool usedCache
        {
            get { return m_UsedCache; }
            set { m_UsedCache = false; }
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

            m_Loader = ResLoader.Allocate();

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
                if (!m_IsLoop)
                {
                    m_TimeItem = Timer.S.Post2Scale(OnSoundPlayFinish, m_LeftDelayTime);
                }
            }

            m_IsPause = false;

            m_Source.Play();
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

            if (!m_IsLoop)
            {
                m_TimeItem = Timer.S.Post2Scale(OnSoundPlayFinish, m_AudioClip.length);
            }

            m_Source.Play();
        }

        private void OnSoundPlayFinish(int count)
        {
            if (m_OnFinishListener != null)
            {
                m_OnFinishListener();
            }
            Release();
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

            m_IsPause = false;
            m_OnFinishListener = null;
            m_LeftDelayTime = -1;

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
