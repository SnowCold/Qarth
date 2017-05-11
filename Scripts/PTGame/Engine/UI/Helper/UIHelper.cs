using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class UIHelper
    {
        public static void AttachUI(GameObject go, Transform parent)
        {
            if (go == null)
            {
                return;
            }

            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;
            Quaternion rotate = Quaternion.identity;

            RectTransform rtTr = go.GetComponent<RectTransform>();
            if (rtTr != null)
            {
                anchorPos = rtTr.anchoredPosition;
                sizeDel = rtTr.sizeDelta;
                scale = rtTr.localScale;
                rotate = rtTr.rotation;
            }

            rtTr.SetParent(parent, false);

            if (rtTr != null)
            {
                rtTr.anchoredPosition = anchorPos;
                rtTr.sizeDelta = sizeDel;
                rtTr.localScale = scale;
                rtTr.rotation = rotate;
            }
        }

        public static void SetUINodeGrey(GameObject uiObj)
        {
            SetUINodeColor(uiObj, Color.gray);
        }

        public static void SetUINodeNormal(GameObject uiObj)
        {
            SetUINodeColor(uiObj, Color.white);
        }

        static void SetUINodeColor(GameObject uiObj, Color color)
        {
            if (uiObj == null)
            {
                return;
            }

            Image image = uiObj.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }

            Text text = uiObj.GetComponent<Text>();
            if (text != null)
            {
                text.color = color;
            }

            for (int i = 0; i < uiObj.transform.childCount; ++i)
            {
                SetUINodeColor(uiObj.transform.GetChild(i).gameObject, color);
            }
        }
    }
}
