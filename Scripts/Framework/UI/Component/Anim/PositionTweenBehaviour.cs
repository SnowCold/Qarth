using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using DG.Tweening;

namespace Qarth
{
    public class PositionTweenBehaviour : TweenBehaviour
    {
        [SerializeField]
        private float m_DelayTime = 0;
        [SerializeField]
        private bool m_IsLocalPosition = true;
        [SerializeField]
        private bool m_AutoSyncStartPosition = false;
        [SerializeField]
        private bool m_AdjustStartPosition = true;
        [SerializeField]
        private Vector3 m_StartPos;
        [SerializeField]
        private Vector3 m_EndPosition;
        [SerializeField]
        private float m_Duration = 0.6f;
        [SerializeField]
        private Ease m_EaseMode = Ease.OutElastic;

        [SerializeField]
        private float m_V0 = 0.1f;
        [SerializeField]
        private float m_V1 = 0.3f;

        private Tweener m_Tweener;
        private bool m_HasInit = false;

        public Vector3 startPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        public Vector3 endPos
        {
            get { return m_EndPosition; }
            set { m_EndPosition = value; }
        }

        public override void StartAnim(float delayTime = 0)
        {
            InitStartPosition();

            if (m_IsLocalPosition)
            {
                if (m_AdjustStartPosition)
                {
                    transform.localPosition = m_StartPos;
                }

                m_Tweener = transform.DOLocalMove(m_EndPosition, m_Duration)
                    .SetEase(m_EaseMode, m_V0, m_V1).OnComplete(OnTweenComplate);
            }
            else
            {
                if (m_AdjustStartPosition)
                {
                    transform.position = m_StartPos;
                }

                m_Tweener = transform.DOMove(m_EndPosition, m_Duration)
                    .SetEase(m_EaseMode).OnComplete(OnTweenComplate);
            }

            if (delayTime > 0)
            {
                m_Tweener.SetDelay(delayTime);
            }
            else if (m_DelayTime > 0)
            {
                m_Tweener.SetDelay(m_DelayTime);
            }
        }

        public override void StopAnim()
        {
            if (m_Tweener != null)
            {
                m_Tweener.Kill();
                m_Tweener = null;
            }
        }

        private void OnTweenComplate()
        {
            m_Tweener = null;
        }

        private void Awake()
        {
            InitStartPosition();
        }

        private void InitStartPosition()
        {
            if (m_HasInit)
            {
                return;
            }

            m_HasInit = true;

            if (m_AutoSyncStartPosition)
            {
                if (m_IsLocalPosition)
                {
                    m_StartPos = transform.localPosition;
                }
                else
                {
                    m_StartPos = transform.position;
                }
            }
        }

    }
}
