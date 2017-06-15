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
using UnityEngine.UI;
using Qarth;
using UnityEngine.EventSystems;

namespace Qarth
{
    public class SoundButton : Button
    {
        private static bool s_EnableSoundButton = true;
        private static string s_defaultClickSound;

        public static string defaultClickSound
        {
            get { return s_defaultClickSound; }
            set { s_defaultClickSound = value; }
        }

        public static bool enableSoundButton
        {
            get { return s_EnableSoundButton; }
            set { s_EnableSoundButton = value; }
        }

        [SerializeField]
        private bool m_IsSoundEnable = true;
        [SerializeField]
        private string m_ClickSound;
        [SerializeField]
        private bool m_UseDefaultSound = true;

        public bool isSoundEnable
        {
            get { return m_IsSoundEnable; }
            set { m_IsSoundEnable = value; }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (isSoundEnable && s_EnableSoundButton)
            {
                if (!string.IsNullOrEmpty(m_ClickSound))
                {
                    AudioMgr.S.PlaySound(m_ClickSound);
                }
                else if (m_UseDefaultSound)
                {
                    AudioMgr.S.PlaySound(s_defaultClickSound);
                }
            }
        }
    }
}
