using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class UIHelper
    {
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
