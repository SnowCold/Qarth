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

namespace Qarth
{
    public class WorldUIBinding
    {
        public enum BindMode
        {
            None,
            Position,
            Transform,
        }

        private Transform   m_TargetUI;
        private Transform   m_TargetObject;
        private Vector3     m_TargetPosition;
        private Vector3     m_WorldOffset;
        private Vector3     m_UIOffset;

        private Vector3     m_OldPos;
        private bool        m_IsDirty = false;
        private BindMode    m_BindMode = BindMode.None;

        public void SetDirty()
        {
            m_IsDirty = true;
        }

        public Vector3 worldOffset
        {
            get { return m_WorldOffset; }
            set
            {
                m_WorldOffset = value;
                m_IsDirty = true;
            }
        }

        public Vector3 uiOffset
        {
            get { return m_UIOffset; }
            set
            {
                m_UIOffset = value;
                m_IsDirty = true;
            }
        }

        public void Set(Transform ui, Transform worldObj, Vector3 uiOffset, Vector3 worldOffset)
        {
            m_BindMode = BindMode.None;
            if (ui == null || worldObj == null)
            {
                return;
            }
            m_TargetUI = ui;
            m_TargetObject = worldObj;
            m_UIOffset = uiOffset;
            m_WorldOffset = worldOffset;
            m_IsDirty = true;
            m_BindMode = BindMode.Transform;
        }

        public void Set(Transform ui, Vector3 position, Vector3 uiOffset, Vector3 worldOffset)
        {
            m_BindMode = BindMode.None;
            if (ui == null)
            {
                return;
            }
            m_TargetUI = ui;
            m_TargetPosition = position;
            m_UIOffset = uiOffset;
            m_WorldOffset = worldOffset;
            m_IsDirty = true;
            m_BindMode = BindMode.Position;
        }

        public void Update()
        {
            if (m_BindMode == BindMode.None)
            {
                return;
            }

            Vector3 newPos = m_TargetPosition;
            if (m_BindMode == BindMode.Transform)
            {
                newPos = m_TargetObject.position;
            }

            newPos += m_WorldOffset;

            try
            {
                ScenePosition2UIPosition(Camera.main, UIMgr.S.uiRoot.uiCamera, newPos, m_TargetUI, m_UIOffset);
            }
            catch (System.Exception ex)
            {
                Log.e(ex);
            }
            
        }

        private void ScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene, Transform uiTarget)
        {
            Vector3 viewportPos = sceneCamera.WorldToViewportPoint(posInScene);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            if (m_IsDirty || m_OldPos.x != worldPos.x || m_OldPos.y != worldPos.y || m_OldPos.z != worldPos.z)
            {
                m_IsDirty = false;
                m_OldPos = worldPos;

                uiTarget.position = worldPos;

                Vector3 localPos = uiTarget.localPosition;

                localPos.z = 0f;

                uiTarget.localPosition = localPos;
            }
        }

        private void ScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene, Transform uiTarget, Vector3 offset)
        {
            ScenePosition2UIPosition(sceneCamera, uiCamera, posInScene, uiTarget);
            uiTarget.localPosition += offset;
        }
    }
}
