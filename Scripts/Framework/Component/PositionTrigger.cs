using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public class PositionTrigger
    {
        public enum DirMode
        {
            X = 0,
            Y = 1,
            Z = 2,
        }

        public delegate Vector3 TargetPositionGetter();
        public delegate float OnTriggerActive();

        private Vector3 m_TriggerPosition;
        private DirMode m_DirMode = DirMode.X;
        private int INDEX = 0;

        private TargetPositionGetter m_PositionGetter;
        private OnTriggerActive m_OnTriggerActive;

        public void Init(Vector3 initTriggerPos, TargetPositionGetter positionGetter, OnTriggerActive callBack, DirMode dirMode)
        {
            m_TriggerPosition = initTriggerPos;
            m_DirMode = dirMode;
            m_PositionGetter = positionGetter;
            m_OnTriggerActive = callBack;
            m_DirMode = dirMode;

            INDEX = (int)m_DirMode;
        }

        public void Update()
        {
            Vector3 targetPosition = m_PositionGetter();
            while (IsPassTrigger(targetPosition))
            {
                AddTriggerOffset(m_OnTriggerActive());
            }
        }

        protected void AddTriggerOffset(float value)
        {
            m_TriggerPosition[INDEX] += value;
        }

        protected bool IsPassTrigger(Vector3 targetPosition)
        {
            float targetValue = targetPosition[INDEX];
            float triggerValue = m_TriggerPosition[INDEX];

            if (targetValue >= triggerValue)
            {
                return true;
            }

            return false;
        }
    }
}
