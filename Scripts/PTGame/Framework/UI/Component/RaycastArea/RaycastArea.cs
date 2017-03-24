using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    [ExecuteInEditMode]
    public class RaycastArea : UnityEngine.UI.MaskableGraphic
    {
        //private static Color HideColor = new Color(0, 0, 0, 0);

        protected RaycastArea()
        {
            useLegacyMeshGeneration = false;
        }
        /*
        protected override void Awake()
        {
            base.Awake();
            useLegacyMeshGeneration = false;
            color = HideColor;
        }
        */

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        protected override void UpdateGeometry()
        {
            base.UpdateGeometry();
            gameObject.layer = LayerDefine.LAYER_HIDE_UI;
        }
    }
}
