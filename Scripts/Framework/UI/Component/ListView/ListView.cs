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
using UnityEngine.UI;

namespace Qarth
{
    [AddComponentMenu("UI/ListView", 50)]
    [DisallowMultipleComponent]
    public class ListView : LoopScrollRect
    {

        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_Direction == eDirection.kHorizontal)
            {
                
                if (m_GridLayout != null)
                {
                    size += m_GridLayout.cellSize.x;
                }
                else
                {
                    size += LayoutUtility.GetPreferredWidth(item);
                }
            }
            else
            {
                if (m_GridLayout != null)
                {
                    size += m_GridLayout.cellSize.y;
                }
                else
                {
                    size += LayoutUtility.GetPreferredHeight(item);
                }
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            if (m_Direction == eDirection.kHorizontal)
            {
                return vector.x;
            }
            else
            {
                return vector.y;
            }
        }

        private Vector2 m_TempVector = new Vector2();
        protected override Vector2 GetVector(float value)
        {
            if (m_Direction == eDirection.kHorizontal)
            {
                m_TempVector.Set(-value, 0);
            }
            else
            {
                m_TempVector.Set(0, value);
            }

            return m_TempVector;
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_Direction == eDirection.kHorizontal)
            {
                directionSign = 1;

                GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
                if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
                {
                    Log.e("[Horizontal Mode ListView] unsupported GridLayoutGroup constraint");
                }
            }
            else
            {
                directionSign = -1;

                GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
                if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    Log.e("[Vertical Mode ListView] unsupported GridLayoutGroup constraint");
                }
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;

            if (m_Direction == eDirection.kHorizontal)
            {
                if (viewBounds.max.x > contentBounds.max.x)
                {
                    float size = NewItemAtEnd();
                    if (size > 0)
                    {
                        if (threshold < size)
                        {
                            // Preventing new and delete repeatly...
                            threshold = size * 1.1f;
                        }
                        changed = true;
                    }
                }
                else if (viewBounds.max.x < contentBounds.max.x - threshold)
                {
                    float size = DeleteItemAtEnd();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }

                if (viewBounds.min.x < contentBounds.min.x)
                {
                    float size = NewItemAtStart();
                    if (size > 0)
                    {
                        if (threshold < size)
                        {
                            threshold = size * 1.1f;
                        }
                        changed = true;
                    }
                }
                else if (viewBounds.min.x > contentBounds.min.x + threshold)
                {
                    float size = DeleteItemAtStart();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }
                return changed;
            }
            else
            {
                if (viewBounds.min.y < contentBounds.min.y + 1)
                {
                    float size = NewItemAtEnd();
                    if (size > 0)
                    {
                        if (threshold < size)
                        {
                            threshold = size * 1.1f;
                        }
                        changed = true;
                    }
                }
                else if (viewBounds.min.y > contentBounds.min.y + threshold)
                {
                    float size = DeleteItemAtEnd();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }
                if (viewBounds.max.y > contentBounds.max.y - 1)
                {
                    float size = NewItemAtStart();
                    if (size > 0)
                    {
                        if (threshold < size)
                        {
                            threshold = size * 1.1f;
                        }
                        changed = true;
                    }
                }
                else if (viewBounds.max.y < contentBounds.max.y - threshold)
                {
                    float size = DeleteItemAtStart();
                    if (size > 0)
                    {
                        changed = true;
                    }
                }
                return changed;
            }
        }
    }
}
