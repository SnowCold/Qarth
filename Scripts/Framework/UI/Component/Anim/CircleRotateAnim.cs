//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace Qarth
{
    public class CircleRotateAnim : MonoBehaviour, IPointerDownHandler
    {

        #region 参数设置
        [SerializeField]
        private float m_MaxOffsetAngle = 45.0f;
        [SerializeField]
        private float m_DecalFactor = 10.0f;
        [SerializeField]
        private float m_RotateSpeed = 10.0f;
        [SerializeField]
        private bool m_RandomDecalFactor = false;
        [SerializeField]
        private Transform m_Target;
        #endregion
        private float m_CurrentAngle;
        private float m_AnimDuration = 0;
        private float m_PreRotateAngle = 0;
        private float m_CurrentMaxOffsetAngle;
        private float m_CurrentDecalFactor;
        public Action OnFinish;

        public void StartAction()
        {
            if (m_RandomDecalFactor)
            {
                m_CurrentDecalFactor = m_DecalFactor + RandomHelper.Range(-5, 5);
                m_CurrentDecalFactor = Mathf.Max(m_CurrentDecalFactor, 2);

                m_CurrentMaxOffsetAngle = m_MaxOffsetAngle + RandomHelper.Range(-10, 10);
                m_CurrentMaxOffsetAngle = Mathf.Max(m_CurrentMaxOffsetAngle, 5);
            }
            else
            {
                m_CurrentMaxOffsetAngle = m_MaxOffsetAngle;
                m_CurrentDecalFactor = m_DecalFactor;
            }

            StopAllCoroutines();
            m_AnimDuration = 0;
            m_CurrentAngle = 0;
            StartCoroutine(UpdateAction());
        }

        private IEnumerator UpdateAction()
        {
            while (true)
            {
                float offsetRad = (m_CurrentMaxOffsetAngle - (m_AnimDuration * m_CurrentDecalFactor));
                if (offsetRad <= 0)
                {
                    break;
                }

                offsetRad *= Mathf.Deg2Rad;
                float radian = Mathf.Sin(m_CurrentAngle * Mathf.Deg2Rad) * offsetRad;
                m_CurrentAngle += m_RotateSpeed * Time.deltaTime;
                m_AnimDuration += Time.deltaTime;
                if (m_Target != null)
                {
                    float targetAngle = radian * Mathf.Rad2Deg;
                    m_Target.Rotate(Vector3.forward, targetAngle - m_PreRotateAngle, Space.Self);
                    m_PreRotateAngle = targetAngle;
                }
                yield return 0;
            }

            if (OnFinish != null)
            {
                OnFinish();
            }
        }

        void OnEnable()
        {
            if (m_Target != null)
            {
                m_Target.localEulerAngles = Vector3.zero;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartAction();
        }
    }
}
