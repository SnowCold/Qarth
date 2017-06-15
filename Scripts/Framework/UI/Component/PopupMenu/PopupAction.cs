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
using DG.Tweening;

namespace Qarth
{
    public class PopupAction : MonoBehaviour
    {
        enum eState
        {
            kNone,
            kPull,
            kPop,
        }

        [Serializable]
        public class PopItem
        {
            [SerializeField]
            private Vector3 m_StartPos;
            [SerializeField]
            private Vector3 m_EndPos;
            [SerializeField]
            private Transform m_ContextRoot;
            [SerializeField]
            private float m_AnimSpeed = 200;//每秒距离
            [SerializeField]
            private Image m_DirImage;

            public float Pull()
            {
                m_ContextRoot.DOKill();

                Vector3 distancePos = m_ContextRoot.localPosition - m_EndPos;

                float duration = Mathf.Abs(distancePos.magnitude) / m_AnimSpeed;

                m_ContextRoot.DOLocalMove(m_EndPos, duration);

                RotateDirImage(Vector3.zero, duration);
                return duration;
            }

            public float Pop()
            {
                m_ContextRoot.DOKill();

                Vector3 distancePos = m_ContextRoot.localPosition - m_StartPos;

                float duration = Mathf.Abs(distancePos.magnitude) / m_AnimSpeed;

                m_ContextRoot.DOLocalMove(m_StartPos, duration);

                RotateDirImage(new Vector3(0, 0, 180), duration);

                return duration;
            }

            private void RotateDirImage(Vector3 angle, float duration)
            {
                if (m_DirImage == null)
                {
                    return;
                }

                m_DirImage.transform.DOKill();

                m_DirImage.transform.DOLocalRotate(angle, duration);
            }
        }
        [SerializeField]
        protected PopItem[] m_PopItemArray;
        [SerializeField]
        protected Button m_ControlButton;

        private eState m_CurrentState = eState.kNone;

        public void Pull(bool anim = true)
        {
            if (m_PopItemArray == null)
            {
                return;
            }

            if (m_CurrentState == eState.kPull)
            {
                return;
            }

            m_CurrentState = eState.kPull;

            for (int i = 0; i < m_PopItemArray.Length; ++i)
            {
                m_PopItemArray[i].Pull();
            }

        }

        public void Pop(bool anim = true)
        {
            if (m_PopItemArray == null)
            {
                return;
            }

            if (m_CurrentState == eState.kPop)
            {
                return;
            }

            m_CurrentState = eState.kPop;

            for (int i = 0; i < m_PopItemArray.Length; ++i)
            {
                m_PopItemArray[i].Pop();
            }
        }

        private void Awake()
        {
            if (m_ControlButton != null)
            {
                m_ControlButton.onClick.AddListener(OnClickControlButton);
            }

            Pop();
        }

        private void OnClickControlButton()
        {
            switch (m_CurrentState)
            {
                case eState.kPop:
                    Pull();
                    break;
                case eState.kNone:
                case eState.kPull:
                    Pop();
                    break;
                default:
                    break;
            }
        }

    }

}
