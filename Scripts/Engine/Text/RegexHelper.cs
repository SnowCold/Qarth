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
using System.Text.RegularExpressions;
using System.Text;

namespace Qarth
{
    public class RegexHelper
    {
        //private Regex m_Regex = new Regex("<(([a-z]|[A-Z]|[0-9])*)>");
        private Regex m_Regex = new Regex("<([^<]*)>");
        private StringBuilder m_Builder = new StringBuilder();
        public delegate string KeyProcess();
        private Dictionary<string, KeyProcess> m_KeyProcess;

        public void RegisterKeyProcess(string key, KeyProcess process)
        {
            if (m_KeyProcess == null)
            {
                m_KeyProcess = new Dictionary<string, KeyProcess>();
            }

            if (m_KeyProcess.ContainsKey(key))
            {
                Log.w("Already Register Key Process:" + key);
                return;
            }

            m_KeyProcess.Add(key, process);
        }

        public void UnRegisterKeyProcess(string key)
        {
            if (m_KeyProcess == null)
            {
                return;
            }

            if (!m_KeyProcess.ContainsKey(key))
            {
                return;
            }

            m_KeyProcess.Remove(key);
        }

        public void RemoveAllKeyProcess()
        {
            if (m_KeyProcess == null)
            {
                return;
            }

            m_KeyProcess.Clear();
        }

        public string ProcessString(string rawValue)
        {
            if (rawValue == null)
            {
                return rawValue;
            }

            m_Builder.Remove(0, m_Builder.Length);
            m_Builder.Append(rawValue);

            MatchCollection match = m_Regex.Matches(rawValue);
            if (match.Count > 0)
            {
                for (int i = 0; i < match.Count; ++i)
                {
                    string result = Key2Value(match[i].Value);
                    m_Builder.Replace(match[i].Value, result);
                }
            }

            return m_Builder.ToString();
        }

        public string Key2Value(string key)
        {
            KeyProcess process = null;
            if (m_KeyProcess.TryGetValue(key, out process))
            {
                return process();
            }
            return key;
        }
    }
}
