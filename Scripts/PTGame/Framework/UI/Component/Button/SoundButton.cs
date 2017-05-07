using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PTGame.Framework;
using UnityEngine.EventSystems;

namespace PTGame.Framework
{
    public class SoundButton : Button
    {
        private static string s_defaultClickSound;

        public static string defaultClickSound
        {
            get { return s_defaultClickSound; }
            set { s_defaultClickSound = value; }
        }

        [SerializeField]
        private string m_ClickSound;
        [SerializeField]
        private bool m_UseDefaultSound;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
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
