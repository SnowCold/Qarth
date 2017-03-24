using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class WorldRoot : MonoBehaviour
    {
        [SerializeField]
        private Camera m_WorldCamera;
        [SerializeField]
        private Transform m_EnvironmentRoot;

        public Camera worldCamera
        {
            get { return m_WorldCamera; }
        }

        public Transform environmentRoot
        {
            get { return m_EnvironmentRoot; }
        }
    }
}
