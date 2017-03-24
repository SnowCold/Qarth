using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class WorldUIBindPos : WorldUI
    {
        [SerializeField]
        private Vector3 m_FollowPosition;

        public Vector3 followPosition
        {
            get { return m_FollowPosition; }
            set { m_FollowPosition = value; UpdateBinding(); }
        }

        protected override bool IsNeedUpdate()
        {
            return IsWorldPositionInView(m_FollowPosition);
        }

        protected override void OnWorldUIBinding()
        {
            if (m_Binding == null)
            {
                m_Binding = new WorldUIBinding();
            }

            m_Binding.Set(m_TargetUI, m_FollowPosition, m_UIOffset, m_WorldOffset);
        }
    }
}
