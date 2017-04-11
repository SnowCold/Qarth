using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[Tools]/AudioMgr")]
    public class AudioMgr : TMonoSingleton<AudioMgr>
    {
        class AudioUnit
        {
            private ResLoader m_Loader;
            private AudioSource m_Source;
            private string m_Name;

            private int m_LoopCount;
            private AudioClip m_AudioClip;

            public void SetAudio(AudioSource source, string name, int loopCount = 1)
            {
                if (m_Name == name)
                {
                    return;
                }

                if (m_Loader != null)
                {
                    Release();
                }

                m_Loader = ResLoader.Allocate();

                m_LoopCount = loopCount;
                m_Name = name;

                m_Loader.Add2Load(name, OnResLoadFinish);

                m_Loader.LoadAsync();
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
                    Release();
                    Log.e("Asset Is Invalid AudioClip:" + m_Name);
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

                m_Source.Play();
            }

            private void Release()
            {
                if (m_Source != null)
                {
                    if (m_Source.clip == m_AudioClip)
                    {
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
        }
        
        private AudioSource m_MainSource;
        private AudioUnit m_MainUnit;

        public override void OnSingletonInit()
        {
            m_MainSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayBg(string name)
        {
            if (m_MainUnit == null)
            {
                m_MainUnit = new AudioUnit();
            }

            m_MainUnit.SetAudio(m_MainSource, name, -1);
        }
    }
}
