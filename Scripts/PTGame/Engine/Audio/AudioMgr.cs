using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/AudioMgr")]
    public class AudioMgr : TMonoSingleton<AudioMgr>
    {
        public class AudioUnit : ICacheAble
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

                if (m_Loader != null)
                {
                    CleanResources();
                }

                m_Loader = ResLoader.Allocate();

                m_IsLoop = loop;
                m_Name = name;

                m_Loader.Add2Load(name, OnResLoadFinish);

                if (m_Loader != null)
                {
                    m_Loader.LoadAsync();
                }
            }

            public void Stop()
            {
                Release();
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
                    ObjectPool<AudioUnit>.S.Recycle(this);
                }
            }

            private void CleanResources()
            {
                m_Name = null;

                m_OnFinishListener = null;

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
        }

        protected int m_MaxSoundCount = 5;
        protected AudioUnit m_MainUnit;

        public override void OnSingletonInit()
        {
            ObjectPool<AudioUnit>.S.Init(m_MaxSoundCount, 1);
            m_MainUnit = new AudioUnit();
            m_MainUnit.usedCache = false;
        }

        public AudioUnit PlayBg(string name)
        {
            m_MainUnit.SetAudio(gameObject, name, true);
            return m_MainUnit;
        }

        public AudioUnit PlaySound(string name, bool loop)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            AudioUnit unit = ObjectPool<AudioUnit>.S.Allocate();

            unit.SetAudio(gameObject, name, loop);

            return unit;
        }
    }
}
