using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public abstract class TMonoSingleton<T> : MonoBehaviour, ISingleton where T : TMonoSingleton<T>
    {
        private static T        m_Instance = null;
        private static bool     m_IsApplicationQuit = false;
        private static object   s_lock = new object();

        public static bool isApplicationQuit
        {
            get { return m_IsApplicationQuit; }
        }

        public static T S
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (s_lock)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = CreateMonoSingleton<T>();
                        }
                    }
                }

                return m_Instance;
            }
        }

        public virtual void OnSingletonInit() { }

        public static K CreateMonoSingleton<K>() where K : MonoBehaviour, ISingleton
        {
            if (m_IsApplicationQuit)
            {
                return null;
            }

            K instance = null;

            if (instance == null && !m_IsApplicationQuit)
            {
                instance = GameObject.FindObjectOfType(typeof(K)) as K;
                if (instance == null)
                {
                    System.Reflection.MemberInfo info = typeof(K);
                    object[] attributes = info.GetCustomAttributes(true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        TMonoSingletonAttribute defineAttri = attributes[i] as TMonoSingletonAttribute;
                        if (defineAttri == null)
                        {
                            continue;
                        }
                        instance = CreateComponentOnGameObject<K>(defineAttri.AbsolutePath, true);
                        break;
                    }

                    if (instance == null)
                    {
                        GameObject obj = new GameObject("Singleton of " + typeof(K).Name);
                        UnityEngine.Object.DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<K>();
                    }

                    instance.OnSingletonInit();
                }
            }

            return instance;
        }

        protected static K CreateComponentOnGameObject<K>(string path, bool dontDestroy) where K : MonoBehaviour
        {
            GameObject obj = GameObjectHelper.FindGameObject(null, path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton of " + typeof(K).Name);
                if (dontDestroy)
                {
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                }
            }

            return obj.AddComponent<K>();
        }

        protected void OnApplicationQuit()
        {
            m_IsApplicationQuit = true;
        }

    }
}
