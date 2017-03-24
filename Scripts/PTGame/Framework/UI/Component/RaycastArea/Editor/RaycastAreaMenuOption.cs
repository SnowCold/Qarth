using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    static internal class RaycastAreaMenuOption
    {
        [MenuItem("GameObject/UI/RaycastArea", false, 2155)]
        static public void AddRaycastGraphic(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null)
            {
                return;
            }

            GameObject element = new GameObject("RaycastArea");
            RectTransform rectTransform = element.AddComponent<RectTransform>();

            element.AddComponent<SCFramework.RaycastArea>();

            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }

}
