using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class UIEffectHelper
    {
        public static void AddUIEffect(Transform parent, GameObject effectRoot, int offsetOrder = 1)
        {
            int sortingOrder = 0;
            Canvas parentCanvas = parent.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                sortingOrder = parentCanvas.sortingOrder;
            }

            effectRoot.transform.SetParent(parent, true);

            Renderer[] childs = effectRoot.GetComponentsInChildren<Renderer>(true);
            if (childs != null)
            {
                for (int i = 0; i < childs.Length; ++i)
                {
                    childs[i].sortingOrder = sortingOrder + 1;
                }
            }
        }
    }
}
